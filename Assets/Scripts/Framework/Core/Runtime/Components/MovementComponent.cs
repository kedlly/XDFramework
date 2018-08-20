
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
				SafeUpdatePosition(value);
				//selfTrans.position = value;
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

		public FieldComponent FieldInLimit;
		public FieldComponent FieldOutLimit;

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

		public void SafeUpdatePosition(Vector3 targetPos)
		{
			if (targetPos == selfTrans.position)
			{
				return;
			}
			if (IsInsideFieldLimitIn(selfTrans.position) && IsOutsideFieldLimitOut(selfTrans.position))
			{
				if (IsInsideFieldLimitIn(targetPos) && IsOutsideFieldLimitOut(targetPos))
				{
					selfTrans.position = targetPos;
				}
			}
		}

		void UpdatePosition()
		{
			//selfTrans.position
			if (Velocity == Vector3.zero)
			{
				return;
			}
			if (IsInsideFieldLimitIn(selfTrans.position) && IsOutsideFieldLimitOut(selfTrans.position))
			{
				var targetPos = selfTrans.position + Velocity * Time.deltaTime;
				if (IsInsideFieldLimitIn(targetPos) && IsOutsideFieldLimitOut(targetPos))
				{
					selfTrans.position = targetPos;
				}
				else
				{
					Velocity = Vector3.zero;
				}
			}
			else
			{
				Velocity = Vector3.zero;
			}
			
		}

		bool IsInsideFieldLimitIn(Vector3 pos)
		{
			if (FieldInLimit != null)
			{
				return FieldInLimit.TestWorldPosition(pos);
			}
			return true;
		}

		bool IsOutsideFieldLimitOut(Vector3 pos)
		{
			if (FieldOutLimit != null)
			{
				return ! FieldOutLimit.TestWorldPosition(pos);
			}
			return true;
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