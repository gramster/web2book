using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;


namespace web2book
{
    [Serializable]
    public abstract class ContentSource
    {
        string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        string comment;

        public string Comment
        {
            get { return comment; }
            set { comment = value; }
        }

        bool enabled;

        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }

        public virtual string ElementName
        {
            get { return String.Empty; }
        }

        public virtual string Author
        {
            get { return "web2book"; }
            set { }
        }

        public virtual bool IsEmpty
        {
            get
            {
                return (Name == null || Name == String.Empty);
            }
        }

        public virtual bool Match(ContentSource cs)
        {
            return cs.ElementName == ElementName && cs.Name == Name;
        }

        public virtual bool IsStructured // is this structured as a sequence of blocks with headings (i.e. does it make sense to generate a TOC?)
        {
            get { return false; } 
        } 

        public ContentSource()
            : this(String.Empty, String.Empty, false)
        { }

        public ContentSource(string name, string comment, bool enabled)
        {
            Name = name;
            Comment = comment;
            Enabled = enabled;
        }

        protected virtual void WriteElements(XmlWriter xw)
        {
            xw.WriteElementString("Name", Name);
            xw.WriteElementString("Comment", Comment);
            xw.WriteElementString("Enabled", XmlConvert.ToString(Enabled));
        }

        protected virtual void SetElement(string name, string value)
        {
            switch (name)
            {
                case "Name":
                    Name = value;
                    break;
                case "Comment":
                    Comment = value;
                    break;
                case "Enabled":
                    Enabled = XmlConvert.ToBoolean(value);
                    break;
            }
        }

        public void Write(XmlWriter xw)
        {
            xw.WriteStartElement(ElementName);
            WriteElements(xw);
            xw.WriteEndElement();
        }

        public void Read(XmlReader xr)
        {
            try
            {
                while (xr.Read())
                {
                    if (xr.NodeType == XmlNodeType.Element)
                    {
                        string val = xr.ReadString();
                        SetElement(xr.LocalName, val);
                    }
                    else if (xr.NodeType == XmlNodeType.EndElement)
                    {
                        if (xr.LocalName == ElementName)
                        {
                            break;
                        }
                    }
                }
            }
            catch (XmlException ex)
            {
            }
        }

        public abstract string GetHtml(ISource mySourceGroup, int displayWidth, int displayHeight, int displayDepth, StringBuilder log);

        public virtual void CleanUp()
        {
        }
    }
}
