
namespace Framework.Library.XMLStateMachine
{

	public interface IStateMachine
	{
		string CurrentStateName { get; }
		void SetTargetObject(object owner);

		void Update();

		void PushEvent(string firedEvent);
	}

	public sealed class StateMachine<T> : IStateMachine where T : class
	{
		FSMExecutor<T> executor = null;
		public StateMachine()
		{

		}

		public StateMachine(string name) : this ()
		{
			executor = FSMExecutorFactory<T>.CreateExecutorInPreloads(name);
		}

		public StateMachine(T owner, string name) : this(name)
		{
			(this as IStateMachine).SetTargetObject(owner);
		}

		string IStateMachine.CurrentStateName
		{
			get
			{
				return executor.CurrentStateName;
			}
		}

		void IStateMachine.PushEvent(string firedEvent)
		{
			if(executor != null)
			{
				executor.PushEvent(firedEvent);
			}
		}

		void IStateMachine.SetTargetObject(object owner)
		{
			if (executor != null)
			{
				executor.SetTargetObject(owner as T);
			}
		}

		void IStateMachine.Update()
		{
			if(executor != null)
			{
				executor.Update();
			}
		}
	}
}
