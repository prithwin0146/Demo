using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Markdig;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace EmployeeApi.Services
{
    public class DocumentConversionService : IDocumentConversionService
    {
        public byte[] ConvertMarkdownToDocx(string markdownContent)
        {
            var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
            var document = Markdown.Parse(markdownContent, pipeline);

            using var stream = new MemoryStream();
            using (var wordDoc = WordprocessingDocument.Create(stream, WordprocessingDocumentType.Document, true))
            {
                var mainPart = wordDoc.AddMainDocumentPart();
                mainPart.Document = new Document(new Body());
                var body = mainPart.Document.Body!;

                AddStylesPart(mainPart);

                foreach (var block in document)
                {
                    ProcessBlock(block, body);
                }

                // Ensure body has at least one paragraph (Word requirement)
                if (!body.Elements<Paragraph>().Any())
                {
                    body.AppendChild(new Paragraph());
                }

                mainPart.Document.Save();
            }

            return stream.ToArray();
        }

        private static void AddStylesPart(MainDocumentPart mainPart)
        {
            var stylesPart = mainPart.AddNewPart<StyleDefinitionsPart>();
            stylesPart.Styles = new Styles();

            var headingStyles = new[]
            {
                ("Heading1", "Heading 1", 36, true),
                ("Heading2", "Heading 2", 28, true),
                ("Heading3", "Heading 3", 24, true),
                ("Heading4", "Heading 4", 20, true),
                ("Heading5", "Heading 5", 18, true),
                ("Heading6", "Heading 6", 16, true),
            };

            foreach (var (styleId, name, fontSize, bold) in headingStyles)
            {
                var style = new Style { Type = StyleValues.Paragraph, StyleId = styleId };
                style.AppendChild(new StyleName { Val = name });
                var rpr = new StyleRunProperties();
                rpr.AppendChild(new FontSize { Val = fontSize.ToString() });
                if (bold) rpr.AppendChild(new Bold());
                style.AppendChild(rpr);
                stylesPart.Styles.AppendChild(style);
            }

            stylesPart.Styles.Save();
        }

        private static void ProcessBlock(Block block, OpenXmlElement parent)
        {
            switch (block)
            {
                case HeadingBlock heading:
                    parent.AppendChild(CreateHeadingParagraph(heading));
                    break;

                case ParagraphBlock para:
                    parent.AppendChild(CreateParagraph(para.Inline));
                    break;

                case ListBlock list:
                    var itemIndex = 1;
                    foreach (var item in list.OfType<ListItemBlock>())
                    {
                        foreach (var child in item)
                        {
                            if (child is ParagraphBlock itemPara)
                            {
                                var p = CreateParagraph(itemPara.Inline);
                                var ppr = p.PrependChild(new ParagraphProperties());
                                var indentation = new Indentation { Left = "720" };
                                ppr.AppendChild(indentation);

                                var prefix = list.IsOrdered ? $"{itemIndex}. " : "• ";
                                var bulletRun = new Run(new Text(prefix) { Space = SpaceProcessingModeValues.Preserve });
                                p.PrependChild(bulletRun);
                                parent.AppendChild(p);
                            }
                        }
                        itemIndex++;
                    }
                    break;

                case FencedCodeBlock code:
                case CodeBlock code2:
                    var rawCode = block is FencedCodeBlock fcb
                        ? string.Join("\n", fcb.Lines.Lines.Select(l => l.ToString()))
                        : string.Join("\n", ((CodeBlock)block).Lines.Lines.Select(l => l.ToString()));
                    parent.AppendChild(CreateCodeParagraph(rawCode));
                    break;

                case ThematicBreakBlock:
                    parent.AppendChild(CreateHorizontalRule());
                    break;

                case QuoteBlock quote:
                    foreach (var child in quote)
                    {
                        ProcessBlock(child, parent);
                    }
                    break;

                case ContainerBlock container:
                    foreach (var child in container)
                    {
                        ProcessBlock(child, parent);
                    }
                    break;
            }
        }

        private static Paragraph CreateHeadingParagraph(HeadingBlock heading)
        {
            var styleId = $"Heading{Math.Clamp(heading.Level, 1, 6)}";
            var p = new Paragraph();
            var ppr = new ParagraphProperties(new ParagraphStyleId { Val = styleId });
            p.AppendChild(ppr);
            if (heading.Inline != null)
            {
                AppendInlines(heading.Inline, p);
            }
            return p;
        }

        private static Paragraph CreateParagraph(ContainerInline? inlines)
        {
            var p = new Paragraph();
            if (inlines != null)
            {
                AppendInlines(inlines, p);
            }
            return p;
        }

        private static Paragraph CreateCodeParagraph(string code)
        {
            var p = new Paragraph();
            var ppr = new ParagraphProperties();
            ppr.AppendChild(new Indentation { Left = "720" });
            p.AppendChild(ppr);

            var run = new Run();
            var rpr = new RunProperties();
            rpr.AppendChild(new RunFonts { Ascii = "Courier New", HighAnsi = "Courier New" });
            rpr.AppendChild(new FontSize { Val = "18" });
            run.AppendChild(rpr);

            foreach (var line in code.Split('\n'))
            {
                run.AppendChild(new Text(line) { Space = SpaceProcessingModeValues.Preserve });
                run.AppendChild(new Break());
            }
            p.AppendChild(run);
            return p;
        }

        private static Paragraph CreateHorizontalRule()
        {
            var p = new Paragraph();
            var ppr = new ParagraphProperties();
            var pBdr = new ParagraphBorders();
            pBdr.AppendChild(new BottomBorder { Val = BorderValues.Single, Size = 6, Space = 1, Color = "auto" });
            ppr.AppendChild(pBdr);
            p.AppendChild(ppr);
            return p;
        }

        private static void AppendInlines(ContainerInline inlines, Paragraph p)
        {
            foreach (var inline in inlines)
            {
                AppendInline(inline, p, false, false);
            }
        }

        private static void AppendInline(Inline inline, Paragraph p, bool bold, bool italic)
        {
            switch (inline)
            {
                case LiteralInline literal:
                    p.AppendChild(CreateRun(literal.Content.ToString(), bold, italic, false));
                    break;

                case EmphasisInline emphasis:
                    bool isBold = emphasis.DelimiterCount == 2;
                    bool isItalic = emphasis.DelimiterCount == 1;
                    foreach (var child in emphasis)
                    {
                        AppendInline(child, p, bold || isBold, italic || isItalic);
                    }
                    break;

                case CodeInline code:
                    p.AppendChild(CreateRun(code.Content, bold, italic, true));
                    break;

                case LineBreakInline:
                    var brRun = new Run();
                    brRun.AppendChild(new Break());
                    p.AppendChild(brRun);
                    break;

                case LinkInline link:
                    foreach (var child in link)
                    {
                        AppendInline(child, p, bold, italic);
                    }
                    break;

                case ContainerInline container:
                    foreach (var child in container)
                    {
                        AppendInline(child, p, bold, italic);
                    }
                    break;
            }
        }

        private static Run CreateRun(string text, bool bold, bool italic, bool code)
        {
            var run = new Run();
            var rpr = new RunProperties();
            if (bold) rpr.AppendChild(new Bold());
            if (italic) rpr.AppendChild(new Italic());
            if (code)
            {
                rpr.AppendChild(new RunFonts { Ascii = "Courier New", HighAnsi = "Courier New" });
                rpr.AppendChild(new FontSize { Val = "18" });
            }
            if (rpr.HasChildren) run.AppendChild(rpr);
            run.AppendChild(new Text(text) { Space = SpaceProcessingModeValues.Preserve });
            return run;
        }
    }
}
