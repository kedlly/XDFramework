
using UnityEngine;

namespace Framework.Core.Runtime
{
	public class SpringArm : MonoBehaviour
	{
		public Transform hand;

		public bool handLookAt = true;
		public bool lockYAxis = true;
		public bool ignoreArmRot = false;
		public float lockSpeed = 1.0f;
		public bool armRotated = true;

		public float coefficientOfRestoringForce = 100;

		public Vector3 exogenic = Vector3.zero;

		Vector3 __targetDirectionInWorldSpace;
		Vector3 __targetDirectionInLocalSpace;

		Matrix4x4 originalHandMat;
		Matrix4x4 originalSelfMat;


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
		Rigidbody __targetRigibody = null;
		SpringArm_HandComponent handcomp = null;
		public void Init()
		{
			if (hand != null)
			{
				__targetRigibody = hand.gameObject.GetComponent<Rigidbody>();
				__targetDirectionInWorldSpace = hand.position - transform.position;
				targetLastPosition = hand.position;
				handcomp = hand.gameObject.GetComponent<SpringArm_HandComponent>();
				if (handcomp == null)
				{
					handcomp = hand.gameObject.AddComponent<SpringArm_HandComponent>();
				}
				handcomp.enabled = true;
				handcomp.onUpdate = _Update;
				handcomp.onFixedUpdate = _FixedUpdate;
				handcomp.onLateUpdate = _LateUpdate;
				originalHandMat = hand.transform.localToWorldMatrix;
				originalSelfMat = transform.localToWorldMatrix;
				__targetDirectionInLocalSpace = transform.worldToLocalMatrix.MultiplyVector(__targetDirectionInWorldSpace);
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
			// hand 变换到当前gameobject的 本地空间
			var mat = transform.worldToLocalMatrix * hand.transform.localToWorldMatrix;
			// hand 在 当前gameobject 本地空间中的目的位置向量 (初始化为开始时位置)
			var targetDirInLocalSpace = __targetDirectionInLocalSpace;
			// 若没有使用arm旋转
			if (!armRotated)
			{
				//hand 在 当前gameobject 本地空间中的目的位置向量 从初始位置反向旋转到arm初始方向
				targetDirInLocalSpace = transform.worldToLocalMatrix.MultiplyVector(targetDirInLocalSpace);
			}
			//取得相对位置向量
			currentDirectionInLocalSpace = mat.GetColumn(3);
			//力的方向为目的位置与当前相对位置两向量之差
			var springForce = targetDirInLocalSpace - currentDirectionInLocalSpace; 

			//合力为 弹力(变换到世界空间) * 系数 + 外力
			compositeForce = transform.TransformVector(springForce) * coefficientOfRestoringForce + exogenic;

		}

		private Vector3 compositeForce = Vector3.zero;
		private Vector3 currentDirectionInLocalSpace;

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
					var originalRotation = originalHandMat.rotation * originalSelfMat.inverse.rotation;
					var rotation = ignoreArmRot ? originalRotation : originalRotation * transform.rotation;
					hand.rotation = Quaternion.RotateTowards(hand.rotation, rotation, lockSpeed * Time.deltaTime);
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
