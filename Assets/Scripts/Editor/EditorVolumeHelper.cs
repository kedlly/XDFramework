using UnityEngine;
using UnityEditor;
using Framework.Core.Runtime;
using System.Collections.Generic;

namespace Framework.Editor
{

#if UNITY_EDITOR
	public class EditorVolumeHelper 
	{		
		public static void InitMatrix(Matrix4x4 mat)
		{
			Handles.matrix = mat;
		}

		public void GetKeyCommands()
		{
			if (Event.current.keyCode == KeyCode.W && Event.current.shift)
			{
				setting = Settings.Position;
			}
			if (Event.current.keyCode == KeyCode.E && Event.current.shift)
			{
				setting = Settings.Rotation;
			}
			if (Event.current.keyCode == KeyCode.R && Event.current.shift)
			{
				setting = Settings.Scale;
			}
		}

		public void DrawVolume(VolumeData shapeData, int colorLevel = 0)
		{
			if (shapeData.type == VolumeType.Sphere)
			{
				shapeData.scale.x = shapeData.scale.x == 0 ? 1 : shapeData.scale.x;
				shapeData.scale.y = shapeData.scale.y == 0 ? 1 : shapeData.scale.y;
				shapeData.scale.z = shapeData.scale.z == 0 ? 1 : shapeData.scale.z;
			}
			DrawShape(shapeData, colorLevel);
			DrawPositionRotationScale(shapeData.center, Quaternion.Euler(shapeData.rotation), shapeData.scale, shapeData, setting);
		}

		public void DrawVolumes(VolumeData[] data, int colorLevel = 0)
		{
			foreach(var shapeData in data)
			{
				DrawVolume(shapeData, colorLevel);
			}
		}

		enum Settings
		{
			Position, Rotation, Scale
		}


		Settings setting = Settings.Position;

		static void DrawPositionRotationScale(Vector3 center, Quaternion rotation, Vector3 scale, VolumeData data, Settings currentSetting)
		{
			if(currentSetting == Settings.Position)
			{
				center = Handles.PositionHandle(center, rotation);
				data.center = center;
			}
			else if(currentSetting == Settings.Rotation)
			{
				var newQuat = Handles.RotationHandle(rotation, center);
				if(Quaternion.Angle(newQuat, rotation) > 1)
				{
					data.rotation = newQuat.eulerAngles;
				}
			}
			else if(currentSetting == Settings.Scale)
			{
				scale = Handles.ScaleHandle(scale, center, rotation, HandleUtility.GetHandleSize(center));
				data.scale = scale;
			}
		}

		static void DrawShape(VolumeData data, int colorLevel)
		{
			Handles.CapFunction fun = null;
			Color color = Color.grey;
			if(data.type == VolumeType.Sphere)
			{
				fun = Handles.SphereHandleCap;
				color = VolumeGizmosHelper.GetColor(VolumeGizmosHelper.SphereColor, colorLevel);	
			}
			else if(data.type == VolumeType.Cuboid)
			{
				fun = Handles.CubeHandleCap;
				color = VolumeGizmosHelper.GetColor(VolumeGizmosHelper.CuboidColor, colorLevel);
			}
			else if (data.type == VolumeType.Cylinder)
			{
				fun = Handles.CylinderHandleCap;
				color = VolumeGizmosHelper.GetColor(VolumeGizmosHelper.CylinderColor, colorLevel);
			}
			color.a *= 0.1f;
			Color oldColor = Handles.color;
			Handles.color = color;
			Matrix4x4 oldMat = Handles.matrix;
			Handles.matrix = Handles.matrix * data.matrix;
			fun(0, Vector3.zero, Quaternion.identity, 1, UnityEngine.EventType.Repaint);
			Handles.matrix = oldMat;
			Handles.color = oldColor;
		}
	}
#endif
}
