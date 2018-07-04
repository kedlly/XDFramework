using UnityEngine;
using UnityEditor;
using Framework.Core.Runtime;
using System.Collections.Generic;

namespace Framework.Editor
{

#if UNITY_EDITOR

	[CustomEditor(typeof(FieldComponent))]
	public class FieldComponentDrawer : UnityEditor.Editor
	{
		void OnSceneGUI()
		{
			var script = (FieldComponent)target;
			using(var scop = new Handles.DrawingScope())
			{
				if(script.fieldDefinition == null)
				{
					return;
				}
				var last = Handles.zTest;
				Handles.zTest = UnityEngine.Rendering.CompareFunction.Less;
				Handles.matrix = script.transform.localToWorldMatrix;
				ClearMatrixStack();
				int levels = script.fieldDefinition.Length;
				int level = 1;
				foreach(var fd in script.fieldDefinition)
				{
					if(fd.data != null && fd.data.Length > 0)
					{
						DrawFields(fd.data, level ++);
					}
				}

				Handles.zTest = last;
			}

		}

		void DrawFields(FieldData[] data, int colorLevel = 0)
		{
			if(Event.current.keyCode == KeyCode.W && Event.current.shift)
			{
				setting = Settings.Position;
			}
			if(Event.current.keyCode == KeyCode.E && Event.current.shift)
			{
				setting = Settings.Rotation;
			}
			if(Event.current.keyCode == KeyCode.R && Event.current.shift)
			{
				setting = Settings.Scale;
			}
			foreach(var shapeData in data)
			{
				if(shapeData.type == FieldStyle.Sphere)
				{
					shapeData.scale.x = shapeData.scale.x == 0 ? 1 : shapeData.scale.x;
					shapeData.scale.y = shapeData.scale.y == 0 ? 1 : shapeData.scale.y;
					shapeData.scale.z = shapeData.scale.z == 0 ? 1 : shapeData.scale.z;
				}
				DrawField(shapeData, colorLevel);
				DrawPositionRotationScale(shapeData.center, Quaternion.Euler(shapeData.rotation), shapeData.scale, shapeData);
			}
		}

		enum Settings
		{
			Position, Rotation, Scale
		}


		Settings setting = Settings.Position;

		Stack<Matrix4x4> saved = new Stack<Matrix4x4>();
		void PushMatrix()
		{
			saved.Push(Handles.matrix);
		}

		void PopMatrix()
		{
			Handles.matrix = saved.Pop();
		}

		void ClearMatrixStack()
		{
			saved.Clear();
		}

		void DrawPositionRotationScale(Vector3 center, Quaternion rotation, Vector3 scale, FieldData data)
		{
			if(setting == Settings.Position)
			{
				center = Handles.PositionHandle(center, rotation);
				data.center = center;
			}
			else if(setting == Settings.Rotation)
			{
				var newQuat = Handles.RotationHandle(rotation, center);
				if(Quaternion.Angle(newQuat, rotation) > 1)
				{
					data.rotation = newQuat.eulerAngles;
				}
			}
			else if(setting == Settings.Scale)
			{
				scale = Handles.ScaleHandle(scale, center, rotation, 1f);
				data.scale = scale;
			}
		}

		void DrawField(FieldData data, int colorLevel)
		{
			Handles.CapFunction fun = null;
			Color color = Color.grey;
			if(data.type == FieldStyle.Sphere)
			{
				fun = Handles.SphereHandleCap;
				color = FieldComponent.GetColor(FieldComponent.SphereColor, colorLevel);	
			}
			else if(data.type == FieldStyle.Cuboid)
			{
				fun = Handles.CubeHandleCap;
				color = FieldComponent.GetColor(FieldComponent.CuboidColor, colorLevel);
			}
			else if (data.type == FieldStyle.Cylinder)
			{
				fun = Handles.CylinderHandleCap;
				color = FieldComponent.GetColor(FieldComponent.CylinderColor, colorLevel);
			}
			Color oldColor = Handles.color;
			Handles.color = color;
			PushMatrix();
			Handles.matrix = Handles.matrix * data.matrix;
			fun(0, Vector3.zero, Quaternion.identity, 1, EventType.Repaint);
			PopMatrix();
			Handles.color = oldColor;
		}
	}
#endif
}
