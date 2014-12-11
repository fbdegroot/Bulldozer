using System.IO;
using System.Web.Configuration;

namespace Bulldozer.Configuration
{
	public static class BulldozerConfiguration
	{
		public static string CacheDirectory { get; set; }
		public static bool InMemory { get; set; }
		public static int ClientCacheMaxAge { get; set; }

		static BulldozerConfiguration()
		{
			BulldozerConfigurationSection config = WebConfigurationManager.GetSection("Bulldozer") as BulldozerConfigurationSection;

			if (config != null) {
				InMemory = string.IsNullOrWhiteSpace(config.CacheDirectory);
				CacheDirectory = Path.Combine(config.CacheDirectory, "BulldozerCache");
				ClientCacheMaxAge = config.ClientCacheMaxAge;
			}
			else {
				// defaults
				InMemory = true;
				ClientCacheMaxAge = 30;
			}
		}
	}
}