
using UnityEngine;

namespace Framework.Core.Runtime
{
	public class SpringArmComponent : MonoBehaviour
	{
		public Transform hand;

		public bool armRotateEnabled
		{
			get
			{
				return _armRotateEnabled;
			}
			set
			{
				if (value != _armRotateEnabled)
				{
					InitDirectionInfo();
				}
				_armRotateEnabled = value;
			}
		}

		public bool handLookAt = true;
		public bool lockYAxis
		{
			get { return _lockYAxis; }
			set
			{
				if (value != _lockYAxis)
				{
					//hand.LookAt(transform.position, Vector3.up);
					InitOriginalMatrix();
				}
				_lockYAxis = value;
			}
		}
		public bool keepDirection = false;
		public float handRotateSpeed = 120.0f;
		public bool breakLocalHand = true;
		

		public float coefficientOfRestoringForce = 100;
		public bool telescopeLimit = false;
		public float coefficientOfTelescope = 1f;

		public Vector3 exogenic = Vector3.zero;

		Vector3 __targetDirectionInWorldSpace;
		Vector3 __targetDirectionInLocalSpace;

		[SerializeField]
		bool _armRotateEnabled  = true;
		[SerializeField]
		bool _lockYAxis = true;

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

		void InitDirectionInfo()
		{
			if (hand != null)
			{
				targetLastPosition = hand.position;
				__targetDirectionInWorldSpace = hand.position - transform.position;
				__targetDirectionInLocalSpace = transform.worldToLocalMatrix.MultiplyVector(__targetDirectionInWorldSpace);
				__targetDirectionInWorldSpace = hand.position - transform.position;
				__targetDirectionInLocalSpace = transform.worldToLocalMatrix.MultiplyVector(__targetDirectionInWorldSpace);
			}
		}

		void InitOriginalMatrix()
		{
			if (hand != null)
			{
				originalHandMat = hand.transform.localToWorldMatrix;
				originalSelfMat = transform.localToWorldMatrix;
			}
		}

		public void InitHandComp()
		{
			if (hand != null)
			{
				if (breakLocalHand && hand.IsChildOf(this.transform))
				{
					hand.transform.parent = null;
				}
				// initialize hand component and rigibody
				__targetRigibody = hand.gameObject.GetComponent<Rigidbody>();
				handcomp = hand.gameObject.GetComponent<SpringArm_HandComponent>();
				if (handcomp == null)
				{
					handcomp = hand.gameObject.AddComponent<SpringArm_HandComponent>();
				}
				handcomp.onUpdate = _Update;
				handcomp.onFixedUpdate = _FixedUpdate;
				handcomp.onLateUpdate = _LateUpdate;
			}
			else
			{
				enabled = false;
			}
		}

		Rigidbody __targetRigibody = null;
		SpringArm_HandComponent handcomp = null;
		public void Init()
		{
			InitHandComp();
			InitOriginalMatrix();
			InitDirectionInfo();
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
			if (!armRotateEnabled)
			{
				//hand 在 当前gameobject 本地空间中的目的位置向量 从初始位置反向旋转到arm初始方向
				targetDirInLocalSpace = transform.worldToLocalMatrix.MultiplyVector(__targetDirectionInWorldSpace);
			}
			//取得相对位置向量
			currentDirectionInLocalSpace = mat.GetColumn(3);
			//力的方向为目的位置与当前相对位置两向量之差
			var springForce = targetDirInLocalSpace - currentDirectionInLocalSpace;
			var externCoefficientOfTelescope = 1.0f;
			if (telescopeLimit)
			{
				//被压缩时
				var originalLength = targetDirInLocalSpace.magnitude;
				var telescopeDelta = originalLength - currentDirectionInLocalSpace.magnitude;
				telescopeDelta /= originalLength;
				if (telescopeDelta > 0)
				{
					externCoefficientOfTelescope = coefficientOfTelescope / (1 - telescopeDelta);
				}
			}
			//合力为 弹力(变换到世界空间) * 系数 + 外力
			compositeForce = transform.TransformVector(springForce) * coefficientOfRestoringForce *externCoefficientOfTelescope + exogenic;
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
			//update hand's position
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
#if UNITY_2018_OR_NEWER
					var rotation = keepDirection ? originalHandMat.rotation : transform.rotation * originalSelfMat.inverse.rotation * originalHandMat.rotation;
					hand.rotation = Quaternion.RotateTowards(hand.rotation, rotation, handRotateSpeed * Time.deltaTime);
#else
					Vector3 pos, scale;
					Quaternion oriHandRotation;
					Quaternion oriSelfInvRotation;
					GetTRS(originalHandMat, out pos, out oriHandRotation, out scale);
					GetTRS(originalSelfMat.inverse, out pos, out oriSelfInvRotation, out scale);
					var rotation = keepDirection ? oriHandRotation : transform.rotation * oriSelfInvRotation * oriHandRotation;
					hand.rotation = Quaternion.RotateTowards(hand.rotation, rotation, handRotateSpeed * Time.deltaTime);
#endif
				}
			}
		}
#if UNITY_2018_OR_NEWER
#else
		private static void GetTRS(Matrix4x4 mat, out Vector3 position, out Quaternion rotation, out Vector3 localScale)
		{
			// Extract new local position
			position = mat.GetColumn(3);

			// Extract new local rotation
			rotation = Quaternion.LookRotation(
				mat.GetColumn(2),
				mat.GetColumn(1)
			);

			// Extract new local scale
			localScale = new Vector3(
				mat.GetColumn(0).magnitude,
				mat.GetColumn(1).magnitude,
				mat.GetColumn(2).magnitude
			);

		}
#endif

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
