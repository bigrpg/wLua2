using System;
using UnityEngine;

namespace LuaInterface
{
	public class GameLogicLua
	{
		internal static LuaL_Reg[] Lua_funcs;

		//---------------------------------
		[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
		static int test(IntPtr L)
		{
			try
			{
				LuaDLL.lua_pushnil(L);
				int exception = 0;
				while( LuaDLL.wlua_next(L,1,out exception) != 0  && exception ==0)
				{
					string k = LuaDLL.lua_tostring(L, -2);
					string v = LuaDLL.lua_tostring(L, -1);
					Debug.LogWarning("k:" + k);
					Debug.LogWarning("v:" + v);
					LuaDLL.lua_pop(L, 1);
				}
				if (exception != 0)
					throw new LuaException(L,LuaDLL.lua_tostring(L, -1));

				return 0;
			}
			catch (Exception e)
			{
				return LuaDLL.wluaL_error(L, e);
			}
		}

		[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
		static int testerror(IntPtr L)
		{
			try
			{
				throw new LuaException(L,"func:testerror");
			}
			catch (Exception e)
			{
				return LuaDLL.wluaL_error(L, e);
			}
		}



		////////////////////////
		public static void Register(IntPtr L)
		{
			Lua_funcs = new LuaL_Reg[]
			{
				new LuaL_Reg("test",new  LuaCSFunction(test)),
				new LuaL_Reg("testerror",new  LuaCSFunction(testerror)),
			};

			LuaDLL.lua_newtable(L);
			LuaDLL.luaL_setfuncs(L, Lua_funcs, 0);
			LuaDLL.wlua_setglobal(L, "Game");

		}
	}
}