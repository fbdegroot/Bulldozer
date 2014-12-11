using System.IO;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Web.Optimization;

namespace Bulldozer.Bundling
{
	public class LessTransform : IBundleTransform
	{
		public void Process(BundleContext context, BundleResponse response)
		{
			StringBuilder builder = new StringBuilder();

			foreach (var file in response.Files) {
				string path = HostingEnvironment.MapPath(file.IncludedVirtualPath);
				string content = File.ReadAllText(path);

				switch (Path.GetExtension(path)) {
					case ".css":
						builder.Append(content);
						break;
					case ".less":
						var result = LessCompiler.Compile(content, Path.GetDirectoryName(path));
						builder.Append(result.Content);
						break;
				}
			}

			response.Content = builder.ToString();
			response.ContentType = "text/css";
			response.Cacheability = HttpCacheability.Public;
		}
	}
}