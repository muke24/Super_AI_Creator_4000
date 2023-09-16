using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using XNode;

public class CreateScriptsAndClassesNode : Node
{
	[Input]
	public string gptResponse;

	[Output]
	public bool scriptsProcessed;

	[HideInInspector]
	public bool isProcessing = false;

	// Initialize the output port when the node is created
	protected override void Init()
	{
		base.Init();
		
	}

	public override object GetValue(NodePort port)
	{
		if (!isProcessing)
		{
			GPTUnityInteraction.CreateScriptsAndClassesFromResponse(GetInputValue("gptResponse", this.gptResponse), this);

			return true;
		}
		else
		{
			return false;
		}
	}
}
