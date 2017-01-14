using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace LuaInterface
{
	public sealed partial class Lua_Register
	{
		public static void _SetTypeTable2Namespace(IntPtr L,string name)
		{
			LuaDLL.wlua_makecsindex(L, -1);                     //unityengine,cswrap,t,indexfunc
			LuaDLL.wlua_setfield(L, -2, "__index");             //unityengine,cswrap,t
			LuaDLL.wlua_setfield(L, -3, name);					//unityengine,cswrap
		}

		public static void _SetParent(IntPtr L,string subclass,string superclass)
		{
			LuaDLL.wlua_getfield(L, -1, subclass); //unityengine,t
			LuaDLL.wlua_getfield(L, -2, superclass); //unityengine,t,pt
												   //t's metatable = pt
			LuaDLL.lua_setmetatable(L, -2); //unityengine,t
			LuaDLL.lua_pop(L, 1); //unityengine
		}
	}
	public partial class LuaObject
	{

		//---------------------------------
		[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
		static int get_name(IntPtr L)
		{
			try
			{
				UnityEngine.Object self = LuaExtend.GetObject(L, 1) as UnityEngine.Object;
				if (self == null)
				{
					throw new LuaException(L,"Object is null");
				}
				LuaDLL.lua_pushstring(L, self.name);
				return 1;
			}
			catch(Exception e)
			{
				return LuaDLL.wluaL_error(L, e);
			}
		}

		[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
		static int set_name(IntPtr L)
		{
			try
			{
				UnityEngine.Object self = LuaExtend.GetObject(L, 1) as UnityEngine.Object;
				if (self == null)
				{
					throw new LuaException(L,"Object is null");
				}
				self.name = LuaDLL.lua_tostring(L, 2);
				return 0;
			}
			catch(Exception e)
			{
				return LuaDLL.wluaL_error(L, e);
			}
		}

		//---------------------------------
		[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
		static int Destroy(IntPtr L)
		{
			try
			{
				UnityEngine.Object self = LuaExtend.GetObject(L, 1) as UnityEngine.Object;
				if (self == null)
				{
					throw new LuaException(L,"Object is null");
				}
				System.Double param1 = default(System.Double);
				param1 = (System.Double)LuaDLL.lua_tonumber(L, 2);
				if (self != null)
				{
					UnityEngine.Object.Destroy(
					(UnityEngine.Object)self,
					(System.Single)param1
					);
				}
				IntPtr idptr = LuaDLL.lua_touserdata(L, 1);
				LuaExtend.RemoveObject(L,idptr);
				return 0;
			}
			catch (Exception e)
			{
				return LuaDLL.wluaL_error(L, e);
			}
		}

		static void _addMemberFunction(IntPtr L)
		{
			LuaDLL.wLua_wrapfunction(L, -2, get_name);
			LuaDLL.wlua_setfield(L, -2, "get_name");
			LuaDLL.wLua_wrapfunction(L, -2, set_name);
			LuaDLL.wlua_setfield(L, -2, "set_name");
			LuaDLL.wLua_wrapfunction(L, -2, Destroy);
			LuaDLL.wlua_setfield(L, -2, "Destroy");
			LuaDLL.wLua_wrapfunction(L, -2, LuaFuncs.removeObjectFromCache);
			LuaDLL.wlua_setfield(L, -2, "__gc");
		}

		static void _InitLink(IntPtr L)
		{
		}


		public static void Init(IntPtr L, InitStep step)
		{
			if (step == InitStep.FIRSTLY)
			{
				LuaDLL.lua_newtable(L);//unityengine,cswrap,t
				_addMemberFunction(L); //unityengine,cswrap,t
				Lua_Register._SetTypeTable2Namespace(L, "Object"); //unityengine,cswrap
			}
			else if (step == InitStep.FINAL)
				_InitLink(L);
		}

	}

} //namespace
