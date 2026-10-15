using Markdig.Syntax;

namespace SideNotes
{
	public class SidenoteLinkReferenceDefinition : LinkReferenceDefinition
	{
		/// <summary>
		/// Initializes a new instance of the FootnoteLinkReferenceDefinition class.
		/// </summary>
		public SidenoteLinkReferenceDefinition(Sidenote footnote)
		{
			Footnote = footnote;
		}

		/// <summary>
		/// Gets or sets the footnote related to this link reference definition.
		/// </summary>
		public Sidenote Footnote { get; set; }
	}
}

