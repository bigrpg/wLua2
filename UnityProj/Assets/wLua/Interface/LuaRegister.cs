using UnityEngine;
using System;

namespace LuaInterface
{
	public enum InitStep
	{
		FIRSTLY = 0,
		FINAL = 1
	}


	public sealed partial class Lua_Register
	{
		public static void Init()
		{
			LuaGameObjectObject.Init();
		}

		public static void Register(IntPtr L,string LuaNameSpace)
		{
			LuaDLL.wlua_getglobal(L, LuaNameSpace);		//unityengine
			LuaDLL.wlua_getglobal(L, "cswrapfunc");		//unityengine,cswrap
			_RegisterEveryThing(L,InitStep.FIRSTLY);
			LuaDLL.lua_pop(L, 1); //unityengine
			_RegisterEveryThing(L,InitStep.FINAL);//unityengine
			LuaDLL.lua_pop(L, 1);//
			GameLogicLua.Register(L);
		}

		static void _RegisterEveryThing(IntPtr L,InitStep step)
		{
			LuaObject.Init(L, step);
			LuaGameObjectObject.Init(L, step);
			LuaCollider.Init(L, step);
			LuaBoxCollider.Init(L, step);
			LuaMeshRenderer.Init(L, step);
			LuaComponent.Init(L, step);
			LuaMeshFilter.Init(L, step);
			LuaRenderer.Init(L, step);
		}


	}


}