using Markdig.Parsers;
using Markdig.Syntax;

namespace Sidenotes
{
	public class SidenoteGroup(BlockParser parser) : ContainerBlock(parser)
	{
		internal int CurrentOrder { get; set; }
	}
}

