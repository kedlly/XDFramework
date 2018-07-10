using UnityEngine;
using UnityEditor;
using Framework.Core.Runtime;
using System.Collections.Generic;

namespace Framework.Editor
{

#if UNITY_EDITOR

	[CustomEditor(typeof(VolumeComponent))]
	public class VolumeComponentDrawer : UnityEditor.Editor
	{
		EditorVolumeHelper helper = new EditorVolumeHelper();
		void OnSceneGUI()
		{
			var script = (VolumeComponent)target;
			using (var scop = new Handles.DrawingScope())
			{
				if (script.Data == null)
				{
					return;
				}
				var last = Handles.zTest;
				Handles.zTest = UnityEngine.Rendering.CompareFunction.Less;
				EditorVolumeHelper.InitMatrix(script.transform.localToWorldMatrix);
				int level = script.colorLevel;
				helper.GetKeyCommands();
				helper.DrawVolume(script.Data, level++);
				Handles.zTest = last;
			}

		}

	}
#endif
}
