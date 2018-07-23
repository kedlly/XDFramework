using System;
using UnityEngine;

namespace Framework.Core.Runtime
{
	public enum VolumeType
	{
		Sphere,         //球形场
		Cuboid,         //长方体
		Cylinder        //柱状体
	}

	[Serializable]
	public class VolumeData
	{
		public VolumeType type;
		public Vector3 center = Vector3.zero;
		public Vector3 rotation = Vector3.zero;
		public Vector3 scale = Vector3.one;

		public Matrix4x4 matrix
		{
			get
			{
				return Matrix4x4.TRS(center, Quaternion.Euler(rotation), scale);
			}
		}

		private const float NormalShapeSize = 1.0f;

		public static bool Contains(VolumeData volumeData, Vector3 position, Matrix4x4 baseMat, float size = NormalShapeSize)
		{
			Matrix4x4 mat2World = baseMat * volumeData.matrix;
			float halfSize = size / 2;
			var modePosition = mat2World.inverse.MultiplyPoint(position);
			if (volumeData.type == VolumeType.Sphere)
			{
				return modePosition.sqrMagnitude <= halfSize * halfSize;
			}
			else if (volumeData.type == VolumeType.Cuboid)
			{
				return -halfSize <= modePosition.x && modePosition.x <= halfSize &&
					-halfSize <= modePosition.y && modePosition.y <= halfSize &&
					-halfSize <= modePosition.z && modePosition.z <= halfSize;

			}
			else if (volumeData.type == VolumeType.Cylinder)
			{
				var vecRadius = new Vector2(modePosition.x, modePosition.y);
				var zPos = modePosition.z;
				return vecRadius.sqrMagnitude <= halfSize * halfSize && -halfSize <= zPos && zPos <= halfSize;
			}
			return false;
		}
	}
}
