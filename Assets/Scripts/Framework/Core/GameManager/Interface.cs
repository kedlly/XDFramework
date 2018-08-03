using UnityEngine;
using Framework.Library.Singleton;
using Framework.Utils.Extensions;
using System.Collections.Generic;
using System.Collections;
using Framework.Core.Runtime;
using System.Linq;

namespace Framework.Core
{

	public interface IManageredObject
	{
		bool TickEnabled {get;}
		bool IsActiving {get;}
		void Tick();
	}
	public interface IManager : IEnumerable<IManageredObject>
	{
		void Register(IManageredObject itf);
		void UnRegister(IManageredObject itf);
		void UnRegisterAll(bool immediately = true);
		void Register(IEnumerable<IManageredObject> items);
		void Tick();
	}


	public abstract class AMananger :  IManager
	{
		HashSet<IManageredObject> registerList = new HashSet<IManageredObject>();
		HashSet<IManageredObject> unregisterList = new HashSet<IManageredObject>();

		public void Register(IManageredObject fc)
		{
			registerList.Add(fc);
		}

		public void UnRegister(IManageredObject fc)
		{
			unregisterList.Add(fc);
		}

		public int ManageredObjectCount { get {return registerList.Count;} }
		public IManageredObject[] ManageredObjects { get { return registerList.ToArray(); } }

		public void Tick()
		{
			foreach (var fc in registerList)
			{
				if(fc.TickEnabled)
				{
					fc.Tick();
				}
				else
				{
					UnRegister(fc);
				}
			}
			if (unregisterList.Count > 0)
			{
				registerList.ExceptWith(unregisterList);
				unregisterList.Clear();
			}
		}
		
		IEnumerator<IManageredObject> IEnumerable<IManageredObject>.GetEnumerator()
		{
			return registerList.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return registerList.GetEnumerator();
		}

		public void UnRegisterAll(bool immediately = true)
		{
			if (immediately)
			{
				registerList.Clear();
			}
			else
			{
				unregisterList.UnionWith(registerList);
			}
		}

		public void Register(IEnumerable<IManageredObject> items)
		{
			registerList.UnionWith(items);
		}
	}
	
}