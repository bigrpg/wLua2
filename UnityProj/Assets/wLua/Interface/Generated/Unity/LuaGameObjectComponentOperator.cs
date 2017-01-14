using System;
using System.Collections.Generic;
using UnityEngine;

namespace LuaInterface
{
	public partial class LuaGameObjectObject
	{

		delegate UnityEngine.Component  ComponentMethodDelegate(GameObject go);
		static readonly Dictionary<string, ComponentMethodDelegate> typesAdd = new Dictionary<string, ComponentMethodDelegate>();
		static readonly Dictionary<string, ComponentMethodDelegate> typesGet = new Dictionary<string, ComponentMethodDelegate>();

		public static void Init()
		{
			typesAdd.Add("Collider", (GameObject go) => { return go.AddComponent<Collider>(); });
			typesGet.Add("Collider", (GameObject go) => { return go.GetComponent<Collider>(); });
			typesAdd.Add("BoxCollider", (GameObject go) => { return go.AddComponent<BoxCollider>(); });
			typesGet.Add("BoxCollider", (GameObject go) => { return go.GetComponent<BoxCollider>(); });
			typesAdd.Add("MeshRenderer", (GameObject go) => { return go.AddComponent<MeshRenderer>(); });
			typesGet.Add("MeshRenderer", (GameObject go) => { return go.GetComponent<MeshRenderer>(); });
			typesAdd.Add("MeshFilter", (GameObject go) => { return go.AddComponent<MeshFilter>(); });
			typesGet.Add("MeshFilter", (GameObject go) => { return go.GetComponent<MeshFilter>(); });
			typesAdd.Add("Component", (GameObject go) => { return go.AddComponent<Component>(); });
			typesGet.Add("Component", (GameObject go) => { return go.GetComponent<Component>(); });
			typesAdd.Add("Renderer", (GameObject go) => { return go.AddComponent<Renderer>(); });
			typesGet.Add("Renderer", (GameObject go) => { return go.GetComponent<Renderer>(); });

		}
	}
}
