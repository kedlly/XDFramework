using Framework.Core.FlowControl;
using UnityEngine;
using Protocol.Request;
using Protocol.RawData;
using Protocol;
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
		byte[] buffer = new byte[1024];
		byte[] buffer_c = new byte[16] { 0x30, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39, 0x41, 0x42, 0x43, 0x44, 0x45, 0x46 };
		int index = 0;
		public override void onAwake()
		{
			delayControl.OnExit += () =>
			{
				if (MovementComp != null && MovementComp.IsMovementStateChanaged())
				{
					var position = selfTrans.position;
					var velocity = MovementComp.Velocity;
					MovementData data = new MovementData();
					data.position = position.ToPV();
					data.velocity = velocity.ToPV();
					Request_Moving request = new Request_Moving();
					request.movement = data;												 
					request.Pack().Serialize(new System.ArraySegment<byte>(buffer)).Send();
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
