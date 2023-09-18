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

	// Initialize the output port when the node is created
	protected override void Init()
	{
		base.Init();

		if (Inputs == null || Inputs.ToArray().Length < 1)
		{
			AddDynamicInput(typeof(string));
		}

		NodePort outputPort = GetPort("output");
		outputPort.ValueType = typeof(string); // Set to a default type
	}

	public override void OnCreateConnection(NodePort from, NodePort to)
	{
		NodePort[] inputPorts = Inputs.ToArray();

		// If the connecting port is the connecting node's first port
		if (to == inputPorts[0])
		{
			AddDynamicInput(from.ValueType);
		}
		else
		{
			if (from.ValueType != inputPorts[0].ValueType)
			{
				from.Disconnect(to);
			}
			else
			{
				AddDynamicInput(from.ValueType);
			}
		}

		CheckPorts();

		UpdatePorts();
		base.OnCreateConnection(from, to);
	}

	void CheckPorts()
	{
		NodePort[] inputPorts = Inputs.ToArray();
		for (int i = 0; i < inputPorts.Length; i++)
		{
			if (inputPorts[i].Connection == null)
			{

				if (i != inputPorts.Length - 1)
				{
					RemoveDynamicPort(inputPorts[i]);

				}
			}
		}
	}

	public override void OnRemoveConnection(NodePort port)
	{
		//CheckPorts();
		base.OnRemoveConnection(port);


		if (port != GetPort("output"))
		{
			UpdatePorts();

			RemoveDynamicPort(port);

		}

		UpdatePorts();
		
	}

	public override object GetValue(NodePort port)
	{
		NodePort[] inputPorts = Inputs.ToArray();
		NodePort outputPort = GetPort("output");
		outputPort.ValueType = inputPorts[0].ValueType; // Set to a default type

		if (inputPorts[0].ValueType == typeof(string))
		{
			if (mathType == MathType.Add)
			{
				string outputString = "";
				for (int i = 0; i < inputPorts.Length; i++)
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
	}
}
