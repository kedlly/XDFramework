using Framework.Library.Singleton;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using Framework.Utils.Extensions;

namespace Framework.Library.ThreadPool
{

	public class FLThreadPool : ToSingleton<FLThreadPool>
	{
		protected override void OnSingletonInit() { }
		private FLThreadPool()
		{
			group = new ThreadGroup();
		}
		private ThreadGroup group;
		public void AddTask(Action action)
		{
			group.AddTask(action);
		}
		public void DropTasks()
		{
			group.Drop();
		}
	}
	
	internal class ThreadGroup
	{
		private const int DefaultThreadCount = 4;

		public int MaxThreadCount { get; private set; }
		public ThreadPriority ThreadPriority { get; private set; }

		private ThreadGroupWorker[] backgroundThreads { get; set; }

		public ThreadGroup(int backgroundThreadCount = DefaultThreadCount, ThreadPriority threadPriority = ThreadPriority.BelowNormal)
		{
			MaxThreadCount = backgroundThreadCount;
			backgroundThreads = new ThreadGroupWorker[MaxThreadCount];
			ThreadPriority = threadPriority;
		}

		private void TryCreateThreadPoolWorker()
		{
			for (int i = 0; i < MaxThreadCount; ++i)
			{
				var tempIndex = i;
				if (backgroundThreads[tempIndex] == null)
				{
					backgroundThreads[tempIndex] = new ThreadGroupWorker(this);
					backgroundThreads[tempIndex].OnExit += () => { backgroundThreads[tempIndex] = null; };
				}
				else
				{
					if (backgroundThreads[tempIndex].IsSuspend)
					{
						backgroundThreads[tempIndex].Resume();
					}
				}
			}
		}

		public void Drop()
		{
			subThreadTaskPool.Clear();
			for (int i = 0; i < MaxThreadCount; ++i)
			{
				if (backgroundThreads[i] != null)
				{
					backgroundThreads[i].Exit();
				}
			}
			mainThreadTaskPool.Clear();
		}

		private TaskPool<Action> subThreadTaskPool = new TaskPool<Action>();

		public void AddTask(Action action)
		{
			if (action == null)
			{
				return;
			}
			subThreadTaskPool.Push(action);
			TryCreateThreadPoolWorker();
		}

		public void AddTask(params Action[] actions)
		{
			if (actions == null && actions.Length == 0)
			{
				return;
			}
			subThreadTaskPool.Push(actions);
			TryCreateThreadPoolWorker();
		}

		private class TaskPool<T> where T : class
		{
			object locker = new object();
			Queue<T> queue = new Queue<T>(4);
			public void Push(T item)
			{
				if (item != null)
				{
					lock(locker)
					{
						queue.Enqueue(item);
					}
				}
			}

			public void Push(T[] items)
			{
				if (items != null && items.Length > 0)
				{
					lock (locker)
					{
						items.Where(it => it != null).ForEach(it => { queue.Enqueue(it); });
					}
				}
			}

			public void Push(IEnumerable<T> items)
			{
				if (items != null)
				{
					lock(locker)
					{
						items.Where(it => it != null).ForEach(it => { queue.Enqueue(it); });
					}
				}
				
			}

			public T Pop()
			{
				lock (locker)
				{
					return queue.Count> 0 ? queue.Dequeue() : null;
				}
			}

			public void Pop(ref T[] array)
			{
				if (array != null && array.Length > 0)
				{
					lock (locker)
					{
						for (int i =0; i < array.Length; ++i)
						{
							array[i] = queue.Count > 0 ? queue.Dequeue() : null;
						}
					}
				}
			}

			public void Clear()
			{
				lock(locker)
				{
					queue.Clear();
				}
			}
		}


		private TaskPool<Action> mainThreadTaskPool = new TaskPool<Action>();

		public void RunInMainThread(Action action)
		{
			if (action == null)
			{
				return;
			}
			mainThreadTaskPool.Push(action);
		}

		object locker = new object();
		public void RunInMainThreadAndWaitDone(Action action) // not completed
		{
			if (FrameworkUtils.Application.MainThreadId == Thread.CurrentThread.ManagedThreadId)
			{
				action();
			}
			else
			{
				Action actionWrapper = () =>
				{
					lock (locker)
					{
						action();
						Monitor.Pulse(locker);
					}
				};
				RunInMainThread(actionWrapper);
			}
		}

		public void CheckPerFrame()  // main thread check it
		{
			TryRunTaskInMainThread();
		}

		private void TryRunTaskInMainThread()
		{
			Action task = null;
			while ((task = mainThreadTaskPool.Pop()) != null)
			{
				task();
			}
		}


		private Action PopTask()
		{
			return subThreadTaskPool.Pop();
		}

		private class ThreadGroupWorker
		{
			private const int ScannerMicroSeconds = 100;
			private const int CloseWhileIdleTime = ScannerMicroSeconds * 30;
			private Thread theThread;
			public bool IsSuspend { get; private set; }
			public ThreadGroupWorker(ThreadGroup group)
			{
				this.group = group;
				theThread = new Thread(Thread_Work);
				theThread.IsBackground = true;
				theThread.Priority = group.ThreadPriority;
				theThread.Start(this);
				IsSuspend = false;
			}

			public ThreadState ThreadState
			{
				get { return theThread != null ? theThread.ThreadState : ThreadState.Aborted; }
			}

			public event Action OnExit;

			public void Exit()
			{
				if (OnExit != null)
				{
					OnExit();
				}
				theThread.Abort();
				theThread = null;
				group = null;
			}

			public void Resume()
			{
				mre.Set();
			}

			ThreadGroup group = null;
			ManualResetEvent mre = new ManualResetEvent(true);

			static void Thread_Work(object arg)
			{
				ThreadGroupWorker self = arg as ThreadGroupWorker;
				if (self == null)
				{
					return;
				}
				try
				{ 
					Action currentTask = null;
					while (true)
					{
						var isSignal = self.mre.WaitOne(-1);
						if (isSignal)
						{
							currentTask = self.group.PopTask();
							if (currentTask != null)
							{
								self.IsSuspend = false;
								currentTask();
								currentTask = null;
							}
							else
							{
								self.mre.Reset();
								self.IsSuspend = true;
							}
						}
						else
						{
							self.Exit();
						}
					}
				}
				catch (ThreadAbortException ex)
				{
					UnityEngine.Debug.Log("Thread in ThreadGroup with id=" + Thread.CurrentThread.ManagedThreadId + " is Exit.");
					self.mre.Close();
				}
				catch (Exception ex)
				{
					UnityEngine.Debug.LogException(ex);
				}
				UnityEngine.Debug.Log("Thread in ThreadGroup with id=" + Thread.CurrentThread.ManagedThreadId + " is Exit.");
			}
		}
	}

}
