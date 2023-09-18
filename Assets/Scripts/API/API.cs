using System;
using System.Collections;
using System.IO;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;

[ExecuteInEditMode]
public class API : Singleton<API>
{
	#region Message Setup
	[System.Serializable]
	public class Message
	{
		public string role;
		public string content;
	}

	/// <summary>
	/// A list of functions the model may generate JSON inputs for.
	/// </summary>
	[System.Serializable]
	public class Function
	{
		/// <summary>
		/// The name of the function to be called. Must be a-z, A-Z, 0-9, or contain underscores and dashes, with a
		/// maximum length of 64.
		/// </summary>
		public string name;
		/// <summary>
		/// A description of what the function does, used by the model to choose when and how to call the function.
		/// </summary>
		public string description;
		/// <summary>
		/// The parameters the functions accepts, described as a JSON Schema object.
		/// </summary>
		public object parameters;
	}

	[System.Serializable]
	public class SimpleRequest
	{
		public string model;
		public Message[] messages;
	}

	[System.Serializable]
	public class ComplexRequest
	{
		// Find out more here: https://platform.openai.com/docs/api-reference/chat/create

		/// <summary>
		/// ID of the model to use.
		/// </summary>
		public string model;
		/// <summary>
		/// A list of messages comprising the conversation so far.
		/// </summary>
		public Message[] messages;
		/// <summary>
		/// A list of functions the model may generate JSON inputs for.
		/// </summary>
		public float? temperature = null;
		/// <summary>
		/// An alternative to sampling with temperature, called nucleus sampling, where the model considers 
		/// the results of the tokens with top_p probability mass. So 0.1 means only the tokens comprising 
		/// the top 10% probability mass are considered.
		/// </summary>
		public float? top_p = null;
		/// <summary>
		/// How many chat completion choices to generate for each input message.
		/// </summary>
		public int? n = null;
		/// <summary>
		/// Up to 4 sequences where the API will stop generating further tokens.
		/// </summary>
		public string stop = null;
		/// <summary>
		/// The maximum number of tokens to generate in the chat completion. The total length of input tokens 
		/// and generated tokens is limited by the model's context length.
		/// </summary>
		public int? max_tokens = null;
		/// <summary>
		/// Number between -2.0 and 2.0. Positive values penalize new tokens based on whether they appear in 
		/// the text so far, increasing the model's likelihood to talk about new topics.
		/// </summary>
		public float? presence_penalty = null;
		/// <summary>
		/// Number between -2.0 and 2.0. Positive values penalize new tokens based on their existing frequency
		/// in the text so far, decreasing the model's likelihood to repeat the same line verbatim.
		/// </summary>
		public float? frequency_penalty = null;
		/// <summary>
		/// A unique identifier representing your end-user, which can help OpenAI to monitor and detect abuse.
		/// </summary>
		public string user = null;
	}

	[System.Serializable]
	public class ComplexRequestStream
	{
		// Find out more here: https://platform.openai.com/docs/api-reference/chat/create

		/// <summary>
		/// ID of the model to use.
		/// </summary>
		public string model;
		/// <summary>
		/// A list of messages comprising the conversation so far.
		/// </summary>
		public Message[] messages;
		/// <summary>
		/// A list of functions the model may generate JSON inputs for.
		/// </summary>
		public float? temperature = null;
		/// <summary>
		/// An alternative to sampling with temperature, called nucleus sampling, where the model considers 
		/// the results of the tokens with top_p probability mass. So 0.1 means only the tokens comprising 
		/// the top 10% probability mass are considered.
		/// </summary>
		public float? top_p = null;
		/// <summary>
		/// How many chat completion choices to generate for each input message.
		/// </summary>
		public int? n = null;
		/// <summary>
		/// If set, partial message deltas will be sent, like in ChatGPT. Tokens will be sent as data-only
		/// server-sent events as they become available, with the stream terminated by a data: [DONE] message.
		/// </summary>
		public bool? stream = null;
		/// <summary>
		/// Up to 4 sequences where the API will stop generating further tokens.
		/// </summary>
		public string stop = null;
		/// <summary>
		/// The maximum number of tokens to generate in the chat completion. The total length of input tokens 
		/// and generated tokens is limited by the model's context length.
		/// </summary>
		public int? max_tokens = null;
		/// <summary>
		/// Number between -2.0 and 2.0. Positive values penalize new tokens based on whether they appear in 
		/// the text so far, increasing the model's likelihood to talk about new topics.
		/// </summary>
		public float? presence_penalty = null;
		/// <summary>
		/// Number between -2.0 and 2.0. Positive values penalize new tokens based on their existing frequency
		/// in the text so far, decreasing the model's likelihood to repeat the same line verbatim.
		/// </summary>
		public float? frequency_penalty = null;
		/// <summary>
		/// A unique identifier representing your end-user, which can help OpenAI to monitor and detect abuse.
		/// </summary>
		public string user = null;
	}

	[System.Serializable]
	public class ComplexFunctionRequest
	{
		// Find out more here: https://platform.openai.com/docs/api-reference/chat/create

		/// <summary>
		/// ID of the model to use.
		/// </summary>
		public string model;
		/// <summary>
		/// A list of messages comprising the conversation so far.
		/// </summary>
		public Message[] messages;
		/// <summary>
		/// A list of functions the model may generate JSON inputs for.
		/// </summary>
		public Function[] functions;
		/// <summary>
		/// Controls how the model responds to function calls. "none" means the model does not call a function, and 
		/// responds to the end-user. "auto" means the model can pick between an end-user or calling a function. 
		/// Specifying a particular function via {"name":\ "my_function"} forces the model to call that function.
		/// "none" is the default when no functions are present. "auto" is the default if functions are present.
		/// </summary>
		public string function_call;
		/// <summary>
		/// What sampling temperature to use, between 0 and 2. Higher values like 0.8 will make the output more random,
		/// while lower values like 0.2 will make it more focused and deterministic. We generally recommend altering 
		/// this or top_p but not both.
		/// </summary>
		public float? temperature = null;
		/// <summary>
		/// An alternative to sampling with temperature, called nucleus sampling, where the model considers 
		/// the results of the tokens with top_p probability mass. So 0.1 means only the tokens comprising 
		/// the top 10% probability mass are considered.
		/// </summary>
		public float? top_p = null;
		/// <summary>
		/// How many chat completion choices to generate for each input message.
		/// </summary>
		public int? n = null;
		/// <summary>
		/// If set, partial message deltas will be sent, like in ChatGPT. Tokens will be sent as data-only
		/// server-sent events as they become available, with the stream terminated by a data: [DONE] message.
		/// </summary>
		public bool? stream = null;
		/// <summary>
		/// Up to 4 sequences where the API will stop generating further tokens.
		/// </summary>
		public string stop = null;
		/// <summary>
		/// The maximum number of tokens to generate in the chat completion. The total length of input tokens 
		/// and generated tokens is limited by the model's context length.
		/// </summary>
		public int? max_tokens = null;
		/// <summary>
		/// Number between -2.0 and 2.0. Positive values penalize new tokens based on whether they appear in 
		/// the text so far, increasing the model's likelihood to talk about new topics.
		/// </summary>
		public float? presence_penalty = null;
		/// <summary>
		/// Number between -2.0 and 2.0. Positive values penalize new tokens based on their existing frequency
		/// in the text so far, decreasing the model's likelihood to repeat the same line verbatim.
		/// </summary>
		public float? frequency_penalty = null;
		/// <summary>
		/// A unique identifier representing your end-user, which can help OpenAI to monitor and detect abuse.
		/// </summary>
		public string user = null;
	}

	[System.Serializable]
	public class Choice
	{
		public Message message;
	}

	[System.Serializable]
	public class Response
	{
		public string id;
		public string object_type;
		public Choice[] choices;
	}
	#endregion

	private void OnEnable()
	{
		Initiate();
	}

	public bool autoTestModels = false;

	[HideInInspector]
	public bool testModels = false;
	//[HideInInspector]
	public bool sendGPT4Request = false;

	public Message[] testMsg;

	[Tooltip("Leave this blank to use the newest and best model available.")]
	public string curModel = "gpt-4";
	public string url = "https://api.openai.com/v1/chat/completions";

	public string[] apiKeys = new string[]
	{
		"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",
	};

	public int curKey = 0;

	public string lastResponse;

	void Initiate()
	{
		StopAllCoroutines();
		if (autoTestModels)
		{
			StartCoroutine(TestChatModels(0));
		}
	}

	private void Update()
	{
		Testing();
	}

	void Testing()
	{
		if (sendGPT4Request)
		{
			sendGPT4Request = false;
			//StartCoroutine(CallGPT4API(curModel, testMsg));
			StartCoroutine(SendGPTMessage(testMsg));
		}

		if (testModels)
		{
			StartCoroutine(TestChatModels(0));
			testModels = false;
		}
	}

	IEnumerator CallGPT4API(string model, Message[] messages)
	{
		SimpleRequest rq = new SimpleRequest { model = model, messages = messages };

		string jsonData = JsonUtility.ToJson(rq);
		UnityWebRequest request = UnityWebRequest.Put(url, jsonData);
		request.method = UnityWebRequest.kHttpVerbPOST;
		request.SetRequestHeader("Authorization", "Bearer " + apiKeys[curKey]);
		request.SetRequestHeader("Content-Type", "application/json");

		yield return request.SendWebRequest();

		if (request.result != UnityWebRequest.Result.Success)
		{
			Debug.Log("Failed to send request to the GPT API: " + request.error);
		}
		else
		{
			Response response = JsonUtility.FromJson<Response>(request.downloadHandler.text);
			foreach (Choice choice in response.choices)
			{
				Debug.Log("GPT Success: " + choice.message.role + ": " + choice.message.content);
			}
		}
	}

	IEnumerator TestChatModels(int count)
	{
		Message[] testMsg = new Message[1];
		testMsg[0] = new Message
		{
			role = "user",
			content = "Hello"
		};

		SimpleRequest payload = new();
		string curModel;

		if (count == 0)
		{
			payload = new SimpleRequest { model = "gpt-4-32k", messages = testMsg };
			curModel = "gpt-4-32k";
		}
		else if (count == 1)
		{
			payload = new SimpleRequest { model = "gpt-4", messages = testMsg };
			curModel = "gpt-4";

		}
		else
		{
			payload = new SimpleRequest { model = "gpt-3.5-turbo", messages = testMsg };
			curModel = "gpt-3.5-turbo";
		}

		string jsonData = JsonUtility.ToJson(payload);
		UnityWebRequest request = UnityWebRequest.Put(url, jsonData);
		request.method = UnityWebRequest.kHttpVerbPOST;
		request.SetRequestHeader("Authorization", "Bearer " + apiKeys[curKey]);
		request.SetRequestHeader("Content-Type", "application/json");

		yield return request.SendWebRequest();

		if (request.result != UnityWebRequest.Result.Success)
		{
			if (count < 3)
			{
				Debug.Log("The model \"" + curModel + "\" failed. Attempting to use the next best model.");
				count++;
				StartCoroutine(TestChatModels(count));
				yield break;
			}
			else
			{
				Debug.Log("Failed to retrieve the best model. Please do this manually.");
				yield break;
			}
		}
		else
		{
			Response response = JsonUtility.FromJson<Response>(request.downloadHandler.text);
			foreach (Choice choice in response.choices)
			{
				Debug.Log(curModel + " Success: " + choice.message.role + ": " + choice.message.content);
			}

			SetModelName(curModel);
		}
	}

	public void Request(string message, float temperature, string model, Action<string> callback)
	{
		StartCoroutine(Send(message, temperature, model, callback));
	}

	private IEnumerator Send(string message, float temperature, string model, Action<string> callback)
	{
		
		ComplexRequest rq = CustomRequest(message, "user", model, temperature);

		string jsonData = JsonUtility.ToJson(rq);
		UnityWebRequest request = UnityWebRequest.Put(url, jsonData);
		request.method = UnityWebRequest.kHttpVerbPOST;
		request.SetRequestHeader("Authorization", "Bearer " + apiKeys[curKey]);
		request.SetRequestHeader("Content-Type", "application/json");

		yield return request.SendWebRequest();

		if (request.result != UnityWebRequest.Result.Success)
		{
			Debug.Log("Failed to send request to the GPT API: " + request.error);
			callback?.Invoke(null); // Invoke the callback with null to indicate an error
		}
		else
		{
			Response response = JsonUtility.FromJson<Response>(request.downloadHandler.text);
			foreach (Choice choice in response.choices)
			{
				//Debug.Log("GPT Success: " + choice.message.role + ": " + choice.message.content);
				callback?.Invoke(choice.message.content); // Invoke the callback with the GPT response
			}
		}

		yield break;
	}

	public string Send(string role, string message, float temperature, string model)
	{
		ComplexRequest rq = CustomRequest(message, role, model, temperature);
		string jsonData = JsonUtility.ToJson(rq);

		HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
		request.Method = "POST";
		request.ContentType = "application/json";
		request.Headers["Authorization"] = "Bearer " + apiKeys[curKey];

		using (StreamWriter streamWriter = new StreamWriter(request.GetRequestStream()))
		{
			streamWriter.Write(jsonData);
			streamWriter.Flush();
			streamWriter.Close();
		}

		string responseContent = null;

		try
		{
			HttpWebResponse response = (HttpWebResponse)request.GetResponse();
			using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
			{
				responseContent = streamReader.ReadToEnd();
				Response parsedResponse = JsonUtility.FromJson<Response>(responseContent);
				foreach (Choice choice in parsedResponse.choices)
				{
					return choice.message.content;
				}
			}
		}
		catch (WebException ex)
		{
			using (StreamReader streamReader = new StreamReader(ex.Response.GetResponseStream()))
			{
				string errorResponse = streamReader.ReadToEnd();
				Debug.Log("Failed to send request to the GPT API: " + ex.Message);
				Debug.Log("Server responded with: " + errorResponse);
			}
			return null;
		}

		return null;
	}

	IEnumerator SendGPTMessage(string message, float temperature, string model)
	{
		ComplexRequest rq = CustomRequest(message, "user", model, temperature);

		string jsonData = JsonUtility.ToJson(rq);
		UnityWebRequest request = UnityWebRequest.Put(url, jsonData);
		request.method = UnityWebRequest.kHttpVerbPOST;
		request.SetRequestHeader("Authorization", "Bearer " + apiKeys[curKey]);
		request.SetRequestHeader("Content-Type", "application/json");

		yield return request.SendWebRequest();

		if (request.result != UnityWebRequest.Result.Success)
		{
			Debug.Log("Failed to send request to the GPT API: " + request.error);
		}
		else
		{
			Response response = JsonUtility.FromJson<Response>(request.downloadHandler.text);
			foreach (Choice choice in response.choices)
			{
				Debug.Log("GPT Success: " + choice.message.role + ": " + choice.message.content);
			}
		}
	}

	IEnumerator SendGPTMessage(Message[] messages)
	{
		ComplexRequest rq = CustomRequest(messages[0].content, messages[0].role, curModel);

		string jsonData = JsonUtility.ToJson(rq);
		UnityWebRequest request = UnityWebRequest.Put(url, jsonData);
		request.method = UnityWebRequest.kHttpVerbPOST;
		request.SetRequestHeader("Authorization", "Bearer " + apiKeys[curKey]);
		request.SetRequestHeader("Content-Type", "application/json");

		yield return request.SendWebRequest();

		if (request.result != UnityWebRequest.Result.Success)
		{
			Debug.Log("Failed to send request to the GPT API: " + request.error);
		}
		else
		{
			Response response = JsonUtility.FromJson<Response>(request.downloadHandler.text);
			foreach (Choice choice in response.choices)
			{
				Debug.Log("GPT Success: " + choice.message.role + ": " + choice.message.content);
			}
		}

		yield break;
	}

	IEnumerator SendGPTMessage(Message msg, string model, Action<Response> methodToCall)
	{
		ComplexRequest rq = CustomRequest(msg.content, msg.role, model);

		string jsonData = JsonUtility.ToJson(rq);
		UnityWebRequest request = UnityWebRequest.Put(url, jsonData);
		request.method = UnityWebRequest.kHttpVerbPOST;
		request.SetRequestHeader("Authorization", "Bearer " + apiKeys[curKey]);
		request.SetRequestHeader("Content-Type", "application/json");

		yield return request.SendWebRequest();

		if (request.result != UnityWebRequest.Result.Success)
		{
			Debug.Log("Failed to send request to the GPT API: " + request.error);
		}
		else
		{
			Response response = JsonUtility.FromJson<Response>(request.downloadHandler.text);

			methodToCall(response);

			Debug.Log("GPT Success: " + response.choices[0].message.role + ": " + response.choices[0].message.content);

			//foreach (Choice choice in response.choices)
			//{
			//	Debug.Log("GPT Success: " + choice.message.role + ": " + choice.message.content);
			//}
		}

		yield break;
	}

	void SetModelName(string model)
	{
		curModel = model;
	}

	/// <summary>
	/// Make sure given method is public :)
	/// </summary>
	/// <param name="msg"></param>
	/// <param name="model"></param>
	/// <param name="methodToCall">Mus be public sir</param>
	public void GPTSendCallback(Message msg, string model, Action<Response> methodToCall)
	{
		StartCoroutine(SendGPTMessage(msg, model, methodToCall));
	}

	#region Request Converters
	/// <summary>
	/// Converts a SimpleRequest to a ComplexRequest
	/// </summary>
	/// <param name="rq"></param>
	/// <returns></returns>
	ComplexRequest ConvertRequest(SimpleRequest rq)
	{
		ComplexRequest request = new ComplexRequest();

		// Messages
		request.messages = rq.messages;
		// Model
		request.model = rq.model;
		// Number of responses to recieve
		request.n = 1;
		// Controls response randomness
		request.temperature = 1;
		// Controls response randomness
		request.top_p = 1;

		// Ignore this
		request.stop = null;
		// Ignore this (don't get this confused with name)
		request.user = "";
		// Ignore this
		request.frequency_penalty = 0;
		// Ignore this
		request.presence_penalty = 0;

		if (curModel.Contains("gpt-4-32k"))
		{
			request.max_tokens = 32000;
		}
		else if (curModel.Contains("gpt-4"))
		{
			request.max_tokens = 8192;
		}
		else
		{
			request.max_tokens = 4096;
		}

		return request;
	}

	ComplexRequest CustomRequest(string msg, string role, string model, float temperature, int responseCount, float top_p)
	{
		ComplexRequest request = new ComplexRequest();
		Message[] msgs = new Message[1];

		msgs[0].content = msg;
		msgs[0].role = role;

		// Messages
		request.messages = msgs;
		// Model
		request.model = model;
		// Number of responses to recieve
		request.n = responseCount;
		// Controls response randomness
		request.temperature = temperature;
		// Controls response randomness
		request.top_p = top_p;

		// Ignore this
		request.stop = null;
		// Ignore this (don't get this confused with name)
		request.user = "";
		// Ignore this
		request.frequency_penalty = 0;
		// Ignore this
		request.presence_penalty = 0;

		if (model.Contains("gpt-4-32k"))
		{
			request.max_tokens = 32768;
		}
		else if (model.Contains("gpt-4"))
		{
			request.max_tokens = 8192;
		}
		else
		{
			request.max_tokens = 4096;
		}

		return request;
	}

	// CHECK: Erroring out, check CustomRequest(string msg, string role, string model) as it doesnt error out.
	ComplexRequest CustomRequest(string msg, string role, string model, float temperature)
	{
		ComplexRequest request = new ComplexRequest();
		Message[] msgs = new Message[]
		{
			new Message()
			{
				content = msg,
				role = role
			}
		};

		// Messages
		request.messages = msgs;
		// Model
		request.model = model;
		// Number of responses to recieve
		request.n = 1;
		// Controls response randomness
		request.temperature = temperature;
		// Controls response randomness
		request.top_p = 1;

		// Ignore this
		request.stop = null;
		// Ignore this (don't get this confused with name)
		request.user = "";
		// Ignore this
		request.frequency_penalty = 0;
		// Ignore this
		request.presence_penalty = 0;

		if (curModel.Contains("gpt-4-32k"))
		{
			request.max_tokens = 32767;
		}
		else if (curModel.Contains("gpt-4"))
		{
			request.max_tokens = 8191;
		}
		else
		{
			request.max_tokens = 4095;
		}

		return request;
	}

	ComplexRequest CustomRequest(string msg, string role, string model)
	{
		ComplexRequest request = new ComplexRequest();
		Message[] msgs = new Message[]
		{
			new Message()
			{
				content = msg,
				role = role
			}
		};

		// Messages
		request.messages = msgs;
		// Model
		request.model = model;
		// Number of responses to recieve
		request.n = 1;
		// Controls response randomness
		request.temperature = 1;
		// Controls response randomness
		request.top_p = 1;

		// Ignore this
		request.stop = null;
		// Ignore this (don't get this confused with name)
		request.user = "";
		// Ignore this
		request.frequency_penalty = 0;
		// Ignore this
		request.presence_penalty = 0;

		if (curModel.Contains("gpt-4-32k"))
		{
			request.max_tokens = 32768;
		}
		else if (curModel.Contains("gpt-4"))
		{
			request.max_tokens = 8191;
		}
		else
		{
			request.max_tokens = 4096;
		}

		return request;
	}

	ComplexRequest CustomRequest(string msg, string role)
	{
		ComplexRequest request = new ComplexRequest();
		Message[] msgs = new Message[1];

		msgs[0].content = msg;
		msgs[0].role = role;

		// Messages
		request.messages = msgs;
		// Model
		request.model = curModel;
		// Number of responses to recieve
		request.n = 1;
		// Controls response randomness
		request.temperature = 1;
		// Controls response randomness
		request.top_p = 1;

		// Ignore this
		request.stop = null;
		// Ignore this (don't get this confused with name)
		request.user = "";
		// Ignore this
		request.frequency_penalty = 0;
		// Ignore this
		request.presence_penalty = 0;

		if (curModel.Contains("gpt-4-32k"))
		{
			request.max_tokens = 32768;
		}
		else if (curModel.Contains("gpt-4"))
		{
			request.max_tokens = 8192;
		}
		else
		{
			request.max_tokens = 4096;
		}

		return request;
	}

	ComplexRequest CustomRequest(string msg)
	{
		ComplexRequest request = new ComplexRequest();
		Message[] msgs = new Message[1];

		msgs[0].content = msg;
		msgs[0].role = "user";

		// Messages
		request.messages = msgs;
		// Model
		request.model = curModel;
		// Number of responses to recieve
		request.n = 1;
		// Controls response randomness
		request.temperature = 1;
		// Controls response randomness
		request.top_p = 1;

		// Ignore this
		request.stop = null;
		// Ignore this (don't get this confused with name)
		request.user = "";
		// Ignore this
		request.frequency_penalty = 0;
		// Ignore this
		request.presence_penalty = 0;

		if (curModel.Contains("gpt-4-32k"))
		{
			request.max_tokens = 32768;
		}
		else if (curModel.Contains("gpt-4"))
		{
			request.max_tokens = 8192;
		}
		else
		{
			request.max_tokens = 4096;
		}

		return request;
	}
	#endregion
}