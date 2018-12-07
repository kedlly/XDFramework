using UnityEditor;
using UnityEngine;


namespace XDDQFrameWork.Editor.ProjectBuilder
{
	class ExportBuildSettingWindow : EditorWindow
	{
		protected string directory4export = "./settings.xml";

		public virtual void Awake()
		{
			//autoRepaintOnSceneChange = true;
			this.titleContent = new GUIContent("ExportBuildSettings");
			
		}

		public void OnGUI()
		{
			EditorGUILayout.LabelField("Export this projectBuildSetting", EditorStyles.boldLabel);
			this.directory4export = EditorGUILayout.TextField("Output Directory (default):", this.directory4export);
			EditorGUILayout.Space();
			if ( GUILayout.Button("Export", GUILayout.Width(100)) )
			{
				DataCollection.ExportBuildSettingToXML(this.directory4export);
			}
			if ( GUILayout.Button("Import", GUILayout.Width(100)) )
			{
				DataCollection.ImportBuildSettingFromXML(this.directory4export);
			}
		}

	}
}