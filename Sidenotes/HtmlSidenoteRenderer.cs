using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;

namespace Sidenotes
{
	public class HtmlSidenoteRenderer : HtmlObjectRenderer<SidenoteLink>
	{
		/// <summary>
		/// Initializes a new instance of the HtmlFootnoteLinkRenderer class.
		/// </summary>
		public HtmlSidenoteRenderer()
		{
			SidenoteClass = "sidenote";
			SidenoteRefClass = "sidenote-ref";
			SidenoteContentClass = "sidenote-content";
		}
		/// <summary>
		/// Gets or sets the class for the sidenote reference
		/// </summary>
		public string SidenoteRefClass { get; set; }

		/// <summary>
		/// Gets or sets the sidenote class.
		/// </summary>
		public string SidenoteClass { get; set; }

		/// <summary>
		/// Gets or sets the sidenote content class.
		/// </summary>
		public string SidenoteContentClass { get; set; }

		/// <summary>
		/// Writes the object to the specified renderer.
		/// </summary>
		protected override void Write(HtmlRenderer renderer, SidenoteLink link)
		{
			var content = link.Sidenote.LastChild as ParagraphBlock;

			if ((content?.Inline) == null)
			{
				return;
			}

			var index = link.Index;

			renderer.Write($"<span class=\"{SidenoteClass}\">");

			renderer.Write($"<sup class=\"{SidenoteRefClass}\">{index}</sup>");

			renderer.Write($"<span class=\"{SidenoteContentClass}\">");

			renderer.Write($"<span class=\"{SidenoteRefClass}\">{index}</span>");

			renderer.WriteChildren(content.Inline);

			renderer.Write($"</span>");

			renderer.Write($"</span>");
		}
	}
}

