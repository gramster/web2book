using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace web2book
{
    public class WriteHtmlDoc : IHtmlConverter
    {
        public string Name
        {
            get { return "HtmlDoc PDF"; }
        }

        public bool MustScrubHtml
        {
            get { return false; }
        }

        public string HtmlHelp
        {
            get { return null; } 
        }

        public string Extension
        {
            get { return ".pdf"; }
        }

        public bool IsFileConverter
        {
            get { return true; }
        }

        public bool HasConfigurationUI
        {
            get { return true; }
        }

        public bool HasConfigurablePageSettings
        {
            get { return true; }
        }

        public IParserBasedHtmlConverter Converter
        {
            get { return null; }
        }

        public string HtmlDocPath
        {
            get
            {
                if (Settings.Default.HtmlDocPath == null || Settings.Default.HtmlDocPath.Length == 0)
                    Settings.Default.HtmlDocPath = @"c:\Program Files\Easy Software Products\HTMLDOC";
                return Settings.Default.HtmlDocPath;
            }
            set
            {
                Settings.Default.HtmlDocPath = value;
            }
        }

        public string HtmlDocOptions
        {
            get
            {
                if (Settings.Default.HtmlDocOptions == null || Settings.Default.HtmlDocOptions.Length == 0)
                    Settings.Default.HtmlDocOptions = @"--links --toclevels 2 --gray --header ... --footer ... --textcolor black --verbose --no-title";
                return Settings.Default.HtmlDocOptions;
            }
            set
            {
                Settings.Default.HtmlDocOptions = value;
            }
        }

        string commandArgs;

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
            string[] FontNames = { "Courier", "Helvetica", "TimesRoman" };
            commandArgs = String.Format("--left {0}mm --right {1}mm --top {2}mm --bottom {3}mm --size {4}x{5}mm --textfont {6} --fontsize {7} {8} {9}",
                                    leftMargin,
                                    rightMargin,
                                    topMargin,
                                    bottomMargin,
                                    pageWidth,
                                    pageHeight,
                                    FontNames[(int)font],
                                    fontSize,
                                    (isStructured ? " --book" : " --webpage"),
                                    HtmlDocOptions);
        }

        public string ConvertHtmlFile(string path, string basename, ref string output)
        {
            if (!HasPrerequisites()) return null;

            output = null;
            string f = Path.Combine(path, basename + ".pdf");
            if (File.Exists(f)) File.Delete(f);
            if (HtmlDocPath.Length > 0)
            {
                try
                {
                    Utils.RunExternalCommand(Path.Combine(HtmlDocPath, "htmldoc.exe"), commandArgs + " -f \"" + basename + ".pdf\" \"" + basename + ".htm\"", path, false, 5 * 60 * 1000, out output);
                }
                catch (Exception ex)
                {
                    output = ex.ToString();
                }
            }
            return f;
        }

        public void Configure()
        {
            new HtmlDocConfigForm(this).ShowDialog();
            HasPrerequisites();
        }

        bool HasPrerequisites()
        {
            if (!File.Exists(HtmlDocPath + "\\htmldoc.exe"))
            {
                if (MessageBox.Show("It appears you either do not have htmldoc installed or the path is set incorrectly in your options.\r\n\r\nIf you do not have htmldoc, you can get it here:\r\n\r\n http://www.paehl.com/open_source/?HTMLDOC_1.8.x_OpenSource_Version\r\n\r\nWould you like to go there now?", "Cannot find htmldoc", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    Utils.OpenUrl("http://www.paehl.com/open_source/?HTMLDOC_1.8.x_OpenSource_Version");
                }
                return false;
            }
            return true;
        }
    }
}
