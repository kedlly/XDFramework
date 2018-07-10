using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Framework.Core.Runtime
{

	[ExecuteInEditMode]
	public class VolumeComponent : MonoBehaviour
	{
		public string VolumeTAG;
		public VolumeData Data;
		

		public bool IsInVolume(Vector3 position)
		{
			bool ret = false;

			return ret;
		}

#if UNITY_EDITOR
		public bool GizmosWired = false;
		[Range(1,10)]
		public int colorLevel = 1;
		private void OnDrawGizmos()
		{
			VolumeGizmosHelper.InitGizmosMatrix(transform.localToWorldMatrix);
			VolumeGizmosHelper.DrawVolume(Data, colorLevel, GizmosWired);
		}
#endif

	}
}
