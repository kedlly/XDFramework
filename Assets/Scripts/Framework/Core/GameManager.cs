using UnityEngine;
using Framework.Library.Singleton;
using Framework.Utils.Extensions;
using System.Collections.Generic;
using Framework.Core.Attributes;
using System;
using System.Linq;

namespace Framework.Core
{

	[PathInHierarchy("/[Game]/GamePlay"), DisallowMultipleComponent]
	public sealed class GameManager : ToSingletonBehavior<GameManager>
	{
		protected override void OnSingletonInit()
		{
			
		}

		protected override void OnDispose()
		{
			base.OnDispose();
			foreach (var item in objMangers)
			{
				var manager = item.Value;
				if (manager != null)
				{
					manager.UnRegisterAll(true);
				}
			}
		}

		private void OnDestroy()
		{
			Dispose();
		}

		private GameManager() {}

		public void Initalize()
		{
			AutoRegisterManagers();
			NetworkManager.Instance.Initalize();
		}

		private void AutoRegisterManagers()
		{
			var managers = System.AppDomain.CurrentDomain.GetAllTypesImplementsInterface<IManager>();
			foreach (var mt in managers)
			{
				var mtaTarget = getFirstMTA(mt);
				if (mtaTarget != null && mtaTarget.AutoRegister)
				{
					AddSubManager(mt);
				}
			}
		}

		Dictionary<Type, IManager> objMangers = new Dictionary<Type, IManager>();

		public void Update()
		{
			foreach (var manager in objMangers)
			{
				manager.Value.Tick();
			}
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
						, managerType.FullName);
			}
			return mtas[0];
		}

		public void AddSubManager(Type managerType)
		{
			if (managerType.IsAbstract || !managerType.ImplementsInterface<IManager>())
			{
				Debug.LogError("Manager Create Failed. Manager Type : {1} is abstract or not implement interface IManager.".FormatEx(managerType.FullName));
				return;
			}
			var managerInstance = createSubManagerInstance(managerType);
			if (managerInstance == null)
			{
				Debug.LogError("Manager Create Failed with type:{1}".FormatEx(managerType.FullName));
				return;
			}
			objMangers.Add(managerType, managerInstance);
		}

		private IManager createSubManagerInstance(Type managerType)
		{
			return Activator.CreateInstance(managerType, true) as IManager;
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
				if (!objMangers.TryGetValue(managerType, out target))
				{
					throw new Exception("Not Exist IManager object in GameManager with Type: " + managerType.FullName);
				}
			}
			else
			{
				throw new Exception("Error Manager Type : " + managerType.FullName + " which is not implement interface : IManager.");
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

			if (objMangers.ContainsKey(managerType))
			{
				objMangers.Remove(managerType);
			}
		}

		public void RemoveSubManager<T>() where T : IManager
		{
			RemoveSubManager(typeof(T));
		}

		public IEnumerable<U> GetRelationedObjects<T, U>()
			where T : class, IManager
			where U : class, IManageredObject
		{
			return GetSubManager<T>().OfType<U>();
		}

	}

	
}