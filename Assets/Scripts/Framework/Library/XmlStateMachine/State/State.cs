
using System.Collections.Generic;
using System.Xml;
using System.Linq;
using System.Reflection;
using Framework.Utils.Extensions;
using UnityEngine;
using Framework.Library.XMLStateMachine.Transition;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace Framework.Library.XMLStateMachine
{
	internal partial class State<T>
	{
		public TransitionRule<T>[] Transitions { get; private set; }

		public StateMap<T> SubStateMap { get; private set; }

		public string Name { get; private set; }

		public State<T> Parent { get; private set; }

		public bool IsEntry { get; private set; }
		public bool IsExit { get; private set; }

		public bool IsReenterable { get; private set; }


		public bool HasEnterPoint { get { return DynamicEnterFunc != null; } }

		public bool HasUpdatePoint { get { return DynamicUpdateFunc != null; } }

		public bool HasExitPoint { get { return DynamicExitFunc != null; } }

		public bool IsConduit {	get; private set;}

		public MethodInfo DynamicEnterFunc { get; private set; }
		public MethodInfo DynamicUpdateFunc { get; private set; }
		public MethodInfo DynamicExitFunc { get; private set; }

		public State<T> AbsoluteRuleState { get; private set; }


		public State<T> ParentAbsoluteRuleState
		{
			get
			{
				return Parent != null ? Parent.AbsoluteRuleState : null;
			}
		}

		public State(string name, State<T> parent)
		{
			this.Name = name;
			IsEntry = false;
			IsExit = false;
			Parent = parent;
			FullName = GetFullName();
			DynamicExitFunc = DynamicUpdateFunc = DynamicEnterFunc = null;
		}

		public void SetTransitions(TransitionRule<T>[] trans)
		{
			Transitions = trans;
			var absTrans = trans.FirstOrDefault(it => it.Style == TransitionRuleStyle.Absoulte);
			AbsoluteRuleState = absTrans != null ? absTrans.Target : null;
			IsConduit = Transitions.Length > 0 && Transitions.All(it => it.Style == TransitionRuleStyle.Conditional);
		}

		[Conditional("DEBUG")]
		private static void TestEntryExitAllExit(XmlElement xml)
		{
			if(FindProperty(xml, StateConstant.XML_TAG_PROPERTY_IS_ENTRY) && FindProperty(xml, StateConstant.XML_TAG_PROPERTY_IS_EXIT))
			{
				Debug.LogError(string.Format("{0} and {1} cannot set both.", StateConstant.XML_TAG_PROPERTY_IS_ENTRY, StateConstant.XML_TAG_PROPERTY_IS_EXIT));
			}
		}

		public void Initialize(XmlElement xmlState)
		{
			TestEntryExitAllExit(xmlState);
			IsEntry = TryGetProperty_Bool(xmlState, StateConstant.XML_TAG_PROPERTY_IS_ENTRY);
			IsExit = TryGetProperty_Bool(xmlState, StateConstant.XML_TAG_PROPERTY_IS_EXIT);
			IsReenterable = IsExit || IsEntry ? false : TryGetProperty_Bool(xmlState, StateConstant.XML_TAG_PROPERTY_IS_REENTERABLE, true);
			LoadSubStates(xmlState, this);
			LoadDynamicExecutor(xmlState);
		}

		public string FullName { get; private set; }

		private string GetFullName()
		{
			if(Parent == null)
			{
				return Name;
			}
			return Name.AddPrefix(Parent.FullName + ".");
		}

		public void LoadSubStates(XmlElement xmlState, State<T> currentState)
		{
			var fsm_stateBuffer = new StateBuffer(currentState);

			foreach(XmlNode xmlNode in xmlState.ChildNodes)
			{
				var xmlStateElements = xmlNode as XmlElement;
				if (xmlStateElements == null)
				{
					continue;
				}
				//XmlComment a = xmlNode as XmlComment;

				if(xmlStateElements.Name == StateConstant.XML_TAG_CATEGORY) // is a sub state description
				{
					string stateName = xmlStateElements.GetAttribute(StateConstant.XML_TAG_NAME);
					if(stateName.IsNullOrEmpty())
					{
						Debug.LogError("".AppendFormat(StateConstant.ParseError_NoName_FMT, Parent.FullName).ToString());
						continue;
					}
					fsm_stateBuffer.LoadState(xmlStateElements);
				}
			}
			SubStateMap = fsm_stateBuffer.StateCount != 0 ? new StateMap<T>(fsm_stateBuffer.GetStates()) : null;
		}

		private static bool TryGetProperty_Bool(XmlElement xmlState, string propertyTag, bool defaultValue = false)
		{
			string data = xmlState.GetAttribute(propertyTag);
			bool result = defaultValue;
			if(data.IsNotNullAndEmpty())
			{
				if(bool.TryParse(data, out result))
				{
				}
			}
			return result;
		}

		private static bool FindProperty(XmlElement xmlState, string propertyTag)
		{
			string data = xmlState.GetAttribute(propertyTag);
			return data.IsTrimNotNullAndEmpty();
		}

		private static string TryGetProperty_String(XmlElement xmlState, string propertyTag, string defaultValue = "")
		{
			string data = xmlState.GetAttribute(propertyTag);
			return data.IsNotNullAndEmpty() ? data : defaultValue ;
		}

		public void LoadDynamicExecutor(XmlElement xmlState)
		{
			foreach(XmlNode xmlNode in xmlState.ChildNodes)
			{
				var xmlStateElements = xmlNode as XmlElement;
				if(xmlStateElements == null)
				{
					continue;
				}
				if(xmlStateElements.Name == StateConstant.XML_TAG_EXECUTABLE_BLOCK_ONENTRY) // onEntry
				{
					string entryMethodName = TryGetProperty_String(xmlStateElements, StateConstant.XML_TAG_EXECUTABLE_BLOCK_ONENTRY_NAME);
					if (entryMethodName.IsNotNullAndEmpty())
					{
						DynamicEnterFunc = typeof(T).GetMethod(entryMethodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					}
				}
				else if(xmlStateElements.Name == StateConstant.XML_TAG_EXECUTABLE_BLOCK_ONEXIT) //onExit
				{
					string exitMethodName = TryGetProperty_String(xmlStateElements, StateConstant.XML_TAG_EXECUTABLE_BLOCK_ONEXIT_NAME);
					if(exitMethodName.IsNotNullAndEmpty())
					{
						DynamicExitFunc = typeof(T).GetMethod(exitMethodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					}
				}
				else if(xmlStateElements.Name == StateConstant.XML_TAG_EXECUTABLE_BLOCK_ONUPDATE) //onUpdate
				{
					string updateMethodName = TryGetProperty_String(xmlStateElements, StateConstant.XML_TAG_EXECUTABLE_BLOCK_ONUPDATE_NAME);
					if(updateMethodName.IsNotNullAndEmpty())
					{
						DynamicUpdateFunc = typeof(T).GetMethod(updateMethodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					}
				}
				else
				{

				}
			}
		}

		[Conditional("DEBUG")]
		private static void TestTransitionRuleTarget(XmlElement xml)
		{
			if (!FindProperty(xml, TransitionRuleConstant.XML_TAG_TARGET_STATE_ID))
			{
				Debug.LogError("transition must be set a target.");
			}
		}

		public void LoadTransitionRules(XmlElement xmlState, StateBuffer fsm_stateBuffer)
		{
			var newStateTrans = new List<TransitionRule<T>>();
			foreach(XmlNode xmlNode in xmlState.ChildNodes)
			{
				var xmlStateElements = xmlNode as XmlElement;
				if(xmlStateElements == null)
				{
					continue;
				}
				if(xmlStateElements.Name == TransitionRuleConstant.XML_TAG_NAME) // transitions
				{
					string eventName = xmlStateElements.GetAttribute(TransitionRuleConstant.XML_TAG_EVENT);
					TestTransitionRuleTarget(xmlStateElements);
					string targetStateId = xmlStateElements.GetAttribute(TransitionRuleConstant.XML_TAG_TARGET_STATE_ID);
					string condition = xmlStateElements.GetAttribute(TransitionRuleConstant.XML_TAG_DYNAMIC_MEMBER);
					List<string> param = new List<string>();
					int count = 1;
					string key = xmlStateElements.GetAttribute(TransitionRuleConstant.XML_TAG_DYNAMIC_MEMBER_PARAM_PREFIX + count);
					while (key.IsNotNullAndEmpty())
					{
						param.Add(key.Trim());
						count++;
						key = xmlStateElements.GetAttribute(TransitionRuleConstant.XML_TAG_DYNAMIC_MEMBER_PARAM_PREFIX + count);
					}
					var newTransObj = new TransitionRule<T>(this
															, eventName
															, condition
															, fsm_stateBuffer.GetState(targetStateId));
					if (param.Count > 0)
					{
						newTransObj.SetDynamicParams(param.ToArray());
					}
					newStateTrans.Add(newTransObj);
				}
			}
			SetTransitions(newStateTrans.ToArray());
		}

		public static readonly object[] ParamDescNoArgs = new object[] { };

		internal void onExit(T target)
		{
			Debug.Log(string.Format("Exit : {0}", this.FullName));
			if (HasExitPoint && target != null)
			{
				DynamicExitFunc.Invoke(target, ParamDescNoArgs);
			}
		}

		internal void onEnter(T target)
		{
			Debug.Log(string.Format("Enter : {0}", this.FullName));
			if (HasEnterPoint && target != null)
			{
				DynamicEnterFunc.Invoke(target, ParamDescNoArgs);
			}
		}

		internal void onUpdate(T target)
		{
			if (HasUpdatePoint && target != null)
			{
				DynamicUpdateFunc.Invoke(target, ParamDescNoArgs);
			}
		}

		internal TransitionRule<T> GetFiredTransition(T target, string firedEvent)
		{
			TransitionRule<T> result = null;
			if (!IsExit)
			{
				if(target != null && Transitions != null && Transitions.Length > 0)
				{
					foreach(var tran in Transitions)
					{
						if(tran.Event == firedEvent && tran.DynamicConditionInvoke(target))
						{
							result = tran;
							break;
						}
					}
				}
			}
			return result;
		}

	}

}