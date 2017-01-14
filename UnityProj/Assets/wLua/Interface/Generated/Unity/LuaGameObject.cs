using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace LuaInterface
{
	public partial class LuaGameObjectObject
	{

		//---------------------------------
		[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
		static int GameObject(IntPtr L)
		{
			try
			{
				System.String param0 = default(System.String);
				param0 = LuaDLL.lua_tostring(L, 1);
				UnityEngine.GameObject ret = new UnityEngine.GameObject(
				param0
				);
				LuaExtend.AddObject2Lua(L, ret, null);
				return 1;
			}
			catch (Exception e)
			{
				return LuaDLL.wluaL_error(L, e);
			}

		}

		[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
		static int get_activeSelf(IntPtr L)
		{
			try
			{
				UnityEngine.GameObject self = LuaExtend.GetObject(L, 1) as UnityEngine.GameObject;
				if (self == null)
				{
					throw new LuaException(L,"Object is null");
				}
				LuaDLL.lua_pushboolean(L, self.activeSelf);
				return 1;
			}
			catch (Exception e)
			{
				return LuaDLL.wluaL_error(L, e);
			}
		}

		[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
		static int set_tag(IntPtr L)
		{
			try
			{
				UnityEngine.GameObject self = LuaExtend.GetObject(L, 1) as UnityEngine.GameObject;
				if (self == null)
				{
					throw new LuaException(L,"Object is null");
				}
				self.tag = LuaDLL.lua_tostring(L, 2);
				return 0;
			}
			catch (Exception e)
			{
				return LuaDLL.wluaL_error(L, e);
			}
		}

		[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
		static int CreatePrimitive(IntPtr L)
		{
			try
			{
				PrimitiveType type  = (PrimitiveType)LuaDLL.lua_tointeger(L, 1);
				GameObject go = UnityEngine.GameObject.CreatePrimitive(type);
				LuaExtend.AddObject2Lua(L, go, null);
				return 1;
			}
			catch (Exception e)
			{
				return LuaDLL.wluaL_error(L, e);
			}
		}

		[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
		static int AddComponent(IntPtr L)
		{
			try
			{
				UnityEngine.GameObject self = LuaExtend.GetObject(L, 1) as UnityEngine.GameObject;
				if (self == null)
				{
					throw new LuaException(L, "Object is null");
				}
				string component = LuaDLL.lua_tostring(L, 2);
				ComponentMethodDelegate func = null;
				if (typesAdd.TryGetValue(component, out func))
				{
					UnityEngine.Component com = func(self);
					LuaExtend.AddObject2Lua(L, com, null);
				}
				else
				{
					throw new LuaException(L, "Component is not registed:" + component);
				}
				return 1;
			}
			catch (Exception e)
			{
				return LuaDLL.wluaL_error(L, e);
			}
		}


		[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
		static int GetComponent(IntPtr L)
		{
			try
			{
				UnityEngine.GameObject self = LuaExtend.GetObject(L, 1) as UnityEngine.GameObject;
				if (self == null)
				{
					throw new LuaException(L, "Object is null");
				}
				string component = LuaDLL.lua_tostring(L, 2);
				ComponentMethodDelegate func = null;
				if (typesGet.TryGetValue(component, out func))
				{
					UnityEngine.Component com = func(self);
					LuaExtend.AddObject2Lua(L, com, null);
				}
				else
				{
					throw new LuaException(L, "Component is not registed:" + component);
				}
				return 1;
			}
			catch (Exception e)
			{
				return LuaDLL.wluaL_error(L, e);
			}
		}



		static void _addMemberFunction(IntPtr L)
		{
			//functions
			LuaDLL.wLua_wrapfunction(L, -2, GameObject,true);
			LuaDLL.wlua_setfield(L, -2, "GameObject");
			LuaDLL.wLua_wrapfunction(L, -2, AddComponent, true);
			LuaDLL.wlua_setfield(L, -2, "AddComponent");
			LuaDLL.wLua_wrapfunction(L, -2, GetComponent, true);
			LuaDLL.wlua_setfield(L, -2, "GetComponent");
			LuaDLL.wLua_wrapfunction(L, -2, CreatePrimitive, true);
			LuaDLL.wlua_setfield(L, -2, "CreatePrimitive");
			LuaDLL.wLua_wrapfunction(L, -2, get_activeSelf);
			LuaDLL.wlua_setfield(L, -2, "get_activeSelf");
			LuaDLL.wLua_wrapfunction(L, -2, set_tag);
			LuaDLL.wlua_setfield(L, -2, "set_tag");
			LuaDLL.wLua_wrapfunction(L, -2, LuaFuncs.removeObjectFromCache);
			LuaDLL.wlua_setfield(L, -2, "__gc");
		}

		static void _InitLink(IntPtr L)
		{
			Lua_Register._SetParent(L, "GameObject", "Object");
		}

		public static void Init(IntPtr L,InitStep step)
		{
			if (step == InitStep.FIRSTLY)
			{
				LuaDLL.lua_newtable(L);//unityengine,cswrap,t
				_addMemberFunction(L);//unityengine,cswrap,t
				Lua_Register._SetTypeTable2Namespace(L, "GameObject"); //unityengine,cswrap

			}
			else if (step == InitStep.FINAL)
				_InitLink(L);
		}
	}
}
