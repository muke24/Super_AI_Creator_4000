using UnityEngine;
using UnityEditor;
using System.IO;

public class CreatePythonScriptEditor
{
	[MenuItem("Assets/Create/Python Script", false, 80)]
	public static void CreatePythonScript()
	{
		string path = AssetDatabase.GetAssetPath(Selection.activeObject);
		if (string.IsNullOrEmpty(path))
		{
			path = "Assets";
		}
		else if (Path.GetExtension(path) != "")
		{
			path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
		}

		string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/NewPythonScript.py");

		// Create the Python script with a sample content
		File.WriteAllText(assetPathAndName, "# Your Python code here\n");

		AssetDatabase.Refresh();
		EditorUtility.FocusProjectWindow();
		Selection.activeObject = AssetDatabase.LoadAssetAtPath(assetPathAndName, typeof(TextAsset));
	}
}
