using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.Library.StateMachine;
using Framework.Core.Attributes;
using System;

[AddComponentMenu("Framework/Component/FSM")]
public class FSMComponent : MonoBehaviour 
{
	public TextAsset StateMachineDescription;

	FSMExecutor ism = null;

	[ShowOnly] public string StateName;

	public void LoadFSM<T>(T Owner) where T : Component
	{
		ism = Utils.GetStateMachineExecutor(Owner, StateMachineDescription);
		StateName = ism.CurrentStateName;
	}

	public void PushEvent(string @event)
	{
		if (ism != null)
		{
			ism.PushEvent(@event);
		}
		StateName = ism.CurrentStateName;
	}

	// Update is called once per frame
	void LateUpdate()
	{
		if (ism != null)
		{
			ism.Update();
		}
	}

	private void OnDestroy()
	{
		
	}
}
