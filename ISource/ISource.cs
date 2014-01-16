using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;

namespace web2book
{
    public interface ISource
    {
        //Type ItemType { get; } // the source type
        IContentSourceList DataSource { get; }
        bool IsType(ContentSource cs); // is a source of this type?
        string[] PrimaryFields { get; } // which fields should be displayed in the main view?
        bool HasVolatileContent { get; } // Does the content change frequently?
        bool HasItemCustomizeUI { get; }
        bool HasGroupCustomizeUI { get; }
        bool CanPublish { get; } // Can this source type be published to www.geekraver.com?
        string SourceName { get; } // Friendly name to display in UI
        string ElementName { get; } // Xml element name to use in config files
        ContentSource MakeSource(); // Factory method for source type
        string FileNamePrefix { get; }
        void CustomizeItem(ContentSource cs, EventHandler test, EventHandler publish); // Do advanced property editing UI, if appropriate
        void CustomizeGroup();
        string DescribeDifferences(ContentSource publishedSource, ContentSource mySource); // Return a description of differences between two sources
        string HtmlHelp { get; }
        void Write(XmlWriter xw);
        void Read(XmlReader xr);
    }
}
