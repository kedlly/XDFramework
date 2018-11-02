using UnityEngine;
using UnityEditor;
using Framework.Core.Runtime;

namespace Framework.Editor
{



	[CustomEditor(typeof(SceneGUIBezier))]
	public class SceneGUIBezierInspector : UnityEditor.Editor
	{
		float radius1 = 10;
		float radius2 = 5;
		void OnSceneGUI()
		{
			var script = (SceneGUIBezier)target;

			script.PointA = Handles.PositionHandle(script.PointA, Quaternion.identity);
			script.PointB = Handles.PositionHandle(script.PointB, Quaternion.identity);
			script.TangentA = Handles.PositionHandle(script.TangentA, Quaternion.identity);
			script.TangentB = Handles.PositionHandle(script.TangentB, Quaternion.identity);
			
			Handles.DrawBezier(script.PointA, script.PointB, script.TangentA, script.TangentB, Color.red, null, 5);
			Handles.zTest = UnityEngine.Rendering.CompareFunction.Less;
			Handles.color = new Color(1,0,0,0.05f);
			Handles.SphereHandleCap(0, script.transform.position, Quaternion.identity, radius1 * 2, UnityEngine.EventType.Repaint);
			Handles.color = new Color(1, 0, 0, 1f);
			//Handles.CircleHandleCap(2, script.transform.position, Quaternion.identity, radius1, EventType.Repaint);
			radius1 = Handles.RadiusHandle(Quaternion.identity, script.transform.position, radius1, true);
			Handles.color = new Color(1, 0, 0, 0.1f);
			Handles.SphereHandleCap(1, script.transform.position, Quaternion.identity, radius2, UnityEngine.EventType.Repaint);
			Handles.DrawSolidDisc(script.transform.position, Vector3.up, 15);
			Handles.color = new Color(1, 1, 0, 1f);
			Handles.DrawWireArc(script.transform.position, Vector3.up, Vector3.right * radius1, 45f, 6f);
			Handles.color = new Color(1, 0, 1, 0.2f);
			Handles.CylinderHandleCap(10, script.transform.position, Quaternion.identity, 1, UnityEngine.EventType.Repaint);
		}
	}
}
