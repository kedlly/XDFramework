
using UnityEngine;
using System;
using System.Collections.Generic;

namespace Framework.Core.Runtime
{

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

		public bool TestWorldPosition(Vector3 position)
		{
			bool isInField = false;
			int layerIndex = 0;
			int fieldDataIndex = 0;
			if (fieldDefinition != null && fieldDefinition.Length > 0)
			{
				for (layerIndex = 0; layerIndex < fieldDefinition.Length; ++layerIndex)
				{
					var shapes = fieldDefinition[layerIndex];
					if (shapes.data != null && shapes.data.Length > 0)
					{
						for (fieldDataIndex = 0; fieldDataIndex < shapes.data.Length; ++fieldDataIndex)
						{
							var shape = shapes.data[fieldDataIndex];
							if (TestInShape(position, shape))
							{
								isInField = true;
								break;
							}
						}
					}
					if (isInField)
					{
						break;
					}
				}
			}
			return isInField;
		}

		bool TestInShape(Vector3 worldPosition, VolumeData data)
		{
			return VolumeData.Contains(data, worldPosition, transform.localToWorldMatrix);
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