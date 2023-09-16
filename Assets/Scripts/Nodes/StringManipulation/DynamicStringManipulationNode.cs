using System.Collections.Generic;
using System.Linq;  // Required for LINQ methods
using UnityEngine;
using XNode;

public class DynamicStringManipulationNode : Node
{
	[Output] public string concatenatedString;
	[Output] public string upperCaseString;
	[Output] public string lowerCaseString;
	[Output] public int totalStringLength;
	[Output] public string reversedString;

	public enum StringOperation { Concatenate, ToUpperCase, ToLowerCase, Length, Reverse }
	public StringOperation stringOperation = StringOperation.Concatenate;

	public int CountConnectedDynamicInputs()
	{
		return DynamicInputs.Count(port => port.IsConnected);
	}

	// Use this to add a new dynamic port
	public void AddDynamicInput()
	{
		int count = DynamicInputs.Count();  // Using LINQ to count elements
		AddDynamicInput(typeof(string), ConnectionType.Multiple, TypeConstraint.None, "inputString" + (count + 1));
	}

	// Use this to remove the last dynamic port
	public void RemoveDynamicInput()
	{
		int count = DynamicInputs.Count();  // Using LINQ to count elements
		if (count > 0)
		{
			NodePort port = DynamicInputs.Last();  // Using LINQ to get the last element
			RemoveDynamicPort(port);
		}
	}

	public override object GetValue(NodePort port)
	{
		List<string> inputStrings = new List<string>();
		foreach (NodePort dynamicPort in DynamicInputs)
		{
			string inputValue = GetInputValue<string>(dynamicPort.fieldName, null);
			if (inputValue != null)
			{
				inputStrings.Add(inputValue);
			}
		}

		if (port.fieldName == "concatenatedString" && stringOperation == StringOperation.Concatenate)
		{
			return string.Join("", inputStrings);
		}
		else if (port.fieldName == "upperCaseString" && stringOperation == StringOperation.ToUpperCase)
		{
			return string.Join("", inputStrings).ToUpper();
		}
		else if (port.fieldName == "lowerCaseString" && stringOperation == StringOperation.ToLowerCase)
		{
			return string.Join("", inputStrings).ToLower();
		}
		else if (port.fieldName == "totalStringLength" && stringOperation == StringOperation.Length)
		{
			return string.Join("", inputStrings).Length;
		}
		else if (port.fieldName == "reversedString" && stringOperation == StringOperation.Reverse)
		{
			char[] charArray = string.Join("", inputStrings).ToCharArray();
			System.Array.Reverse(charArray);
			return new string(charArray);
		}

		return null;
	}
}
