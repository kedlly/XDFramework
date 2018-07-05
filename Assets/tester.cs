using Framework.Utils.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.Library.XMLStateMachine;
using System;

public class tester : MonoBehaviour 
{
	[SerializeField] GameObject parent;
	[SerializeField] GameObject obj;
	public GameObject target;


	private void Awake()
	{
	}

	void instance()
	{
		GameObject.Instantiate(target);
	}

	private void output()
	{
		Debug.Log(FSMExecutorFactory<NewBehaviourScript2>.RefCount("test3"));
	}

	private void clean()
	{
		Debug.Log("clean");
		//Resources.UnloadUnusedAssets();
		//FSMExecutorFactory<NewBehaviourScript2>.RefClean();
	}

	private void gc()
	{
		System.GC.Collect();
	}

}
