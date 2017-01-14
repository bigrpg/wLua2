using UnityEngine;
using System.Collections;
using LuaInterface;
using System.IO;

public class Example : MonoBehaviour {

	// Use this for initialization
	void Start () {

        //
		Lua_Register.Init();

		wLua.Init();
		wLua.luaPath = Path.Combine(UnityEngine.Application.streamingAssetsPath, "Lua");
		Debug.Log("LuaPath is:" + wLua.luaPath);

		System.IntPtr L = wLua.L;
		if (!LuaExtend.DoString(L, " require 'mytest'"))
		{
			//UnityEngine.Debug.LogWarning("lua2:" + LuaDLL.lua_tostring(L, -1));
			//throw new LuaException(LuaDLL.lua_tostring(L, -1));
		}

		LuaDLL.wlua_getglobal(L, "func2");
		LuaExtend.PCall(L, 0, 0);

		LuaDLL.wlua_getglobal(L, "mytable");
		if (LuaDLL.wlua_len(L, -1) == LuaDLL.LUA_OK)
		{
			int len = (int)LuaDLL.lua_tointeger(L, -1);
			Debug.Log("xxx:" + len);
		}
		LuaDLL.lua_pop(L, 2);

	}

	// Update is called once per frame
	void Update () {
	
	}
}
