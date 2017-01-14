using System;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;


namespace LuaInterface
{

#pragma warning disable 414
	public class MonoPInvokeCallbackAttribute : System.Attribute
	{
		private Type type;
		public MonoPInvokeCallbackAttribute(Type t) { type = t; }
	}
#pragma warning restore 414

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate int LuaCSFunction(IntPtr L);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate int lua_KFunction(IntPtr L, int status, long ctx);

	sealed class ObjectRefEqualityComparer : IEqualityComparer<System.Object>
	{
		private static ObjectRefEqualityComparer s_instance = null;

		bool IEqualityComparer<System.Object>.Equals(System.Object x, System.Object y)
		{
			return System.Object.ReferenceEquals(x, y);
		}

		int IEqualityComparer<System.Object>.GetHashCode(System.Object obj)
		{
			return System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(obj);
		}

		public static ObjectRefEqualityComparer Instance
		{
			get
			{
				if (s_instance == null)
					s_instance = new ObjectRefEqualityComparer();
				return s_instance;
			}
		}
	}


	sealed class LuaStateCache
	{
		int exception = 0;
		readonly Dictionary<Int64, System.Object> objs = new Dictionary<Int64, System.Object>();
		readonly Dictionary<System.Object, IntPtr> luaObjs = new Dictionary<System.Object, IntPtr>(ObjectRefEqualityComparer.Instance);

		int weakRefForUserData;

		public void Init(IntPtr L)
		{
			LuaDLL.lua_newtable(L);
			LuaDLL.lua_newtable(L);
			LuaDLL.lua_pushstring(L, "v");
			LuaDLL.wlua_setfield(L, -2, "__mode");
			LuaDLL.lua_setmetatable(L, -2);
			weakRefForUserData = LuaDLL.luaL_ref(L, LuaDLL.LUA_REGISTRYINDEX);

		}

		public System.Object GetObject(IntPtr idptr)
		{
			System.Object obj = null;
			objs.TryGetValue(idptr.ToInt64(), out obj);
			return obj;
		}

		//L:  [namespace Table]
		public bool AddObject(IntPtr L,System.Object obj,string metatable /* nullable */)
		{
			IntPtr userdata = IntPtr.Zero;
			if(luaObjs.TryGetValue(obj,out userdata))
			{
				LuaDLL.lua_rawgeti(L, LuaDLL.LUA_REGISTRYINDEX, weakRefForUserData);  //namespace,reftable
				LuaDLL.lua_pushlightuserdata(L, userdata); //namespace,reftable,userdataKey
				LuaDLL.lua_rawget(L, -2); //namespace,reftable,userdata
				LuaDLL.lua_remove(L, -2);  //namespace,userdata
				if (LuaDLL.lua_isuserdata(L, -1))
				{
					return true;
				}
				else
				{
					LuaDLL.lua_pop(L, 1); //namespace
				}
			}

			userdata = LuaDLL.lua_newuserdata(L, 1); //namespace,obj

			if(metatable == null)
			{
				Type type = obj.GetType();
				while (type != null)
				{
					LuaDLL.wlua_getfield(L, -2, type.Name);//namespace,obj,typet
					if (LuaDLL.lua_isnil(L, -1))
					{
						LuaDLL.lua_pop(L, 1); //namespace,obj
						type = type.BaseType;
						continue;
					}
					if (LuaDLL.lua_istable(L, -1))
					{
						metatable = type.Name;
						break;
					}
					else
					{
						LuaDLL.lua_pop(L, 2); //namespace
						throw new LuaException(L, "metatable must be a table:" + type.Name);
					}
				}
			}
			else
			{
				LuaDLL.wlua_getfield(L, -2, metatable);//namespace,obj,typet
				if (LuaDLL.lua_isnil(L, -1))
				{
					LuaDLL.lua_pop(L, 2); //namespace
					throw new LuaException(L, "failed to find metatable:" + metatable);
				}
			}

			//namespace,obj,typet
			LuaDLL.lua_setmetatable(L, -2); //namespace,obj
			objs[userdata.ToInt64()] = obj;
			luaObjs[obj] = userdata;

			LuaDLL.lua_rawgeti(L, LuaDLL.LUA_REGISTRYINDEX, weakRefForUserData); //namespace,obj,reftable
			LuaDLL.lua_pushlightuserdata(L, userdata); //namespace,obj,reftable,userdatakey
			LuaDLL.lua_pushvalue(L, -3);  //namespace,obj,reftable,userdatakey,obj
			LuaDLL.lua_rawset(L, -3); //namespace,obj,reftable
			LuaDLL.lua_pop(L, 1); //namespace,obj

			return true;
		}

		public bool RemoveObject(IntPtr userdata)
		{
			System.Object obj;
			Int64 id = userdata.ToInt64();
			if (objs.TryGetValue(id, out obj))
			{
				objs.Remove(id);
				luaObjs.Remove(obj);
				return true;
			}
			return false;
		}

		public bool isException() { return exception>0; }
		public void setException() { ++exception; }
		public void clearException()
		{
			if(exception >0)
				--exception;
		}

	}

	//you should create all LuaStateCache you need when game started for to avoid multithreading problem.
	sealed class LuaStateCacheMan
	{
#if MULTILUASTATE_SUPPORT
		static readonly Dictionary<IntPtr, LuaStateCache> caches = new Dictionary<IntPtr, LuaStateCache>();
#else
		static LuaStateCache cache = null;
#endif
		public static LuaStateCache GetLuaStateCache(IntPtr L)
		{
#if MULTILUASTATE_SUPPORT
			LuaStateCache cache = null;
			if (caches.TryGetValue(L, out cache))
				return cache;
			throw new LuaException("failed to find luaState cache");
#else
			return cache;
#endif

		}
		public static void AddLuaStateCache(IntPtr L)
		{
#if MULTILUASTATE_SUPPORT
			LuaStateCache cache = null;
			if(caches.TryGetValue(L,out cache))
			{
				return;
			}
			cache = new LuaStateCache();
			cache.Init(L);
			caches[L] = cache;
#else
			if(cache == null)
			{
				cache = new LuaStateCache();
				cache.Init(L);
			}
#endif
		}
	}

	//the only class for public access
	public sealed class LuaExtend
	{
		public static System.Object GetObject(IntPtr L, int index)
		{
			LuaTypes type = (LuaTypes)LuaDLL.lua_type(L, index);
			if (type != LuaTypes.LUA_TUSERDATA)
				return null;
			IntPtr idptr = LuaDLL.lua_touserdata(L, index);
			LuaStateCache cache = LuaStateCacheMan.GetLuaStateCache(L);
			return cache.GetObject(idptr);
		}

		//L:  [namespace Table]
		public static bool AddObject2Lua(IntPtr L, System.Object obj, string metatable /*nullable*/)
		{
			LuaStateCache cache = LuaStateCacheMan.GetLuaStateCache(L);
			return cache.AddObject(L,obj,metatable);
		}

		public static void RemoveObject(IntPtr L, IntPtr userdata)
		{
			LuaStateCache cache = LuaStateCacheMan.GetLuaStateCache(L);
			cache.RemoveObject(userdata);
		}

		public static void SetSearcher(IntPtr L, LuaCSFunction loader)
		{
			int top = LuaDLL.lua_gettop(L);
			LuaDLL.wlua_getglobal(L, "package"); //package
			LuaDLL.wlua_getfield(L, -1, "searchers"); //package,searchers
			LuaDLL.wlua_getglobal(L, "cswrapfunc"); //package,searchers,wrap
			LuaDLL.wLua_wrapfunction(L, -1, loader);//package,searchers,wrap,loader
			LuaDLL.wlua_pushcclosure(L, LuaFuncs.searcher, 1); //package,searchers,wrap,searcher
			LuaDLL.lua_remove(L, -2); //package,searchers,searcher
 
			int searchersIndex = LuaDLL.lua_gettop(L) - 1;
			for (int e = (int)LuaDLL.lua_rawlen(L, searchersIndex) + 1; e > 1; e--)
			{
				LuaDLL.lua_rawgeti(L, searchersIndex, e - 1);//package,searchers,searcher,value
				LuaDLL.lua_rawseti(L, searchersIndex, e);//package,searchers,searcher
			}
			LuaDLL.lua_rawseti(L, searchersIndex, 1);//package,searchers
			LuaDLL.lua_settop(L, top);
		}

		public static bool PCall( IntPtr L,int nargs,int nresult)
		{
			int msgHandlerIndex = LuaDLL.lua_gettop(L) - nargs;
			LuaDLL.lua_pushcfunction(L,LuaFuncs.msgHandler);
			LuaDLL.lua_insert(L, -nargs-2); //msghandler,f,nargs
			int ret = LuaDLL.lua_pcall(L, nargs, nresult, -nargs-2);
			if( ret != LuaDLL.LUA_OK) //msghandler,errmsg
			{
				LuaDLL.lua_remove(L, -2);
				Debug.LogError(LuaDLL.lua_tostring(L, -1));
				return false;
			}
			LuaDLL.lua_remove(L, msgHandlerIndex);
			return true;
		}

		public static bool DoString(IntPtr L,string chunk)
		{
			int result = LuaDLL.luaL_loadstring(L, chunk);
			if (result != 0)
				return false;
			return PCall(L, 0, 0);
		}


	}
}