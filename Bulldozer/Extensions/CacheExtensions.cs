using System.Web.Caching;

namespace Bulldozer.Extensions
{
	public static class CacheExtensions
	{
		public static T Get<T>(this Cache cache, string key)
		{
			object item = cache[key];
			if (item == null)
				return default(T);

			return (T)item;
		}
	}
}