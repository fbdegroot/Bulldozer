using Bulldozer.Compilers;
using Microsoft.Ajax.Utilities;
using System;
using System.IO;
using System.Linq;

namespace Bulldozer
{
	public class CssHttpHandler : BaseHttpHandler
	{
		public CssHttpHandler()
		{
			ContentType = "text/css";
			CompileExtension = ".less";
		}

		protected override Tasks GetTasks(string path)
		{
			Tasks tasks = new Tasks();

			if (path.EndsWith(".min.less")) {
				if (File.Exists(path.Replace(".min.less", ".less")))
					tasks.SourcePath = path.Replace(".min.less", ".less");
				else if (File.Exists(path.Replace(".min.less", ".css")))
					tasks.SourcePath = path.Replace(".min.less", ".css");
				else
					tasks.NotFound = true;

				tasks.Compile = true;
				tasks.Minify = true;
			}
			else if (path.EndsWith(".less")) {
				if (File.Exists(path))
					tasks.SourcePath = path;
				else if (File.Exists(path.Replace(".less", ".css")))
					tasks.SourcePath = path.Replace(".less", ".css");
				else
					tasks.NotFound = true;

				tasks.Compile = true;
			}
			else if (path.EndsWith(".min.css")) {
				if (File.Exists(path.Replace(".min.css", ".less"))) {
					tasks.SourcePath = path.Replace(".min.css", ".less");
					tasks.Compile = true;
				}
				else if (File.Exists(path.Replace(".min.css", ".css")))
					tasks.SourcePath = path.Replace(".min.css", ".css");
				else
					tasks.NotFound = true;

				tasks.Minify = true;
			}
			else if (path.EndsWith(".css")) {
				if (File.Exists(path.Replace(".css", ".less")))
					tasks.SourcePath = path.Replace(".css", ".less");
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
			return LessCompiler.Compile(content, path);
		}

		protected override string Minify(string content)
		{
			Minifier minifier = new Minifier();
			CssSettings settings = new CssSettings();
			settings.CommentMode = CssComment.None;

			string minifiedContent = minifier.MinifyStyleSheet(content, settings);
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