namespace Bulldozer
{
	internal class RequestCache
	{
		public bool Exists { get; set; }
		public string SourcePath { get; set; }
		public string ResultPath { get; set; }
		public string Content { get; set; }
		public string ContentType { get; set; }
		public int StatusCode { get; set; }
		public string[] FileDependencies { get; set; }
	}
}