using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EditorGraph))]
public class EditorGraphEditor : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		EditorGraph graph = (EditorGraph)target;

		if (GUILayout.Button(graph.isProcessing ? "Stop Processing" : "Start Processing"))
		{
			if (graph.isProcessing)
			{
				graph.StopProcessing();
			}
			else
			{
				graph.StartProcessing();
			}
		}
	}
}
