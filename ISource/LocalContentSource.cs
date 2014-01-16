using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace web2book
{
    [Serializable]
    public abstract class LocalContentSource : ContentSource
    {
        string path;

        public string Path
        {
            get { return path; }
            set { path = value; }
        }

        public override bool IsEmpty
        {
            get
            {
                return (Path == null || Path == String.Empty || base.IsEmpty);
            }
        }

        public override bool Match(ContentSource cs)
        {
            return (cs is LocalContentSource) && ((LocalContentSource)cs).Path == Path && base.Match(cs);
        }

        public LocalContentSource()
            : this(String.Empty, String.Empty, false, String.Empty)
        { }

        public LocalContentSource(string name, string comment, bool enabled, string path)
            : base(name, comment, enabled)
        {
            Path = path;
        }

        protected override void WriteElements(XmlWriter xw)
        {
            base.WriteElements(xw);
            xw.WriteElementString("Path", Path);
        }

        protected override void SetElement(string name, string value)
        {
            switch (name)
            {
                case "Path":
                    Path = value;
                    break;
                default:
                    base.SetElement(name, value);
                    break;
            }
        }
    }
}
