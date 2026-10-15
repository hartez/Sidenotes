using Markdig.Syntax.Inlines;

namespace SideNotes
{
	public class SidenoteLink : Inline
	{
		/// <summary>
		/// Initializes a new instance of the FootnoteLink class.
		/// </summary>
		/// </summary>
		public SidenoteLink(Sidenote footnote)
		{
			Footnote = footnote;
		}

		/// <summary>
		/// Gets or sets a value indicating whether this instance is back link (from a footnote to the link)
		/// </summary>
		public bool IsBackLink { get; set; }

		/// <summary>
		/// Gets or sets the global index number of this link.
		/// </summary>
		public int Index { get; set; }

		/// <summary>
		/// Gets or sets the footnote this link refers to.
		/// </summary>
		public Sidenote Footnote { get; set; }
	}
}

