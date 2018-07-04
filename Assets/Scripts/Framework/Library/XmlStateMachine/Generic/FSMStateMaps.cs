
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;

namespace Framework.Library.XMLStateMachine
{

	internal class StateMap<T>
	{
		public State<T> Entry { get; private set; }
		public State<T>[] StateSet { get; private set; }
		public State<T> Exit { get; private set; }
		public StateMap(State<T>[] states)
		{
			StateSet = states;
			var entryState = this.StateSet.FirstOrDefault(k => k.IsEntry);
			Entry = entryState != null ? entryState.AbsoluteRuleState : null;
			var exitState = this.StateSet.FirstOrDefault(k => k.IsExit);
			Exit = exitState != null && exitState.Parent != null ? exitState.Parent.AbsoluteRuleState : null;
		}
	}

	public static class FSMExecutorFactory<T> where T : class
	{
		internal const string XML_TAG_NAME = "scxml";

		internal static StateMap<T> LoadFromXML(string xml)
		{
			XmlDocument doc = new XmlDocument();
			try
			{
				doc.LoadXml(xml);
			}
			catch
			{
				throw;
			}
			StateMap<T> stateMap = null;
			XmlElement scm = doc.FirstChild as XmlElement;
			if(scm != null && scm.Name == XML_TAG_NAME)
			{
				var fsm_stateBuffer = new State<T>.StateBuffer();
				foreach(XmlElement xmlState in scm.ChildNodes)
				{
					if(xmlState.Name == StateConstant.XML_TAG_CATEGORY)
					{
						if(!fsm_stateBuffer.LoadState(xmlState))
						{

						}

					}
				}
				stateMap = new StateMap<T>(fsm_stateBuffer.GetStates());
			}
			return stateMap;
		}

		private static Dictionary<string, StateMap<T>> dictionary = new Dictionary<string, StateMap<T>>();

		public static void Preload(string name, string xml)
		{
			if (dictionary.ContainsKey(name))
			{
				Debug.LogWarning(string.Format("name : {0} is already in dictionary", name));
				return;
			}
			dictionary[name] = LoadFromXML(xml);
		}

		public static FSMExecutor<T> CreateExecutorInPreloads(string name)
		{
			if(!dictionary.ContainsKey(name))
			{
				Debug.LogError(string.Format("name : {0} is not in dictionary, call Preload and try again !", name));
				return null;
			}
			return new FSMExecutor<T>(dictionary[name]);
		}

		public static void DropStateMap(string name)
		{
			if (dictionary.ContainsKey(name))
			{
				dictionary.Remove(name);
			}
		}

		public static void ClearStateMap(string name)
		{
			dictionary.Clear();
		}

		public static bool Contains(string name)
		{
			return dictionary.ContainsKey(name);
		}
	}
}