
using UnityEngine;

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
		private FSMExecutor<T> executor = null;

		public string MapName { get; private set; }

		public StateMachine()
		{

		}

		public bool IsValidated { get { return executor != null; } }

		public StateMachine(string name) : this ()
		{
			executor = FSMExecutorFactory<T>.CreateExecutorInPreloads(name);
			MapName = name;
		}

		public static void PreLoadConfigure(string name, string xml)
		{
			FSMExecutorFactory<T>.Preload(name, xml);
		}

		public static void Release(string name)
		{
			FSMExecutorFactory<T>.ClearStateMap(name);
		}

		public static bool ContainsStateMap(string name)
		{
			return FSMExecutorFactory<T>.Contains(name);
		}

		public static void Release(TextAsset textAsset)
		{
			if (textAsset != null)
			{
				FSMExecutorFactory<T>.ClearStateMap(textAsset.name);
			}
			FSMExecutorFactory<T>.ClearMemory();
		}

		public StateMachine(T owner, TextAsset textAsset)
		{
			if (textAsset != null)
			{
				if (!FSMExecutorFactory<T>.Contains(textAsset.name))
				{
					PreLoadConfigure(textAsset.name, textAsset.text);
				}
				executor = FSMExecutorFactory<T>.CreateExecutorInPreloads(textAsset.name);
				MapName = textAsset.name;
				(this as IStateMachine).SetTargetObject(owner);
			}
		}

		public StateMachine(T owner, string name) : this(name)
		{
			(this as IStateMachine).SetTargetObject(owner);
		}

		string IStateMachine.CurrentStateName
		{
			get
			{
				return executor != null ? executor.CurrentStateName : "";
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
