using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;

namespace SideNotes
{
	public class HtmlSidenoteRenderer : HtmlObjectRenderer<SidenoteLink>
	{
		/// <summary>
		/// Initializes a new instance of the HtmlFootnoteLinkRenderer class.
		/// </summary>
		public HtmlSidenoteRenderer()
		{
			BackLinkString = "&#8617;";
			FootnoteLinkClass = "footnote-ref";
			FootnoteBackLinkClass = "footnote-back-ref";
		}
		/// <summary>
		/// Gets or sets the back link string.
		/// </summary>
		public string BackLinkString { get; set; }

		/// <summary>
		/// Gets or sets the footnote link class.
		/// </summary>
		public string FootnoteLinkClass { get; set; }

		/// <summary>
		/// Gets or sets the footnote back link class.
		/// </summary>
		public string FootnoteBackLinkClass { get; set; }

		/// <summary>
		/// Writes the object to the specified renderer.
		/// </summary>
		protected override void Write(HtmlRenderer renderer, SidenoteLink link)
		{
			var order = link.Footnote.Order;


			if (!link.IsBackLink)
			{
				var p = link.Footnote.LastChild as ParagraphBlock;

				if (p?.Inline != null )
				{
					renderer.Write($"<span class=\"sidenote\">");
					renderer.WriteChildren(p.Inline);
					renderer.Write($"</span>");
				}
			}

			//renderer.Write(link.IsBackLink
			//	? $"<a href=\"#fnref:{link.Index}\" class=\"{FootnoteBackLinkClass}\">{BackLinkString}</a>"
			//	: $"<a id=\"fnref:{link.Index}\" href=\"#fn:{order}\" class=\"{FootnoteLinkClass}\"><sup>{order}</sup></a>");
		}
	}
}

