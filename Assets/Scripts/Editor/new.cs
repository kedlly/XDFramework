using UnityEditor;
using System;using System.Collections;using System.Collections.Generic;
using UnityEngine;

public class TestWindow : EditorWindow
{
	public string filePath = "";

	public Texture2D texture = null;

	public string[] fileFilters = new string[] { "jgp", "jpg", "png", "png", "bmp", "bmp", "tif", "tif", "tiff", "tiff", "所有文件", "*" };

	[MenuItem("Test/测试窗口")]
	public static void Init()
	{
		TestWindow window = (TestWindow)EditorWindow.GetWindow(typeof(TestWindow));
		window.Show();
	}

	public void OnEnable()
	{
		this.titleContent = new GUIContent("测试窗口");
	}

	public Texture2D LoadTextureFromLocalDisk(string path)
	{
		WWW www = new WWW("file://" + path);
		if (www != null)
		{
			return www.texture;
		}
		return null;
	}

	public void OnGUI()
	{
		if (GUILayout.Button("加载图片0"))
		{
			if (string.IsNullOrEmpty(filePath)) filePath = Application.dataPath;
			filePath = EditorUtility.OpenFilePanelWithFilters("选择图片", System.IO.Path.GetDirectoryName(filePath), fileFilters);
			if (System.IO.File.Exists(filePath))
			{
				Debug.Log("加载图片 0: " + filePath);
				if (texture) UnityEngine.Object.DestroyImmediate(texture);
				texture = LoadTextureFromLocalDisk(filePath);
			}
		}
		if (GUILayout.Button("加载图片1 - 将路径信息中 / 替换为 \\"))
		{
			if (string.IsNullOrEmpty(filePath)) filePath = Application.dataPath;
			filePath = EditorUtility.OpenFilePanelWithFilters("选择图片", System.IO.Path.GetDirectoryName(filePath), fileFilters);
			if (System.IO.File.Exists(filePath))
			{
				filePath = filePath.Replace("/", "\\");
				Debug.Log("加载图片 1: " + filePath);
				if (texture) UnityEngine.Object.DestroyImmediate(texture);
				texture = LoadTextureFromLocalDisk(filePath);
			}
		}
		if (texture) GUILayout.Box(texture, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
	}
}