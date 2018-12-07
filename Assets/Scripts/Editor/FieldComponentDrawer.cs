using UnityEngine;
using UnityEditor;
using Framework.Core.Runtime;
using System.IO;

namespace Framework.Editor
{

#if UNITY_EDITOR

	[CustomEditor(typeof(FieldComponent))]
	public class FieldComponentDrawer : UnityEditor.Editor
	{
		EditorVolumeHelper helper = new EditorVolumeHelper();

		public
			//override 
			void _OnInspectorGUI()
		{
			string assetPath = AssetDatabase.GetAssetPath(target.GetInstanceID());
			string libraryPath = Path.ChangeExtension(assetPath, null);
			bool isInAnEditorFolder = libraryPath.Contains("/Editor/");

			GUILayout.BeginHorizontal();
			GUILayout.Label(assetPath, EditorStyles.boldLabel);
			GUILayout.FlexibleSpace();
// 			if (isInAnEditorFolder && m_EditButtonClickedCallback != null && GUILayout.Button("Edit...", s_EditButtonStyle))
// 			{
// 				if (m_EditButtonClickedCallback != null)
// 					m_EditButtonClickedCallback(libraryPath);
// 			}
			GUILayout.EndHorizontal();

			GUILayout.Space(6);

		}
		void OnSceneGUI()
		{
			var script = (FieldComponent)target;
			using (var scop = new Handles.DrawingScope())
			{
				if (script.fieldDefinition == null)
				{
					return;
				}
				var last = Handles.zTest;
				Handles.zTest = UnityEngine.Rendering.CompareFunction.Less;
				EditorVolumeHelper.InitMatrix(script.transform.localToWorldMatrix);
				int levels = script.fieldDefinition.Length;
				int level = 1;
				helper.GetKeyCommands();
				foreach (var fd in script.fieldDefinition)
				{
					if (fd.data != null && fd.data.Length > 0)
					{
						helper.DrawVolumes(fd.data, level++);
					}
				}

				Handles.zTest = last;
			}

		}

	}
#endif
}
