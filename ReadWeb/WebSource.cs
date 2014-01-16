using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;
using System.Xml;

namespace web2book
{
    public class WebSource
    {
        public virtual bool CanPublish { get { return false; } }

        string sourceName;

        public string SourceName
        {
            get { return sourceName; }
        }

        string elementName;

        public string ElementName
        {
            get { return elementName; }
        }

        bool hasVolatileContent;

        public bool HasVolatileContent
        {
            get { return hasVolatileContent; }
        }

        bool hasItemCustomizeUI;

        public bool HasItemCustomizeUI
        {
            get { return hasItemCustomizeUI; }
        }

        bool hasGroupCustomizeUI;

        public bool HasGroupCustomizeUI
        {
            get { return hasGroupCustomizeUI; }
        }

        string[] primaryFields;

        public string[] PrimaryFields
        {
            get { return primaryFields; }
        }

        Type itemType;

        public Type ItemType
        {
            get { return itemType; }
        }

        protected WebSource(string name, Type itemType, string[] primaryFields, bool hasVolatileContent, bool hasItemCustomizeUI, bool hasGroupCustomizeUI)
        {
            this.sourceName = name;
            this.elementName = name.Replace(" ", "").Replace(".", "");
            this.hasVolatileContent = hasVolatileContent;
            this.hasItemCustomizeUI = hasItemCustomizeUI;
            this.hasGroupCustomizeUI = hasGroupCustomizeUI;
            this.itemType = itemType;
            this.primaryFields = primaryFields;
        }

        protected void AddDiff(StringBuilder sb, string label, string remote, string local, string defaultVal)
        {
            if (remote == null) remote = "";
            if (local == null) local = "";
            string r = remote;
            string l = local;
            if (r.Length == 0) r = defaultVal;
            if (l.Length == 0) l = defaultVal;
            if (r != l)
            {
                if (sb.Length == 0)
                {
                    sb.Append("An entry with this name already exists.\r\n\r\nHere are the differences with your entry:\r\n\r\n");
                }
                sb.Append("\r\n\r\n");
                sb.Append(label.ToUpper());
                sb.Append(":\r\n\r\nExisting: ");
                sb.Append(remote);
                sb.Append("\r\nYours:   ");
                sb.Append(local);
            }
        }

        public virtual void CustomizeItem(ContentSource cs, EventHandler test, EventHandler publish)
        {
            Debug.Assert(!HasItemCustomizeUI);
        }

        public virtual void CustomizeGroup()
        {
            Debug.Assert(!HasGroupCustomizeUI);
        }

        public virtual string DescribeDifferences(ContentSource publishedSource, ContentSource mySource)
        {
            return null;
        }

        public virtual void Write(XmlWriter xw)
        {
        }

        public virtual void Read(XmlReader xr)
        {
        }
    }
}

