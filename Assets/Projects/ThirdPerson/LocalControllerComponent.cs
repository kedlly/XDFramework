using Framework.Core.FlowControl;
using UnityEngine;
using Protocal.Request;
using Protocal;
using Framework.Core;

namespace Projects.ThirdPerson
{
	public class LocalControllerComponent : ActorControllerComponent
	{
		public override void onUpdate()
		{

			if (RotationComp != null)
			{
				var speed = RotationComp.Speed;
				var last = selfTrans.rotation.eulerAngles;
				var newEA = last + new UnityEngine.Vector3(0, Input.GetAxis("Horizontal") * speed * Time.deltaTime, 0);
				var newDirection = selfTrans.rotation * UnityEngine.Vector3.forward;
				selfTrans.rotation = Quaternion.Euler(newEA);
				//RotationComp.Direction = newDirection;
			}
			if (MovementComp != null)
			{
				var speed = Input.GetAxis("Vertical") * MovementComp.MaxSpeed;
				var direction = selfTrans.rotation * UnityEngine.Vector3.forward;
				MovementComp.Velocity = speed * direction;
			}
			delayControl.Execute();
		}

		Delay delayControl = new Delay(0.25f);

		public override void onAwake()
		{
			delayControl.OnExit += () =>
			{
				if (MovementComp != null && MovementComp.IsMovementStateChanaged())
				{
					var position = selfTrans.position;
					var velocity = MovementComp.Velocity;
					Protocal.RawData.MovementData data = new Protocal.RawData.MovementData();
					data.position = position.ToPV();
					data.velocity = velocity.ToPV();
					Request_Moving request = new Request_Moving();
					request.movement = data;
					request.Pack().Serialize().Send();
					MovementComp.RefreshStateRecord();
				}
			};
		}

		public override void onDestory()
		{
			if (delayControl != null)
			{
				delayControl.Reset();
				delayControl = null;
			}
		}
	}
}
