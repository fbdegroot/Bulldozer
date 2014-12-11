using Bulldozer.Compilers;
using Bulldozer.Configuration;
using Bulldozer.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Web.Hosting;

namespace Bulldozer
{
	public abstract class BaseHttpHandler : IHttpHandler
	{
		protected abstract string Minify(string content);

		protected abstract CompileResult Compile(string content, string path);
		protected abstract Tasks GetTasks(string requestPath);

		protected string ContentType { get; set; }
		protected string CompileExtension { get; set; }

		public void ProcessRequest(HttpContext context)
		{
			RequestCache requestCache = context.Cache.Get<RequestCache>(context.Request.Url.AbsolutePath);
			if (requestCache == null) {
				lock (context.Request.Url) {
					requestCache = context.Cache.Get<RequestCache>(context.Request.Url.AbsolutePath);
					if (requestCache == null)
						requestCache = Process(context);
				}
			}

			context.Response.StatusCode = requestCache.StatusCode;

			if (requestCache.StatusCode == 200) {
				TimeSpan maxAge = TimeSpan.FromSeconds(1440 * BulldozerConfiguration.ClientCacheMaxAge);

				context.Response.Expires = 1440 * BulldozerConfiguration.ClientCacheMaxAge; // 1440 = 24 uur
				context.Response.ContentType = requestCache.ContentType;

				if (requestCache.ResultPath != null)
					context.Response.WriteFile(requestCache.ResultPath);
				else
					context.Response.Write(requestCache.Content);

				context.Response.Cache.SetCacheability(HttpCacheability.Public);
				context.Response.Cache.SetValidUntilExpires(true);
				context.Response.Cache.VaryByParams.IgnoreParams = true;
				context.Response.Cache.SetETagFromFileDependencies();
				context.Response.Cache.SetMaxAge(maxAge);

				CacheDependency cacheDependency = new CacheDependency(requestCache.FileDependencies, DateTime.Now);
				if (cacheDependency != null)
					context.Response.AddCacheDependency(cacheDependency);
			}
		}

		private RequestCache Process(HttpContext context)
		{
			string path = HostingEnvironment.MapPath(context.Request.AppRelativeCurrentExecutionFilePath);

			// request direct returnen als het verwijst naar een file op disk
			if (Path.GetExtension(path) != CompileExtension && File.Exists(path))
				return new RequestCache { StatusCode = 200, ContentType = ContentType, SourcePath = path, ResultPath = context.Request.AppRelativeCurrentExecutionFilePath, FileDependencies = new List<string> { path }.ToArray(), Exists = true };

			// bepalen wat er gedaan moet worden
			var tasks = GetTasks(path);
			if (tasks.NotFound) {
				return new RequestCache { StatusCode = 404 };
			}
			else if (tasks.UnknownExtension) {
				// vervangen door een unknown extension exception
				return new RequestCache { StatusCode = 403 };
			}

			// de source processen
			string content = File.ReadAllText(tasks.SourcePath);
			List<string> fileDependencies = new List<string>();
			if (tasks.Compile) {
				CompileResult result = Compile(content, Path.GetDirectoryName(path));
				content = result.Content;
				fileDependencies.AddRange(result.Dependencies);
			}
			if (tasks.Minify) {
				content = Minify(content);
			}

			// opslaan op disk en returnen
			if (BulldozerConfiguration.InMemory == false) {
				string cachePath = Path.Combine(BulldozerConfiguration.CacheDirectory, context.Request.Url.AbsolutePath.ToLower().Substring(1).Replace("/", @"\"));
				Directory.CreateDirectory(Path.GetDirectoryName(cachePath));
				File.WriteAllText(cachePath, content);

				return new RequestCache { StatusCode = 200, ContentType = ContentType, SourcePath = tasks.SourcePath, ResultPath = cachePath, FileDependencies = fileDependencies.ToArray() };
			}

			return new RequestCache { StatusCode = 200, ContentType = ContentType, SourcePath = tasks.SourcePath, Content = content, FileDependencies = fileDependencies.ToArray() };
		}

		public static void RemovedCallback(string key, object value, CacheItemRemovedReason reason)
		{
			RequestCache requestCache = (RequestCache)value;
			if (requestCache.Exists == false && requestCache.ResultPath != null && File.Exists(requestCache.ResultPath))
				File.Delete(requestCache.ResultPath);
		}

		public bool IsReusable
		{
			get { return true; }
		}
	}
}