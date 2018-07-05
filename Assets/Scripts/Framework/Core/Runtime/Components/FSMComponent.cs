using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.Library.XMLStateMachine;
using Framework.Core.Attributes;

[AddComponentMenu("Framework/Component/FSM")]
public class FSMComponent : MonoBehaviour 
{
	public TextAsset StateMachineDescription;

	IStateMachine ism = null;

	[ShowOnly] public string StateName;

	public void LoadFSM<T>(T Owner) where T : Component
	{
		ism = new StateMachine<T>(Owner, StateMachineDescription);
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
}
