using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace LuaInterface
{
	public partial class LuaCollider
	{
		
		[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
		static int get_enabled(IntPtr L)
		{
			try
			{
				UnityEngine.Collider self = LuaExtend.GetObject(L, 1) as UnityEngine.Collider;
				if (self == null)
				{
					throw new LuaException(L,"Object is null");
				}
				LuaDLL.lua_pushboolean(L, self.enabled);
				return 1;
			}
			catch (Exception e)
			{
				return LuaDLL.wluaL_error(L, e);
			}
		}

		[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
		static int set_enabled(IntPtr L)
		{
			try
			{
				UnityEngine.Collider self = LuaExtend.GetObject(L, 1) as UnityEngine.Collider;
				if (self == null)
				{
					throw new LuaException(L,"Object is null");
				}
				self.enabled = LuaDLL.lua_toboolean(L, 2);
				return 0;
			}
			catch (Exception e)
			{
				return LuaDLL.wluaL_error(L, e);
			}
		}


		static void _addMemberFunction(IntPtr L)
		{
			LuaDLL.wLua_wrapfunction(L, -2, get_enabled);
			LuaDLL.wlua_setfield(L, -2, "get_enabled");
			LuaDLL.wLua_wrapfunction(L, -2, set_enabled);
			LuaDLL.wlua_setfield(L, -2, "set_enabled");


		}

		static void _InitLink(IntPtr L)
		{
			Lua_Register._SetParent(L, "Collider", "Component");
		}


		public static void Init(IntPtr L, InitStep step)
		{
			if (step == InitStep.FIRSTLY)
			{
				LuaDLL.lua_newtable(L);//unityengine,cswrap,t
				_addMemberFunction(L);//unityengine,cswrap,t
				Lua_Register._SetTypeTable2Namespace(L, "Collider"); //unityengine,cswrap

			}
			else if (step == InitStep.FINAL)
				_InitLink(L);
		}

	}

} //namespace
