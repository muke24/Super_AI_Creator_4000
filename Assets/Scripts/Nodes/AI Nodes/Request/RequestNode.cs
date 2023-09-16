using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using XNode;

public class RequestNode : Node
{
	[Serializable]
	public class Values
	{
		public string model = "gpt-4";
		public float temperature = 1;
		public string content = "Hello";

		public string response;
	}

	[Tooltip("This will use a previously stored response if the same parameters were used.")]
	public bool saveBandwidth = true;
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

	/// <summary>
	/// If a request was previously sent within the last x seconds, then another request will not be made.
	/// </summary>
	[HideInInspector]
	public float requestTimeout = 5f;
	[HideInInspector]
	public bool canSendRq = true;

	[HideInInspector]
	public List<Values> storedValues;

	// Initialize the output port when the node is created
    protected override void Init()
    {
        base.Init();
		Debug.Log(Application.persistentDataPath + "/rqStoredValues.json");

		//ManualDeserializationTest();

		if (storedValues == null)
		{
			storedValues = new List<Values>();
		}


		LoadStoredValues();
    }


	public override object GetValue(NodePort port)
	{
		// Update the input variables based on connected output ports
		model = GetInputValue<string>("model", this.model);
		temperature = GetInputValue<float>("temperature", this.temperature);
		content = GetInputValue<string>("content", this.content);

		if (saveBandwidth)
		{
			LoadStoredValues();
			Debug.Log($"StoredValues Count: {storedValues.Count}");

			for (int i = 0; i < storedValues.Count; i++)
			{
				var v = storedValues[i];
				if (v.content == content && v.temperature == temperature && v.model == model)
				{
					response = v.response;
					Debug.Log("Using a previously used response with same parametres to save bandwidth to GPT servers." +
						" \n" + $"Model: {v.model}. Temperature: {v.temperature}. Content: {v.content}. Response: {v.response}.");
					return response;
				}
			}

			if (!isProcessing && canSendRq)
			{
				API.instance.StartCoroutine(Timeout());

				// Process the data for the Request Node
				string _model = model;
				float _temperature = temperature;
				string _content = content;

				Debug.Log($"Model: {_model}. Temp: {_temperature}. Content: \"{_content}\"");

				API.instance.Request(_content, _temperature, _model, (_response) =>
				{
					if (!string.IsNullOrEmpty(_response))
					{
						response = _response;
						isProcessing = false;

						storedValues.Add(new Values { content = content, model = model, response = response, temperature = temperature });
						SaveStoredValues(); // Save the updated list to a JSON file
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
		else
		{
			if (!isProcessing)
			{
				
				// Process the data for the Request Node
				string _model = model;
				float _temperature = temperature;
				string _content = content;

				Debug.Log($"Model: {_model}. Temp: {_temperature}. Content: \"{_content}\"");

				API.instance.Request(_content, _temperature, _model, (_response) =>
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

	// Save storedValues to a JSON file
	private void SaveStoredValues()
	{
		string path = Application.persistentDataPath + "/rqStoredValues.json";
		// Debugging: Check count before serialization
		Debug.Log($"Before serialization, list count: {storedValues.Count}");

		if (storedValues.Count > 100)
		{
			int valuesToRemove = storedValues.Count - 100;
			for (int i = 0; i < valuesToRemove; i++)
			{
				storedValues.RemoveAt(0);
			}
		}

		string json = JsonUtility.ToJson(new SerializableListWrapper<Values> { list = storedValues });
		// Debugging: Check the JSON string to be saved
		Debug.Log($"JSON to be saved: {json}");
		File.WriteAllText(path, json);
	}

	// Load storedValues from a JSON file
	private void LoadStoredValues()
	{
		string path = Application.persistentDataPath + "/rqStoredValues.json";
		if (File.Exists(path))
		{
			string json = File.ReadAllText(path);
			SerializableListWrapper<Values> data = JsonUtility.FromJson<SerializableListWrapper<Values>>(json);
			// Debugging: Check count after deserialization
			if (data.list != null)
			{
				Debug.Log($"After deserialization, list count: {data.list.Count}");
				storedValues = data.list;
			}
			else
			{
				Debug.LogWarning("Deserialized list is null.");
				storedValues = new List<Values>();
			}
		}
		else
		{
			Debug.LogWarning("File does not exist. Initializing a new list.");
			storedValues = new List<Values>();
		}
	}

	[System.Serializable]
	public class SerializableListWrapper<T>
	{
		public List<T> list;
	}

	IEnumerator Timeout()
	{
		float timer = requestTimeout;
		canSendRq = false;

		while (true)
		{
			if (timer > 0)
			{
				timer -= Time.deltaTime;
				yield return null;
			}
			else
			{
				canSendRq = true;
				Debug.Log("Can send request again.");
				yield break;
			}
			yield break;
		}
	}
}
