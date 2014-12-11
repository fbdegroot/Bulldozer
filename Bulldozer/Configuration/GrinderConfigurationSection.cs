using System.Configuration;

namespace Bulldozer.Configuration
{
	public sealed class BulldozerConfigurationSection : ConfigurationSection
	{
		[ConfigurationProperty("cacheDirectory", IsRequired = false)]
		public string CacheDirectory
		{
			get { return (string)base["cacheDirectory"]; }
		}

		[ConfigurationProperty("clientCacheMaxAge", IsRequired = false)]
		public int ClientCacheMaxAge
		{
			get { return (int)base["clientCacheMaxAge"]; }
		}
	}
}