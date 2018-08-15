
using System;
using UnityEngine;

namespace Framework.Core.Runtime
{
	public class SpringArm_HandComponent : MonoBehaviour
	{
		public Action onUpdate;

		public Action onLateUpdate;

		public Action onFixedUpdate;
		private void Update()
		{
			if (onUpdate != null)
			{
				onUpdate();
			}
		}

		private void FixedUpdate()
		{
			if (onFixedUpdate != null)
			{
				onFixedUpdate();
			}
		}

		private void LateUpdate()
		{
			if (onLateUpdate != null)
			{
				onLateUpdate();
			}
		}
	}


}
