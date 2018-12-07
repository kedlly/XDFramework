using Framework.Library.StateMachine.Template;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Framework.Utils.Extensions;

namespace Framework.Library.StateMachine
{
	public abstract class StateBase
	{
		public abstract string Name { get; }
		public abstract string FullName { get; }
		public StateMachine Owner { get; internal set; }
		internal abstract void BuildTransitions();
	}
	public class State : StateBase // where T : class
	{
		public bool IsReenterable { get { return Template.IsReenterable; } }
		public bool IsConduit { get { return Template.IsConduit; } }
		public string EntryFuncName { get { return Template.EntryFuncName; } }
		public string UpdateFuncName { get { return Template.UpdateFuncName; } }
		public string ExitFuncName { get { return Template.ExitFuncName; } }
		public Action<object> OnEnter { get; internal set; }
		public Action<object> OnExit { get; internal set; }
		public Action<object> OnUpdate { get; internal set; }
		public Transition[] Transitions { get; internal set; }

		public Transition GetFiredTransition(object obj, string firedEvent)
		{
			foreach (var tran in Transitions)
			{
				if ((tran.Event.IsNullOrEmpty() || firedEvent == tran.Event) 
					&& (tran.Condition == null || tran.Condition(obj)))
				{
					return tran;
				}
			}
			return null;
		}
		internal State(StateTemplate template) { Template = template; }
		internal StateTemplate Template { get; set; }

		public override string Name	{get{return Template.Name;	}	}

		public override string FullName { get { return Template.FullName; } }

		internal override void BuildTransitions()
		{
			foreach (var tran in Transitions)
			{
				var ownerFSM = Owner as StateMachine;
				if (ownerFSM != null)
				{
					ownerFSM = ownerFSM.Exit != this ? ownerFSM : ownerFSM.Owner;
					if (ownerFSM != null)
					{
						var item = ownerFSM.GetNamedSubItem(tran.TargetName);
						State target = null;
						if (item != null)
						{
							target = item as State;
							if (target == null)
							{
								var targetFSM = item as StateMachine;
								if (targetFSM != null)
								{
									target = targetFSM.Entry;
								}
							}
						}
						if (target != null && target.Owner.Any == target)
						{
							throw new Exception("Transition Target must be not Statemachine's Any state :" + target.Name);
						}
						tran.SetTarget(target);
					}
				}
			}
		}
	}

	internal static class StateExtension
	{
		internal static State CreateState<T>(this StateTemplate template, StateMachine owner, T correlatedObject = null) where T : class
		{
			State state = new State(template);
			state.Owner = owner;
			List<Transition> trans = new List<Transition>();
			foreach (var tt in state.Template.Transitions)
			{
				var newTransition = tt.CreateTransition<T>();
				trans.Add(newTransition);
			}
			state.Transitions = trans.ToArray();
			state.Binding<T>();
			return state;
		}

		internal static void Binding<T>(this State state) where T : class
		{
			state.OnEnter = TryGetAction<T>(state.Template.EntryFuncName);
			state.OnExit = TryGetAction<T>(state.Template.ExitFuncName);
			state.OnUpdate = TryGetAction<T>(state.Template.UpdateFuncName);
		}

		private static Action<object> TryGetAction<T>(string methodName) where T : class
		{
			return FSMCache.Instance.GetEventAction<T>(methodName);
		}
	}
}