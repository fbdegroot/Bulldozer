using System.IO;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Web.Optimization;

namespace Bulldozer.Bundling
{
	public class CoffeeTransform : IBundleTransform
	{
		public void Process(BundleContext context, BundleResponse response)
		{
			StringBuilder builder = new StringBuilder();

			foreach (var file in response.Files) {
				string path = HostingEnvironment.MapPath(file.IncludedVirtualPath);
				string content = File.ReadAllText(path);

				switch (Path.GetExtension(path)) {
					case ".js":
						builder.Append("(function() { " + content + " }).call(this);");
						break;
					case ".coffee":
						var result = CoffeeScriptCompiler.Compile(content);
						builder.Append(result.Content + ";");
						break;
				}
			}

			response.Content = builder.ToString();
			response.ContentType = "application/javascript";
			response.Cacheability = HttpCacheability.Public;
		}
	}
}