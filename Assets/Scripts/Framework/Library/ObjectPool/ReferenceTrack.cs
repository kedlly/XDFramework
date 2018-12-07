
using System.Collections.Generic;


namespace Framework.Library.ObjectPool
{

	public class FastReferenceCountWrapper<T> : IReferenceTrack where T : class
	{
		public T Target { get; private set; }

		public virtual int RetainCount
		{
			get;
			private set;
		}

		private int _RetainCount = 0;

		public FastReferenceCountWrapper(T obj) { Target = obj; }

		public virtual void Retain(object owner)
		{
			RetainCount++;
		}

		public virtual void Release(object owner)
		{
			RetainCount--;
			CheckAndReleaseResource();
		}

		protected void CheckAndReleaseResource()
		{
			if (RetainCount == 0)
			{
				ReleaseResource();
			}
		}
		protected virtual void ReleaseResource() { }
	}
	public class SafeReferenceCountWrapper<T> : FastReferenceCountWrapper<T> where T : class
	{
		public SafeReferenceCountWrapper(T obj) : base(obj) { }

		public override int RetainCount	{ get{	return owners.Count;} }

		HashSet<object> owners = new HashSet<object>();

		public override void Retain(object owner)
		{
			owners.Add(owner);
		}

		public override void Release(object owner)
		{
			owners.Remove(owner);
			CheckAndReleaseResource();
		}
	}

}
