using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.Client.GameRules;
using Framework.Library.Log;
using System;
using Framework.Library.Singleton;
using Framework.Utils.Extensions;

[PathInHierarchyAttribute("/Client//Hello/world/simple/test/one/two/three///"), DisallowMultipleComponent]
public class test1 : ToSingletonBehavior<test1>
{
	public Camera[] cameras;
	protected override void OnSingletonInit()
	{

	}

	private void Awake()
	{
		Debug.LogWarning("<<<<<<<<<<");
	}

	// Use this for initialization
	void Start () {
		Debug.Log(LevelLogic.Instance);
		//LevelLogic.Instance.Dispose();
		//Debug.LogError(LevelLogic.Instance);

		Array.Sort(cameras, delegate(Camera a, Camera b){ return (int)(a.depth - b.depth); } );
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Space))
		{
			var tex2D = cameras.CaptureCameras(new Rect(0, 0, Screen.width, Screen.height));
			byte[] data = tex2D.EncodeToPNG();
			string filename = Application.dataPath + "/Screenshot.png";
			System.IO.File.WriteAllBytes(filename, data);
		}
	}
}
