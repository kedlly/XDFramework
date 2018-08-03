
using UnityEngine;
#if !UNITY_EDITOR
using System;
using Framework.Core.Attributes;
#endif

namespace Framework.Core.Runtime
{
	[AddComponentMenu("Framework/Component/Rotation")]
	public class RotationComponent : MonoBehaviour//, IManageredObject
	{
#if !UNITY_EDITOR
		[NonSerialized]
		[ShowOnly]
#endif
		public Vector3 Direction;
		//[NonSerialized]
		public float Speed = 1;

		Transform selfTrans;

		void Awake()
		{
			selfTrans = this.transform;
			this.Direction = selfTrans.rotation * Vector3.forward;
		}

		// 		void OnEnable() {
		// 			GameManager.Instance.GetSubManager<RotationSystem>().Register(this);
		// 		}
		// 
		// 		void OnDisable()
		// 		{
		// 			var rs = GameManager.Instance.GetSubManager<RotationSystem>();
		// 			if (rs != null)
		// 			{
		// 				rs.UnRegister(this);
		// 			}
		// 		}

		// 		bool IManageredObject.TickEnabled { get {return this.isActiveAndEnabled;} }
		// 		bool IManageredObject.IsActiving  { get { return selfTrans.rotation != Quaternion.LookRotation(Direction, Vector3.up); } }
		// 
		// 		void IManageredObject.Tick()
		// 		{
		// 			this.UpdateRotation();
		// 		}

		private void Update()
		{
			UpdateRotation();
		}

		void UpdateRotation()
		{
			//Direction = Direction.normalized;
			//selfTrans.rotation = Quaternion.Lerp(selfTrans.rotation, Quaternion.LookRotation(Direction, Vector3.up), Time.deltaTime * Speed);
			//Debug.Log(((IManageredObject)this).IsActiving);
			var last = selfTrans.rotation.eulerAngles;
			var newEA = last + new Vector3(0, Input.GetAxis("Horizontal") * Speed * Time.deltaTime, 0);
			selfTrans.rotation = Quaternion.Euler(newEA);
		}
	}
}