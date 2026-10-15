using Markdig.Parsers;
using Markdig.Syntax;

namespace Sidenotes
{
	/// <summary>
	/// Initializes a new instance of the Sidenote class.
	/// </summary>
	public class Sidenote(BlockParser parser) : ContainerBlock(parser)
	{
		/// <summary>
		/// Gets or sets the label used by this sidenote.
		/// </summary>
		public string? Label { get; set; }

		/// <summary>
		/// Gets or sets the order of this sidenote (determined by the order of the <see cref="SidenoteLink"/> in the document)
		/// </summary>
		public int Order { get; set; } = -1;

		/// <summary>
		/// Gets the links referencing this sidenote.
		/// </summary>
		public List<SidenoteLink> Links { get; } = new();

		/// <summary>
		/// The label span
		/// </summary>
		public SourceSpan LabelSpan;

		internal bool IsLastLineEmpty { get; set; }
	}
}

