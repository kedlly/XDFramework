
using Framework.Library.StateMachine.Template;
using System;

namespace Framework.Library.StateMachine
{

	public class FSMExecutor
	{
		protected State currentState { get; set; }
		public WeakReference Owner { get; private set; }
		public string CurrentStateName
		{
			get
			{
				if (currentState == null)
				{
					return "";
				}
				return currentState.FullName;
			}
		}

		public void SetTargetObject(object owner)
		{
			if (owner != null)
			{
				Owner = new WeakReference(owner);
				if (Owner.IsAlive && Owner.Target != null)
				{
					if (currentState.OnEnter != null)
					{
						currentState.OnEnter(Owner.Target);
					}
				}
			}
		}

		public void Update()
		{
			if (Owner.IsAlive && Owner.Target != null)
			{
				if (currentState.OnUpdate != null)
				{
					currentState.OnUpdate(Owner.Target);
				}
			}
		}

		public void PushEvent(string firedEvent)
		{
			var OwnerObject = Owner.Target;
			if (OwnerObject == null)
			{
				return;
			}
			var targetTrans = currentState.GetFiredTransition(OwnerObject, firedEvent);
			if (targetTrans == null && currentState.Owner.Any != null)
			{
				targetTrans = currentState.Owner.Any.GetFiredTransition(OwnerObject, firedEvent);
			}
			if (targetTrans != null)
			{
				if (targetTrans.Target == null)
				{
					throw new Exception("Transition : " + targetTrans.Template.Event + "|" + targetTrans.Template.Condition + "|>" + targetTrans.Template.Target + " is not exist.");
				}
				if (currentState.OnExit != null)
				{
					currentState.OnExit.Invoke(OwnerObject);
				}
				currentState = targetTrans.Target;
				if (currentState.OnEnter != null)
				{
					currentState.OnEnter.Invoke(OwnerObject);
				}
				if (currentState.IsConduit)
				{
					PushEvent(null);
				}
			}
		}
	}

	public class FSMExecutor<T> : FSMExecutor where T : class
	{
		internal StateMachine stateMachine { get; private set; }

 		public FSMExecutor(FSMTemplate template)
 		{
			if (stateMachine == null)
			{
				stateMachine = FSMCache.Instance.GetStateMachine<T>(template);
			}

			currentState = stateMachine.Entry as State;
		}

		public FSMExecutor(FSMTemplate template, T target) : this(template)
		{
			SetTargetObject(target);
		}
	}

}
