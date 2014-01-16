using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Windows.Forms;
using System.Drawing;
using System.Net;
using System.Diagnostics;

namespace web2book
{
    public class Feed : WebContentSource
    {
        public override string ElementName
        {
            get { return "Feed"; }
        }

        public override bool IsStructured // is this structured as a sequence of blocks with headings (i.e. does it make sense to generate a TOC?)
        {
            get { return true; }
        } 

        int numberOfDays;

        public int NumberOfDays
        {
            get { return numberOfDays; }
            set { numberOfDays = value; }
        }

        string itemElement;

        public string ItemElement
        {
            get { return itemElement; }
            set { itemElement = value; }
        }

        string titleElement;

        public string TitleElement
        {
            get { return titleElement; }
            set { titleElement = value; }
        }

        string dateElement;

        public string DateElement
        {
            get { return dateElement; }
            set { dateElement = value; }
        }

        string contentElement;

        public string ContentElement
        {
            get { return contentElement; }
            set { contentElement = value; }
        }

        string linkElement;

        public string LinkElement
        {
            get { return linkElement; }
            set { linkElement = value; }
        }

        string linkExtractorPattern;

        public string LinkExtractorPattern
        {
            get { return linkExtractorPattern; }
            set { linkExtractorPattern = value; }
        }

        bool extractLinkFromContent;

        public bool ExtractLinkFromContent
        {
            get { return extractLinkFromContent; }
            set { extractLinkFromContent = value; }
        }

        string linkReformatter;

        public string LinkReformatter
        {
            get { return linkReformatter; }
            set { linkReformatter = value; }
        }

        string linkProcessor;

        public string LinkProcessor
        {
            get { return linkProcessor; }
            set { linkProcessor = value; }
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

        bool reverseOrder;

        public bool ReverseOrder
        {
            get { return reverseOrder; }
            set { reverseOrder = value; }
        }


        public Feed()
            : this(String.Empty, String.Empty, false, String.Empty, 3, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, false, String.Empty, String.Empty, String.Empty, false)
        { }

        public Feed(string name, string comment, bool enabled, string url, int numDays, string contentElt, string itemElt, string titleElt, string dateElt, string linkElt, string extract, bool extractFromContent, string format, string contentExtractor, string contentFormatter, bool reverseOrder)
            : base(name, comment, enabled, url)
        {
            NumberOfDays = numDays;
            ContentElement = contentElt;
            ItemElement = itemElement;
            TitleElement = titleElement;
            DateElement = dateElement;
            LinkElement = linkElt;
            LinkExtractorPattern = extract;
            ExtractLinkFromContent = extractFromContent;
            LinkReformatter = format;
            ContentExtractor = contentExtractor;
            ContentFormatter = contentFormatter;
            ReverseOrder = reverseOrder;
        }

        protected override void WriteElements(XmlWriter xw)
        {
            base.WriteElements(xw);
            xw.WriteElementString("Days", XmlConvert.ToString(NumberOfDays));
            xw.WriteElementString("ContentElement", ContentElement);
            xw.WriteElementString("ItemElement", ItemElement);
            xw.WriteElementString("TitleElement", TitleElement);
            xw.WriteElementString("DateElement", DateElement);
            xw.WriteElementString("LinkElement", LinkElement);
            xw.WriteElementString("LinkExtractorPattern", LinkExtractorPattern);
            xw.WriteElementString("ExtractLinkFromContent", XmlConvert.ToString(ExtractLinkFromContent));
            xw.WriteElementString("LinkReformatter", LinkReformatter);
            xw.WriteElementString("ContentExtractor", ContentExtractor);
            xw.WriteElementString("ContentFormatter", ContentFormatter);
            xw.WriteElementString("ReverseOrder", XmlConvert.ToString(ReverseOrder));
        }

        protected override void SetElement(string name, string value)
        {
            switch (name)
            {
                case "Days":
                    NumberOfDays = XmlConvert.ToInt32(value);
                    break;
                case "ContentElement":
                    ContentElement = value;
                    break;
                case "LinkElement":
                    LinkElement = value;
                    break;
                case "ItemElement":
                    ItemElement = value;
                    break;
                case "TitleElement":
                    TitleElement = value;
                    break;
                case "DateElement":
                    DateElement = value;
                    break;
                case "LinkExtractorPattern":
                    LinkExtractorPattern = value;
                    break;
                case "ExtractLinkFromContent":
                    ExtractLinkFromContent = XmlConvert.ToBoolean(value);
                    break;
                case "LinkReformatter":
                    LinkReformatter = value;
                    break;
                case "ContentExtractor":
                    ContentExtractor = value;
                    break;
                case "ContentFormatter":
                    ContentFormatter = value;
                    break;
                case "ReverseOrder":
                    ReverseOrder = XmlConvert.ToBoolean(value);
                    break;
                default:
                    base.SetElement(name, value);
                    break;
            }
        }

        string ProcessContent(string html, string link, StringBuilder log)
        {
            // Run the link through the link extractor pattern

            string[] args = null;
            if (link == null) link = "";
            if (LinkExtractorPattern != String.Empty || link != String.Empty)
            {
                if (link == String.Empty)
                {
                    throw new ApplicationException("Can't process empty link in feed " + Name);
                }
                string txt = link;
                if (ExtractLinkFromContent)
                {
                    string err = null;
                    txt = Utils.ReadWebPage(link, ref err);
                    if (err != null)
                    {
                        throw new ApplicationException("Getting web page " + link + " returned error " + err);
                    }
                }
                if (LinkExtractorPattern != String.Empty)
                {
                    log.Append("\r\nApplying link extractor pattern to\r\n================\r\n" + txt + "\r\n===============\r\n");
                    Regex re = new Regex(LinkExtractorPattern);
                    MatchCollection matches = re.Matches(txt);
                    if (matches.Count > 0)
                    {
                        args = new string[matches[0].Groups.Count - 1];
                        for (int i = 1; i < matches[0].Groups.Count; i++)
                        {
                            args[i - 1] = matches[0].Groups[i].Value;
                        }
                    }
                    else
                    {
                        log.Append("\r\nNo matches found during link extraction");
                    }
                }

                if (args == null) args = new string[] { link }; // sanity

                log.Append("\r\nDone link extraction");
                for (int i = 0; i < args.Length; i++)
                {
                    log.Append("{" + i.ToString() + "} = " + args[i]);
                }

                // Apply the link reformatter

                if (LinkReformatter == String.Empty)
                {
                    link = args.Length > 0 ? args[0] : "";
                }
                else
                {
                    link = LinkReformatter;
                    for (int i = 0; i < args.Length; i++)
                    {
                        link = link.Replace("{" + i.ToString() + "}", args[i]);
                    }
                    int p = link.IndexOf("{");
                    if (p >= 0 && p < (link.Length-1) && Char.IsDigit(link[p+1]))
                    {
                        throw new ApplicationException("Insufficient extracted fields for link reformatter");
                    }
                }

                log.Append("\r\nReformatted link is " + link);
            }

            return Utils.GetContent(link, html, null, ContentExtractor, ContentFormatter, 0, log);
        }

        bool MatchElement(string eltname, string userSpecified, string[] defaultValues)
        {
            Debug.Assert(eltname != null && userSpecified != null);
            if (userSpecified != String.Empty)
                return eltname == userSpecified;
            if (defaultValues != null)
            {
                for (int i = 0; i < defaultValues.Length; i++)
                {
                    if (eltname == defaultValues[i])
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        static readonly string[] itemElementDefaults = { "item", "entry" };
        static readonly string[] titleElementDefaults = { "title" };
        static readonly string[] contentElementDefaults = { "description", "summary", "content", "encoded" };
        static readonly string[] dateElementDefaults = { "pubDate", "updated", "date" };

        string Rss2Html(XmlTextReader reader, DateTime start, int headerLevel, bool reverseOrder, StringBuilder log)
        {
            int cnt = 0;
            string hl = headerLevel.ToString();
            ArrayList articles = new ArrayList();
            try
            {
                bool inItem = false, inDateRange = true;
                string html = null, link = null, title = null;
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        if (MatchElement(reader.LocalName, ItemElement, itemElementDefaults))
                        {
                            inItem = true;
                            inDateRange = true;
                            title = null;
                            html = null;
                            link = null;
                            continue;
                        }
                        if (!inItem) 
                            continue;
                        if (MatchElement(reader.LocalName, TitleElement, titleElementDefaults))
                        {
                            title = reader.ReadString();
                        }
                        else if (LinkElement == String.Empty && MatchElement(reader.LocalName, ContentElement, contentElementDefaults))
                        {
                            html = reader.ReadString();
                        }
                        else if (MatchElement(reader.LocalName, DateElement, dateElementDefaults))
                        {
                            string s = reader.ReadString();
                            DateTime pubDate = start;
                            try
                            {
                                pubDate = RFC2822DateTime.fromString(s);
                            }
                            catch (FormatException)
                            {
                                DateTime.TryParse(s, out pubDate);
                            }
                            inDateRange = pubDate >= start;
                            log.AppendLine(s/*pubDate.ToString()*/ + (inDateRange ? " is in" : " is out of") + " range");
                        }
                        else if (MatchElement(reader.LocalName, LinkElement, null))
                        {
                            link = reader.ReadString();
                            log.AppendLine("Got link from RSS: " + link);
                        }
                    }
                    else if (reader.NodeType == XmlNodeType.EndElement)
                    {
                        if (MatchElement(reader.LocalName, ItemElement, itemElementDefaults))
                        {
                            inItem = false;
                            if (inDateRange)
                            {
                                html = ProcessContent(html, link, log);
                                if (html != null)
                                {
                                    StringBuilder sb = new StringBuilder();
                                    if (title != null)
                                        sb.Append("<h" + hl + ">" + title + "</h" + hl + ">\r\n");
                                    sb.Append(html);
                                    sb.Append("\r\n");
                                    articles.Add(sb);
                                    ++cnt;
                                }
                                else log.AppendLine("No content for " + title);
                            }
                        }
                    }
                }
            }
            catch (XmlException ex)
            {
            }
            catch (WebException ex)
            {
            }
            catch (Exception ex)
            {
            }

            log.AppendLine("Added " + cnt.ToString() + " articles");

            StringBuilder rtn = new StringBuilder();
            if (reverseOrder)
            {
                for (int i = articles.Count; --i >= 0; )
                {
                    rtn.Append((StringBuilder)articles[i]);
                }
            }
            else
            {
                for (int i = 0; i < articles.Count; i++)
                {
                    rtn.Append((StringBuilder)articles[i]);
                }
            }

            return (rtn.Length > 0) ? rtn.ToString() : null;
        }

        DateTime GetStart()
        {
            DateTime start;
            if (NumberOfDays == 0)
            {
                start = DateTime.MinValue;
            }
            else
            {
                start = DateTime.Today.AddDays(-NumberOfDays + 1);
            }
            return start;
        }

        public override string GetHtml(ISource source, int displayWidth, int displayHeight, int displayDepth, StringBuilder log)
        {
            try
            {
                XmlTextReader reader = new XmlTextReader(Url);
                if (reader != null)
                {
                    return Rss2Html(reader, GetStart(), 1, ReverseOrder, log);
                }
            }
            catch
            {
            }
            return null;
        }
    }

    public class FeedSource : WebSource, ISource
    {

        public FeedSource()
            : base("Feed", typeof(Feed), new string[] { "Enabled", "Name", "Url" }, true, true, false)
        { }

        public string HtmlHelp { get { return null; } }

        public IContentSourceList DataSource
        {
            get { return new ContentSourceBindingList<Feed>(this); }
        }

        public override bool CanPublish { get { return true; } }

        public string FileNamePrefix { get { return "rss"; } }

        public bool IsType(ContentSource cs)
        {
            return cs is Feed;
        }

        public ContentSource MakeSource()
        {
            return new Feed();
        }

        public override string DescribeDifferences(ContentSource publishedSource, ContentSource mySource)
        {
            Feed publishedFeed = (Feed)publishedSource;
            Feed myFeed = (Feed)mySource;
            StringBuilder sb = new StringBuilder();
            AddDiff(sb, "Url", publishedFeed.Url.ToLower(), myFeed.Url.ToLower(), "");
            AddDiff(sb, "Content Element", publishedFeed.ContentElement, myFeed.ContentElement, "");
            AddDiff(sb, "Item Element", publishedFeed.ItemElement, myFeed.ContentElement, "");
            AddDiff(sb, "Title Element", publishedFeed.TitleElement, myFeed.ContentElement, "");
            AddDiff(sb, "Date Element", publishedFeed.DateElement, myFeed.ContentElement, "");
            AddDiff(sb, "Link Element", publishedFeed.LinkElement, myFeed.LinkElement, "");
            AddDiff(sb, "Link Extractor Pattern", publishedFeed.LinkExtractorPattern, myFeed.LinkExtractorPattern, "(.*)");
            AddDiff(sb, "Extract From Content", publishedFeed.ExtractLinkFromContent.ToString(), myFeed.ExtractLinkFromContent.ToString(), "");
            AddDiff(sb, "Link Reformatter", publishedFeed.LinkReformatter, myFeed.LinkReformatter, "{0}");
            AddDiff(sb, "Link Processor", publishedFeed.LinkProcessor, myFeed.LinkProcessor, "get $link");
            AddDiff(sb, "Content Extraction Pattern", publishedFeed.ContentExtractor, myFeed.ContentExtractor, "(.*)");
            AddDiff(sb, "Content Reformatter", publishedFeed.ContentFormatter, myFeed.ContentFormatter, "{0}");
            return sb.ToString();
        }

        public override void CustomizeItem(ContentSource cs, EventHandler test, EventHandler publish)
        {
            new RssFeedCustomizeForm(cs, test, publish).ShowDialog();
        }
    }
}
