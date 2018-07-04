using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.Core.Runtime;

public class test : MonoBehaviour 
{
	void Hello()
	{
		Debug.LogWarning("Hello ....");
	}

	void World()
	{
		Debug.LogWarning("World ....");
	}

	void End()
	{
		Debug.LogWarning("End ....");
		//GetComponent<TimelineComponent>().TimelineObject.CurrenTime
	}

	// Use this for initialization
	void Start () 
	{
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}
}
