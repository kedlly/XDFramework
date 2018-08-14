﻿
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
#if !UNITY_EDITOR && false
		[NonSerialized]
		[ShowOnly]
#endif
		public Vector3 Velocity;
		public Vector3 Position
		{
			get
			{
				return selfTrans.position;
			}
			set
			{
				selfTrans.position = value;
			}
		}
		//[NonSerialized]
		public float Speed
		{
			get
			{
				return Velocity.magnitude;
			}
		}

		Transform selfTrans;
		MovementSystem ms;

		void Awake()
		{
			selfTrans = this.transform;
			RefreshStateRecord();
		}
#if true
		void OnEnable() {
			ms = GameManager.Instance.GetSubManager<MovementSystem>();
			if (ms != null)
			{
				ms.Register(this);
			}
		}

		void OnDisable()
		{
			if (ms != null)
			{
				ms.UnRegister(this);
			}
		}
#endif

		public float MaxSpeed = 50;
#if true
		private void _Update()
#else
		private void Update()
#endif
		{
			UpdatePosition();
		}

		void UpdatePosition()
		{
			selfTrans.position = selfTrans.position + Velocity * Time.deltaTime;
		}

		bool IManageredObject.TickEnabled { get { return this.isActiveAndEnabled; } }
		bool IManageredObject.IsActiving { get { return Speed == 0f; } }

		void IManageredObject.Tick()
		{
			//this.UpdatePosition();
			_Update();
		}

		Vector3 lastPostion;
		Vector3 lastVelocity;

		public bool IsMovementStateChanaged()
		{
			return Vector3.Distance(lastPostion, selfTrans.position) > 0.2f || lastVelocity != Velocity;
		}

		public void RefreshStateRecord()
		{
			lastPostion = selfTrans.position;
			lastVelocity = Velocity;
		}
	}
}