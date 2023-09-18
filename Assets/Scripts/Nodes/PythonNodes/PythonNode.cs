using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using XNode;

public class PythonNode : Node
{
	public DefaultAsset pythonFile;
	public DefaultAsset pythonDllFile;

	[Input]
	public string pythonDll;
	[Input]
	public string pythonFolder;
	[Input]
	public string pythonLibFolder;

	[Output]
	public string output;

	// Use this for initialization
	protected override void Init()
	{
		base.Init();

	}

	// Return the correct value of an output port when requested
	public override object GetValue(NodePort port)
	{
		if (canProcess)
		{
			var dll = Path.GetDirectoryName(AssetDatabase.GetAssetPath(pythonDllFile)) + "\\" + pythonDllFile.name + ".dll";
			Debug.Log(Path.GetDirectoryName(AssetDatabase.GetAssetPath(pythonFile)));
			Debug.Log("DLL: " + dll);

			try
			{
				PythonRunner.RunPythonScript(dll, pythonFolder, Path.GetDirectoryName(AssetDatabase.GetAssetPath(pythonFile)));
				lastValue = true;
				canProcess = false;
				return true;
			}
			catch (System.Exception)
			{
				lastValue = false;
				return false;
			}
		}
		else
		{
			return lastValue;
		}
	}
}