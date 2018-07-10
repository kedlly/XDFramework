
using UnityEngine;
using System;
using System.Collections.Generic;

namespace Framework.Core.Runtime
{

	public enum VolumeStyle
	{
		Sphere,			//球形场
		Cuboid,			//长方体
		Cylinder		//柱状体
	}

	[Serializable]
	public class VolumeData
	{
		public VolumeStyle type;
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

	}

	[Serializable]
	public class FieldDefinition
	{
		public VolumeData[] data;
	}


	public struct FieldCheckResult
	{
		public int layerIndex;
		public int dataIndex;
	}

	[ExecuteInEditMode]
	public class FieldComponent : MonoBehaviour
	{

		public FieldDefinition[] fieldDefinition;

		public bool TestWorldPosition(Vector3 position, out FieldCheckResult[] checkResult)
		{
			bool isInField = false;
			List<FieldCheckResult> resultList = new List<FieldCheckResult>();
			int layerIndex = 0;
			int fieldDataIndex = 0;
			if (fieldDefinition != null && fieldDefinition.Length > 0)
			{
				for (layerIndex = 0; layerIndex < fieldDefinition.Length; ++ layerIndex)
				{
					var shapes = fieldDefinition[layerIndex];
					if (shapes.data != null && shapes.data.Length > 0 )
					{
						for (fieldDataIndex = 0; fieldDataIndex < shapes.data.Length; ++ fieldDataIndex)
						{
							var shape = shapes.data[fieldDataIndex];
							if (TestInShape(position, shape))
							{
								isInField = true;
								resultList.Add(new FieldCheckResult() { dataIndex = fieldDataIndex, layerIndex = layerIndex });
							}
						}
					}
				}
			}
			checkResult = resultList.ToArray();
			return isInField;
		}

		bool TestInShape(Vector3 worldPosition, VolumeData data)
		{
			var target2World = transform.localToWorldMatrix;
			Matrix4x4 mat2Target = Matrix4x4.TRS(data.center, Quaternion.Euler(data.rotation), data.scale);
			Matrix4x4 mat2World = target2World * mat2Target;
			var modePosition = mat2World.inverse.MultiplyPoint(worldPosition);
			if (data.type == VolumeStyle.Sphere)
			{
				return modePosition.sqrMagnitude <= 0.5f * 0.5f;
			}
			else if (data.type == VolumeStyle.Cuboid)
			{
				return -0.5f <= modePosition.x && modePosition.x <= 0.5f &&
					-0.5f <= modePosition.y && modePosition.y <= 0.5f &&
					-0.5f <= modePosition.z && modePosition.z <= 0.5f;

			}
			else if (data.type == VolumeStyle.Cylinder)
			{
				var vecRadius = new Vector2(modePosition.x, modePosition.y);
				var zPos = modePosition.z;
				return vecRadius.magnitude <= 0.5f && -0.5f <= zPos && zPos <= 0.5f;
			}

			return false;
		}

#if UNITY_EDITOR

		private void OnDrawGizmos()
		{
			if (fieldDefinition == null || fieldDefinition.Length == 0)
			{
				return;
			}
			VolumeGizmosHelper.InitGizmosMatrix(transform.localToWorldMatrix);
			int levels = fieldDefinition.Length;
			int level = 1;
			foreach(var fd in fieldDefinition)
			{
				if(fd.data != null && fd.data.Length > 0)
				{
					VolumeGizmosHelper.DrawVolumes(fd.data, level ++);
				}
			}
		}
#endif
	}
}