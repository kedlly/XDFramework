using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.Library.XMLStateMachine;
using Framework.Core.Attributes;
using System;

[AddComponentMenu("Framework/Component/FSM")]
public class FSMComponent : MonoBehaviour 
{
	public TextAsset StateMachineDescription;

	IStateMachine ism = null;

	[ShowOnly] public string StateName;

	private Action FreeMemory;

	public void LoadFSM<T>(T Owner) where T : Component
	{
		var fsm = new StateMachine<T>(Owner, StateMachineDescription);
		ism = fsm;
		StateName = ism.CurrentStateName;
		if (fsm.IsValidated)
		{
			FreeMemory = () =>
			{
				if(StateMachineDescription != null)
				{
					StateMachine<T>.Release(StateMachineDescription);
				}
			};
		}
		
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
		if (FreeMemory != null)
		{
			FreeMemory();
		}
	}
}
