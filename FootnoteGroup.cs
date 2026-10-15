using Markdig.Parsers;
using Markdig.Syntax;

namespace SideNotes
{
	public class FootnoteGroup : ContainerBlock
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="FootnoteGroup"/> class.
		/// </summary>
		/// <param name="parser">The parser used to create this block.</param>
		public FootnoteGroup(BlockParser parser) : base(parser)
		{
		}

		internal int CurrentOrder { get; set; }
	}
}

