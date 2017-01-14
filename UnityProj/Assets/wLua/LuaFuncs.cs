using System;
using UnityEngine;

namespace LuaInterface
{

    public sealed class LuaFuncs
    {

        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        public static int panic(IntPtr L)
		{
			Debug.LogError("This message is little more than a fig leaf for your ill-structured codes!!!");
			return 0;
		}

		[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
		public static int msgHandler(IntPtr L)
		{
			if (!LuaDLL.lua_isstring(L, 1))
				return 1;
			LuaDLL.wlua_getglobal(L,"debug"); //msg,debug
			if (!LuaDLL.lua_istable(L, -1))
			{
				LuaDLL.lua_pop(L, 1);
				return 1;
			}
			LuaDLL.wlua_getfield(L, -1, "traceback"); //msg,debug,traceback
			if (!LuaDLL.lua_isfunction(L, -1))
			{
				LuaDLL.lua_pop(L, 2);
				return 1;
			}
			LuaStateCache cache = LuaStateCacheMan.GetLuaStateCache(L);
			LuaDLL.lua_pushvalue(L, 1);  //msg,debug,traceback,msg
			LuaDLL.lua_pushinteger(L, 2 + (cache.isException() ? 2 : 0) );  //msg,debug,traceback,msg,level
			LuaDLL.lua_pcall(L, 2, 1,0);  //msg,debug,tracebackstring
			cache.clearException();
			return 1;
		}


		[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        public static int print(IntPtr L)
        {
            // For each argument we'll 'tostring' it
            int n = LuaDLL.lua_gettop(L);
            string s = String.Empty;

            int ret = LuaDLL.wlua_getglobal(L, "tostring");
            if (ret == LuaDLL.LUA_ERROR)
                return 0;

            for (int i = 1; i <= n; i++)
            {
                LuaDLL.lua_pushvalue(L, -1);  /* function to be called */
                LuaDLL.lua_pushvalue(L, i);   /* value to print */
                ret = LuaDLL.lua_pcall(L, 1, 1, 0);
                if (ret != LuaDLL.LUA_OK)
                {
                    LuaDLL.lua_pop(L, 1);
                    continue;
                }
                string str = LuaDLL.lua_tostring(L, -1);
                if (str == null)
                {
                    Debug.LogError("lua print return null....");
                }
                else
                    s += str;

                if (i < n)
                {
                    s += "\t";
                }

                LuaDLL.lua_pop(L, 1);  /* pop result */
            }
            Debug.Log("LUA: " + s);
            return 0;
        }

		[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
		public static int searcher(IntPtr L)  
		{
			LuaDLL.lua_pushvalue(L, LuaDLL.lua_upvalueindex(1));
			LuaDLL.lua_pushvalue(L, 1);
			return 2;
		}

		[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
		public static int removeObjectFromCache(IntPtr L)
		{
			IntPtr idptr = LuaDLL.lua_touserdata(L, 1);
			LuaExtend.RemoveObject(L,idptr);
			return 0;
		}

	}
}