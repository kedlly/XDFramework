
using System.Collections.Generic;
using Framework.Utils.Extensions;
using System.Linq;


namespace Framework.Library.StateMachine.Template
{
	public class FSMNodeTemplate
	{
		protected const char STATE_NAME_SPLIT_CHAR = '.';
		public FSMTemplate Parent { get; protected set; }
		public string Name { get; protected set; }
		public string FullName { get; protected set; }

		public FSMNodeTemplate(string name, FSMTemplate parent = null)
		{
			this.Name = name;
			this.Parent = parent;
			this.FullName = Parent == null ? Name : Name.AddPrefix(Parent.FullName + ".");
		}
	}

	public class FSMTemplate : FSMNodeTemplate
	{
		public FSMTemplate(string name, FSMTemplate parent) : base(name, parent) { }
		public string EntryState { get; set; }
		public string ExitState { get; set; }
		public string AnyState { get; set; }
		public FSMNodeTemplate[] SubStates { get; set; }
	}

	internal class StateTemplate : FSMNodeTemplate
	{
		public bool IsReenterable { get; private set; }
		public bool IsConduit { get; private set; }
		public string EntryFuncName { get; private set; }
		public string UpdateFuncName { get; private set; }
		public string ExitFuncName { get; private set; }
		public TransitionTemplate[] Transitions { get { return transitions; } }

		public StateTemplate(string name, FSMTemplate parent, bool reenterable = false) : base(name, parent)
		{
			IsReenterable = reenterable;
		}

		private TransitionTemplate[] transitions;
		public StateTemplate SetTransitions(TransitionTemplate[] transitions)
		{
			IsConduit = transitions.Any(it => it.Event.IsNullOrEmpty());
			if (IsConduit)
			{
				this.transitions = transitions.Where(it => it.Event.IsNullOrEmpty()).ToArray();
			}
			else
			{
				this.transitions = transitions;
			}
			return this;
		}

		public StateTemplate SetFunc(string onEntry, string onUpdate, string onExit)
		{
			EntryFuncName = onEntry;
			UpdateFuncName = onUpdate;
			ExitFuncName = onExit;
			return this;
		}
	}

	internal class TransitionTemplate
	{
		public string Event { get; private set; }

		public string Condition { get; private set; }

		public string[] Params { get; private set; }

		public bool Invert { get; private set; }
		public string Target { get; private set; }
		public TransitionTemplate(string strEvent, string target, string condition, bool isConditionInvert, string[] stringParams)
		{
			Event = strEvent;
			Condition = condition;
			Params = stringParams;
			Target = target;
			Invert = isConditionInvert;
		}
	}

}