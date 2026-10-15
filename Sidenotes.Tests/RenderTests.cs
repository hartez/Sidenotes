using Markdig;

namespace Sidenotes.Tests
{
	public class RenderTests
	{
		[Fact]
		public void NotesShouldRenderAsSpan()
		{
					string markdown = @"
This is a paragraph.[^1]

[^1]: First sidenote
";

			var pipeline = new MarkdownPipelineBuilder().UseSidenotes().Build();
			var html = Markdown.ToHtml(markdown, pipeline);

			Assert.Contains("<span class=\"sidenote\">", html);
		}

		[Fact]
		public void NotesShouldAllowMultipleLines()
		{
							string markdown = @"
This is a paragraph.[^1]

[^1]: First line of sidenote
	Second line of sidenote
";
			var pipeline = new MarkdownPipelineBuilder().UseSidenotes().Build();
			var html = Markdown.ToHtml(markdown, pipeline);

			Assert.Contains("First line of sidenote", html);
			Assert.Contains("Second line of sidenote</span>", html);
		}

		[Fact]
		public void NotesWithoutRefsShouldNotBeOutput()
		{
					string markdown = @"
This is a paragraph.

This is also a paragraph.[^2]

[^1]: First sidenote
[^2]: Second sidenote
";

			var pipeline = new MarkdownPipelineBuilder().UseSidenotes().Build();
			var html = Markdown.ToHtml(markdown, pipeline);

			Assert.DoesNotContain("First sidenote", html);
		}

		[Fact]
		public void MultipleNotesShouldHaveIncrementingRefNumbers()
		{
			string markdown = @"
This is a paragraph.[^1]

This is also a paragraph.[^2]

[^1]: First sidenote
[^2]: Second sidenote
";
			var pipeline = new MarkdownPipelineBuilder().UseSidenotes().Build();
			var html = Markdown.ToHtml(markdown, pipeline);

			var firstExpectedRef = "<span class=\"sidenote-ref\">1</span>";
			var secondExpectedRef = "<span class=\"sidenote-ref\">2</span>";

			Assert.Contains(firstExpectedRef, html);
			Assert.Contains(secondExpectedRef, html);
		}
	}
}
