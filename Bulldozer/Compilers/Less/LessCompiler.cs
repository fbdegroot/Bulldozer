using Bulldozer.Compilers;
using Bulldozer.JavaScriptRuntime.InternetExplorer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Bulldozer
{
	public static class LessCompiler
	{
		private static string globals;
		private static string es5;
		private static string less;
		private static string compile;

		private static Regex regex = new Regex(@"(@import ""?(.+?)""?;)", RegexOptions.IgnoreCase);

		static LessCompiler()
		{
			Assembly assembly = Assembly.GetAssembly(typeof(LessCompiler));

			using (Stream stream = assembly.GetManifestResourceStream("Bulldozer.Compilers.Less.Globals.js"))
			using (StreamReader reader = new StreamReader(stream))
				globals = reader.ReadToEnd();

			using (Stream stream = assembly.GetManifestResourceStream("Bulldozer.Compilers.Less.ES5.js"))
			using (StreamReader reader = new StreamReader(stream))
				es5 = reader.ReadToEnd();

			using (Stream stream = assembly.GetManifestResourceStream("Bulldozer.Compilers.Less.Less.js"))
			using (StreamReader reader = new StreamReader(stream))
				less = reader.ReadToEnd();

			using (Stream stream = assembly.GetManifestResourceStream("Bulldozer.Compilers.Less.Compile.js"))
			using (StreamReader reader = new StreamReader(stream))
				compile = reader.ReadToEnd();
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
				if (path != null) {
					IEnumerable<Match> matches = from Match match in regex.Matches(code) select match;
					foreach (Match match in matches.Reverse()) {
						string include = match.Groups[2].Value;
						if (include.EndsWith(".less") == false)
							include += ".less";

						string includePath = Path.Combine(path, include);

						string content = File.ReadAllText(includePath);
						code = code.Remove(match.Index, match.Length)
								   .Insert(match.Index, content);

						result.Dependencies.Add(includePath);
					}
				}

				using (var runtime = new InternetExplorerJavascriptRuntime()) {
					runtime.Load(globals);
					runtime.Load(es5);
					runtime.Load(less);
					runtime.Load(compile);

					var output = runtime.ExecuteFunction<string>("Compile", code);
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