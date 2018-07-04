using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.Client.GameRules;
using Framework.Library.Log;
using System;
using Framework.Library.Singleton;


[GameObjectPathAttribute("/Client//Hello/world/simple/test/one/two/sfs///")]
public class test2 : ToSingletonBehavior<test2>
{
	protected override void OnSingletonInit()
	{
		Debug.LogError("Init>>>>>>>>>>>>>>>");
	}

	private void Awake()
	{
		Debug.LogWarning("<<<<<<<<<<");
	}

	// Use this for initialization
	void Start () {
// 		Debug.Log(LevelLogic.Instance);
// 		LevelLogic.Instance.Dispose();
// 		Debug.Log(LevelLogic.Instance);
// 		LogUtil.SetLogHelper(new ulog());
 		LogUtil.Info("AAAA");
 		LogUtil.Error("BBBB");
 		LogUtil.Warning("CCC");

	}
	
	// Update is called once per frame
	void Update () {
		 
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		Graphics.Blit(source, destination);
	}
}
