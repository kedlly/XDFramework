using Projects.Course;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using Projects.DataStruct.Courseware;
using UnityEngine;
using Framework.Utils.Extensions;

namespace Projects
{
	class TestEditor
	{
		

		[MenuItem("Tool/hello")]
		static void test()
		{
			var obj = Selection.activeGameObject;
			/*var f = obj.GetComponentCollectionFullPath<MeshRenderer>();
			foreach (var v in f)
			{
				//Debug.Log(v.Key);
				//Debug.Log(GameObject.Find(v.Key)  == v.Value.gameObject);
			}*/
			//Debug.Log(GameObject.Find("A/GameObject") == GameObject.Find("/A/GameObject"));
			//var o = GameObject.Find("A/GameObject").SubObject( "Hello/world/GameObject");
			//Debug.Log(o);

			//var o = new GameObject("0");
			//obj.GetComponent<tester>().test();
			Debug.Log(obj.transform.root);

		}

		[MenuItem("Tool/null create")]
		static void test2()
		{

			GameObject u = null;
			var a = u.AddSubObject("");
			var b = u.AddSubObject("A/B/C/D");
			var c = u.AddSubObject("A/B/C/E");
			var d = u.AddSubObject("/A/B/C/E");
			var e = u.AddSubObject("A/B//C//E");
			Debug.Log(a);
			Debug.Log(b);
			Debug.Log(c);
			Debug.Log(d);
			Debug.Log(e);
			Debug.Log(d == e);
			Debug.Log(d == u.GetSubObject("A/B//C//E"));
			Debug.Log(u.GetSubObject("A/B//C//E/f"));
			Debug.Log(u.GetSubObject("A/B//C").GetSubObject("E").AddSubObject("S/F").transform.root);
			Debug.Log(u.GetPath());
			
		}
	}
}
