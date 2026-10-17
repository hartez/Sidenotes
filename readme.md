Sidenotes is a [Markdig](https://xoofx.github.io/markdig/) extension which renders Markdown [footnotes](https://www.markdownguide.org/extended-syntax/#footnotes) notation as sidenotes (numbered notes in the margin of the page) rather than endnotes (numbered notes as a list at the end of the page). 

The markdown footnote format looks something like this:

```markdown
This is some text. At the end of this sentence is a footnote link.[^1]

[^1]: Elsewhere in the document, the definition of the footnote.
```

Markdig includes an extension to handle footnotes and render them as endnotes; this includes a superscript link to the note, and a list of all the notes in the document at the end of the document. Each note at the end of the document contains a link back to the point in the document where it is referenced.

This extension replaces the included footnote extension. It follows the same syntax, but renders the notes in the HTML at the reference site. The rendered notes look like this:

```html
<span class="sidenote"><sup class="sidenote-ref">1</sup><span class="sidenote-content"><span class="sidenote-ref">1</span>This is the text of the note.</span></span>
```

To render them as sidenotes requires CSS. Here's an example:

```css
/* Small screens */
.sidenote-content {
    float: left;
    width: 100%;
    margin: 1.5rem 0px;
    position: relative;
    text-align: left;
    padding-left: 5%;
}

/* Wider screens (> 992px) */
@include media-breakpoint-up(lg) {
	.sidenote-content {
		float: right;
		clear: both;
		width: 9rem;
		margin-right: calc(-9rem - 2rem);
		position: relative;
	}
}

/* Widest screens (> 1200px) */
@include media-breakpoint-up(xl) {
	.sidenote-content {
		float: right;
		clear: both;
		width: 14rem;
		margin-right: calc(-14rem - 2rem);
		position: relative;
	}
}
```

On smaller screens (with tiny margins), the sidenotes are rendered inline with the rest of the text with a slight inset. 

Once the screen is wide enough (as determined by a media query), the `sidenote-content` portions of the note are floated to the right of the container and given a _negative_ right margin; this pulls them into the margin of the page. 

This extension is not compatible with the Footnotes extension, so adding it to your pipeline will remove the Footnotes extension if it's already been added. 