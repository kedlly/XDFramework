

using Framework.Core.DataStruct;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Utils.Extensions
{
	public class KeyCodeWrapper
	{
		KeyCode _keyCode;
		Action _OnKeyDown;
		Action _OnKeyUp;
		Action _OnKey;
		public void Check()
		{
			_keyCode.OnKeyDown(this._OnKeyDown).OnKey(this._OnKey).OnKeyUp(this._OnKeyUp);
		}
		internal KeyCodeWrapper(KeyCode keyCode)
		{
			_keyCode = keyCode;
		}
		public KeyCodeWrapper OnKeyDown(Action action) { _OnKeyDown = action; return this; }
		public KeyCodeWrapper OnKey(Action action) { _OnKey = action; return this; }
		public KeyCodeWrapper OnKeyUp(Action action) { _OnKeyUp = action; return this; }
	}

	public static class KeyCodeExtension
	{
		public static KeyCodeWrapper MakeWrapper(this KeyCode keyCode)
		{
			return new KeyCodeWrapper(keyCode);
		}
		public static KeyCode OnKeyDown(this KeyCode keyCode, Action action)
		{
			if (Input.GetKeyDown(keyCode) && action != null)
			{
				action();
			}
			return keyCode;
		}
		public static KeyCode OnKeyUp(this KeyCode keyCode, Action action)
		{
			if (Input.GetKeyUp(keyCode) && action != null)
			{
				action();
			}
			return keyCode;
		}

		public static KeyCode OnKey(this KeyCode keyCode, Action action)
		{
			if (Input.GetKey(keyCode) && action != null)
			{
				action();
			}
			return keyCode;
		}
	}

	public abstract class ValueMonitor<T> where T : struct
	{
		public readonly T Base = default(T);
		public T Current = default(T);
		public ValueMonitor(T value) { Current = value; Base = value; }
		public ValueMonitor() { }
		public T Next
		{
			set
			{
				T curDelta = Sub(value, Current);
				Current = value;
				T Domain = Sub(Current, Base);
				Test(curDelta, Domain);
			}
		}

		protected virtual void Test(T delta, T domain)
		{
			if (deltaActionList != null)
			{
				deltaActionList.ForEach(it =>
				{
					bool remove = Check(it, delta);
					it.RemoveTag = remove;
				});
				deltaActionList.RemoveAll(it => it.RemoveTag);
			}
			if (domainActionList != null)
			{
				domainActionList.ForEach(it =>
				{
					bool remove = Check(it, domain);
					it.RemoveTag = remove;
				});
				domainActionList.RemoveAll(it => it.RemoveTag);
			}
			if (stepActionList != null)
			{
				stepActionList.ForEach(it =>
				{
					bool remove = Calc(it, Current);
					it.RemoveTag = remove;
				});
				stepActionList.RemoveAll(it => it.RemoveTag);
			}
		}

		private bool Check(Tuple<T, Action<T>, bool> it, T target)
		{
			if (Lt(it.Item1, Abs(target)) || Eq(it.Item1, Abs(target)))
			{
				it.Item2(target);
				return it.Item3;
			}
			return false;
		}
		
		private bool Calc(CalcItem it, T current)
		{
			if (Lt(it.Item1, Abs(Sub(current, it.LastValue))))
			{
				it.Item2(current);
				it.LastValue = current;
				return it.Item3;
			}
			return false;
		}

		protected abstract T Add(T t1, T t2);
		protected abstract T Sub(T t1, T t2);
		protected abstract bool Gt(T t1, T t2);
		protected abstract bool Lt(T t1, T t2);
		protected abstract bool Eq(T t1, T t2);
		protected abstract T Abs(T t);

		List<CheckableItem> deltaActionList = null;
		List<CheckableItem> domainActionList = null;
		List<CalcItem> stepActionList = null;

		private class CheckableItem : Tuple<T, Action<T>, bool>
		{
			public CheckableItem(T value, Action<T> action, bool autoRemove) : base(value, action, autoRemove) { RemoveTag = false; }
			public bool RemoveTag { get; set; }
		}

		private class CalcItem : CheckableItem
		{
			public CalcItem(T value, Action<T> action, bool autoRemove) : base(value, action, autoRemove) { }

			public T LastValue { get; set; }
		}

		public virtual ValueMonitor<T> OnStep(T step, Action<T> action, bool autoRemove = false)
		{
			if (action != null)
			{
				if (stepActionList == null)
				{
					stepActionList = new List<CalcItem>();
				}
				var stepParams = new CalcItem(Abs(step), action, autoRemove);
				stepParams.LastValue = this.Base;
				stepActionList.Add(stepParams);
			}
			return this;
		}

		public virtual ValueMonitor<T> OnDelta(T delta, Action<T> action, bool autoRemove = false)
		{
			if (action != null)
			{
				if (deltaActionList == null)
				{
					deltaActionList = new List<CheckableItem>();
				}
				deltaActionList.Add(new CheckableItem(Abs(delta), action, autoRemove));
			}
			return this;
		}

		public virtual ValueMonitor<T> OnDomain(T delta, Action<T> action, bool autoRemove = false)
		{
			if (action != null)
			{
				if (domainActionList == null)
				{
					domainActionList = new List<CheckableItem>();
				}
				domainActionList.Add(new CheckableItem(Abs(delta), action, autoRemove));
			}
			return this;
		}
	}

	public class FloatMonitor : ValueMonitor<float>
	{
		public FloatMonitor(float value) : base(value) { }

		protected override float Abs(float t)
		{
			return Mathf.Abs(t);
		}

		protected override float Add(float t1, float t2)
		{
			return t1 + t2;
		}

		protected override bool Eq(float t1, float t2)
		{
			return t1 == t2;
		}

		protected override bool Gt(float t1, float t2)
		{
			return t1 > t2;
		}

		protected override bool Lt(float t1, float t2)
		{
			return t1 < t2;
		}

		protected override float Sub(float t1, float t2)
		{
			return t1 - t2;
		}
	}

	public class IntMonitor : ValueMonitor<int>
	{
		public IntMonitor(int value) : base(value) { }
		protected override int Abs(int t)
		{
			return Mathf.Abs(t);
		}

		protected override int Add(int t1, int t2)
		{
			return t1 + t2;
		}

		protected override bool Eq(int t1, int t2)
		{
			return t1 == t2;
		}

		protected override bool Gt(int t1, int t2)
		{
			return t1 > t2;
		}

		protected override bool Lt(int t1, int t2)
		{
			return t1 < t2;
		}

		protected override int Sub(int t1, int t2)
		{
			return t1 - t2;
		}
	}
	public static class FloatMonitorExtensions
	{
		public static FloatMonitor MakeWrapper(this float value)
		{
			return new FloatMonitor(value);
		}

		public static IntMonitor MakeWrapper(this int value)
		{
			return new IntMonitor(value);
		}
	}
}
