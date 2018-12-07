
using Framework.Library.StateMachine.Template;
using Framework.Utils.Extensions;
using System.Collections.Generic;
using System.Xml;

namespace Framework.Library.StateMachine.Parser
{

	internal class FSMConstant
	{
		internal const string XML_TAG_CATEGORY = "FSM";
		internal const string XML_TAG_NAME = "name";
		internal const string XML_TAG_PROPERTY_ENTRY = "entry";
		internal const string XML_TAG_PROPERTY_EXIT = "exit";
		internal const string XML_TAG_PROPERTY_ANY = "any";

		internal const string ParseError_NoName_FMT = "xml parser find state with no state name error in state: {0}, may be fsm not working in runtime .";
	}

	internal class StateConstant
	{
		internal const string XML_TAG_CATEGORY = "state";
		internal const string XML_TAG_CATEGORY_ANY = "stateAny";
		internal const string XML_TAG_NAME = "name";
		internal const string XML_TAG_PROPERTY_IS_REENTERABLE = "reenterable";
		internal const string XML_TAG_EXECUTABLE_BLOCK_ONENTRY = "onEnter";
		internal const string XML_TAG_EXECUTABLE_BLOCK_ONENTRY_NAME = XML_TAG_NAME;
		internal const string XML_TAG_EXECUTABLE_BLOCK_ONEXIT = "onExit";
		internal const string XML_TAG_EXECUTABLE_BLOCK_ONEXIT_NAME = XML_TAG_NAME;
		internal const string XML_TAG_EXECUTABLE_BLOCK_ONUPDATE = "onUpdate";
		internal const string XML_TAG_EXECUTABLE_BLOCK_ONUPDATE_NAME = XML_TAG_NAME;
		internal const string XML_TAG_TAG_KEY = "tag";
		internal const string XML_TAG_CONDUIT_VALUE = "conduit";

		internal const string ParseError_NoName_FMT = "xml parser find state with no state name error in state: {0}, may be fsm not working in runtime .";
	}

	internal partial class TransitionConstant
	{
		internal const string XML_TAG_CATEGORY = "transition";
		internal const string XML_TAG_EVENT = "event";
		internal const string XML_TAG_TARGET_STATE_ID = "target";
		internal const string XML_TAG_DYNAMIC_MEMBER = "condition";
		internal const string XML_TAG_DYNAMIC_MEMBER_PARAM_PREFIX = "param";
	}

	public static class FSMXMLParser
	{
		private const string XML_TAG_NAME = "scxml";

		public static FSMTemplate LoadFromXML(string xml)
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
			XmlElement scm = doc.FirstChild as XmlElement;
			FSMTemplate target = null;
			if (scm != null && scm.Name == XML_TAG_NAME)
			{
				XmlElement firstFsm = scm.FirstChild as XmlElement;
				if (firstFsm.Name == FSMConstant.XML_TAG_CATEGORY)
				{
					target = CreateFsmFromXmlElement(firstFsm, null);
				}
			}
			return target;
		}

		private static bool TryGetProperty_Bool(XmlElement xmlState, string propertyTag, bool defaultValue = false)
		{
			string data = xmlState.GetAttribute(propertyTag);
			bool result = defaultValue;
			if (data.IsNotNullAndEmpty())
			{
				if (bool.TryParse(data, out result))
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
			return data.IsNotNullAndEmpty() ? data : defaultValue;
		}

		private static TransitionTemplate CreateTransitionTemplateFromXmlElement(XmlElement transitionXml)
		{
			TransitionTemplate newTrans = null;
			if (transitionXml != null && transitionXml.Name == TransitionConstant.XML_TAG_CATEGORY)
			{
				string eventString = TryGetProperty_String(transitionXml, TransitionConstant.XML_TAG_EVENT);
				string condition = TryGetProperty_String(transitionXml, TransitionConstant.XML_TAG_DYNAMIC_MEMBER);
				string target = TryGetProperty_String(transitionXml, TransitionConstant.XML_TAG_TARGET_STATE_ID);
				List<string> stringParams = new List<string>();
				int count = 1;
				while (true)
				{
					string key = transitionXml.GetAttribute(TransitionConstant.XML_TAG_DYNAMIC_MEMBER_PARAM_PREFIX + count);
					if (key.IsNotNullAndEmpty())
					{
						stringParams.Add(key.Trim());
						count++;
					}
					else
					{
						break;
					}
				}
				bool isConditionInvert = false;
				if (condition.IsNotNullAndEmpty() && condition.StartsWith("!"))
				{
					condition = condition.Substring(1);
					isConditionInvert = true;
				}
				newTrans = new TransitionTemplate(eventString, target, condition, isConditionInvert, stringParams.ToArray());
			}
			return newTrans;
		}

		private static FSMTemplate CreateFsmFromXmlElement(XmlElement fsmNode, FSMTemplate parent)
		{
			var name = TryGetProperty_String(fsmNode, FSMConstant.XML_TAG_NAME, "");
			if (name.IsNullOrEmpty())
			{
				throw new System.Exception("[xml] FSM Node must have a \"name\" property in node {" + fsmNode.Name + ":" + name + "}");
			}
			FSMTemplate newTemplate = new FSMTemplate(name, parent);
			newTemplate.AnyState = TryGetProperty_String(fsmNode, FSMConstant.XML_TAG_PROPERTY_ANY);
			newTemplate.EntryState = TryGetProperty_String(fsmNode, FSMConstant.XML_TAG_PROPERTY_ENTRY);
			newTemplate.ExitState = TryGetProperty_String(fsmNode, FSMConstant.XML_TAG_PROPERTY_EXIT);
			List<FSMNodeTemplate> subStates = new List<FSMNodeTemplate>();
			foreach (XmlNode node in fsmNode.ChildNodes)
			{
				XmlElement item = node as XmlElement;
				if (item == null)
				{
					continue;
				}
				if (item.Name == StateConstant.XML_TAG_CATEGORY)
				{
					var stateName = TryGetProperty_String(item, StateConstant.XML_TAG_NAME, "");
					if (stateName.IsNullOrEmpty())
					{
						throw new System.Exception("[xml] State Node must have a \"name\" property in node {" + fsmNode.Name + ":" + name + "}");
					}
					
					List<TransitionTemplate> trans = new List<TransitionTemplate>();
					foreach(var stateChild in item.ChildNodes)
					{
						XmlElement transitionXml = stateChild as XmlElement;
						if (transitionXml != null)
						{
							var transition = CreateTransitionTemplateFromXmlElement(transitionXml);
							if (transition != null)
							{
								trans.Add(transition);
							}
						}
					}
					var onEntry = TryGetProperty_String(item, StateConstant.XML_TAG_EXECUTABLE_BLOCK_ONENTRY);
					var onExit = TryGetProperty_String(item, StateConstant.XML_TAG_EXECUTABLE_BLOCK_ONEXIT);
					var onUpdate = TryGetProperty_String(item, StateConstant.XML_TAG_EXECUTABLE_BLOCK_ONUPDATE);
					var isReenterable = TryGetProperty_Bool(item, StateConstant.XML_TAG_PROPERTY_IS_REENTERABLE);
					var newState = new StateTemplate(stateName, newTemplate, isReenterable)
										.SetTransitions(trans.ToArray())
										.SetFunc(onEntry, onUpdate, onExit);
					subStates.Add(newState);
				}
				else if (item.Name == FSMConstant.XML_TAG_CATEGORY)
				{
					var subFSM = CreateFsmFromXmlElement(item, newTemplate);
					subStates.Add(subFSM);
				}
				else
				{
					continue;
				}
			}
			newTemplate.SubStates = subStates.ToArray();
			return newTemplate;
		}
	}
}