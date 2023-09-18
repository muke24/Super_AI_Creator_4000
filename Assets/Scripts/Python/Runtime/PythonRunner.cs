using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Python.Runtime;
using System;
using System.Linq;
using System.IO;

public static class PythonRunner
{
	public static void RunLines(string[] code, string dll, string pythonHome, string pythonPath)
	{
		try
		{
			Runtime.PythonDLL = dll;
			PythonEngine.PythonHome = pythonHome;
			PythonEngine.PythonPath = pythonPath;
			Environment.SetEnvironmentVariable("PYTHONNET_PYTHON", @"C:\Users\Peter\AppData\Local\Programs\Python\Python38\python.exe");


			PythonEngine.Initialize();
			Debug.Log($"Using python {PythonEngine.Version}");

			using (Py.GIL()) // acquire the Python GIL (Global Interpreter Lock)
			{
				for (int i = 0; i < code.Length; i++)
				{
					PythonEngine.RunSimpleString(code[i]);
				}
			}

		}
		catch (Exception e)
		{
			Debug.LogError("An error occurred: " + e.Message);
		}
		finally
		{
			PythonEngine.Shutdown();
		}
	}

	//public static void RunPythonScript(string filePath, string dll, string pythonHome, string pythonPath)
	//{
	//	try
	//	{
	//		Runtime.PythonDLL = dll;
	//		PythonEngine.PythonHome = pythonHome;
	//		PythonEngine.PythonPath = pythonPath;
	//		Environment.SetEnvironmentVariable("PYTHONNET_PYTHON", @"C:\Users\Peter\AppData\Local\Programs\Python\Python38\python.exe");

	//		PythonEngine.Initialize();
	//		Debug.Log($"Using python {PythonEngine.Version}");

	//		using (Py.GIL())
	//		{
	//			dynamic py = Py.Import("__main__");
	//			PyDict locals = new PyDict();
	//			PyObject result = null;

	//			// Load your Python script
	//			py.exec(File.ReadAllText(filePath), locals);

	//			// Run a Python function and capture its return value
	//			result = locals.GetItem("name_of_your_python_function");
	//			if (result != null)
	//			{
	//				Debug.Log("Result: " + result.ToString());
	//			}
	//		}
	//	}
	//	catch (Exception e)
	//	{
	//		Debug.LogError("An error occurred: " + e.Message);
	//	}
	//	finally
	//	{
	//		PythonEngine.Shutdown();
	//	}
	//}

	public static void RunPythonScript(string dll, string pythonHome, string pythonPath)
	{
		try
		{
			//string projectPath = Directory.GetCurrentDirectory();
			//string assetsPath = @Path.Combine(projectPath, "Assets");
			//Debug.Log("Asset path: " + assetsPath);

			//Runtime.PythonDLL = "C:\\Python38\\python38.dll";
			//PythonEngine.PythonHome = @"C:\Python38";
			//PythonEngine.PythonPath = assetsPath;   //pythonPath;

			//Environment.SetEnvironmentVariable("PYTHONNET_PYTHON", @"C:\Python38\python.exe");

			Environment.SetEnvironmentVariable("PYTHONNET_PYTHON", @"C:\Python38\python.exe");
			PythonEngine.PythonHome = @"C:\Python38";
			PythonEngine.PythonPath = @"C:\Python38\Lib";
			Runtime.PythonDLL = @"C:\Python38\python38.dll";

			PythonEngine.Initialize();
			Debug.Log($"Using python {PythonEngine.Version}");

			using (Py.GIL())
			{
				// Run your Python script from the file
				PyObject py = Py.Import("data_folder_creator");
			}
		}
		catch (Exception e)
		{
			Debug.LogError("Python Error: " + e.Message);
		}
		finally
		{
			PythonEngine.Shutdown();
		}
	}

}
