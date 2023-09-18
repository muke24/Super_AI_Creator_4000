using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using XNode;
using System;

public static class GPTUnityInteraction
{
	public static void CreateScript(string ScriptName)
	{
		if (!AssetDatabase.IsValidFolder("Assets/GPT_Scripts"))
		{
			AssetDatabase.CreateFolder("Assets", "GPT_Scripts");
			Debug.Log("Creating GPT script folder");

		}

		Debug.Log("Creating GPT script file");

		// Create a TextAsset in the Resources folder
		string path = $"Assets/GPT_Scripts/{ScriptName}.cs";
		string content = "";
		File.WriteAllText(path, content);

		// Refresh the asset database to recognize the new asset
		AssetDatabase.Refresh();
	}

	public static void AddClassToScript(string scriptPath, string className)
	{
		Debug.Log($"Adding {className} class to {scriptPath}");

		// Read the existing content of the file
		string existingContent = File.ReadAllText(scriptPath);

		// Create a new class definition
		string newClassContent = $@"
public class {className}
{{
    // TODO: Add your class implementation here
}}
";

		// Append the new class definition to the existing content
		string updatedContent = existingContent + newClassContent;

		// Write the updated content back to the file
		File.WriteAllText(scriptPath, updatedContent);

		// Refresh the asset database to recognize the updated asset
		AssetDatabase.Refresh();
	}

	public static void AddClassToScript(string scriptPath, string className, string baseClassName = null)
	{
		// Read the existing content of the file
		string existingContent = File.ReadAllText(scriptPath);

		// Create the "extends" clause if a base class name is provided
		string extendsClause = string.IsNullOrEmpty(baseClassName) ? "" : $" : {baseClassName}";

		// Create a new class definition
		string newClassContent = $@"
public class {className}{extendsClause}
{{
    // TODO: Add your class implementation here
}}
";

		// Append the new class definition to the existing content
		string updatedContent = existingContent + newClassContent;

		// Write the updated content back to the file
		File.WriteAllText(scriptPath, updatedContent);

		// Refresh the asset database to recognize the updated asset
		AssetDatabase.Refresh();
	}


	public static void AddMethodInClass(string scriptPath, string className, string methodName, string methodBody)
	{
		string existingContent = File.ReadAllText(scriptPath);

		// Find the class definition
		string pattern = $@"public class {className}\s*{{([\s\S]*?)}}";
		Match match = Regex.Match(existingContent, pattern);

		if (match.Success)
		{
			string classContent = match.Groups[1].Value;

			// Add the new method
			string newMethodContent = $@"
    public void {methodName}()
    {{
        {methodBody}
    }}
";
			string updatedClassContent = classContent + newMethodContent;

			// Replace the old class content with the updated one
			string updatedContent = Regex.Replace(existingContent, pattern, $"public class {className}{{ {updatedClassContent}}}");

			// Write the updated content back to the file
			File.WriteAllText(scriptPath, updatedContent);

			// Refresh the asset database to recognize the updated asset
			AssetDatabase.Refresh();
		}
		else
		{
			Debug.LogError($"Class {className} not found in {scriptPath}");
		}
	}

	public static void AddMethodBody(string scriptPath, string className, string methodName, string newMethodBody)
	{
		string existingContent = File.ReadAllText(scriptPath);

		// Find the class definition
		string classPattern = $@"public class {className}\s*{{([\s\S]*?)}}";
		Match classMatch = Regex.Match(existingContent, classPattern);

		if (classMatch.Success)
		{
			string classContent = classMatch.Groups[1].Value;

			// Find the method definition
			string methodPattern = $@"public void {methodName}\s*\(\)\s*{{([\s\S]*?)}}";
			Match methodMatch = Regex.Match(classContent, methodPattern);

			if (methodMatch.Success)
			{
				string methodContent = methodMatch.Groups[1].Value;

				// Replace the old method body with the new one
				string updatedMethodContent = Regex.Replace(classContent, methodPattern, $"public void {methodName}(){{\n        {newMethodBody}\n    }}");

				// Replace the old class content with the updated one
				string updatedContent = Regex.Replace(existingContent, classPattern, $"public class {className}{{\n{updatedMethodContent}\n}}");

				// Write the updated content back to the file
				File.WriteAllText(scriptPath, updatedContent);

				// Refresh the asset database to recognize the updated asset
				AssetDatabase.Refresh();
			}
			else
			{
				Debug.LogError($"Method {methodName} not found in class {className}");
			}
		}
		else
		{
			Debug.LogError($"Class {className} not found in {scriptPath}");
		}
	}

	public static string[] GetScriptNamesFromResponse(string response)
	{
		// Parse the response to get a dictionary of script names and base classes
		Dictionary<string, string> scriptsAndBases = ParseScriptAndBaseClasses(response);
		List<string> scriptNames = new List<string>();
		// Create a script and add a class for each script name found
		foreach (var pair in scriptsAndBases)
		{
			string scriptName = pair.Key;
			scriptNames.Add(scriptName);
		}

		return scriptNames.ToArray();
	}

	public static void CreateScriptsAndClassesFromResponse(string response, CreateScriptsAndClassesNode node)
	{
		// Parse the response to get a dictionary of script names and base classes
		Dictionary<string, string> scriptsAndBases = ParseScriptAndBaseClasses(response);

		// Create a script and add a class for each script name found
		foreach (var pair in scriptsAndBases)
		{
			string scriptName = pair.Key;
			string baseName = pair.Value;

			// Create a new script
			CreateScript(scriptName);

			// Add a new class to the script, specifying the base class if any
			string scriptPath = $"Assets/GPT_Scripts/{scriptName}.cs";
			AddClassToScript(scriptPath, scriptName, baseName);
		}

		// Refresh the asset database to recognize the new assets
		AssetDatabase.Refresh();

		node.isProcessing = false;
	}

	public static Dictionary<string, string> ParseScriptAndBaseClasses(string response)
	{
		Dictionary<string, string> scriptBasePairs = new Dictionary<string, string>();

		// Use Regex to extract script and base class pairs
		MatchCollection matches = Regex.Matches(response, @"\*\*(.*?)\*\*");

		foreach (Match match in matches)
		{
			string pair = match.Groups[1].Value;

			// Split the pair by ':' to separate the script name and its base class
			string[] nameAndBase = pair.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);

			string scriptName = nameAndBase[0].Trim();
			string baseName = nameAndBase.Length > 1 ? nameAndBase[1].Trim() : null;

			scriptBasePairs.Add(scriptName, baseName);
		}

		return scriptBasePairs;
	}
}
