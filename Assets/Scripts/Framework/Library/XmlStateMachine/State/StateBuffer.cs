
using System.Collections.Generic;
using System.Xml;
using Framework.Utils.Extensions;
using UnityEngine;

namespace Framework.Library.XMLStateMachine
{
	internal partial class State<T>
	{
		internal class StateBuffer
		{
			private List<State<T>> stateArray = new List<State<T>>();

			State<T> Parent;
			public StateBuffer(State<T> parent)
			{
				Parent = parent;
			}

			public StateBuffer() : this(null) { }

			public State<T>[] GetStates()
			{
				return stateArray.ToArray();
			}

			public int StateCount
			{
				get
				{
					return stateArray.Count;
				}
			}

			public State<T> GetState(string stateName)
			{
				if (stateName.IsNullOrEmpty())
				{
					return null;
				}
				foreach(var so in stateArray)
				{
					if(so.Name == stateName)
					{
						return so;
					}
				}
				var newState = new State<T>(stateName, Parent);
				stateArray.Add(newState);
				return newState;
			}

			public bool LoadState(XmlElement xmlState)
			{
				string stateName = xmlState.GetAttribute(StateConstant.XML_TAG_NAME);
				if(stateName.IsNullOrEmpty())
				{
					Debug.LogError("".AppendFormat("xml parser find state with no state name error in {0}, may be fsm not working in runtime .", "").ToString());
					return false;
				}
				State<T> newState = GetState(stateName);
				newState.Initialize(xmlState);
				newState.LoadTransitionRules(xmlState, this);
				return true;
			}
		}
	}

}