using UnityEngine;
using Framework.Library.Singleton;
using Framework.Utils.Extensions;
using System.Collections.Generic;
using Framework.Core.Attributes;
using System;

namespace Framework.Core
{

	[GameObjectPath("/[Game]/GameManager"), DisallowMultipleComponent]
	public sealed class GameManager : ToSingletonBehavior<GameManager>
	{
		protected override void OnSingletonInit()
		{
			base.OnSingletonInit();
		}

		public void Initalize()
		{
			AssetsManager.Instance.Initalize();
			PoolsManager.Instance.Initalize();
			var managers = System.AppDomain.CurrentDomain.GetAllTypesImplementsInterface<IManager>();
			foreach (var mt in managers)
			{
				if (mt.IsAbstract)
				{
					continue;
				}
				var mtaTarget = getFirstMTA(mt);
				if (mtaTarget != null && mtaTarget.AutoRegister)
				{
					AddSubManager(mt);
				}
			}
		}

		Dictionary<string, IManager> objMangers = new Dictionary<string, IManager>();

		void Update()
		{
			foreach (var manager in objMangers.Values)
			{
				manager.Tick();
			}
		}

		public IManager GetSubManager(string name)
		{
			if (objMangers.ContainsKey(name))
			{
				return objMangers[name];
			}
			return null;
		}

		public void AddSubManager<T>() where T : IManager, new()
		{
			this.AddSubManager(typeof(T));
		}
		
		private ManagerTemplateAttribute getFirstMTA(Type managerType)
		{
			ManagerTemplateAttribute[] mtas = managerType.GetCustomAttributes<ManagerTemplateAttribute>();
			if (mtas == null || mtas.Length == 0)
			{
				Debug.LogWarning("Manager type: {0} has No ManagerTemplateAttribute".FormatEx( managerType.FullName));
				return null;
			}
			if (mtas.Length > 1)
			{
				Debug.LogWarningFormat("Manager Type: {0} has more then one ManagerTemplateAttribute, use the first one with name :"
						, managerType.FullName, mtas[0].Name);
			}
			return mtas[0];
		}

		public void AddSubManager(Type managerType)
		{
			if (managerType.IsAbstract || !managerType.ImplementsInterface<IManager>())
			{
				return;
			}
			
			ManagerTemplateAttribute mtaTarget = getFirstMTA(managerType);
			string managerName =  null;
			if (mtaTarget == null || mtaTarget.Name.IsNullOrEmpty())
			{
				managerName = managerType.FullName;
			}
			else
			{
				managerName = mtaTarget.Name;
			}
			var managerInstance = createSubManagerInstance(managerType);
			if (managerInstance == null)
			{
				Debug.LogError("Manager Create Failed. {0}/{1}".FormatEx(mtaTarget.Name, managerType.FullName));
				return;
			}
			objMangers.Add(managerName, managerInstance);
		}

		private IManager createSubManagerInstance(Type managerType)
		{
			return System.Activator.CreateInstance(managerType, true) as IManager;
		}

		public T GetSubManager<T>() where T : class, IManager
		{
			return GetSubManager(typeof(T)) as T;
		}

		public IManager GetSubManager(Type managerType)
		{
			IManager target = null;
			if (managerType.ImplementsInterface<IManager>())
			{
				foreach (var manager in objMangers.Values)
				{
					target = manager;
					if (target.GetType() == managerType)
					{
						break;
					}
				}
			}
			return target;
		}

		public void RemoveSubManager(Type managerType)
		{
			if (managerType.IsAbstract || !managerType.ImplementsInterface<IManager>())
			{
				return;
			}
			
			if (GetSubManager(managerType) == null)
			{
				return;
			}

			ManagerTemplateAttribute mtaTarget = getFirstMTA(managerType);
			string managerName =  null;
			if (mtaTarget == null || mtaTarget.Name.IsNullOrEmpty())
			{
				managerName = managerType.FullName;
			}
			else
			{
				managerName = mtaTarget.Name;
			}
			
			if (objMangers.ContainsKey(managerName))
			{
				objMangers.Remove(managerName);
			}
		}

		public void RemoveSubManager<T>() where T : IManager
		{
			RemoveSubManager(typeof(T));
		}

	}

	
}