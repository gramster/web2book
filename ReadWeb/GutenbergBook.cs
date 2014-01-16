using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace web2book
{
    public class GutenbergBook : Book
    {
        bool isHtml;

        string eTextNumber = String.Empty;

        public string ETextNumber
        {
            get { return eTextNumber; }
            set { eTextNumber = value; }
        }

        public string ContentExtractor
        {
            get { return null; }
        }

        static string postDataFormat = 
                "-----------------------------7d734b74613ea\r\n"+
                "Content-Disposition: form-data; name=\"author\"\r\n"+
                "\r\n{0}\r\n"+
                "-----------------------------7d734b74613ea\r\n"+
                "Content-Disposition: form-data; name=\"title\"\r\n"+
                "\r\n{1}\r\n"+
                "-----------------------------7d734b74613ea\r\n" +
                "Content-Disposition: form-data; name=\"subject\"\r\n" +
                "\r\n\r\n" +
                "-----------------------------7d734b74613ea\r\n" +
                "Content-Disposition: form-data; name=\"lang\"\r\n" +
                "\r\n{3}\r\n" +
                "-----------------------------7d734b74613ea\r\n" +
                "Content-Disposition: form-data; name=\"category\"\r\n" +
                "\r\n\r\n" +
                "-----------------------------7d734b74613ea\r\n" +
                "Content-Disposition: form-data; name=\"locc\"\r\n" +
                "\r\n\r\n" +
                "-----------------------------7d734b74613ea\r\n" +
                "Content-Disposition: form-data; name=\"filetype\"\r\n"+
                "\r\n{2}\r\n"+
                "-----------------------------7d734b74613ea\r\n" +
                "Content-Disposition: form-data; name=\"etextnr\"\r\n" +
                "\r\n{4}\r\n" +
                "-----------------------------7d734b74613ea\r\n"+
                "Content-Disposition: form-data; name=\"fulltext\"\r\n"+
                "\r\n\r\n"+
                "-----------------------------7d734b74613ea--\r\n";

        protected override string GetUrl(string author, string title)
        {
            string rtn = GetUrl(author, title, "html", "en", ETextNumber);
            if (rtn == null)
                rtn = GetUrl(author, title, "txt", "en", ETextNumber);
            if (rtn == null)
                rtn = GetUrl(author, title, "html", "", ETextNumber);
            if (rtn == null)
                rtn = GetUrl(author, title, "txt", "", ETextNumber);
            return rtn;
        }

        string GetUrl(string author, string title, string fileType, string lang, string etext)
        {
            string error = null;
            string postData = String.Format(postDataFormat, author, title, fileType, lang, etext);
            string href = null;

            string resp = Utils.GetWebResponse("www.gutenberg.org/catalog/world/results", ref error, postData,
                    null,
                    "multipart/form-data; boundary=---------------------------7d734b74613ea");
            if (error == null)
            {
                int p = resp.IndexOf("href=\"/etext/");
                if (p >= 0)
                {
                    p += 6;
                    int ep = resp.IndexOf("\"", p);
                    if (ep > 0)
                    {
                        resp = Utils.GetWebResponse("www.gutenberg.org" + resp.Substring(p, ep - p), ref error, null, null, null);
                        if (error == null)
                        {
                            p = -1;
                            while ((p = resp.IndexOf("<a href=\"/", p + 1)) >= 0)
                            {
                                p += 9;
                                ep = resp.IndexOf("\"", p);
                                if (ep < 0) break;
                                string hr = resp.Substring(p, ep - p);
                                int sp = hr.LastIndexOf(".");
                                if (sp < 0) continue;
                                string suf = hr.Substring(sp);
                                if (href== null && suf == ".txt" && !isHtml)
                                {
                                    href = hr;
                                }
                                else if (suf.StartsWith(".htm"))
                                {
                                    href = hr;
                                    isHtml = true;
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            if (href != null)
            {
                /*int p = href.IndexOf("?");
                if (p < 0) p = href.LastIndexOf("/");
                if (p >= 0 && p < (href.Length-1))
                {
                    ++p;
                    href = href.Substring(0, p) + Uri.EscapeDataString(href.Substring(p));
                }*/
                return "http://www.gutenberg.org" + href;

            }
            return null;
        }

        public override string GetHtml(ISource mySourceGroup, int displayWidth, int displayHeight, int displayDepth, StringBuilder log)
        {
            if (Url == null)
                return null;
            string htm = Utils.GetHtml(Url, 0, null, ContentExtractor, null, 0, log);
            if (!isHtml) htm = htm.Replace("\r\n\r\n", "\r\n<p>\r\n");
            return htm;
        }
    }

    public class GutenbergSource : WebSource, ISource
    {
        public GutenbergSource()
            : base("Gutenberg Book", typeof(GutenbergBook), new string[] { "Enabled", "Author", "Name", "ETextNumber" }, false, false, false)
        { }

        public string HtmlHelp { get { return null; } }

        public IContentSourceList DataSource
        {
            get { return new ContentSourceBindingList<GutenbergBook>(this); }
        }

        public string FileNamePrefix { get { return "gut"; } }

        public bool IsType(ContentSource cs)
        {
            return cs is GutenbergBook;
        }

        public ContentSource MakeSource()
        {
            return new GutenbergBook();
        }
    }

}
