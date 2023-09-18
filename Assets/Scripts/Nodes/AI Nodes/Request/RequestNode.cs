using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using XNode;

public class RequestNode : Node
{
	[Input]
	public string model = "gpt-4";
	[Input]
	public float temperature = 1;
	[Input]
	public string content = "Hello";

	[Output]
	public string response;

	[HideInInspector]
	public bool isProcessing = false;


	// Initialize the output port when the node is created
    protected override void Init()
    {
        base.Init();
    }


	public override object GetValue(NodePort port)
	{
		// Update the input variables based on connected output ports
		model = GetInputValue<string>("model", this.model);
		temperature = GetInputValue<float>("temperature", this.temperature);
		content = GetInputValue<string>("content", this.content);

		if (canProcess)
		{
			if (!isProcessing)
			{
				canProcess = false;
				// Process the data for the Request Node
				string resp = API.instance.Send("user", GetInputValue<string>("content", content), GetInputValue<float>("temperature", temperature), GetInputValue<string>("model", model));
				Debug.Log(resp);
				return resp;

				API.instance.Request(GetInputValue<string>("content"), GetInputValue<float>("temperature"), GetInputValue<string>("model"), (_response) =>
				{
					if (!string.IsNullOrEmpty(_response))
					{
						response = _response;
						isProcessing = false;

						UpdatePorts();

					}
					else
					{

						Debug.LogWarning("Failed to receive response.");
					}
				});
			}
			else
			{
				//Debug.Log("Unable to get response as it is still being processed.");
			}
		}

		return response;
	}

}
