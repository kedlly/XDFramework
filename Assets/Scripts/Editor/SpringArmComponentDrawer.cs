using UnityEngine;
using UnityEditor;
using Framework.Core.Runtime;
using System.Collections.Generic;

namespace Framework.Editor
{

	[CustomEditor(typeof(SpringArmComponent))]
	class SpringArmComponentDrawer : UnityEditor.Editor
	{

		SerializedProperty hand;


		void OnEnable()
		{
			// Setup the SerializedProperties
			hand = serializedObject.FindProperty("hand");
		}

		public override void OnInspectorGUI()
		{
			//base.OnInspectorGUI();
			SpringArmComponent src = (SpringArmComponent)target;

			EditorGUILayout.PropertyField(hand, new GUIContent("Arm's Hand"));
			src.handLookAt = EditorGUILayout.Toggle(new GUIContent("LookArmPosition"), src.handLookAt);
			EditorGUI.indentLevel = 1;
			EditorGUI.BeginDisabledGroup(!src.handLookAt);
			EditorGUI.indentLevel = 2;
			src.lockYAxis = EditorGUILayout.Toggle(new GUIContent("Lock Y Axis"), src.lockYAxis);
			EditorGUI.BeginDisabledGroup(src.lockYAxis);
			EditorGUI.indentLevel = 3;
			src.lookAtOrignal = EditorGUILayout.Toggle(new GUIContent("LookOrignalDirection"), src.lookAtOrignal);
			EditorGUI.BeginDisabledGroup(src.lookAtOrignal);
			src.handRotateSpeed = EditorGUILayout.FloatField(new GUIContent("HandRotSpeed"), src.handRotateSpeed);
			EditorGUI.EndDisabledGroup();
			EditorGUI.EndDisabledGroup();
			EditorGUI.EndDisabledGroup();			
		}
	}
}
