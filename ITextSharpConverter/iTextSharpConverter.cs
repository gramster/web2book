using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.rtf;

namespace web2book
{
    public abstract class iTextSharpConverter : IHtmlConverter, IParserBasedHtmlConverter
    {
        protected Document myDocument;
        protected MemoryStream docStream;
        Font[,] fonts;
        Stack<Phrase> phrases;
        Stack<List> lists;
        int headerLevel;
        TypeFace defaultFont;
        bool isStructured;
        int chapterNum, sectionNum;
        Chapter chapter;
        Section section;
        Paragraph headerParagraph;

        public virtual string HtmlHelp { get { return null; } }

        public bool MustScrubHtml
        {
            get { return false; }
        }

        public TypeFace DefaultFont
        {
            get { return defaultFont; }
        }

        float mm2pt(int mm)
        {
            return (float)(72.0 / 25.4 * (double)mm);
        }

        public virtual string Name
        {
            get { return null; }
        }

        public virtual string Extension
        {
            get { return null; }
        }

        public bool IsFileConverter
        {
            get { return false; }
        }

        public bool HasConfigurationUI
        {
            get { return false; }
        }

        public bool HasConfigurablePageSettings
        {
            get { return true; }
        }

        public void Configure()
        {
        }
       
        public void Initialize(
                string author,
                string title,
                int leftMargin,
                int rightMargin,
                int topMargin,
                int bottomMargin,
                int pageWidth,
                int pageHeight,
                TypeFace font,
                int fontSize,
                int displayWidth, // display res if synching to device, else 0,0
                int displayHeight,
                int displayDepth, 
                bool isStructured
                )
        {
            Debug.WriteLine(String.Format("Making new document, pagewidth={0}, pageheight={1}, leftMargin={2}, rightMargin={3}, topMargin={4}, bottomMargin={5}",
                mm2pt(pageWidth), mm2pt(pageHeight), mm2pt(leftMargin), mm2pt(rightMargin), mm2pt(topMargin), mm2pt(bottomMargin)));
            defaultFont = font;
            myDocument = new Document(new Rectangle(mm2pt(pageWidth), mm2pt(pageHeight)), mm2pt(leftMargin), mm2pt(rightMargin), mm2pt(topMargin), mm2pt(bottomMargin));
            docStream = new MemoryStream();
            ConfigureWriterType();
            myDocument.Open();
            InitFonts(font, fontSize);
            lists = new Stack<List>();
            phrases = new Stack<Phrase>();
            phrases.Push(new Phrase());
            this.isStructured = true;
            if (isStructured) AddTOC(myDocument);
            chapterNum = sectionNum = 1;
            chapter = null;
            section = null;
        }

        protected virtual void AddTOC(Document doc)
        {
        }

        public void PageBreak()
        {
            myDocument.NewPage();
        }

        public string ConvertHtmlFile(string path, string basename, ref string log)
        {
            Debug.Fail("Not a file converter");
            return null;
        }

        public IParserBasedHtmlConverter Converter
        {
            get { return this; }
        }

        protected abstract void ConfigureWriterType();

        void FlushSection()
        {
            if (section != null)
            {
                if (chapter != null)
                    chapter.Add(section);
                else
                    myDocument.Add(section);
                section = null;
            }
        }

        void FlushChapter()
        {
            FlushSection();
            if (chapter != null)
            {
                myDocument.Add(chapter);
                chapter = null;
            }
        }

        public void EnterHeader(int level)
        {
            if (isStructured)
            {
                if (level == 1)
                    FlushChapter();
                else if (level == 2)
                    FlushSection();
            }
            headerLevel = level;
        }

        public void ExitHeader()
        {
            if (isStructured)
            {
                if (headerLevel == 1)
                {
                    chapter = new Chapter(headerParagraph, chapterNum++);
                    sectionNum = 1;
                }
                else if (headerLevel == 2)
                {
                    section = chapter.AddSection(headerParagraph, sectionNum++);
                }
            }
            headerLevel = 0;
        }

        protected void InitFonts(TypeFace font, int fontSize)
        {
            fonts = new Font[8, 6];
            int face = 0;
            switch (font)
            {
                case TypeFace.Courier: // courier
                    face = Font.COURIER;
                    break;
                case TypeFace.Helvetica: // helvetica
                    face = Font.HELVETICA;
                    break;
                case TypeFace.TimesRoman: // times roman
                    face = Font.TIMES_ROMAN;
                    break;
            }

            for (int i = 0; i < 8; i++)
            {
                int f = Font.NORMAL;
                if ((i & (int)FontStyle.Italic) != 0) f |= Font.ITALIC;
                if ((i & (int)FontStyle.Bold) != 0) f |= Font.BOLD;
                if ((i & (int)FontStyle.Underline) != 0) f |= Font.UNDERLINE;

                for (int j = 0; j < 6; j++)
                {
                    int sz = fontSize;
                    if (j > 0)
                    {
                        if (j < 5) sz += 1 * (4 - j);
                        f |= Font.BOLD;
                    }
                    fonts[i, j] = new Font(face, (float)sz, f);
                    Debug.WriteLine(String.Format("fonts[{0},{1}] = Font({2}, {3}, {4})", i, j, face, sz, f));
                }
            }
        }

        public void HandleText(string text, TypeFace face, FontStyle style)
        {
            Debug.WriteLine(String.Format("Adding new chunk, font={0} text=\"{1}\"", fonts[(int)style, headerLevel], text));
            Chunk c = new Chunk(text, fonts[(int)style, headerLevel]);
            phrases.Peek().Add(c);
        }

        protected virtual bool FlushParagraph(Paragraph p)
        {
            return false;
        }

        void FlushParagraph(Stack<Phrase> phrases, float spacingBefore, float spacingAfter, float leading)
        {
            Debug.WriteLine(String.Format("Making paragraph from popped phrase, spacingBefore={0}, spacingAfter={1}, leading={2}", spacingBefore, spacingAfter, leading));
            Paragraph p = new Paragraph(phrases.Pop());
            p.SpacingAfter = spacingAfter;
            p.SpacingBefore = spacingBefore;
            p.Leading = leading;
            if (!FlushParagraph(p))
            {
                if (isStructured && headerLevel != 0)
                    headerParagraph = p;
                else if (section != null)
                    section.Add(p);
                else if (chapter != null)
                    chapter.Add(p);
                else
                    myDocument.Add(p);
            }
            phrases.Push(new Phrase());
        }

        void FlushParagraph(Stack<Phrase> phrases, float fontSize)
        {
            FlushParagraph(phrases, fontSize / 2, fontSize / 2, fontSize * 1.25F);
        }

        public void FlushParagraph()
        {
            FlushParagraph(phrases, fonts[0, headerLevel].Size);
        }

        public void LineBreak()
        {
            float fontSize = fonts[0, headerLevel].Size;
            FlushParagraph(phrases, fontSize / 2, 0, fontSize * 1.25F);
        }

        public void StartUnorderedList()
        {
            lists.Push(new List(false, 20));
        }

        public void StartOrderedList()
        {
            lists.Push(new List(true, 20));
        }

        public void FlushListItem()
        {
            lists.Peek().Add(new ListItem(phrases.Pop()));
            phrases.Push(new Phrase());
        }

        public void EndList()
        {
            lists.Peek().Add(phrases.Pop());
            myDocument.Add(lists.Pop());
            phrases.Push(new Phrase());
        }

        public virtual void StartTable()
        {
        }

        public virtual void StartRow()
        {
        }

        public virtual void StartCell(int span)
        {
        }

        public virtual void EndCell()
        {
        }

        public virtual void EndRow()
        {
        }

        public virtual void EndTable()
        {
        }

        public void AddImage(string fname)
        {
            Image img = null;
            try
            {
                img = Image.GetInstance(fname);
                if (img != null)
                {
                    float maxW = myDocument.PageSize.Width - myDocument.LeftMargin - myDocument.RightMargin;
                    float maxH = myDocument.PageSize.Height - myDocument.TopMargin - myDocument.BottomMargin;
                    float wScale = 1.0F, hScale = 1.0F;
                    if (img.Width > maxW)
                    {
                        wScale = maxW / img.Width;
                    }
                    if (img.Height > maxH)
                    {
                        hScale = maxH / img.Height;
                    }
                    if (wScale < hScale) hScale = wScale;
                    if (hScale < 1.0) img.ScaleAbsolute(hScale * img.Width, hScale * img.Height);
                    myDocument.Add(img);
                }
            }
            catch
            {
            }

        }

        public byte[] GetBytes()
        {
            FlushChapter();
            myDocument.Close();
            byte[] rtn = docStream.ToArray();
            docStream.Close();
            return rtn;
        }
    }
}
