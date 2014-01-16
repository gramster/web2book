//#define OLD

using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Net;
using System.IO;
using System.Xml;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;
using System.Configuration;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Runtime.InteropServices;
#if OLD
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.rtf;
#endif

namespace web2book
{
    public class PublishedSource
    {
        string url;

        public string Url
        {
            get { return url; }
            set { url = value; }
        }

        bool enabled;

        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }

        string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        string contributor;

        public string Contributor
        {
            get { return contributor; }
            set { contributor = value; }
        }

        DateTime added;

        public DateTime Added
        {
            get { return added; }
            set { added = value; }
        }

        string comment;

        public string Comment
        {
            get { return comment; }
            set { comment = value; }
        }

        ContentSource source;

        public ContentSource Source
        {
            get { return source; }
            set { source = value; }
        }
    }

    public class Utils
    {
        static public int RELEASE = 25;

        static List<IHtmlConverter> converters = null;

        public static List<IHtmlConverter> Converters
        {
            get
            {
                if (converters == null)
                {
                    converters = GetConverters();
                }
                return converters;
            }
        }

        static List<ISyncDevice> devices = null;

        public static List<ISyncDevice> Devices
        {
            get
            {
                if (devices == null)
                {
                    devices = GetDevices();
                }
                return devices;
            }
        }

        static List<ISource> sources = null;

        public static List<ISource> Sources
        {
            get
            {
                if (sources == null)
                {
                    sources = GetPlugins<ISource>(".", "read*.dll");
                }
                return sources;
            }
        }

        static public bool RunExternalCommand(string cmd, string args, string workdir, bool useShell, int timeout, out string output)
        {
            output = null;
            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.EnableRaisingEvents = false;
            proc.StartInfo.FileName = cmd;
            proc.StartInfo.Arguments = args;
            proc.StartInfo.WorkingDirectory = workdir;
            proc.StartInfo.UseShellExecute = useShell;
            if (!useShell) proc.StartInfo.RedirectStandardOutput = true;
            bool rtn = proc.Start();
            if (timeout > 0)
            {
                int t = timeout;
                for (; t >= 0; t -= 1000)
                {
                    if (proc.WaitForExit(1000)) break;
                    Application.DoEvents();
                    // TODO: fix this
                    /*if (MainForm.progressForm != null && MainForm.progressForm.Cancelled)
                    {
                        t = -1;
                        break;
                    }*/
                }
                if (t < 0)
                {
                    proc.Kill();
                    return rtn;
                }
            }
            if (!useShell) output = proc.StandardOutput.ReadToEnd();
            return rtn;
        }

        static public void OpenUrl(string url)
        {
            string foo;
            RunExternalCommand(url, null, ".", true, 0, out foo);
        }

        static public string BrowsePath(string description)
        {
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
            // Set the help text description for the FolderBrowserDialog.
            folderBrowser.Description = description;

            // Do not allow the user to create new files via the FolderBrowserDialog.
            folderBrowser.ShowNewFolderButton = false;

            folderBrowser.RootFolder = System.Environment.SpecialFolder.MyComputer;

            // Show the FolderBrowserDialog.
            DialogResult result = folderBrowser.ShowDialog();
            if (result == DialogResult.OK)
            {
                return folderBrowser.SelectedPath;
            }
            return null;
        }

        static public string[] BrowseFiles(string description, string filter, bool mustExist, bool multiSelect)
        {
            OpenFileDialog fileBrowser = new OpenFileDialog();
            fileBrowser.Title = description;
            //fileBrowser.InitialDirectory = System.Environment.SpecialFolder.MyComputer;
            fileBrowser.Filter = filter;
            fileBrowser.CheckFileExists = mustExist;
            fileBrowser.Multiselect = multiSelect;
            // Show the Dialog.
            DialogResult result = fileBrowser.ShowDialog();
            if (result == DialogResult.OK)
            {
                return fileBrowser.FileNames;
            }
            return null;
        }

        static public string[] BrowseXmlFileRead(string description)
        {
            return BrowseFiles(description, "Xml files (*.xml)|*.xml|All files (*.*)|*.*", true, true);
        }

        static public string BrowseXmlFileWrite(string description, string fname)
        {
            SaveFileDialog fileBrowser = new SaveFileDialog();
            fileBrowser.Title = description;
            fileBrowser.AddExtension = true;
            fileBrowser.DefaultExt = "xml";
            fileBrowser.FileName = fname + ".xml";
            //fileBrowser.InitialDirectory = System.Environment.SpecialFolder.MyComputer;
            fileBrowser.CheckPathExists = true;
            fileBrowser.Filter = "Xml files (*.xml)|*.xml|All files (*.*)|*.*";
            // Show the Dialog.
            DialogResult result = fileBrowser.ShowDialog();
            if (result == DialogResult.OK)
            {
                return fileBrowser.FileName;
            }
            return null;
        }

        static public void SaveString(string path, string text)
        {
            StreamWriter fs = null;
            try
            {
                fs = File.CreateText(path);
                fs.Write(text);
            }
            catch (System.Exception e)
            {
            }
            finally
            {
                fs.Close();
            }
        }

        static public void SaveBytes(string path, byte[] text)
        {
            FileStream fs = null;
            try
            {
                fs = File.Create(path);
                fs.Write(text, 0, text.Length);
            }
            catch (System.Exception e)
            {
            }
            finally
            {
                if (fs != null) fs.Close();
            }
        }

        static public byte[] LoadBytes(string path)
        {
            FileStream fs = null;
            try
            {
                fs = File.OpenRead(path);
                byte[] rtn = new byte[fs.Length];
                fs.Read(rtn, 0, (int)fs.Length);
                return rtn;
            }
            catch (System.Exception e)
            {
            }
            finally
            {
                if (fs != null) fs.Close();
            }
            return null;
        }

        public static bool SetAllowUnsafeHeaderParsing()
        {
            //Get the assembly that contains the internal class
            Assembly aNetAssembly = Assembly.GetAssembly(typeof(System.Net.Configuration.SettingsSection));
            if (aNetAssembly != null)
            {
                //Use the assembly in order to get the internal type for the internal class
                Type aSettingsType = aNetAssembly.GetType("System.Net.Configuration.SettingsSectionInternal");
                if (aSettingsType != null)
                {
                    //Use the internal static property to get an instance of the internal settings class.
                    //If the static instance isn't created allready the property will create it for us.
                    object anInstance = aSettingsType.InvokeMember("Section",
                      BindingFlags.Static | BindingFlags.GetProperty | BindingFlags.NonPublic, null, null, new object[] { });

                    if (anInstance != null)
                    {
                        //Locate the private bool field that tells the framework is unsafe header parsing should be allowed or not
                        FieldInfo aUseUnsafeHeaderParsing = aSettingsType.GetField("useUnsafeHeaderParsing", BindingFlags.NonPublic | BindingFlags.Instance);
                        if (aUseUnsafeHeaderParsing != null)
                        {
                            aUseUnsafeHeaderParsing.SetValue(anInstance, true);
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        static HttpWebResponse GetUrlResponse(string url, ref string error, string postData, ICredentials creds, string contentType)
        {
            SetAllowUnsafeHeaderParsing();
            if (url.IndexOf("://") < 0) url = "http://" + url;
            Uri uri = new Uri(url);
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(uri);
            req.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1; .NET CLR 1.1.4322)";
            req.Timeout = 15000;
            req.Method = (postData == null) ? "GET" : "POST";
            CookieContainer cookies = GetUriCookieContainer(uri);
            if (cookies != null)
                req.CookieContainer = cookies;
            if (contentType != null) req.ContentType = contentType;
            if (creds != null) req.Credentials = creds;
            if (postData != null)
            {
                Stream ws = req.GetRequestStream();
                byte[] pdata = System.Text.Encoding.UTF8.GetBytes(postData);
                ws.Write(pdata, 0, pdata.Length);
                ws.Close();
            }
            HttpWebResponse resp;
            try
            {
                WebResponse wr = req.GetResponse();
                resp = wr as HttpWebResponse;
            }
            catch (WebException e)
            {
                error += "Got web exception " + e.Message + "\r\n";
                resp = null;
            }
            return resp;
        }

        public static string GetWebResponse(string url, ref string error, string postData, ICredentials creds, string contentType)
        {
            string rsp = null;
            HttpWebResponse resp = GetUrlResponse(url, ref error, postData, creds, contentType);
            if (resp != null)
            {
                if (resp.StatusCode == HttpStatusCode.OK)
                {
                    StreamReader rdr = null;
                    try
                    {
                        rdr = new StreamReader(resp.GetResponseStream());
                        rsp = rdr.ReadToEnd();
                    }
                    catch (Exception e)
                    {
                        error += "Got exception: " + e.Message + "\r\n";
                    }
                    finally
                    {
                        if (rdr != null)
                        {
                            rdr.Close();
                        }
                    }
                }
            }
            return rsp;
        }

        public static string ReadWebPage(string url, ref string error)
        {
            return GetWebResponse(url, ref error, null, null, null);
        }

        static public byte[] GetBinaryFileFromWeb(string url, ref string err)
        {
            HttpWebResponse resp = GetUrlResponse(url, ref err, null, null, null);
            if (resp != null && resp.StatusCode == HttpStatusCode.OK)
            {
                Stream s = resp.GetResponseStream();
                int sz = 1024;
                byte[] data = new byte[sz];
                int b, cnt = 0;
                while ((b = s.ReadByte()) != -1)
                {
                    data[cnt++] = (byte)b;
                    if (cnt == sz)
                    {
                        byte[] tmp = new byte[sz * 2];
                        Array.Copy(data, tmp, sz);
                        data = tmp;
                        sz *= 2;
                    }
                }
                byte[] rtn = new byte[cnt];
                Array.Copy(data, rtn, cnt);
                return rtn;
            }
            return null;
        }

        static bool GetFileFromWeb(string inputPath, string url, string fname, ref string error)
        {
            return GetFileFromWeb(inputPath, url, fname, ref error, 0);
        }

        static bool GetFileFromWeb(string inputPath, string url, string fname, ref string error, int size)
        {
            Stream s = null;
            if (url.StartsWith("file://"))
            {
                s = File.OpenRead(url.Substring(7).Replace('/', '\\'));
            }
            else
            {
                HttpWebResponse resp = GetUrlResponse(url, ref error, null, null, null);
                if (resp != null && resp.StatusCode == HttpStatusCode.OK)
                {
                    s = resp.GetResponseStream();
                }
            }
            if (s != null)
            {
                bool showProgress = (size > 0);
                ProgressForm f = null;
                if (showProgress)
                {
                    f = new ProgressForm();
                    f.Message = "Downloading " + fname;
                    f.Start(size / 512);
                    f.Show();
                }

                string path = inputPath + @"\" + fname;
                if (File.Exists(path)) File.Delete(path);
                FileStream fs = File.Create(path);
                int b, cnt = 0;
                while ((b = s.ReadByte()) != -1)
                {
                    fs.WriteByte((byte)b);
                    if (showProgress)
                    {
                        if (f.Cancelled)
                        {
                            fs.Close();
                            File.Delete(path);
                            return false;
                        }
                        ++cnt;
                        if ((cnt % 512) == 0)
                        {
                            f.Next();
                        }
                    }
                }
                if (showProgress)
                {
                    f.Close();
                }
                fs.Close();
                s.Close();
                return true;
            }
            return false;
        }

        static public bool Update()
        {
            try
            {
                string err = null;
                string info = ReadWebPage("http://www.geekraver.com/web2book.inf", ref err);
                if (err == null)
                {
                    // First line should have release, second line should have size, subsequent lines should have info
                    string[] ln = info.Split(new char[] { '\n' }, 3);
                    if (ln.Length == 3)
                    {
                        int rel;
                        if (int.TryParse(ln[0].Trim(), out rel) && rel > RELEASE)
                        {
                            int size = 0;
                            int.TryParse(ln[1].Trim(), out size);
                            if (MessageBox.Show(
                                "A new version of web2book is available with the following changes:\r\n\r\n"+ln[2]+"\r\n\r\nWould you like to install it?", "Update Available", MessageBoxButtons.YesNo) == DialogResult.Yes)
                            {
                                if (GetFileFromWeb(".", "http://www.geekraver.com/Web2BookSetup" + rel.ToString() + ".msi", "Web2BookSetup.msi", ref err, size))
                                {
                                    System.Diagnostics.Process proc = new System.Diagnostics.Process();
                                    proc.EnableRaisingEvents = false;
                                    proc.StartInfo.FileName = "Web2BookSetup.msi";
                                    proc.StartInfo.UseShellExecute = true;
                                    return proc.Start();
                                }
                            }
                        }
                    }
                }
            }
            catch 
            { }
            return false;
        }

        static bool GetTitlepageFromWeb(string inputPath, string url, out string fname, ref string error)
        {
            if (url.EndsWith(".gif"))
            {
                fname = "titlepage.gif";
            }
            else
            {
                fname = "titlepage.jpg";
            }
            return GetFileFromWeb(inputPath, url, fname, ref error);
        }

        public static byte[] CleanText(String txt)
        {
            byte[] itmIn = Encoding.UTF8.GetBytes(txt); // is this right?
            Encoding target = Encoding.GetEncoding("iso-8859-15");
            byte[] itmOut = Encoding.Convert(Encoding.UTF8, target, itmIn);
            //return Encoding.ASCII.GetString(itmOut);
            return itmOut;
        }

        static CredentialCache MakeCreds(string user, string password)
        {
            CredentialCache creds = new CredentialCache();
            creds.Add(new Uri("http://www.geekraver.com/rss2book/"), "Digest", new NetworkCredential(user, password, "Domain"));
            return creds;
        }

        public static void MakeFeedCollectionOverHttp(string user, string password)
        {
            WebClient client = new WebClient();
            client.Credentials = MakeCreds(user, password);
            Stream writeStream = client.OpenWrite("http://www.geekraver.com/rss2book/feeds", "MKCOL");
            writeStream.Flush();
            writeStream.Close();
        }

        public static bool PublishOverHttp(string text, string targetFile, string user, string password)
        {
            try
            {
                WebClient client = new WebClient();
                client.Credentials = MakeCreds(user, password);
                Stream writeStream = client.OpenWrite("http://www.geekraver.com/rss2book/" + targetFile, "PUT");
                using (StreamWriter sw = new StreamWriter(writeStream))
                {
                    sw.Write(text);
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool PublishOverHttp(byte[] text, string targetFile, string user, string password)
        {
            try
            {
                WebClient client = new WebClient();
                client.Credentials = MakeCreds(user, password);
                string uri = Uri.EscapeUriString("http://www.geekraver.com/rss2book/feeds/" + targetFile);
                Stream writeStream = client.OpenWrite(uri, "PUT");
                using (BinaryWriter bw = new BinaryWriter(writeStream))
                {
                    bw.Write(text);
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static XmlTextReader MakeXmlReaderFromString(string xml)
        {
            return new XmlTextReader(xml, XmlNodeType.Document, new XmlParserContext(null, null, null, XmlSpace.None));
        }

        public static ArrayList GetPublishedSources(string username, string password)
        {
            ArrayList rtn = new ArrayList();
            try
            {
                string error = null;
                string response = GetWebResponse("http://www.geekraver.com/rss2book/feeds", ref error, null, MakeCreds(username, password), null);
                if (error == null)
                {
                    int p = -1;
                    while ((p = response.IndexOf("href=\"", p + 1)) >= 0)
                    {
                        p += 6;
                        int p2 = response.IndexOf("\"", p);
                        if (p2 < 0) break;
                        string href = Uri.UnescapeDataString(response.Substring(p, p2 - p));
                        if (href.StartsWith("./")) href = href.Substring(2);

                        if (href.EndsWith("]"))
                        {
                            int pp = href.LastIndexOf('[');
                            if (pp > 0)
                            {
                                int pp2 = href.IndexOf(':', pp);
                                if (pp2 > 0)
                                {
                                    PublishedSource f = new PublishedSource();
                                    f.Url = "http://www.geekraver.com/rss2book/feeds/" + href;
                                    f.Name = href.Substring(0, pp);
                                    f.Contributor = href.Substring(pp + 1, pp2 - pp - 1);
                                    f.Added = new DateTime(long.Parse(href.Substring(pp2 + 1, href.Length - pp2 - 2))).ToLocalTime();
                                    ContentSource pf = Utils.GetSource(f.Url, username, password);
                                    if (pf != null)
                                    {
                                        f.Comment = pf.Comment;
                                        f.Source = pf;
                                        rtn.Add(f);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return rtn;
        }

        public static string GetSourceXml(string url, string user, string password)
        {
            try
            {
                string error = null;
                string response = GetWebResponse(url, ref error, null, MakeCreds(user, password), null);
                if (error == null)
                {
                    return response;
                }
            }
            catch (Exception ex)
            {
            }
            return null;
        }

        public static ContentSource LoadContentSourceFromXml(XmlTextReader xr)
        {
            while (xr.Read())
            {
                if (xr.NodeType == XmlNodeType.Element)
                {
                    foreach (ISource s in Sources)
                    {
                        if (xr.LocalName == s.ElementName)
                        {
                            ContentSource cs = s.MakeSource();
                            cs.Read(xr);
                            return cs;
                        }
                    }
                }
            }
            return null;
        }

        public static ContentSource GetSource(string url, string username, string password)
        {
            string sourceXml = Utils.GetSourceXml(url, username, password);
            if (sourceXml != null)
            {
                using (XmlTextReader xr = Utils.MakeXmlReaderFromString(sourceXml))
                {
                    return Utils.LoadContentSourceFromXml(xr);
                }
            }
            return null;
        }

        static char Hex(int v)
        {
            if (v < 10) return (char)(v + '0');
            return (char)(v + 'A');
        }

        const string parBreak = "\r\n\\par\\pard\\plain\\sb120\\qj\\fs24\\lang1036";

        static string[,] SimpleXlate = 
            {
                { "em",     "{\\i"  },
                { "bf",     "{\\b"  },
                { "/em",    "}"  },
                { "/bf",    "}"  },
                { "p",      parBreak  },
                { "h1",     "\r\n{\\par\\b\\fs34" },
                { "/h1",    "\\par}" },
                { "h2",     "\r\n{\\par\\b\\fs32" },
                { "/h2",    "\\par}" },
                { "h3",     "\r\n{\\par\\b\\fs30" },
                { "/h3",    "\\par}" },
                { "h4",     "\r\n{\\par\\b\\fs28" },
                { "/h4",    "\\par}" },
                { "h5",     "\r\n{\\par\\b\\fs26" },
                { "/h5",    "\\par}" },
                { "h6",     "\r\n{\\par\\b\\fs24" },
                { "/h6",    "\\par}" },
                { "br",     parBreak },
                { null,     null    }
            };

        static public byte[] OldHtmlToRtf(string html)
        {
            if (html == null) return null;

            StringBuilder rtf = new StringBuilder();
            StringBuilder text = new StringBuilder();
            rtf.Append(@"{\rtf1\ansi\ansicpg1252\deff0\deflang1033\deflangfe1033\fs24");
            rtf.Append(@"{\fonttbl{\f0\fswiss\fprq2\fcharset238 Arial;}{\f1\fswiss\fprq2\fcharset238{\*\fname Arial;}Arial CE;}}");
            rtf.Append(@"{\colortbl ;\red0\green0\blue0;}");
            rtf.Append("\r\n\r\n");
            ParseHTML parse = new ParseHTML();
            parse.Source = html;
            while (!parse.Eof())
            {
                char ch = parse.Parse();
                if (ch == 0)
                {
                    rtf.Append(" ");
                    rtf.Append(text.ToString().Replace("&nbsp;", "\\~").Replace("&amp;", "&").Replace("&lt;", "<").Replace("&gt;", ">"));
                    text.Length = 0;

                    bool done = false;

                    AttributeList tag = parse.GetTag();
                    for (int i = 0; SimpleXlate[i, 0] != null; i++)
                    {
                        if (tag.Name == SimpleXlate[i, 0])
                        {
                            rtf.Append(SimpleXlate[i, 1]);
                            done = true;
                            break;
                        }
                    }

                    if (done) continue;

                    switch (tag.Name)
                    {
                        case "html":
                            break;
                        case "/html":
                            break;
                        case "head":
                            break;
                        case "/head":
                            break;
                        case "body":
                            break;
                        case "/body":
                            break;
                        case "title":
                            break;
                        case "/title":
                            break;
                        case "a":
                            break;
                        case "/a":
                            break;
                        case "table":
                            break;
                        case "/table":
                            break;
                        case "tr":
                            break;
                        case "/tr":
                            break;
                        case "td":
                            break;
                        case "/td":
                            break;
                        case "p":
                            break;
                        case "br":
                            break;
                    }
                }
                else
                {
                    if (ch > 127)
                    {
                        if (ch < 256)
                        {
                            text.Append("\\'");
                            text.Append(Hex(ch / 16));
                            text.Append(Hex(ch % 16));
                        }
                        else
                        {
                            //TODO: need a translation table for some common unicode chars
                            text.Append(' ');
                        }
                    }
                    else if (ch == '{')
                    {
                        text.Append("\\{");
                    }
                    else if (ch == '}')
                    {
                        text.Append("\\}");
                    }
                    else if (ch == '\\')
                    {
                        // TODO: handle escapes
                        text.Append("\\\\");
                    }
                    else if (!Char.IsWhiteSpace(ch))
                    {
                        text.Append(ch);
                    }
                    else if (text.Length == 0 || !Char.IsWhiteSpace(text[text.Length - 1]))
                    {
                        text.Append(' ');
                    }
                }
            }
            rtf.Append("\\par}}}}}}}}");

            // Merge multiple par breaks

            string rtfs = rtf.ToString();
            int l = rtfs.Length;
            do
            {
                l = rtfs.Length;
                rtfs = rtfs.Replace(parBreak + parBreak, parBreak).Replace(" " + parBreak, parBreak);
            }
            while (l > rtfs.Length);

            return System.Text.Encoding.ASCII.GetBytes(rtfs);
            //return CleanText(rtf.ToString());
        }

        public static string GetContent(string link, string html, string linkProcessor, string contentExtractor, string contentFormatter, int depth, StringBuilder log)
        {
            if (link != String.Empty)
            {
                if (linkProcessor == null || linkProcessor == String.Empty)
                {
                    linkProcessor = "get $link";
                }

                // Now get the processor

                string processor = linkProcessor.Replace("$link", link).Trim();
                if (processor.StartsWith("get "))
                {
                    string error = null;
                    link = processor.Substring(4);
                    html = ReadWebPage(link, ref error);
                    if (error != null)
                    {
                        throw new ApplicationException("Getting web page " + link + " returned error " + error);
                    }
                }
                else
                {
                    int p = processor.IndexOf(' ');
                    if (p > 0)
                    {
                        string output;
                        string cmd = processor.Substring(0, p);
                        string a = processor.Substring(p + 1);
                        if (Utils.RunExternalCommand(cmd, a, ".", false, 60, out output))
                        {
                            html = output;
                            if (html == null)
                            {
                                throw new ApplicationException("External command " + cmd + " with args " + a + " returns no content");
                            }
                        }
                    }
                }
            }

            return ExtractContent(contentExtractor, contentFormatter, link, html, linkProcessor, depth, log);
        }

        public static string ExtractContent(string contentExtractor, string contentFormatter, string url, string html, string linkProcessor, int depth, StringBuilder log)
        {
            string[] parts = null;

            ArrayList recursiveContent = new ArrayList();

            if (contentExtractor != null && contentExtractor != String.Empty) // extract subset of content
            {
                log.Append("\r\nApplying content extractor to input: " + html + "\r\n");
                Regex re = new Regex(contentExtractor, RegexOptions.Singleline);
                MatchCollection matches = re.Matches(html);
                if (matches.Count > 0 && matches[0].Groups.Count > 1)
                {
                    parts = new string[matches[0].Groups.Count - 1];
                    for (int i = 1; i < matches[0].Groups.Count; i++)
                    {
                        parts[i - 1] = matches[0].Groups[i].Value;
                    }
                }
                else html = "";
                log.Append("========================\r\n");
            }
            if (parts == null) parts = new string[] { html }; // sanity

            // Apply the content reformatter; need to refactor this

            if (contentFormatter == null || contentFormatter == String.Empty)
            {
                html = parts.Length > 0 ? parts[0] : "";
            }
            else
            {
                html = contentFormatter;
                for (int i = 0; i < parts.Length; i++)
                {
                    html = html.Replace("{" + i.ToString() + "}", parts[i]);
                }
                if (html.IndexOf("{" + parts.Length.ToString() + "}") >= 0)
                {
                    throw new ApplicationException("Insufficient extracted fields for content reformatter");
                }
            }

            if (html != null && url.Length > 0)
            {
                // Make any relative URLs absolute
                int p = -1;
                Uri uri = null;
                char[] attribEnd = new char[] { ' ', '\t', '>' };
                while ((p = html.IndexOf("<IMG", p + 1, StringComparison.OrdinalIgnoreCase)) >= 0)
                {
                    if (uri == null) uri = new Uri(url);
                    int p2 = html.IndexOf("src=", p, StringComparison.OrdinalIgnoreCase);
                    if (p2 > 0)
                    {
                        p2 += 4;
                        int ep;
                        if (html[p2] == '"')
                        {
                            ++p2;
                            ep = html.IndexOf('"', p2);
                        }
                        else
                        {
                            ep = html.IndexOfAny(attribEnd, p2);
                        }
                        string src = html.Substring(p2, ep - p2);
                        if (src.IndexOf("://") < 0)
                        {
                            if (src[0] == '/')
                            {
                                src = "http://" + uri.Host + src;
                            }
                            else
                            {
                                string s = "http://" + uri.Host;
                                for (int i = 0; i < uri.Segments.Length - 1; i++)
                                    s += uri.Segments[i];
                                s += src;
                                src = s;
                            }
                            html = html.Substring(0, p2) + src + html.Substring(ep);
                            p = ep;
                        }
                    }
                }
                p = -1;
                while ((p = html.IndexOf("<A", p + 1, StringComparison.OrdinalIgnoreCase)) >= 0)
                {
                    if (!Char.IsWhiteSpace(html[p + 2])) continue;
                    if (uri == null) uri = new Uri(url);
                    int p2 = html.IndexOf("href=", p, StringComparison.OrdinalIgnoreCase);
                    if (p2 > 0)
                    {
                        p2 += 5;
                        int ep;
                        if (html[p2] == '"')
                        {
                            ++p2;
                            ep = html.IndexOf('"', p2);
                        }
                        else
                        {
                            ep = html.IndexOfAny(attribEnd, p2);
                        }
                        string href = html.Substring(p2, ep - p2);
                        if (href.Length == 0) continue;
                        if (href.IndexOf("://") < 0)
                        {
                            if (href[0] == '/')
                            {
                                href = "http://" + uri.Host + href;
                            }
                            else
                            {
                                string s = "http://" + uri.Host;
                                for (int i = 0; i < uri.Segments.Length - 1; i++)
                                    s += uri.Segments[i];
                                s += href;
                                href = s;
                            }
                            html = html.Substring(0, p2) + href + html.Substring(ep);
                            p = ep;
                        }
                        if (depth > 1)
                        {
                            Uri u = new Uri(href);
                            if (u.Host == uri.Host)
                            {
                                recursiveContent.Add(u); // TODO: make the link to this content relative
                            }
                        }
                    }
                }
            }

            if (recursiveContent.Count > 0)
            {
                StringBuilder final = new StringBuilder(html);
                foreach (Uri u in recursiveContent)
                {
                    string h = GetContent(u.AbsoluteUri, null, linkProcessor, contentExtractor, contentFormatter, depth - 1, log);
                    if (h != null) final.Append(h);
                }
                html = final.ToString();
            }
            log.Append("\r\nFinal content:\r\n===================\r\n");
            log.Append(html == null ? "null" : html);
            log.Append("\r\n===================\r\n");
            return html;
        }

        static public string PatchLocalRefs(string html, string path)
        {
            if (html != null)
            {
                path = path.Replace('\\', '/');
                int p = -1;
                char[] attribEnd = new char[] { ' ', '\t', '>' };
                while ((p = html.IndexOf("<IMG", p + 1, StringComparison.OrdinalIgnoreCase)) >= 0)
                {
                    int p2 = html.IndexOf("src=", p, StringComparison.OrdinalIgnoreCase);
                    if (p2 > 0)
                    {
                        p2 += 4;
                        int ep;
                        if (html[p2] == '"')
                        {
                            ++p2;
                            ep = html.IndexOf('"', p2);
                        }
                        else
                        {
                            ep = html.IndexOfAny(attribEnd, p2);
                        }
                        string src = html.Substring(p2, ep - p2);
                        if (src.IndexOf("://") < 0)
                        {
                            if (src[0] == '/')
                            {
                                src = "file://c:" + src;
                            }
                            else
                            {
                                src = "file://" + path + "/" + src;
                            }
                            html = html.Substring(0, p2) + src + html.Substring(ep);
                            p = ep;
                        }
                    }
                }
                p = -1;
                while ((p = html.IndexOf("<A", p + 1, StringComparison.OrdinalIgnoreCase)) >= 0)
                {
                    if (!Char.IsWhiteSpace(html[p + 2])) continue;
                    int p2 = html.IndexOf("href=", p, StringComparison.OrdinalIgnoreCase);
                    if (p2 > 0)
                    {
                        p2 += 5;
                        int ep;
                        if (html[p2] == '"')
                        {
                            ++p2;
                            ep = html.IndexOf('"', p2);
                        }
                        else
                        {
                            ep = html.IndexOfAny(attribEnd, p2);
                        }
                        string href = html.Substring(p2, ep - p2);
                        if (href.Length == 0) continue;
                        if (href.IndexOf("://") < 0)
                        {
                            if (href[0] == '/')
                            {
                                href = "file://c:" + href;
                            }
                            else
                            {
                                href = "file://" + path + "/" + href;
                            }
                            html = html.Substring(0, p2) + href + html.Substring(ep);
                            p = ep;
                        }
                    }
                }
            }
            return html;
        }


        static string IntTo2CharString(int v)
        {
            string rtn = "0" + v.ToString();
            if (rtn.Length == 2) return rtn;
            return rtn.Substring(1);
        }

        static public string GetHtml(string url, int numberOfDays, string linkProcessor, string contentExtractor, string contentFormatter, int depth, StringBuilder log)
        {
            if (url == null)
            {
                log.Append("No valid URL for item");
                return null;
            }
            if (url.IndexOf("@yy") >= 0 || url.IndexOf("@mm") >= 0 || url.IndexOf("@dd") >= 0)
            {
                StringBuilder rtn = new StringBuilder();
                for (int i = numberOfDays; i >= 0; --i)
                {
                    DateTime d = DateTime.Today.AddDays(-i);
                    string u = url.Replace("@mm", IntTo2CharString(d.Month)).Replace("@dd", IntTo2CharString(d.Day)).Replace("@yyyy", d.Year.ToString()).Replace("@yy", IntTo2CharString(d.Year - 1900));
                    string contents = Utils.GetContent(u, null, linkProcessor, contentExtractor, contentFormatter, depth, log);
                    if (contents != null)
                    {
                        rtn.Append(contents);
                    }
                }
                return rtn.ToString();
            }
            else
            {
                return Utils.GetContent(url, null, linkProcessor, contentExtractor, contentFormatter, depth, log);
            }
        }

        static public void ShowHelp(Control parent, string chmFile, string topic)
        {
            // TODO: topixs
            string exeName = Application.ExecutablePath;
            string chmPath = Path.Combine(Path.GetDirectoryName(exeName), chmFile);
            Help.ShowHelp(parent, chmPath);
        }

        // Get the plugins

        static public List<T> GetPlugins<T>(string subfolder, string namePat)
        {
            string exeName = Application.ExecutablePath;
            string folder = Path.Combine(Path.GetDirectoryName(exeName), subfolder);
            string[] files = Directory.GetFiles(folder, namePat);
            List<T> tList = new List<T>();
            Debug.Assert(typeof(T).IsInterface);
            foreach (string file in files)
            {
                try
                {
                    Assembly assembly = Assembly.LoadFile(file);
                    foreach (Type type in assembly.GetTypes())
                    {
                        if (!type.IsClass || type.IsNotPublic) continue;
                        Type[] interfaces = type.GetInterfaces();
                        if (((IList)interfaces).Contains(typeof(T)))
                        {
                            object obj = Activator.CreateInstance(type);
                            T t = (T)obj;
                            tList.Add(t);
                        }
                    }
                }
                catch (Exception ex)
                {
                    //LogError(ex);
                }
            }
            return tList;
        }

        static public List<IHtmlConverter> GetConverters()
        {
            return GetPlugins<IHtmlConverter>(".", "write*.dll");
        }

        static public List<ISyncDevice> GetDevices()
        {
            return GetPlugins<ISyncDevice>(".", "sync*.dll");
        }

        public static byte[] Convert(
                                IParserBasedHtmlConverter converter,
                                string html,
                                ref string output)
        {
            ParseHTML parse = new ParseHTML();
            StringBuilder text = new StringBuilder();
            ArrayList images = new ArrayList();
            parse.Source = html;
            bool gobble = false;

            try
            {
                Stack<FontStyle> fontStyleStack = new Stack<FontStyle>();
                fontStyleStack.Push(0);
                Stack<bool> gotFirstListItemStack = new Stack<bool>();

                while (!parse.Eof())
                {
                    char ch = parse.Parse();
                    if (ch == 0)
                    {
                        string txt = text.ToString().Replace("&nbsp;", " ").Replace("&amp;", "&").Replace("&lt;", "<").Replace("&gt;", ">");
                        txt = txt.Replace("&middot;", "*").Replace("&mdash;", "-");
                        converter.HandleText(txt, converter.DefaultFont, fontStyleStack.Peek());
                        text.Length = 0;
                        AttributeList tag = parse.GetTag();
                        switch (tag.Name.ToLower())
                        {
                            case "script":
                            case "head":
                            case "style":
                            case "meta": // need this one?
                                gobble = true;
                                break;
                            case "/script":
                            case "/head":
                            case "/style":
                            case "/meta": // need this one?
                                gobble = false;
                                break;
                            case "i":
                            case "em":
                                fontStyleStack.Push(fontStyleStack.Peek() | FontStyle.Italic);
                                break;
                            case "b":
                            case "bf":
                                fontStyleStack.Push(fontStyleStack.Peek() | FontStyle.Bold);
                                break;
                            case "a":
                                fontStyleStack.Push(fontStyleStack.Peek() | FontStyle.Underline);
                                break;
                            case "/em":
                            case "/bf":
                            case "/b":
                            case "/a":
                            case "/i":
                                if (fontStyleStack.Count > 1) fontStyleStack.Pop();
                                break;
                            case "h1":
                            case "h2":
                            case "h3":
                            case "h4":
                            case "h5":
                                converter.FlushParagraph();
                                int hl = tag.Name[1] - '0';
                                if (hl > 5) hl = 5;
                                converter.EnterHeader(hl);
                                break;
                            case "/h1":
                            case "/h2":
                            case "/h3":
                            case "/h4":
                            case "/h5":
                                converter.FlushParagraph();
                                converter.ExitHeader();
                                break;
                            /*case "font":
                                // Hack!
                                Attribute sat = tag["size"];
                                int fs = (sat == null) ? 1 : int.Parse(sat.Value);
                                int hdl = 5 - fs;
                                if (hdl < 3) hdl = 3;
                                converter.EnterHeader(hdl);
                                break;
                            case "/font":
                                converter.ExitHeader();
                                break;*/
                            case "table":
                                converter.FlushParagraph();
                                converter.StartTable();
                                break;
                            case "/table":
                                converter.EndTable();
                                break;
                            case "tr":
                                converter.StartRow();
                                break;
                            case "/tr":
                                converter.EndRow();
                                break;
                            case "td":
                                int cspan = 1;
                                Attribute a = tag["colspan"];
                                string span = (a == null) ? null : a.Value;
                                if (span != null && span.Length > 0)
                                {
                                    cspan = int.Parse(span);
                                }
                                converter.StartCell(cspan);
                                break;
                            case "/td":
                                converter.EndCell();
                                break;
                            case "p":
                                converter.FlushParagraph();
                                break;
                            case "br":
                                converter.LineBreak();
                                break;
                            case "ul":
                                converter.FlushParagraph();
                                converter.StartUnorderedList();
                                gotFirstListItemStack.Push(false);
                                break;
                            case "ol":
                                converter.FlushParagraph();
                                converter.StartOrderedList();
                                gotFirstListItemStack.Push(false);
                                break;
                            case "li":
                                if (gotFirstListItemStack.Count > 0)
                                {
                                    // This isn't going to work for the first list item. We need a way to identify the first list item
                                    // and skip the next line. One crude way it to add a stack of bools parallel to the lists stack
                                    if (!gotFirstListItemStack.Peek())
                                    {
                                        gotFirstListItemStack.Pop();
                                        gotFirstListItemStack.Push(true);
                                    }
                                    else
                                    {
                                        converter.FlushListItem();
                                    }
                                }
                                break;
                            case "/ul":
                            case "/ol":
                                if (gotFirstListItemStack.Count > 0)
                                {
                                    converter.EndList();
                                    gotFirstListItemStack.Pop();
                                }
                                break;
                            case "img":
                                converter.FlushParagraph();
                                try
                                {
                                    string url = tag["src"].Value;
                                    string extension = Path.GetExtension(url.Substring(url.LastIndexOf('/') + 1));
                                    string fname = Path.GetTempFileName() + extension;
                                    string basename = Path.GetFileName(fname);
                                    images.Add(fname);
                                    string error = null;
                                    // TODO: Need to handle file:// URIs
                                    if (GetFileFromWeb(Path.GetTempPath(), url, basename, ref error))
                                    {
                                        if (extension.ToLower() != ".jpg")
                                        {
                                            System.Drawing.Bitmap b = new System.Drawing.Bitmap(fname);
                                            string jpgName = fname.Substring(0, fname.Length - 4) + ".jpg";
                                            b.Save(jpgName, System.Drawing.Imaging.ImageFormat.Jpeg);
                                            b.Dispose();
                                            images.Add(fname = jpgName);
                                        }
                                        converter.AddImage(fname);
                                    }
                                }
                                catch
                                { }
                                break;
                        }
                    }
                    else if (!gobble)
                    {
                        if (ch > 127)
                        {
                            if (ch < 256)
                            {
                                text.Append("\\'");
                                text.Append(Hex(ch / 16));
                                text.Append(Hex(ch % 16));
                            }
                            else
                            {
                                //TODO: need a translation table for some common unicode chars
                                text.Append(' ');
                            }
                        }
                        else if (!Char.IsWhiteSpace(ch))
                        {
                            text.Append(ch);
                        }
                        else if (text.Length == 0 || !Char.IsWhiteSpace(text[text.Length - 1]))
                        {
                            text.Append(' ');
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                output = ex.Message;
            }
            byte[] rtn = converter.GetBytes();
            foreach (string image in images)
            {
                try
                {
                    File.Delete(image);
                }
                catch
                { }
            }
            return rtn;
        }

        [DllImport("wininet.dll", SetLastError = true)]
        public static extern bool InternetGetCookie(
          string url, string cookieName,
          StringBuilder cookieData, ref int size);

        private static CookieContainer GetUriCookieContainer(Uri uri)
        {
            CookieContainer cookies = null;

            // Determine the size of the cookie
            int datasize = 256;
            StringBuilder cookieData = new StringBuilder(datasize);

            if (!InternetGetCookie(uri.ToString(), null, cookieData,
              ref datasize))
            {
                if (datasize < 0)
                    return null;

                // Allocate stringbuilder large enough to hold the cookie
                cookieData = new StringBuilder(datasize);
                if (!InternetGetCookie(uri.AbsoluteUri, null, cookieData,
                  ref datasize))
                    return null;
            }

            if (cookieData.Length > 0)
            {
                cookies = new CookieContainer();
                cookies.SetCookies(uri, cookieData.ToString().Replace(';', ','));
            }
            return cookies;
        }

        public static string CleanHtml(string html, ref string output)
        {
            ParseHTML parse = new ParseHTML();
            StringBuilder rtn = new StringBuilder();
            parse.Source = html;
            bool gobble = false;

            try
            {
                while (!parse.Eof())
                {
                    char ch = parse.Parse();
                    if (ch == 0)
                    {
                        AttributeList tag = parse.GetTag();
                        switch (tag.Name.ToLower())
                        {
                            case "head":
                            case "script":
                            case "style":
                            case "meta": // need this one?
                                gobble = true;
                                break;
                            case "body":
                            case "/script":
                            case "/style":
                            case "/meta": // need this one?
                                gobble = false;
                                break;
                            case "i":
                            case "em":
                            case "b":
                            case "bf":
                            case "a":
                            case "/em":
                            case "/b":
                            case "/bf":
                            case "/a":
                            case "/i":
                            case "h1":
                            case "h2":
                            case "h3":
                            case "h4":
                            case "h5":
                            case "/h1":
                            case "/h2":
                            case "/h3":
                            case "/h4":
                            case "/h5":
                            case "table":
                            case "/table":
                            case "tr":
                            case "/tr":
                            case "/td":
                            case "p":
                            case "br":
                            case "ul":
                            case "ol":
                            case "li":
                            case "/ul":
                            case "/ol":
                                rtn.Append("<");
                                rtn.Append(tag.Name.ToLower());
                                rtn.Append(">");
                                break;
                            case "td":
                                int cspan = 1;
                                Attribute a = tag["colspan"];
                                string span = (a == null) ? null : a.Value;
                                if (span != null && span.Length > 1 && span != "1")
                                {
                                    rtn.Append("<td span=\"" + span + "\">");
                                }
                                else
                                {
                                    rtn.Append("<td>");
                                }
                                break;
                            case "img":
                                try
                                {
                                    string url = tag["src"].Value;
                                    rtn.Append("<img src=\"" + url + "\">");
                                }
                                catch
                                { }
                                break;
                            default:
                                Console.WriteLine("Unhandled tag " + tag.Name);
                                break;
                        }
                    }
                    else if (!gobble)
                    {
                        if (!Char.IsWhiteSpace(ch))
                        {
                            rtn.Append(ch);
                        }
                        else if (rtn.Length == 0 || !Char.IsWhiteSpace(rtn[rtn.Length - 1]))
                        {
                            rtn.Append(' ');
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                output = ex.Message;
            }
            return rtn.ToString();
        }
    }
}
