
using UnityEngine;

namespace Framework.Core.Runtime
{
	public class SpringArmComponent : MonoBehaviour
	{
		public Transform targetTrans;

		public bool lockTargetLookAt = true;
		public bool lockLookAtYAxis = true;
		public float lockSpeed = 1.0f;
		public bool lockSelfRotation = true;

		public float coefficientOfRestoringForce = 1;

		public Vector3 exogenic = Vector3.zero;

		Vector3 __targetDirection;
		Rigidbody __targetRigibody = null;
		Quaternion __targetQuat;
		Vector3 __targetLookAt;

		public void Start()
		{
			if (targetTrans != null)
			{
				__targetRigibody = targetTrans.gameObject.GetComponent<Rigidbody>();
				__targetDirection = targetTrans.position - transform.position;
				targetLastPosition = targetTrans.position;
				__targetQuat = targetTrans.rotation;
				__targetLookAt = targetTrans.forward;
			}
			else
			{
				enabled = false;
			}
		}

		Vector3 targetLastPosition;
		private void Update()
		{
			if (targetTrans == null)
			{
				return;
			}
			var targetDir = __targetDirection;
			if (lockSelfRotation)
			{
				targetDir = transform.TransformVector(targetDir);
			}
			currentDirection = targetTrans.position - transform.position;
			var springForce = (targetDir - currentDirection) * coefficientOfRestoringForce;

			compositeForce = springForce + exogenic;

			var delta = compositeForce * Time.deltaTime * Time.deltaTime;

			if (__targetRigibody == null || __targetRigibody.isKinematic)
			{
				targetLastPosition = targetTrans.position;
				targetTrans.position += delta;
			}
		}

		private Vector3 compositeForce = Vector3.zero;
		private Vector3 currentDirection;

		private void FixedUpdate()
		{
			if (targetTrans == null)
			{
				return;
			}
			if (__targetRigibody != null && !__targetRigibody.isKinematic)
			{
				__targetRigibody.AddForce(compositeForce, ForceMode.Force);
			}
		}

		private void LateUpdate()
		{
			if (targetTrans == null)
			{
				return;
			}
			if (lockTargetLookAt)
			{
				if (lockLookAtYAxis)
				{
					targetTrans.LookAt(transform.position, Vector3.up);
				}
				else
				{
					var v = Quaternion.FromToRotation(__targetDirection, currentDirection);
					targetTrans.rotation = Quaternion.Lerp(targetTrans.rotation, v * __targetQuat, Time.deltaTime * lockSpeed);
				}
			}
		}

		private void OnDrawGizmos()
		{
			if (targetTrans == null)
			{
				return;
			}
			Gizmos.color = Color.red;
			Gizmos.DrawLine(transform.position, targetTrans.position);
		}
	}


}
