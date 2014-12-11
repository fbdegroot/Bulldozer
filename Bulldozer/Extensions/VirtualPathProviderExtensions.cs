using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;

namespace Bulldozer.Extensions
{
	public static class VirtualPathProviderExtensions
	{
		public static string ReadFile(this VirtualPathProvider virtualPathProvider, string virtualPath)
		{
			string fileContent = null;
			using (Stream fileStream = virtualPathProvider.GetFile(virtualPath).Open())
			using (StreamReader reader = new StreamReader(fileStream)) {
				fileContent = reader.ReadToEnd();
			}
			return fileContent;
		}
	}
}
