using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Request Values")]
public class RequestNodeStoredValues : ScriptableObject
{
	public List<Values> storedValues;

	[Serializable]
	public class Values
	{
		public string model = "gpt-4";
		public float temperature = 1;
		public string content = "Hello";

		public string response;
	}
}
