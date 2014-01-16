using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;


namespace web2book
{
    public class WebContentSource : ContentSource
    {
        protected string url;

        public virtual string Url
        {
            get { return url; }
            set { url = value; }
        }

        public override bool IsStructured // is this structured as a sequence of blocks with headings (i.e. does it make sense to generate a TOC?)
        {
            get { return false; }
        } 

        public override bool IsEmpty
        {
            get
            {
                return (Url == null || Url == String.Empty || base.IsEmpty);
            }
        }

        public WebContentSource()
            : this(String.Empty, String.Empty, false, String.Empty)
        { }

        public WebContentSource(string name, string comment, bool enabled, string url)
            : base(name, comment, enabled)
        {
            Url = url;
        }

        protected override void WriteElements(XmlWriter xw)
        {
            base.WriteElements(xw);
            xw.WriteElementString("Url", Url);
        }

        protected override void SetElement(string name, string value)
        {
            switch (name)
            {
                case "Url":
                    Url = value;
                    break;
                default:
                    base.SetElement(name, value);
                    break;
            }
        }

        public override bool Match(ContentSource cs)
        {
            return (cs is WebContentSource && cs.Name == Name && ((WebContentSource)cs).Url == Url);
        }

        public override string GetHtml(ISource mySource, int displayWidth, int displayHeight, int displayDepth, StringBuilder log)
        {
            return Utils.GetHtml(Url, 0, null, null, null, 0, log);
        }
    }
}
