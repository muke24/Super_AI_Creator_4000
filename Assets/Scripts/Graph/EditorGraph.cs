using UnityEngine;
using XNode;

[CreateAssetMenu]
public class EditorGraph : NodeGraph
{
	public bool isProcessing = false;

	public void StartProcessing()
	{
		isProcessing = true;
		// Add logic to start processing nodes
	}

	public void StopProcessing()
	{
		isProcessing = false;
		// Add logic to stop processing nodes
	}
}