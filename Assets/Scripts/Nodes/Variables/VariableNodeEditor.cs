using UnityEditor;
using UnityEngine;
using XNode;
using XNodeEditor;

[CustomNodeEditor(typeof(VariableNode))]
public class VariableNodeEditor : NodeEditor
{
	public override void OnBodyGUI()
	{
		base.OnBodyGUI();

		VariableNode node = target as VariableNode;

		EditorGUIUtility.labelWidth = 90;
		node.variableType = (VariableNode.VariableType)EditorGUILayout.EnumPopup("Variable Type", node.variableType);

		// Update the output port type based on the selected variable type
		NodePort outputPort = node.GetPort("output");
		switch (node.variableType)
		{
			case VariableNode.VariableType.String:
				outputPort.ValueType = typeof(string);
				node.stringValue = EditorGUILayout.TextField("String Value", node.stringValue);
				break;
			case VariableNode.VariableType.Float:
				outputPort.ValueType = typeof(float);
				node.floatValue = EditorGUILayout.FloatField("Float Value", node.floatValue);
				break;
			case VariableNode.VariableType.Int:
				outputPort.ValueType = typeof(int);
				node.intValue = EditorGUILayout.IntField("Int Value", node.intValue);
				break;
			case VariableNode.VariableType.Bool:
				outputPort.ValueType = typeof(bool);
				node.boolValue = EditorGUILayout.Toggle("Bool Value", node.boolValue);
				break;
		}
	}
}
