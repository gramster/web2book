using System;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.rtf;

namespace web2book
{
    public class iTextSharpRtfConverter : iTextSharpConverter
    {
        public override string HtmlHelp { get { return null; } }

        public override string Name
        {
            get { return "RTF"; }
        }

        public override string Extension
        {
            get { return ".rtf"; }
        }

        RtfWriter2 w;

        protected override void ConfigureWriterType()
        {
            w = RtfWriter2.GetInstance(myDocument, docStream);
        }

        protected override void AddTOC(Document doc)
        {/*
          * This doesn't work :-(
            w.SetAutogenerateTOCEntries(true);
            Paragraph p = new Paragraph();
            p.Add(new RtfTOC("Table of Contents", new Font()));
            doc.Add(p);
          */ 
        }
    }
}
