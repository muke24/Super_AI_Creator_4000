using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DefaultAsset))]
public class PythonFileInspector : Editor
{
	private Vector2 scrollPosition;

	public override void OnInspectorGUI()
	{
		DefaultAsset defAsset = (DefaultAsset)target;

		if (defAsset != null && Path.GetExtension(AssetDatabase.GetAssetPath(defAsset)) == ".py")
		{
			// Start a scrolling view inside GUILayout
			scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));

			// Display the text of the Python file
			EditorGUILayout.TextArea(File.ReadAllText(AssetDatabase.GetAssetPath(defAsset)), GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));

			// End the scrolling view
			EditorGUILayout.EndScrollView();
		}
		else
		{
			// Fall back to the default inspector
			base.OnInspectorGUI();
		}
	}
}
