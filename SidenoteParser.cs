using Markdig.Extensions.Footnotes;
using Markdig.Helpers;
using Markdig.Parsers;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace SideNotes
{
	public class SidenoteParser : BlockParser
	{

		/// <summary>
		/// The key used to store at the document level the pending <see cref="FootnoteGroup"/>
		/// </summary>
		private static readonly object DocumentKey = typeof(Sidenote);

		/// <summary>
		/// Initializes a new instance of the FootnoteParser class.
		/// </summary>
		public SidenoteParser()
		{
			OpeningCharacters = ['['];
		}

		/// <summary>
		/// Attempts to open a block at the current parser position.
		/// </summary>
		public override BlockState TryOpen(BlockProcessor processor)
		{
			return TryOpen(processor, false);
		}

		private BlockState TryOpen(BlockProcessor processor, bool isContinue)
		{
			// We expect footnote to appear only at document level and not indented more than a code indent block
			var currentContainer = processor.GetCurrentContainerOpened();
			if (processor.IsCodeIndent || (!isContinue && currentContainer.GetType() != typeof(MarkdownDocument)) || (isContinue && !(currentContainer is FootnoteGroup)))
			{
				return BlockState.None;
			}

			var saved = processor.Column;
			int start = processor.Start;
			if (!LinkHelper.TryParseLabel(ref processor.Line, false, out string? label, out SourceSpan labelSpan) || !label.StartsWith("^") || processor.CurrentChar != ':')
			{
				processor.GoToColumn(saved);
				return BlockState.None;
			}

			// Advance the column
			int deltaColumn = processor.Start - start;
			processor.Column += deltaColumn;

			processor.NextChar(); // Skip ':'

			var note = new Sidenote(this)
			{
				Label = label,
				LabelSpan = labelSpan,
				Column = processor.Column,
				Span = new SourceSpan(processor.Start, processor.Line.End),
			};

			// Maintain a list of all footnotes at document level
			var footnotes = processor.Document.GetData(DocumentKey) as FootnoteGroup;
			if (footnotes == null)
			{
				footnotes = new FootnoteGroup(this);
				processor.Document.Add(footnotes);
				processor.Document.SetData(DocumentKey, footnotes);
				processor.Document.ProcessInlinesEnd += Document_ProcessInlinesEnd;
			}
			footnotes.Add(note);

			var linkRef = new SidenoteLinkReferenceDefinition(note)
			{
				CreateLinkInline = CreateLinkToFootnote,
				Line = processor.LineIndex,
				Column = saved,
				Span = new SourceSpan(start, processor.Start - 2), // account for ]:
				LabelSpan = labelSpan,
				Label = label
			};
			processor.Document.SetLinkReferenceDefinition(note.Label, linkRef, true);
			processor.NewBlocks.Push(note);
			return BlockState.Continue;
		}

		/// <summary>
		/// Attempts to continue parsing the specified block.
		/// </summary>
		public override BlockState TryContinue(BlockProcessor processor, Block block)
		{
			var footnote = (Sidenote)block;

			if (processor.CurrentBlock == null || processor.CurrentBlock.IsBreakable)
			{
				if (processor.IsBlankLine)
				{
					footnote.IsLastLineEmpty = true;
					return BlockState.ContinueDiscard;
				}

				if (processor.Column == 0)
				{
					if (footnote.IsLastLineEmpty)
					{
						// Close the current footnote
						processor.Close(footnote);

						// Parse any opening footnote
						return TryOpen(processor);
					}

					// Make sure that consecutive footnotes without a blanklines are parsed correctly
					if (TryOpen(processor, true) == BlockState.Continue)
					{
						processor.Close(footnote);
						return BlockState.Continue;
					}
				}
			}
			footnote.IsLastLineEmpty = false;

			if (processor.IsCodeIndent)
			{
				processor.GoToCodeIndent();
			}

			return BlockState.Continue;
		}

		/// <summary>
		/// Add footnotes to the end of the document
		/// </summary>
		/// <param name="state">The processor.</param>
		/// <param name="inline">The inline.</param>
		private void Document_ProcessInlinesEnd(InlineProcessor state, Inline? inline)
		{
			var footnotes = (FootnoteGroup)state.Document.GetData(DocumentKey)!;
			// Remove the footnotes from the document and readd them at the end
			state.Document.Remove(footnotes);
			state.Document.Add(footnotes);
			state.Document.RemoveData(DocumentKey);

			footnotes.Sort(
				(leftObj, rightObj) =>
				{
					var left = (Sidenote)leftObj;
					var right = (Sidenote)rightObj;

					return left.Order >= 0 && right.Order >= 0
						? left.Order.CompareTo(right.Order)
						: 0;
				});

			int linkIndex = 0;
			for (int i = 0; i < footnotes.Count; i++)
			{
				var footnote = (Sidenote)footnotes[i];
				if (footnote.Order < 0)
				{
					// Remove this footnote if it doesn't have any links
					footnotes.RemoveAt(i);
					i--;
					continue;
				}

				// Insert all footnote backlinks
				var paragraphBlock = footnote.LastChild as ParagraphBlock;
				if (paragraphBlock is null)
				{
					paragraphBlock = new ParagraphBlock();
					footnote.Add(paragraphBlock);
				}

				paragraphBlock.Inline ??= new ContainerInline();

				foreach (var link in footnote.Links)
				{
					linkIndex++;
					link.Index = linkIndex;
					var backLink = new SidenoteLink(footnote)
					{
						Index = linkIndex,
						IsBackLink = true
					};
					paragraphBlock.Inline.AppendChild(backLink);
				}
			}
		}

		private static Inline CreateLinkToFootnote(InlineProcessor state, LinkReferenceDefinition linkRef, Inline? child)
		{
			var footnote = ((SidenoteLinkReferenceDefinition)linkRef).Footnote;
			if (footnote.Order < 0)
			{
				var footnotes = (FootnoteGroup)state.Document.GetData(DocumentKey)!;
				footnotes.CurrentOrder++;
				footnote.Order = footnotes.CurrentOrder;
			}

			var link = new SidenoteLink(footnote);
			footnote.Links.Add(link);

			return link;
		}
	}
}

