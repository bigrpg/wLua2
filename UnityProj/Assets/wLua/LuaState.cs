using System;

namespace LuaInterface
{

    public class LuaState : IDisposable
    {
        public IntPtr L;

		internal LuaCSFunction panicCallback;

		public LuaState(string LuaNameSpace)
        {
            // Create State
            L = LuaDLL.luaL_newstate();

			string errmsg = null;
            // Create LuaInterface library
            if(LuaDLL.wluaL_openlibs(L) == LuaDLL.LUA_ERROR)
            {
                errmsg = LuaDLL.lua_tostring(L, -1);
				throw new LuaException(L,errmsg);
            }

			if (LuaDLL.wlua_init(L, LuaNameSpace) == LuaDLL.LUA_ERROR)
			{
				errmsg = LuaDLL.lua_tostring(L, -1);
				throw new LuaException(L,errmsg);
			}

			panicCallback = new LuaCSFunction(LuaFuncs.panic);
			LuaDLL.lua_atpanic(L, panicCallback);

		}


		#region IDisposable Members

		public void Dispose()
        {
            Dispose(true);

            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
        }

        public virtual void Dispose(bool dispose)
        {
        }

        #endregion

    }
}