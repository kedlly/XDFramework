using Framework.Library.StateMachine.Template;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Framework.Utils.Extensions;

namespace Framework.Library.StateMachine
{
	public class StateMachine : StateBase // where T : class
	{
		internal FSMTemplate Template { get; set; }
		public StateBase[] SubStates { get; internal set; }
		public State Entry { get; internal set; }
		public State Exit { get; internal set; } 
		public State Any { get; internal set; }

		public override string Name { get { return Template.Name; } }

		public override string FullName { get { return Template.FullName; } }
		internal StateMachine(FSMTemplate template) { Template = template; }
		internal override void BuildTransitions()
		{
			foreach(var sb in SubStates)
			{
				sb.BuildTransitions();
			}
		}

		internal T GetNamedSubItem<T>(string name) where T : StateBase
		{
			return GetNamedSubItem(name) as T;
		}

		internal StateBase GetNamedSubItem(string name)
		{
			if (name.IsNotNullAndEmpty())
			{
				foreach (var sb in SubStates)
				{
					if (sb.Name == name)
					{
						return sb;
					}
				}
			}
			return null;
		}
	}

	public static class FSMTemplateExtension
	{
		private static StateMachine internalCreateFSM<T>(
				this FSMTemplate template
				, StateMachine Owner = null
			) where T : class
		{
			StateMachine newFSM = new StateMachine(template);
			newFSM.Owner = Owner;
			List<StateBase> subStates = new List<StateBase>();
			foreach (var node in template.SubStates)
			{
				FSMTemplate fsm = node as FSMTemplate;
				if (fsm != null)
				{
					subStates.Add(fsm.internalCreateFSM<T>(newFSM));
				}
				StateTemplate st = node as StateTemplate;
				if (st != null)
				{
					subStates.Add(st.CreateState<T>(newFSM));
				}
			}
			newFSM.SubStates = subStates.ToArray();
			newFSM.Entry = newFSM.GetNamedSubItem<State>(template.EntryState);
			if (newFSM.Entry == null)
			{
				throw new Exception("state machine must be add a \"entry\" point in FSM path:" + newFSM.FullName);
			}
			newFSM.Exit = newFSM.GetNamedSubItem<State>(template.ExitState);
			newFSM.Any = newFSM.GetNamedSubItem<State>(template.AnyState);
			return newFSM;
		}

		public static StateMachine Create<T>(this FSMTemplate template, StateMachine upLevelSM = null) where T : class
		{
			var newFSM = template.internalCreateFSM<T>(upLevelSM);
			newFSM.BuildTransitions();
			return newFSM;
		}
	}
}