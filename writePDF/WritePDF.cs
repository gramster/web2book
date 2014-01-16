using System;
using System.Collections;
using System.Collections.Generic;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.rtf;

namespace web2book
{
    public class iTextSharpPdfConverter : iTextSharpConverter
    {
        // no nested table support yet

        Stack<ArrayList> rowStack = new Stack<ArrayList>();
        Stack<PdfPTable> tableStack = new Stack<PdfPTable>();
        Stack<int> colCountStack = new Stack<int>();
        PdfPTable table;
        ArrayList cells;
        int cols;

        public override string HtmlHelp { get { return null; } }

        public override string Name
        {
            get { return "PDF"; }
        }

        public override string Extension
        {
            get { return ".pdf"; }
        }

        protected override void ConfigureWriterType()
        {
            PdfWriter w = PdfWriter.GetInstance(myDocument, docStream);
        }

        protected override bool FlushParagraph(Paragraph p)
        {
            if (cells != null && cells.Count > 0)
            {
                PdfPCell c = (PdfPCell)cells[cells.Count-1];
                c.AddElement(p);
                return true;
            }
            return false;
        }

        public override void StartTable()
        {
            if (cols != 0)
            {
                tableStack.Push(table);
                colCountStack.Push(cols);
                rowStack.Push(cells);
            }
            cols = 0;
            cells = null;
            table = null;
        }

        public override void StartRow()
        {
            cells = new ArrayList();
        }

        public override void StartCell(int span)
        {
            PdfPCell c = new PdfPCell();
            if (span > 1) c.Colspan = span;
            cells.Add(c);
            cols += span;
        }
        
        public override void EndCell()
        {
        }
        
        public override void EndRow()
        {
            if (table == null)
            {
                table = new PdfPTable(cols);
            }
            for (int i = 0; i < cells.Count; i++)
            {
                table.AddCell((PdfPCell)cells[i]);
            }
            cells = null;
        }

        public override void EndTable()
        {
            if (tableStack.Count > 0)
            {
                cells = rowStack.Pop();
                if (cells.Count > 0)
                {
                    PdfPCell c = (PdfPCell)cells[cells.Count - 1];
                    if (table != null) c.AddElement(table);
                }
                table = tableStack.Pop();
                cols = colCountStack.Pop();
            }
            else
            {
                if (table != null) 
                    myDocument.Add(table);
                table = null;
                cells = null;
                cols = 0;
            }
        }
    }
}
