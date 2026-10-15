using Markdig.Syntax.Inlines;

namespace Sidenotes
{
	/// <summary>
	/// Initializes a new instance of the SidenoteLink class.
	/// </summary>
	/// </summary>
	public class SidenoteLink(Sidenote sidenote) : Inline
	{
		/// <summary>
		/// Gets or sets the global index number of this link.
		/// </summary>
		public int Index { get; set; }

		/// <summary>
		/// Gets or sets the sidenote this link refers to.
		/// </summary>
		public Sidenote Sidenote { get; set; } = sidenote;
	}
}

