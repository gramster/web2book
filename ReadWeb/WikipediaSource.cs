using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace web2book
{
    public class WikipediaArticle : WebContentSource
    {
        public override string ElementName
        {
            get { return "WikipediaArticle"; }
        }

        public override bool IsStructured // is this structured as a sequence of blocks with headings (i.e. does it make sense to generate a TOC?)
        {
            get { return depth > 1; }
        } 

        const string contentExtractor = @"<!-- start content -->(.*)<!-- end content -->";

        int depth;

        public int Depth
        {
            get { return depth; }
            set { depth = value; }
        }

        public override string Url
        {
            get { url = "http://en.wikipedia.org/wiki/" + Name.Replace(" ", "_"); return url; }
            set {  }
        }

        public WikipediaArticle()
            : this(String.Empty, String.Empty, false, 1)
        { }

        public WikipediaArticle(string name, string comment, bool enabled, int depth)
            : base(name, comment, enabled, null)
        {
            Depth = depth;
        }

        protected override void WriteElements(XmlWriter xw)
        {
            base.WriteElements(xw);
            xw.WriteElementString("Depth", XmlConvert.ToString(Depth));
        }

        protected override void SetElement(string name, string value)
        {
            switch (name)
            {
                case "Depth":
                    Depth = XmlConvert.ToInt32(value);
                    break;
                default:
                    base.SetElement(name, value);
                    break;
            }
        }


        public override string GetHtml(ISource mySourceGroup, int displayWidth, int displayHeight, int displayDepth, StringBuilder log)
        {
            return Utils.GetHtml(Url, 0, null, contentExtractor, null, Depth, log);
        }
    }

    public class WikipediaSource : WebSource, ISource
    {
        public WikipediaSource()
            : base("Wikipedia Article", typeof(WikipediaArticle), new string[] { "Enabled", "Name", "Depth" }, false, false, false)
        { }

        public string HtmlHelp { get { return null; } }

        public IContentSourceList DataSource
        {
            get { return new ContentSourceBindingList<WikipediaArticle>(this); }
        }

        public bool IsType(ContentSource cs)
        {
            return cs is WikipediaArticle;
        }

        public ContentSource MakeSource()
        {
            return new WikipediaArticle();
        }

        public string FileNamePrefix { get { return "wikipedia"; } }
    }
}
