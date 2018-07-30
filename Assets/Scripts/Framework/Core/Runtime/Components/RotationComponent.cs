
using UnityEngine;

namespace Framework.Core.Runtime
{
	[AddComponentMenu("Framework/Component/Rotation")]
	public class RotationComponent : MonoBehaviour, IManageredObject
	{
#if !UNITY_EDITOR
		[NonSerialized]
		[ShowOnly]
#endif
		public Vector3 Driection;
		//[NonSerialized]
		public float Speed = 1;

		Transform selfTrans;

		void Awake()
		{
			selfTrans = this.transform;
			this.Driection = selfTrans.rotation * Vector3.forward;
		}

		void OnEnable() {
			GameManager.Instance.GetSubManager<RotationSystem>().Register(this);
		}

		void OnDisable()
		{
			GameManager.Instance.GetSubManager<RotationSystem>().UnRegister(this);
		}

		bool IManageredObject.TickEnabled { get {return this.isActiveAndEnabled;} }

		void IManageredObject.Tick()
		{
			this.UpdateRotation();
		}

		void UpdateRotation()
		{
			selfTrans.rotation = Quaternion.Lerp(selfTrans.rotation, Quaternion.LookRotation(Driection, Vector3.up), Time.deltaTime * Speed);
		}
	}
}