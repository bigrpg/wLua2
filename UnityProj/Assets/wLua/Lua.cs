using System;
using System.IO;

namespace LuaInterface
{
	public sealed class wLua
	{
		public static IntPtr L;
		public static string luaPath;

		internal static LuaState luaState;
		internal static LuaCSFunction printFunction;

		public static string LuaNameSpace = "wlua";


		public static void Init()
		{
			luaState = new LuaState(LuaNameSpace);
			L = luaState.L;
			LuaStateCacheMan.AddLuaStateCache(L);

			printFunction = new LuaCSFunction(LuaFuncs.print);
			LuaDLL.wlua_pushcfunction(L, printFunction);
			LuaDLL.wlua_setglobal(L, "print");

			Lua_Register.Register(L, LuaNameSpace);
			LuaExtend.SetSearcher(L,loader);

		}


		[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
		public static int loader(IntPtr L)
		{
			try
			{
				int top = LuaDLL.lua_gettop(L);
				string fileName = String.Empty;
				fileName = LuaDLL.lua_tostring(L, 1);
				fileName = fileName.Replace('.', '/');
				fileName += ".lua";

				string fullPath = Path.Combine(luaPath, fileName);
				if (File.Exists(fullPath))
				{
					byte[] fileData = StreamingAssetsHelper.ReadAllBytes(fullPath);
					if(LuaDLL.luaL_loadbuffer(L, fileData, fileData.Length, "@" + fileName) != LuaDLL.LUA_OK)
					{
						UnityEngine.Debug.LogWarning("lua2:" + LuaDLL.lua_tostring(L, -1));
						throw new LuaException(L,LuaDLL.lua_tostring(L, -1));
					}

					LuaDLL.lua_pushvalue(L, 1);
					if(!LuaExtend.PCall(L, 1, LuaDLL.LUA_MULTRET))
					{
						throw new LuaException(L,"require failed:" + fileName);  //avoid redundancy error msg log, because PCall has log error msg
					}
					return LuaDLL.lua_gettop(L) - top;
				}
				else
				{
					throw new LuaException(L,"file is not found:"+fileName);
				}
			}
			catch (Exception e)
			{
				return LuaDLL.wluaL_error(L, e);
			}
		}

	}

}