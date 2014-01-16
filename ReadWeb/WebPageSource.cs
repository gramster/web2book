using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using System.Drawing;

namespace web2book
{
    public class WebPage : WebContentSource
    {
        public override string ElementName
        {
            get { return "WebPage"; }
        }

        int numberOfDays;

        public int NumberOfDays
        {
            get { return numberOfDays; }
            set { numberOfDays = value; }
        }

        int depth;

        public int Depth
        {
            get { return depth; }
            set { depth = value; }
        }

        string contentExtractor;

        public string ContentExtractor
        {
            get { return contentExtractor; }
            set { contentExtractor = value; }
        }

        string contentFormatter;

        public string ContentFormatter
        {
            get { return contentFormatter; }
            set { contentFormatter = value; }
        }

        public WebPage()
            : this(String.Empty, String.Empty, false, String.Empty, 0, 1, String.Empty, String.Empty)
        { }

        public WebPage(string name, string comment, bool enabled, string url, int numDays, int depth, string contentExtractor, string contentFormatter)
            : base(name, comment, enabled, url)
        {
            NumberOfDays = numDays;
            Depth = depth;
            ContentExtractor = contentExtractor;
            ContentFormatter = contentFormatter;
        }

        protected override void WriteElements(XmlWriter xw)
        {
            base.WriteElements(xw);
            xw.WriteElementString("Days", XmlConvert.ToString(NumberOfDays));
            xw.WriteElementString("Depth", XmlConvert.ToString(Depth));
            xw.WriteElementString("ContentExtractor", ContentExtractor);
            xw.WriteElementString("ContentFormatter", ContentFormatter);
        }

        protected override void SetElement(string name, string value)
        {
            switch (name)
            {
                case "Days":
                    NumberOfDays = XmlConvert.ToInt32(value);
                    break;
                case "Depth":
                    Depth = XmlConvert.ToInt32(value);
                    break;
                case "ContentExtractor":
                    ContentExtractor = value;
                    break;
                case "ContentFormatter":
                    ContentFormatter = value;
                    break;
                default:
                    base.SetElement(name, value);
                    break;
            }
        }

        public override string GetHtml(ISource mySourceGroup, int displayWidth, int displayHeight, int displayDepth, StringBuilder log)
        {
            return Utils.GetHtml(Url, NumberOfDays, null, ContentExtractor, ContentFormatter, Depth, log);
        }
    }

    public class WebPageSource : WebSource, ISource
    {
        public WebPageSource()
            : base("Web Page", typeof(WebPage), new string[] { "Enabled", "Name", "Url", "Depth" }, true, true, false)
        { }

        public string HtmlHelp { get { return null; } }

        public IContentSourceList DataSource
        {
            get { return new ContentSourceBindingList<WebPage>(this); }
        }

        public string FileNamePrefix { get { return "www"; } }

        public override bool CanPublish { get { return true; } }

        public bool IsType(ContentSource cs)
        {
            return cs is WebPage; 
        }

        public ContentSource MakeSource()
        {
            return new WebPage();
        }

        public override string DescribeDifferences(ContentSource publishedSource, ContentSource mySource)
        {
            WebPage publishedPage = (WebPage)publishedSource;
            WebPage myPage = (WebPage)mySource;
            StringBuilder sb = new StringBuilder();
            AddDiff(sb, "Url", publishedPage.Url.ToLower(), myPage.Url.ToLower(), "");
            AddDiff(sb, "Content Extraction Pattern", publishedPage.ContentExtractor, myPage.ContentExtractor, "(.*)");
            AddDiff(sb, "Content Reformatter", publishedPage.ContentFormatter, myPage.ContentFormatter, "{0}");
            return sb.ToString();
        }

        public override void CustomizeItem(ContentSource cs, EventHandler test, EventHandler publish)
        {
            new WebPageCustomizeForm(cs, test, publish).ShowDialog();
        }
    }

}
