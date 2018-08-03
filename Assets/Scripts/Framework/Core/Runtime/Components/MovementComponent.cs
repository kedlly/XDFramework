
using UnityEngine;
#if !UNITY_EDITOR
using System;
using Framework.Core.Attributes;
#endif

namespace Framework.Core.Runtime
{
	[AddComponentMenu("Framework/Component/Movement")]
	public class MovementComponent : MonoBehaviour, IManageredObject
	{
#if !UNITY_EDITOR
		[NonSerialized]
		[ShowOnly]
#endif
		public Vector3 Velocity;
		//[NonSerialized]
		public float Speed
		{
			get
			{
				return Velocity.magnitude;
			}
		}

		Transform selfTrans;

		void Awake()
		{
			selfTrans = this.transform;
		}

		void OnEnable() {
			GameManager.Instance.GetSubManager<MovementSystem>().Register(this);
		}
		
		void OnDisable()
		{
			var ms = GameManager.Instance.GetSubManager<MovementSystem>();
			if (ms != null)
			{
				ms.UnRegister(this);
			}
		}
		public float MaxSpeed = 50;
		private void _Update()
		{
			if (Input.GetKeyDown(KeyCode.Q))
			{
				Application.Quit();
			}
			var speed = Input.GetAxis("Vertical") * MaxSpeed;
			var direction = selfTrans.rotation * Vector3.forward;
			Velocity = speed * direction;
			UpdatePosition();
		}

		void UpdatePosition()
		{
			selfTrans.position = selfTrans.position + Velocity * Time.deltaTime;
		}

		bool IManageredObject.TickEnabled { get {return this.isActiveAndEnabled;} }
		bool IManageredObject.IsActiving  { get { return Speed == 0f; } }

		void IManageredObject.Tick()
		{
			this.UpdatePosition();
		}
	}
}