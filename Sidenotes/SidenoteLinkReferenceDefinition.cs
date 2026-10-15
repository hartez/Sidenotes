using Markdig.Syntax;

namespace Sidenotes
{
	/// <summary>
	/// Initializes a new instance of the SidenoteLinkReferenceDefinition class.
	/// </summary>
	public class SidenoteLinkReferenceDefinition(Sidenote sidenote) : LinkReferenceDefinition
	{

		/// <summary>
		/// Gets or sets the footnote related to this link reference definition.
		/// </summary>
		public Sidenote Sidenote { get; set; } = sidenote;
	}
}

