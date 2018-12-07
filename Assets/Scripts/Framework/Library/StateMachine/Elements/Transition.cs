
using Framework.DataStruct;
using Framework.Library.StateMachine.Template;
using Framework.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Framework.Library.StateMachine
{

	public class Transition
	{
		internal TransitionTemplate Template { get; set; }

		public string Event { get { return Template.Event; } }
		public string TargetName { get { return Template.Target; } }
		public State Target { get; private set; }
		public Func<object, bool> Condition { get; internal set; }

		internal Transition(TransitionTemplate template) { Template = template; }
		public void SetTarget(State target)
		{
			Target = target;
		}
	}

	internal static class TransitionExtensions
	{
		internal static Transition CreateTransition<T>(this TransitionTemplate template) where T : class
		{
			Transition newTransition = new Transition(template);
			newTransition.Condition = newTransition.Binding<T>();
			return newTransition;
		}

		internal static Func<object, bool> Binding<T>(this Transition tran) where T : class
		{
			return FSMCache.Instance.GetTransitionCondition<T>(tran.Template.Condition, tran.Template.Invert, tran.Template.Params);
		}
	}
}