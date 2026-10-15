using Markdig;
using Markdig.Renderers;

namespace SideNotes
{
	public class SidenoteExtension : IMarkdownExtension
	{
		public void Setup(MarkdownPipelineBuilder pipeline)
		{
			if (!pipeline.BlockParsers.Contains<SidenoteParser>())
			{
				// Insert the parser before any other parsers
				pipeline.BlockParsers.Insert(0, new SidenoteParser());
			}
		}

		public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
		{
			if (renderer is HtmlRenderer htmlRenderer)
			{
				htmlRenderer.ObjectRenderers.AddIfNotAlready(new HtmlSidenoteRenderer());
			}
		}
	}
}

