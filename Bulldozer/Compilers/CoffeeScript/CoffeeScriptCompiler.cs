using Bulldozer.Compilers;
using Bulldozer.JavaScriptRuntime.InternetExplorer;
using Bulldozer.JavaScriptRuntime.InternetExplorer.ActiveScript;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Bulldozer
{
	public static class CoffeeScriptCompiler
	{
		private static string coffee;
		private static string compile;

		static CoffeeScriptCompiler()
		{
			Assembly assembly = Assembly.GetAssembly(typeof(CoffeeScriptCompiler));

			using (Stream stream = assembly.GetManifestResourceStream("Bulldozer.Compilers.CoffeeScript.CoffeeScript.js"))
			using (StreamReader reader = new StreamReader(stream)) {
				coffee = reader.ReadToEnd();
			}

			using (Stream stream = assembly.GetManifestResourceStream("Bulldozer.Compilers.CoffeeScript.Compile.js"))
			using (StreamReader reader = new StreamReader(stream)) {
				compile = reader.ReadToEnd();
			}
		}

		public static CompileResult Compile(string code)
		{
			return Compile(code, null);
		}

		public static CompileResult Compile(string code, string path)
		{
			CompileResult result = new CompileResult();
			result.Dependencies.Add(path);

			try {
				using (var runtime = new InternetExplorerJavascriptRuntime()) {
					runtime.Load(coffee);
					runtime.Load(compile);

					var output = runtime.ExecuteFunction<string>("Compile", code, false, true);
					if (output != null)
						result.Content = output.Trim();
				}
			}
			catch (Exception ex) {
				result.Content = ex.Message;
			}

			return result;
		}
	}
}