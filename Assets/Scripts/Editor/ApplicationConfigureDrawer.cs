using UnityEngine;
using UnityEditor;
using Framework.Core;

namespace Framework.Editor
{
	[CustomEditor(typeof(ApplicationManager))]
	class ApplicationConfigureDrawer : UnityEditor.Editor
	{
		SerializedProperty hand;

		public override void OnInspectorGUI()
		{
			//base.OnInspectorGUI();
			IApplication src = (ApplicationManager)target;
			if (src != null)
			{
				EditorGUILayout.TextField("LogPath", src.AppConfigure.LogPath);
			}
			
			/*
			src.hand = (Transform)hand.objectReferenceValue;
			EditorGUI.BeginDisabledGroup(src.hand == null);
			src.breakLocalHand = EditorGUILayout.Toggle(new GUIContent("BreakLocal"), src.breakLocalHand);
			src.coefficientOfRestoringForce = EditorGUILayout.FloatField(new GUIContent("Force"), src.coefficientOfRestoringForce);
			EditorGUI.indentLevel = 1;
			src.telescopeLimit = EditorGUILayout.Toggle(new GUIContent("TelescopeLimit"), src.telescopeLimit);
			EditorGUI.BeginDisabledGroup(src.hand == null || !src.telescopeLimit);
			src.coefficientOfTelescope = EditorGUILayout.FloatField(new GUIContent("Telescope/Stretch(1)"), src.coefficientOfTelescope);
			EditorGUI.EndDisabledGroup();
			EditorGUI.indentLevel = 0;
			src.armRotateEnabled = EditorGUILayout.Toggle(new GUIContent("EnableArmRotate"), src.armRotateEnabled);
			src.handLookAt = EditorGUILayout.Toggle(new GUIContent("LookArmPosition"), src.handLookAt);

			EditorGUI.indentLevel = 1;
			EditorGUI.BeginDisabledGroup(!src.handLookAt);
			EditorGUI.indentLevel = 1;
			src.lockYAxis = EditorGUILayout.Toggle(new GUIContent("Lock Y Axis"), src.lockYAxis);
			//src._lockYAxis = src.lockYAxis;
			EditorGUI.BeginDisabledGroup(src.lockYAxis);
			EditorGUI.indentLevel = 2;
			src.keepDirection = EditorGUILayout.Toggle(new GUIContent("LookOrignalDirection"), src.keepDirection);
			EditorGUI.BeginDisabledGroup(src.keepDirection);
			src.handRotateSpeed = EditorGUILayout.FloatField(new GUIContent("HandRotSpeed"), src.handRotateSpeed);
			EditorGUI.EndDisabledGroup();
			EditorGUI.EndDisabledGroup();
			EditorGUI.EndDisabledGroup();
			EditorGUI.EndDisabledGroup();*/
		}
	}
}
