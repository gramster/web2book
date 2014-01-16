using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace web2book
{
    public class WriteLRF: IHtmlConverter
    {
        public string HtmlHelp { get { return null; } }

        public bool MustScrubHtml
        {
            get { return true; }
        }

        public string Name
        {
            get { return "LRF"; }
        }

        public string Extension
        {
            get { return ".lrf"; }
        }

        public bool IsFileConverter
        {
            get { return true; }
        }

        public bool HasConfigurationUI
        {
            get { return false; }
        }

        public bool HasConfigurablePageSettings
        {
            get { return false; }
        }

        public IParserBasedHtmlConverter Converter
        {
            get { return null; }
        }

        string title, author;

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
            this.author = author;
            this.title = title;
        }

        internal class NativeMethods
        {
            [DllImport("LBWrapper.dll")]
            static public extern void StartBook([MarshalAs(UnmanagedType.LPStr)]string title, [MarshalAs(UnmanagedType.LPStr)] string author, [MarshalAs(UnmanagedType.LPStr)] string fname);

            [DllImport("LBWrapper.dll")]
            static public extern void AddContent([MarshalAs(UnmanagedType.LPStr)]string htmlfile);

            [DllImport("LBWrapper.dll")]
            static public extern void FinishBook();
        }

        public string ConvertHtmlFile(string path, string basename, ref string output)
        {
            output = null;
            string html = Path.Combine(path, basename + ".htm");
            string f = Path.Combine(path, basename + ".lrf");
            if (File.Exists(f)) File.Delete(f);
            try
            {
                string h = "file://" + html;
                h = h.Replace('\\', '/');
                NativeMethods.StartBook(title, author, f);
                NativeMethods.AddContent(h);
                NativeMethods.FinishBook();
            }
            catch (Exception ex)
            {
                output = ex.ToString();
            }
            return f;
        }

        public void Configure()
        {
        }
    }
}

