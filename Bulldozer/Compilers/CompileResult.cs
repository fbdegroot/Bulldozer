using System.Collections.Generic;

namespace Bulldozer.Compilers
{
	public class CompileResult
	{
		public string Content { get; set; }
		public List<string> Dependencies { get; set; }

		public CompileResult()
		{
			Dependencies = new List<string>();
		}
	}
}