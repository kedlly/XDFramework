
using UnityEngine;
using System;
using System.Collections.Generic;

namespace Framework.Core.Runtime
{

	public enum FieldStyle
	{
		Sphere,			//球形场
		Cuboid,			//长方体
		Cylinder		//柱状体
	}

	[Serializable]
	public class FieldData
	{
		public FieldStyle type;
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
		public FieldData[] data;
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

		bool TestInShape(Vector3 worldPosition, FieldData data)
		{
			var target2World = transform.localToWorldMatrix;
			Matrix4x4 mat2Target = Matrix4x4.TRS(data.center, Quaternion.Euler(data.rotation), data.scale);
			Matrix4x4 mat2World = target2World * mat2Target;
			var modePosition = mat2World.inverse.MultiplyPoint(worldPosition);
			if (data.type == FieldStyle.Sphere)
			{
				return modePosition.sqrMagnitude <= 0.5f * 0.5f;
			}
			else if (data.type == FieldStyle.Cuboid)
			{
				return -0.5f <= modePosition.x && modePosition.x <= 0.5f &&
					-0.5f <= modePosition.y && modePosition.y <= 0.5f &&
					-0.5f <= modePosition.z && modePosition.z <= 0.5f;

			}
			else if (data.type == FieldStyle.Cylinder)
			{
				var vecRadius = new Vector2(modePosition.x, modePosition.y);
				var zPos = modePosition.z;
				return vecRadius.magnitude <= 0.5f && -0.5f <= zPos && zPos <= 0.5f;
			}

			return false;
		}

#if UNITY_EDITOR

		public static Color SphereColor = new Color(1f, 0f, 0f, 0.1f);
		public static Color CuboidColor = new Color(0f, 1f, 0, 0.1f);
		public static Color CylinderColor = new Color(1f, 1f, 0f, 0.1f);

		public static Color GetColor(Color color, int Level)
		{
			color.a *= Level;
			return color;
		}

		private void OnDrawGizmos()
		{
			if (fieldDefinition == null || fieldDefinition.Length == 0)
			{
				return;
			}
			Gizmos.matrix = transform.localToWorldMatrix;
			ClearMatrixStack();
			int levels = fieldDefinition.Length;
			int level = 1;
			foreach(var fd in fieldDefinition)
			{
				if(fd.data != null && fd.data.Length > 0)
				{
					DrawFields(fd.data, level ++);
				}
			}
		}


		void DrawFields(FieldData[] data, int colorLevel)
		{
			foreach(var p in data)
			{
				if(p.type == FieldStyle.Sphere)
				{
					DrawSphereField(p, GetColor(SphereColor, colorLevel));
				}
				if(p.type == FieldStyle.Cuboid)
				{
					DrawCuboidField(p, GetColor(CuboidColor, colorLevel));
				}
				if(p.type == FieldStyle.Cylinder)
				{
					DrawCylinderField(p, GetColor(CylinderColor, colorLevel));
				}
			}
		}

		void ClearMatrixStack()
		{
			saved.Clear();
		}

		Stack<Matrix4x4> saved = new Stack<Matrix4x4>();
		void PushMatrix()
		{
			saved.Push(Gizmos.matrix);
		}

		void PopMatrix()
		{
			Gizmos.matrix = saved.Pop();
		}

		void DrawSphereField(FieldData data, Color32 color)
		{
			Color oldColor = Gizmos.color;
			Gizmos.color = color;
			PushMatrix();
			Gizmos.color = color;
			Gizmos.matrix = Gizmos.matrix * data.matrix;
			Gizmos.DrawSphere(Vector3.zero, 0.5f);
			PopMatrix();
			Gizmos.color = oldColor;
			
		}

		void DrawCuboidField(FieldData data, Color cuboidColor)
		{
			Color oldColor = Gizmos.color;
			Gizmos.color = cuboidColor;
			PushMatrix();
			Gizmos.matrix = Gizmos.matrix * data.matrix;
			Gizmos.DrawCube(Vector3.zero, Vector3.one);
			PopMatrix();
			Gizmos.color = oldColor;
		}

		void DrawCylinderField(FieldData data, Color color)
		{
			Color oldColor = Gizmos.color;
			Gizmos.color = color;
			PushMatrix();
			Gizmos.color = color;
			Gizmos.matrix = Gizmos.matrix * data.matrix;
			Gizmos.DrawCube(Vector3.zero, Vector3.one);
			PopMatrix();
			Gizmos.color = oldColor;
		}
#endif
	}
}