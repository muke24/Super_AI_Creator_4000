using static XNode.Node;
using XNode;
using System.Collections.Generic;
using System.Linq;

public class MathNode : Node
{
	public bool stringSpaces = true;
	[Output]
	public string output;

	public MathType mathType = MathType.Add;
	public enum MathType { Add, Subtract, Multiply, Divide }

	NodePort firstInput = null;

	// Create a variable to count the dynamic variables
	private int dynamicVariableCount = 0;

	// Initialize the output port when the node is created
	protected override void Init()
	{
		base.Init();

		//connections = new List<object>();

		firstInput = AddDynamicInput(typeof(string), fieldName: "New Variable");


		NodePort outputPort = GetPort("output");
		outputPort.ValueType = typeof(string); // Set to a default type
	}

	public void ProcessNode()
	{

	}

	public override void OnCreateConnection(NodePort from, NodePort to)
	{
		List<NodePort> ports = new List<NodePort>();
		for (int i = 0; i < Ports.Count(); i++)
		{
			if (Ports.ToArray()[i] != GetPort("output"))
			{
				ports.Add(Ports.ToArray()[i]);
			}
		}

		if (to == firstInput)
		{
			firstInput.ValueType = from.ValueType;
			dynamicVariableCount++;
			firstInput.fieldName = "Variable " + dynamicVariableCount;
			UpdatePorts();

			AddDynamicInput(from.ValueType, fieldName: "New Variable");
		}
		else
		{
			if (from.ValueType != firstInput.ValueType)
			{
				from.Disconnect(to);
			}
			else
			{
				// Rename the existing "New Variable" to "Variable #"
				dynamicVariableCount++;
				GetPort("New Variable").fieldName = "Variable " + dynamicVariableCount;
				UpdatePorts();


				// Add a new dynamic input named "New Variable"
				AddDynamicInput(from.ValueType, fieldName: "New Variable");
			}
		}
		UpdatePorts();
		base.OnCreateConnection(from, to);
	}

	public override void OnRemoveConnection(NodePort port)
	{
		if (port != GetPort("output"))
		{
			RemoveDynamicPort(port);
			// Update the count if a dynamic variable port is removed
			if (port.fieldName.StartsWith("Variable "))
			{
				dynamicVariableCount--;
			}
			UpdatePorts();
		}
		base.OnRemoveConnection(port);
	}

	public override object GetValue(NodePort port)
	{
		NodePort outputPort = GetPort("output");
		outputPort.ValueType = firstInput.ValueType; // Set to a default type

		if (firstInput.ValueType == typeof(string))
		{
			if (mathType == MathType.Add)
			{
				List<NodePort> inputPorts = Inputs.ToList();
				string outputString = "";
				for (int i = 0; i < inputPorts.Count; i++)
				{
					if (stringSpaces && i != 0)
					{
						outputString += " " + (string)inputPorts[i].GetInputValue();
					}
					else
					{
						outputString += (string)inputPorts[i].GetInputValue();
					}
				}
				output = outputString;
				return outputString;
			}
			else
			{
				return null;
			}
		}
		return null;

		//float a = GetInputValue<float>("a", this.a);
		//float b = GetInputValue<float>("b", this.b);

		//if (port.fieldName == "result")
		//{
		//	switch (mathType)
		//	{
		//		case MathType.Add: default: return a + b;
		//		case MathType.Subtract: return a - b;
		//		case MathType.Multiply: return a * b;
		//		case MathType.Divide: return a / b;
		//	}
		//}
		//else if (port.fieldName == "sum") return a + b;
		//else return 0f;
	}
}
