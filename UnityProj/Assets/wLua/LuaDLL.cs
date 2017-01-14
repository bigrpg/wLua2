using System;
using System.Runtime.InteropServices;
using System.Text;


namespace LuaInterface
{
	public enum LuaTypes
	{
		LUA_TNONE = -1,
		LUA_TNIL = 0,
		LUA_TBOOLEAN = 1,
		LUA_TLIGHTUSERDATA = 2,
		LUA_TNUMBER = 3,
		LUA_TSTRING = 4,
		LUA_TTABLE = 5,
		LUA_TFUNCTION = 6,
		LUA_TUSERDATA = 7,
		LUA_TTHREAD = 8,
	}

	public struct LuaL_Reg
	{
		public string name;
		public LuaCSFunction func;

		public LuaL_Reg(string _name, LuaCSFunction _func)
		{
			name = _name;
			func = _func;
		}
	}


	public class LuaDLL
    {
        public static int LUA_MULTRET = -1;
        public static int LUA_ERROR = -1;
        public static int LUA_OK = 0;

		public static int LUA_REGISTRYINDEX = -1001000;
		public static int LUA_RIDX_GLOBALS = 2;
		public static int LUA_RIDX_MAINTHREAD = 1;

		public static int lua_upvalueindex(int index) { return LUA_REGISTRYINDEX - index; }

#if UNITY_IPHONE || UNITY_XBOX360
        const string LUADLL = "__Internal";
#else
		const string LUADLL = "wLua2";
#endif

		//size_t  --> long


        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int luaL_loadbufferx(IntPtr luaState, byte[] buff, long size, string name, string mode);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int luaL_loadbufferx(IntPtr luaState, IntPtr buff, long size, string name, string mode);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr luaL_newstate();

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern void lua_close(IntPtr luaState);

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int wluaL_openlibs(IntPtr luaState);

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool lua_checkstack(IntPtr luaState,int n);

		//private
        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        static extern void lua_pushcclosure(IntPtr luaState, LuaCSFunction fn, int n);                              //[-n, +1, m]

		public static void lua_pushcfunction(IntPtr luaState, LuaCSFunction fn)
		{
			lua_pushcclosure(luaState, fn, 0);
		}

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int lua_gettop(IntPtr luaState);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int wlua_getglobal(IntPtr luaState,string name);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int lua_createtable(IntPtr luaState, int narr, int nrec);                      //[-0, +1, m]

        public static int lua_newtable(IntPtr luaState)                                                     //[-0, +1, m]
		{
			return lua_createtable(luaState,0,0);
		}

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int lua_rotate(IntPtr luaState,int index,int n);   
		                                                 
        public static int lua_insert(IntPtr luaState,int index)
		{
			return lua_rotate(luaState, index, 1);
		}

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int lua_copy(IntPtr luaState,int fromidx,int toidx);   

		public static void lua_replace(IntPtr luaState,int index)
		{
			lua_copy(luaState, -1, index);
			lua_pop(luaState, 1);
		}

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int wlua_setglobal(IntPtr luaState,string name);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int lua_pushvalue(IntPtr luaState,int index);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern LuaTypes lua_type(IntPtr luaState,int index);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int lua_setmetatable(IntPtr luaState,int index);

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern int lua_getmetatable(IntPtr luaState, int index);

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int wlua_gettable(IntPtr luaState,int index);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern void wlua_settable(IntPtr luaState, int index);

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int wlua_getfield(IntPtr luaState,int index,string k);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int wlua_setfield(IntPtr luaState,int index,string k);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int wlua_init(IntPtr luaState,string wluaNamespace);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int wlua_next(IntPtr luaState,int index, out int exception);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern long lua_rawlen(IntPtr luaState,int index);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int wlua_len(IntPtr luaState,int index);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern long lua_rawgeti(IntPtr luaState,int index,Int64 n);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_rawseti(IntPtr luaState,int index,Int64 i);                   //[-0, +1, m]

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern void lua_rawget(IntPtr luaState, int index);

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]                            //[-0, +1, m]
		public static extern void lua_rawset(IntPtr luaState, int index);

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern void lua_pushboolean(IntPtr luaState, bool b);

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern void lua_pushnumber(IntPtr luaState, double number);

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern void lua_pushinteger(IntPtr luaState, Int64 i); 
		                     
		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern void lua_pushnil(IntPtr luaState);

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern void lua_pushlightuserdata(IntPtr luaState, IntPtr udata);

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr lua_pushlstring(IntPtr luaState, byte[] data, long size);		//[-0, +1, m]

		public static IntPtr lua_pushstring(IntPtr luaState, string str)							//[-0, +1, m]
		{
			byte[] s = Encoding.UTF8.GetBytes(str);
			return lua_pushlstring(luaState, s, s.Length);
		}

		//private
		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        static extern int lua_pcallk(IntPtr luaState, int nArgs, int nResults, int errfunc,long ctx, lua_KFunction k);

        public static int lua_pcall(IntPtr luaState, int nArgs, int nResults, int errfunc)
		{
			return lua_pcallk(luaState, nArgs, nResults, errfunc, 0, null);
		}
        public static int luaL_loadstring(IntPtr luaState, string chunk)
        {
            byte[] s = Encoding.UTF8.GetBytes(chunk);
            return luaL_loadbufferx(luaState, s, s.Length, chunk,"bt");
        }
		public static int luaL_loadbuffer(IntPtr luaState, byte[] buff, long size, string name)
		{
			return luaL_loadbufferx(luaState, buff, size, name, null);
		}

		//public static int luaL_dostring(IntPtr luaState, string chunk)
  //      {
  //          int result = LuaDLL.luaL_loadstring(luaState, chunk);
  //          if (result != 0)
  //              return result;

  //          return LuaDLL.lua_pcall(luaState, 0, LUA_MULTRET, 0);
  //      }

        public static void wlua_pushcfunction(IntPtr L, LuaCSFunction f)         
        {
			LuaDLL.wlua_getglobal(L, "cswrapfunc"); //wrap
			LuaDLL.wLua_wrapfunction(L, -1, f);//wrap,f0
			LuaDLL.lua_remove(L, -2);//f0
        }

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr lua_touserdata(IntPtr luaState, int index);

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr lua_tothread(IntPtr luaState, int index);

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern bool lua_toboolean(IntPtr luaState, int index);

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern double lua_tonumberx(IntPtr luaState, int index, out int isnum);

		public static double lua_tonumber(IntPtr luaState, int index)
		{
			int isnum = 0;
			return lua_tonumberx(luaState, index, out isnum);
		}

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern Int64 lua_tointegerx(IntPtr luaState, int index, out int isnum);
		public static Int64 lua_tointeger(IntPtr luaState, int index)
		{
			int isnum = 0;
			return lua_tointegerx(luaState, index, out isnum);
		}

		//private
		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr lua_tolstring(IntPtr luaState, int index, out long strLen);    //[-0, +0, m]

        public static IntPtr lua_tolstring(IntPtr luaState, int index, out int strLen)      //[-0, +0, m]
        {
            long len = 0;
            IntPtr pData = lua_tolstring(luaState, index, out len);
            strLen = (int)len;
            return pData;
        }

        public static byte[] lua_tolstring(IntPtr luaState, int index)                      //[-0, +0, m]
        {
            int len;
            IntPtr pData = lua_tolstring(luaState, index, out len);
            if (pData == IntPtr.Zero)
                return null;
            byte[] result = new byte[len];
            Marshal.Copy(pData, result, 0, len);
            return result;
        }

        public static string lua_tostring(IntPtr luaState, int index)                       //[-0, +0, m]
        {
            int strlen;

            IntPtr str = lua_tolstring(luaState, index, out strlen);
            if (str != IntPtr.Zero)
            {
				return Marshal.PtrToStringAnsi(str, strlen);
            }
            return null;
        }

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_settop(IntPtr luaState, int newTop);
        public static void lua_pop(IntPtr luaState, int amount)
        {
            LuaDLL.lua_settop(luaState, -(amount) - 1);
        }

		public static int wluaL_error(IntPtr L, Exception e, string msg = null)
		{
			msg = msg == null ? e.Message : msg;
			lua_pushboolean(L, false);
			lua_pushstring(L, msg);
			return 2;
		}

		//index: cswrapfunc index
		//efficiently than wlua_pushcfunction due to not pop wrapfunc
		public static void wLua_wrapfunction(IntPtr L,int index, LuaCSFunction f, bool isOctr = false) 
		{
			LuaDLL.lua_pushvalue(L, index); //cswrapfunc
			LuaDLL.lua_pushcclosure(L, f,0);	//cswrapfunc,f
			if(isOctr)
			{
				LuaDLL.lua_pushboolean(L, true); //cswrapfunc,f,b
			}
			LuaDLL.lua_pcall(L, isOctr ? 2 : 1, 1, 0); //					no error should be returned

		}

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr lua_typename(IntPtr luaState, LuaTypes type);

		public static string luaL_typename(IntPtr luaState, int stackPos)
		{
			LuaTypes tp = LuaDLL.lua_type(luaState, stackPos);
			IntPtr ptr = LuaDLL.lua_typename(luaState, tp);
			string ret = Marshal.PtrToStringAnsi(ptr);
			return ret;
		}


		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void wlua_makecsindex(IntPtr luaState,int index);                //[0,1,m]

		public static bool lua_isnil(IntPtr luaState, int index)
		{
			return (LuaDLL.lua_type(luaState, index) == LuaTypes.LUA_TNIL);
		}
		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern bool lua_isnumber(IntPtr luaState, int index);

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern bool lua_isinteger(IntPtr luaState, int index);

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern bool lua_isnone(IntPtr luaState, int index);

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern bool lua_isnoneornil(IntPtr luaState, int index);

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern bool lua_isstring(IntPtr luaState, int index);

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern bool lua_isuserdata(IntPtr luaState, int index);

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern bool lua_iscfunction(IntPtr luaState, int index);

		public static bool lua_isboolean(IntPtr luaState, int index)
		{
			return LuaDLL.lua_type(luaState, index) == LuaTypes.LUA_TBOOLEAN;
		}

		public static bool lua_istable(IntPtr luaState, int index)
		{
			return LuaDLL.lua_type(luaState, index) == LuaTypes.LUA_TTABLE;
		}
		public static bool lua_isfunction(IntPtr luaState, int index)
		{
			return LuaDLL.lua_type(luaState, index) == LuaTypes.LUA_TFUNCTION;
		}

		public static bool lua_islightuserdata(IntPtr luaState, int index)
		{
			return lua_type(luaState, index) == LuaTypes.LUA_TLIGHTUSERDATA;
		}

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr lua_newuserdata(IntPtr luaState, int size);

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern int luaL_ref(IntPtr luaState, int registryIndex);              //[0,1,m]

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern void luaL_unref(IntPtr luaState, int registryIndex, int reference);

		[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
		public static extern void lua_atpanic(IntPtr luaState, LuaCSFunction panicf);

		public static void luaL_setfuncs(IntPtr luaState, LuaL_Reg[] funcs, int nup)
		{
			LuaDLL.wlua_getglobal(luaState, "cswrapfunc"); //nup,wrap

			for (int i=0;i<funcs.Length;++i)
			{
				LuaDLL.lua_pushvalue(luaState, -1);  //nup,wrap,wrap
				LuaL_Reg reg = funcs[i];
				for (int j = 0;j < nup;++j)
				{
					lua_pushvalue(luaState, -nup);
				} //nup,wrap,wrap,nup
				lua_pushcclosure(luaState, reg.func, nup);//nup,wrap,wrap,f
				LuaDLL.lua_pcall(luaState, 1, 1, 0);  //nup,wrap,f0
				wlua_setfield(luaState, -(nup + 3), reg.name); //nup,wrap
			}
			lua_pop(luaState, nup+1);
		}

		public static void wlua_pushcclosure(IntPtr luaState, LuaCSFunction f,int nup )
		{
			LuaDLL.wlua_getglobal(luaState, "cswrapfunc"); //nup,wrap
			LuaDLL.lua_insert(luaState, -nup - 1);  //wrap,nup
			LuaDLL.lua_pushcclosure(luaState, f, nup); //wrap,f0
			LuaDLL.lua_pcall(luaState, 1, 1, 0); //f
		}

		public static void lua_remove(IntPtr luaState,int index)
		{
			lua_rotate(luaState, index, -1);
			lua_pop(luaState, 1);
		}
	}

}
