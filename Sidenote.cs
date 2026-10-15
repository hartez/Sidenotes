using Markdig.Extensions.Footnotes;
using Markdig.Parsers;
using Markdig.Syntax;

namespace SideNotes
{
	public class Sidenote : ContainerBlock
	{
		/// <summary>
		/// Initializes a new instance of the Footnote class.
		/// </summary>
		public Sidenote(BlockParser parser) : base(parser)
		{
			Order = -1;
		}

		/// <summary>
		/// Gets or sets the label used by this footnote.
		/// </summary>
		public string? Label { get; set; }

		/// <summary>
		/// Gets or sets the order of this footnote (determined by the order of the <see cref="FootnoteLink"/> in the document)
		/// </summary>
		public int Order { get; set; }

		/// <summary>
		/// Gets the links referencing this footnote.
		/// </summary>
		public List<SidenoteLink> Links { get; } = new();

		/// <summary>
		/// The label span
		/// </summary>
		public SourceSpan LabelSpan;

		internal bool IsLastLineEmpty { get; set; }
	}
}

