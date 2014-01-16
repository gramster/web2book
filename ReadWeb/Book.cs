using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace web2book
{
    public abstract class Book : WebContentSource
    {
        public override string ElementName
        {
            get { return "Book"; }
        }

        string oldAuthor, oldName;

        public override string Url
        {
            get {
                if (url == null || url.Length == 0 || oldAuthor != Author || oldName != Name)
                {
                    oldName = Name;
                    oldAuthor = author;
                    url = GetUrl(author, Name);
                }
                return url;                
            }
            set { }
        }

/*        public override string ContentExtractor
        {
            get
            {
                //return @"</pre>.*(<h1>.*)<pre>"; // Gutenberg HTML
                return null;
            }
            set
            {
            }
        }
        */
        string author;

        public override string Author
        {
            get { return author; }
            set { author = value; }
        }

        public Book()
            : this(String.Empty, String.Empty, String.Empty, false)
        { }

        public Book(string title, string comment, string author, bool enabled)
            : base(title, comment, enabled, null)
        {
            Author = author;
        }

        protected override void WriteElements(XmlWriter xw)
        {
            base.WriteElements(xw);
            xw.WriteElementString("Author", Author);
        }

        protected override void SetElement(string name, string value)
        {
            switch (name)
            {
                case "Author":
                    Author = value;
                    break;
                default:
                    base.SetElement(name, value);
                    break;
            }
        }

        protected abstract string GetUrl(string author, string title);
    }
}
