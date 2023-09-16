using UnityEditor;
using XNodeEditor;
using System.Linq;

[CustomNodeEditor(typeof(DynamicStringManipulationNode))]
public class DynamicStringManipulationNodeEditor : NodeEditor
{
	public override void OnBodyGUI()
	{
		base.OnBodyGUI();

		DynamicStringManipulationNode node = target as DynamicStringManipulationNode;

		int connectedInputs = node.CountConnectedDynamicInputs();
		int totalInputs = node.DynamicInputs.Count();

		// If no strings are attached, show one input
		if (connectedInputs == 0 && totalInputs == 0)
		{
			node.AddDynamicInput();
		}
		// If there are strings attached, show an extra input
		else if (connectedInputs >= totalInputs)
		{
			node.AddDynamicInput();
		}
		// If there are more total inputs than connected inputs, remove the extra
		else if (totalInputs > connectedInputs + 1)
		{
			node.RemoveDynamicInput();
		}
	}
}
