using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class PythonRunnerNode : Node
{
	[Input]
	public string pythonDll;
	[Input]
	public string pythonFolder;
	[Input]
	public string pythonLibFolder;

	[Input]
	public string[] pythonLines = new string[]
	{
		"import sys",
		"print(sys.version)"
	};

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
		PythonRunner.RunLines(pythonLines, GetInputValue<string>("pythonDll"), GetInputValue<string>("pythonFolder"), GetInputValue<string>("pythonLibFolder"));

		return null; // Replace this
	}
}