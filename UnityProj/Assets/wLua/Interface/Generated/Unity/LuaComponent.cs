using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace LuaInterface
{

	public partial class LuaComponent
	{

		//---------------------------------
		[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
		static int set_tag(IntPtr L)
		{
			try
			{
				UnityEngine.Component self = LuaExtend.GetObject(L, 1) as UnityEngine.Component;
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


		static void _addMemberFunction(IntPtr L)
		{
			LuaDLL.wLua_wrapfunction(L, -2, set_tag);
			LuaDLL.wlua_setfield(L, -2, "set_tag");
		}

		static void _InitLink(IntPtr L)
		{
			Lua_Register._SetParent(L, "Component", "Object");
		}


		public static void Init(IntPtr L, InitStep step)
		{
			if (step == InitStep.FIRSTLY)
			{
				LuaDLL.lua_newtable(L);//unityengine,cswrap,t
				_addMemberFunction(L); //unityengine,cswrap,t
				Lua_Register._SetTypeTable2Namespace(L, "Component"); //unityengine,cswrap
			}
			else if (step == InitStep.FINAL)
				_InitLink(L);
		}

	}

} //namespace
