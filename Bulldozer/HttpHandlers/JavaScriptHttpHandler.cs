using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web.Hosting;
using System.Linq;
using Bulldozer.Compilers;

namespace Bulldozer
{
	public class JavaScriptHttpHandler : BaseHttpHandler
	{
		public JavaScriptHttpHandler()
		{
			ContentType = "application/javascript";
			CompileExtension = ".coffee";
		}

		protected override Tasks GetTasks(string path)
		{
			Tasks tasks = new Tasks();

			if (path.EndsWith(".min.coffee")) {
				if (File.Exists(path.Replace(".min.coffee", ".coffee")))
					tasks.SourcePath = path.Replace(".min.coffee", ".coffee");
				else if (File.Exists(path.Replace(".min.coffee", ".js")))
					tasks.SourcePath = path.Replace(".min.coffee", ".js");
				else
					tasks.NotFound = true;

				tasks.Compile = true;
				tasks.Minify = true;
			}
			else if (path.EndsWith(".coffee")) {
				if (File.Exists(path))
					tasks.SourcePath = path;
				else if (File.Exists(path.Replace(".coffee", ".js")))
					tasks.SourcePath = path.Replace(".coffee", ".js");
				else
					tasks.NotFound = true;

				tasks.Compile = true;
			}
			else if (path.EndsWith(".min.js")) {
				if (File.Exists(path.Replace(".min.js", ".coffee"))) {
					tasks.SourcePath = path.Replace(".min.js", ".coffee");
					tasks.Compile = true;
				}
				else if (File.Exists(path.Replace(".min.js", ".js")))
					tasks.SourcePath = path.Replace(".min.js", ".js");
				else
					tasks.NotFound = true;

				tasks.Minify = true;
			}
			else if (path.EndsWith(".js")) {
				if (File.Exists(path.Replace(".js", ".coffee")))
					tasks.SourcePath = path.Replace(".js", ".coffee");
				else
					tasks.NotFound = true;

				tasks.Compile = true;
			}
			else
				tasks.UnknownExtension = true;

			return tasks;
		}

		protected override CompileResult Compile(string content, string path)
		{
			return CoffeeScriptCompiler.Compile(content, path);
		}
		
		protected override string Minify(string content)
		{
			Minifier minifier = new Minifier();
			CodeSettings settings = new CodeSettings();
			settings.EvalTreatment = EvalTreatment.MakeImmediateSafe;
			settings.PreserveImportantComments = false;

			string minifiedContent = minifier.MinifyJavaScript(content, settings);
			if (minifier.ErrorList.Count == 0)
				return minifiedContent;

			return string.Format(@"Minification failed, returning unminified content.{0}Errors:{0}{2}{0}{1}",
				Environment.NewLine,
				content,
				string.Join(Environment.NewLine, minifier.ErrorList.Select(x => x.ToString()))
			);
		}
	}
}