using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
using static VariableNode;

public class IfNode : Node
{
	public enum Type { Equal, Contain }
	public Type type = Type.Equal;

	[Input]
	public string a;
	[Input]
	public string b;

	public override object GetValue(NodePort port)
	{
		if (type == Type.Equal)
		{
			if (GetInputValue("a", a) == GetInputValue("b", b))
			{
				return "true";
			}
			else
			{
				return "false";
			}
		}
		else
		{
			if (GetInputValue("a", a).Contains(GetInputValue("b", b)))
			{
				return "true";
			}
			else
			{
				return "false";
			}
		}
		
	}
}
