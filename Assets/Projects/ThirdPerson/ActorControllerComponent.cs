
using Framework.Core.Runtime;
using UnityEngine;

namespace Projects.ThirdPerson
{
    public abstract class ActorControllerComponent : MonoBehaviour
    {
		public Player player { get; set; }
		protected Transform selfTrans;

		public MovementComponent MovementComp { get; private set; }
		public RotationComponent RotationComp { get; private set; }

		public virtual void onAwake() { }
		private void Awake()
		{
			selfTrans = transform;
			MovementComp = GetComponent<MovementComponent>();
			RotationComp = GetComponent<RotationComponent>();
			onAwake();
		}

		public virtual void onUpdate() { }
		private void Update()
		{
			onUpdate();
		}


		public virtual void onDestory() { }
		private void OnDestroy()
		{
			onDestory();
		}
	}
}