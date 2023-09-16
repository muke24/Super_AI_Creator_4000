using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Python.Runtime;

public static class PythonRunner
{
	public static void RunPythonCode()
	{
		PythonEngine.Initialize();

		using (Py.GIL()) // acquire the Python GIL (Global Interpreter Lock)
		{
			dynamic py = Py.CreateScope();
			PythonEngine.RunString("import sys", py);
			PythonEngine.RunString("print(sys.version)", py);
		}

		PythonEngine.Shutdown();
	}

}
