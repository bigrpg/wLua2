using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace LuaInterface
{
	public partial class LuaBoxCollider
	{
	
		static void _addMemberFunction(IntPtr L)
		{

		}

		static void _InitLink(IntPtr L)
		{
			Lua_Register._SetParent(L,"BoxCollider", "Collider");
		}


		public static void Init(IntPtr L, InitStep step)
		{
			if (step == InitStep.FIRSTLY)
			{
				LuaDLL.lua_newtable(L);//unityengine,cswrap,t
				_addMemberFunction(L);//unityengine,cswrap,t
				Lua_Register._SetTypeTable2Namespace(L, "BoxCollider"); //unityengine,cswrap

			}
			else if (step == InitStep.FINAL)
			{
				_InitLink(L);
			}
		}

	}

} //namespace
