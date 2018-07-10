﻿using UnityEngine;
using UnityEditor;
using Framework.Core.Runtime;
using System.Collections.Generic;

namespace Framework.Editor
{

#if UNITY_EDITOR

	[CustomEditor(typeof(FieldComponent))]
	public class FieldComponentDrawer : UnityEditor.Editor
	{
		EditorVolumeHelper helper = new EditorVolumeHelper();
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
