using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EditorGUILevelSaver : EditorWindow
{
	string levelName;

	[MenuItem("Levels/Save...")]
	static void Init()
	{
		var window = GetWindow<EditorGUILevelSaver>();
		window.position = new Rect(0, 0, 150, 120);
		window.Show();
	}

	void OnGUI()
	{
		levelName = GUILayout.TextField(levelName);

		if (GUILayout.Button("Save Level") && LevelController.Instance != null)
			LevelController.Instance.SerializeLevel(System.IO.Path.Combine(Application.dataPath, "Scenes", "Levels"), levelName);
	}
}
