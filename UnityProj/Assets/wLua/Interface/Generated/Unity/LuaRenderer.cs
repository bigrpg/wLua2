using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace LuaInterface
{
	public partial class LuaRenderer
	{

		[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
		static int get_receiveShadows(IntPtr L)
		{
			try
			{
				UnityEngine.Renderer self = LuaExtend.GetObject(L, 1) as UnityEngine.Renderer;
				if (self == null)
				{
					throw new LuaException(L, "Object is null");
				}
				LuaDLL.lua_pushboolean(L, self.receiveShadows);
				return 1;
			}
			catch (Exception e)
			{
				return LuaDLL.wluaL_error(L, e);
			}
		}

		[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
		static int set_receiveShadows(IntPtr L)
		{
			try
			{
				UnityEngine.Renderer self = LuaExtend.GetObject(L, 1) as UnityEngine.Renderer;
				if (self == null)
				{
					throw new LuaException(L, "Object is null");
				}
				self.receiveShadows = LuaDLL.lua_toboolean(L, 2);
				return 0;
			}
			catch (Exception e)
			{
				return LuaDLL.wluaL_error(L, e);
			}
		}

		static void _addMemberFunction(IntPtr L)
		{
			LuaDLL.wLua_wrapfunction(L, -2, get_receiveShadows);
			LuaDLL.wlua_setfield(L, -2, "get_receiveShadows");
			LuaDLL.wLua_wrapfunction(L, -2, set_receiveShadows);
			LuaDLL.wlua_setfield(L, -2, "set_receiveShadows");

		}

		static void _InitLink(IntPtr L)
		{
			Lua_Register._SetParent(L, "Renderer", "Component");
		}


		public static void Init(IntPtr L, InitStep step)
		{
			if (step == InitStep.FIRSTLY)
			{
				LuaDLL.lua_newtable(L);//unityengine,cswrap,t
				_addMemberFunction(L);//unityengine,cswrap,t
				Lua_Register._SetTypeTable2Namespace(L, "Renderer"); //unityengine,cswrap

			}
			else if (step == InitStep.FINAL)
				_InitLink(L);
		}

	}

} //namespace
