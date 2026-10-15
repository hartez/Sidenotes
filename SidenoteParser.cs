using Markdig.Helpers;
using Markdig.Parsers;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace Sidenotes
{
	public class SidenoteParser : BlockParser
	{
		/// <summary>
		/// The key used to store at the document level the pending <see cref="SidenoteGroup"/>
		/// </summary>
		private static readonly object DocumentKey = typeof(Sidenote);

		/// <summary>
		/// Initializes a new instance of the SidenoteParser class.
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
			// We expect sidenote to appear only at document level and not indented more than a code indent block
			var currentContainer = processor.GetCurrentContainerOpened();
			if (processor.IsCodeIndent || (!isContinue && currentContainer.GetType() != typeof(MarkdownDocument)) || (isContinue && !(currentContainer is SidenoteGroup)))
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

			// Maintain a list of all sidenotes at document level (so we can clean them up later)
			var notes = processor.Document.GetData(DocumentKey) as SidenoteGroup;
			if (notes == null)
			{
				notes = new SidenoteGroup(this);
				processor.Document.Add(notes);
				processor.Document.SetData(DocumentKey, notes);
				processor.Document.ProcessInlinesEnd += Document_ProcessInlinesEnd;
			}
			notes.Add(note);

			var linkRef = new SidenoteLinkReferenceDefinition(note)
			{
				CreateLinkInline = CreateLinkToSidenote,
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

		private void Document_ProcessInlinesEnd(InlineProcessor state, Inline? inline)
		{
			var notes = (SidenoteGroup)state.Document.GetData(DocumentKey)!;

			// Remove the sidenotes from the document 
			state.Document.Remove(notes);
			state.Document.RemoveData(DocumentKey);

			notes.Sort(
			(leftObj, rightObj) =>
			{
				var left = (Sidenote)leftObj;
				var right = (Sidenote)rightObj;

				return left.Order >= 0 && right.Order >= 0
					? left.Order.CompareTo(right.Order)
					: 0;
			});

			int linkIndex = 0;
			for (int i = 0; i < notes.Count; i++)
			{
				var note = (Sidenote)notes[i];
				if (note.Order < 0)
				{
					// Remove this note if it doesn't have any links
					notes.RemoveAt(i);
					i--;
					continue;
				}

				foreach (var link in note.Links)
				{
					linkIndex++;
					link.Index = linkIndex;
				}
			}
		}

		private static Inline CreateLinkToSidenote(InlineProcessor state, LinkReferenceDefinition linkRef, Inline? child)
		{
			var note = ((SidenoteLinkReferenceDefinition)linkRef).Sidenote;
			if (note.Order < 0)
			{
				var notes = (SidenoteGroup)state.Document.GetData(DocumentKey)!;
				notes.CurrentOrder++;
				note.Order = notes.CurrentOrder;
			}

			var link = new SidenoteLink(note);
			note.Links.Add(link);

			return link;
		}
	}
}

