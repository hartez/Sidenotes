using Markdig;
using Markdig.Helpers;
using Markdig.Renderers;

namespace Sidenotes
{
	public static class Extensions
	{
		public static MarkdownPipelineBuilder UseSidenotes(this MarkdownPipelineBuilder pipeline, Action<HtmlRenderer, StringSlice>? renderKind = null, bool allowNestedAlerts = false)
		{
			pipeline.Extensions.AddIfNotAlready<SidenotesExtension>();
			return pipeline;
		}
	}
}

