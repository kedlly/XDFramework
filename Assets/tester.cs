using Framework.Utils.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tester : MonoBehaviour 
{
	[SerializeField] GameObject parent;
	[SerializeField] GameObject obj;
	// Use this for initialization
	void Start () 
	{
		
	}

	public void test()
	{
		Debug.Log(obj.GetPath());
		Debug.Log(obj.GetPathToParent(parent));
		Debug.Log(obj == parent.GetSubObject(obj.GetPathToParent(parent)));
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}
}
