using UnityEngine;
using XNode;

public class VariableNode : Node
{
	//public string nodeName;

	public enum VariableType { String, Float, Int, Bool }

	[HideInInspector]
	public VariableType variableType = VariableType.String; // Initialize to a default type

	[HideInInspector]
	public string stringValue;

	[HideInInspector]
	public float floatValue;

	[HideInInspector]
	public int intValue;

	[HideInInspector]
	public bool boolValue;

	[Output]
	public string output;

	// Initialize the output port when the node is created
	protected override void Init()
	{
		base.Init();
		NodePort outputPort = GetPort("output");
		outputPort.ValueType = typeof(string); // Set to a default type
	}

	public override object GetValue(NodePort port)
	{
		switch (variableType)
		{
			case VariableType.String:
				output = stringValue;
				return stringValue;
			case VariableType.Float:
				output = floatValue.ToString();
				return floatValue;
			case VariableType.Int:
				output = intValue.ToString();
				return intValue;
			case VariableType.Bool:
				output = boolValue.ToString();
				return boolValue;
			default:
				return stringValue;
		}
	}
}
