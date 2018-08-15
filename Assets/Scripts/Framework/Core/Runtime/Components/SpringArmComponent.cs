
using UnityEngine;

namespace Framework.Core.Runtime
{
	public class SpringArmComponent : MonoBehaviour
	{
		public Transform hand;

		public bool handLookAt = true;
		public bool lockYAxis = true;
		public bool lockOriDirection = false;
		public float lockSpeed = 1.0f;
		public bool armRotated = true;

		public float coefficientOfRestoringForce = 1;

		public Vector3 exogenic = Vector3.zero;

		Vector3 __targetDirection;
		Rigidbody __targetRigibody = null;
		Quaternion __targetQuat;
		Vector3 __targetLookAt;

		SpringArm_HandComponent handcomp = null;

		public void Start()
		{
			Init();
		}

		private void OnEnable()
		{
			if (handcomp != null)
			{
				handcomp.enabled = true;
			}
		}

		private void OnDisable()
		{
			if (handcomp != null)
			{
				handcomp.enabled = false;
			}
		}

		public void Init()
		{
			if (hand != null)
			{
				__targetRigibody = hand.gameObject.GetComponent<Rigidbody>();
				__targetDirection = hand.position - transform.position;
				targetLastPosition = hand.position;
				__targetQuat = hand.rotation;
				__targetLookAt = hand.forward;
				handcomp = hand.gameObject.GetComponent<SpringArm_HandComponent>();
				if (handcomp == null)
				{
					handcomp = hand.gameObject.AddComponent<SpringArm_HandComponent>();
				}
				handcomp.enabled = true;
				handcomp.onUpdate = _Update;
				handcomp.onFixedUpdate = _FixedUpdate;
				handcomp.onLateUpdate = _LateUpdate;
			}
			else
			{
				enabled = false;
			}
		}



		Vector3 targetLastPosition;
		private void _Update()
		{
			if (hand == null)
			{
				return;
			}
			var targetDir = __targetDirection;
			if (armRotated)
			{
				targetDir = transform.TransformVector(targetDir);
			}
			currentDirection = hand.position - transform.position;
			var springForce = (targetDir - currentDirection) * coefficientOfRestoringForce;

			compositeForce = springForce + exogenic;

			
		}

		private Vector3 compositeForce = Vector3.zero;
		private Vector3 currentDirection;

		private void _FixedUpdate()
		{
			if (hand == null)
			{
				return;
			}
			if (__targetRigibody != null && !__targetRigibody.isKinematic)
			{
				__targetRigibody.AddForce(compositeForce, ForceMode.Force);
			}
		}

		private void _LateUpdate()
		{
			if (hand == null)
			{
				return;
			}
			var delta = compositeForce * Time.deltaTime * Time.deltaTime;
			if (__targetRigibody == null || __targetRigibody.isKinematic)
			{
				targetLastPosition = hand.position;
				hand.position += delta;
			}
			if (handLookAt)
			{
				if (lockYAxis)
				{
					hand.LookAt(transform.position, Vector3.up);
				}
				else
				{
					if (lockOriDirection)
					{
						hand.rotation = Quaternion.Lerp(hand.rotation, __targetQuat * Quaternion.Inverse(hand.rotation), Time.deltaTime * lockSpeed);
					}
					else
					{
						hand.LookAt(transform.position);
					}
				}
			}
		}

		private void OnDrawGizmos()
		{
			if (hand == null)
			{
				return;
			}
			Gizmos.color = Color.red;
			Gizmos.DrawLine(transform.position, hand.position);
		}
	}


}
