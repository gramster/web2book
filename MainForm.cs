//#define OLD
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Net;
using System.Text.RegularExpressions;
using System.Diagnostics;

// TODO:
// - separate debug from error logs, and log errors even when not testing
//- Cancel buttons should revert changes


namespace web2book
{
    public partial class MainForm : Form
    {
        Timer autoUpdateTimer = new Timer();
        
        string xmlSourcesFile;
        const int feedFileVersion = 3;
        TestForm logger, errorlogger;
        public static ProgressForm progressForm;
        ContentSourceList currentContentSource;
        ContentSourceList[] sources;

        public MainForm()
        {
            InitializeComponent();
            this.Text = "Web2Book rel " + Utils.RELEASE.ToString() + " by GeekRaver";

            List<ISource> sourceConfigurations = Utils.Sources;
            sources = new ContentSourceList[sourceConfigurations.Count];

            this.SuspendLayout();

            for (int i = 0; i < sources.Length; i++)
            {
                sources[i] = new ContentSourceList(sourceConfigurations[i]);
            }

            this.ResumeLayout();

            xmlSourcesFile = Directory.GetCurrentDirectory() + @"\sources.xml";
            if (ImportSources(xmlSourcesFile) == 0) // fall back to old file temporarily
            {
                ImportSources(Directory.GetCurrentDirectory() + @"\feeds.xml");
            }

            // Add the sources to the tab control in order of # of entries

            for (int i = 0; i < (sources.Length - 1); i++)
            {
                for (int j = i + 1; j < sources.Length; j++)
                {
                    if (sources[i].Count < sources[j].Count)
                    {
                        ContentSourceList csl = sources[i];
                        sources[i] = sources[j];
                        sources[j] = csl;
                    }
                }
            }

            for (int i = 0; i < sources.Length; i++)
            {
                this.tabControl.Controls.Add(sources[i].TabPage);
            }

            toolTip1.SetToolTip(this.propertiesButton, "Customize properties of a item"); // TODO: make modal
            toolTip1.SetToolTip(this.deleteButton, "Delete an item"); // TODO: make modal
            toolTip1.SetToolTip(this.exportButton, "Export an item");
            toolTip1.SetToolTip(this.publishButton, "Publish item settings to the web");
            toolTip1.SetToolTip(this.goButton, "Start synching new content"); // TODO make modal
            toolTip1.SetToolTip(this.donateButton, "Make a donation to support further development of web2book");
            autoUpdateTimer.Tick += new EventHandler(AutoUpdate);
            ScheduleAutoUpdate();
            Debug.Assert(sources.Length > 0);
            tabControl.SelectedTab = sources[0].TabPage;
            SetCurrentSource();
        }

        #region Get currently selected source class or source item

        ContentSourceList CurrentContentSourceClass()
        {
            for (int i = 0; i < sources.Length; i++)
            {
                if (tabControl.SelectedTab == sources[i].TabPage)
                {
                    return sources[i];
                }
            }
            Debug.Fail("No tab page match to source");
            return null;
        }

        ContentSource CurrentSource()
        {
            return currentContentSource.CurrentItem();
        }

        #endregion

        #region Logging (source independent)

        void Log(string txt)
        {
            if (logger != null) logger.Log(txt);
        }

        void Error(string txt)
        {
            if (errorlogger != null) errorlogger.Log(txt);
        }

        #endregion

        #region Progress Form Handling 

        void OpenProgressForm()
        {
            progressForm = new ProgressForm();
            progressForm.Show();
        }

        void CloseProgressForm()
        {
            if (progressForm != null)
            {
                if (progressForm.Cancelled) Error("Cancelled by user");
                progressForm.Close();
                progressForm = null;
            }
        }

        #endregion

        #region Menu Item Handlers 

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            autoUpdateTimer.Enabled = false;
            new OptionsForm().ShowDialog();
            ScheduleAutoUpdate();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            autoUpdateTimer.Enabled = false;
            new AboutForm().ShowDialog();
            ScheduleAutoUpdate();
        }

        private void regExpHelperToolStripMenuItem_Click(object sender, EventArgs e)
        {
            autoUpdateTimer.Enabled = false;
            new RegExpTesterForm().ShowDialog();
            ScheduleAutoUpdate();
        }

        private void enableAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BulkEnable(true);
        }

        private void disableAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BulkEnable(false);
        }

        #endregion

        #region Auto-Updating

        void ScheduleAutoUpdate()
        {
            autoUpdateTimer.Enabled = false;
            if (Settings.Default.AutoUpdate)
            {
                DateTime when = DateTime.Today.AddSeconds(Settings.Default.AutoUpdateTime);
                if (when < DateTime.Now) when = when.AddDays(1);
                autoUpdateTimer.Interval = (int)((when.Ticks - DateTime.Now.Ticks) / TimeSpan.TicksPerMillisecond);
                autoUpdateTimer.Start();
            }
        }

        void AutoUpdate(object sender, EventArgs ea)
        {
            autoUpdateTimer.Enabled = false;
            for (int i = 0; i < sources.Length; i++)
            {
                if (sources[i].Source.HasVolatileContent)
                {
                    Go(sources[i], true);
                }
            }
            ScheduleAutoUpdate();
        }

        #endregion

        #region Convert Html to target format

        string RunConverter(IHtmlConverter converter, string fname, byte[] html8, ref string output)
        {
            string rtn = Path.Combine(Settings.Default.OutputPath, fname + converter.Extension.ToLower());
            Utils.SaveBytes(rtn, Utils.Convert(converter.Converter,
                                        new string(System.Text.Encoding.ASCII.GetChars(html8)),
                                        ref output));
            return rtn;
        }

        IHtmlConverter GetConverter()
        {
            foreach (IHtmlConverter c in Utils.Converters)
            {
                if (c.Name == Settings.Default.OutputFormat)
                {
                    return c;
                }
            }
            return null;
        }

        string ConvertHtml(StringBuilder htm, string fname, ContentSource cs, bool isAutoUpdate)
        {
            string output = null;
            string rtn = null;
            fname = fname.Trim(Path.GetInvalidFileNameChars());
            string html = htm.ToString();
            IHtmlConverter converter = GetConverter();
            if (converter != null && converter.MustScrubHtml)
                html = Utils.CleanHtml(html, ref output);
            byte[] html8 = Utils.CleanText(html);
            ISyncDevice device = isAutoUpdate ? null : GetSyncDevice();

            if (converter != null)
            {
                TypeFace f = TypeFace.Courier;
                switch (Settings.Default.Typeface)
                {
                    case 0: // courier
                        f = TypeFace.Courier;
                        break;
                    case 1: // helvetica
                        f = TypeFace.Helvetica;
                        break;
                    case 2: // times roman
                        f = TypeFace.TimesRoman;
                        break;
                }
                converter.Initialize(
                                    cs.Author,
                                    cs.Name,
                                    Settings.Default.LeftMargin,
                                    Settings.Default.RightMargin,
                                    Settings.Default.TopMargin,
                                    Settings.Default.BottomMargin,
                                    Settings.Default.PageWidth,
                                    Settings.Default.PageHeight,
                                    f,
                                    Settings.Default.FontSize,
                                    ((device == null) ? 0 : device.DisplayWidth), // display res if synching to device, else 0,0
                                    ((device == null) ? 0 : device.DisplayHeight),
                                    ((device == null) ? 0 : device.DisplayDepth),
                                    cs.IsStructured
                                    );
            }

            // TODO: Must canonicalize any funky chars in fname!
            string htmlFile = null;

            if (converter == null || converter.IsFileConverter || Settings.Default.HtmlTidyPath.Length > 0)
            {
                htmlFile = Path.Combine(Settings.Default.OutputPath, fname + ".htm");
                Utils.SaveBytes(htmlFile, html8);
                try
                {
                    if (Settings.Default.HtmlTidyPath.Length > 0 && File.Exists(Settings.Default.HtmlTidyPath))
                        Utils.RunExternalCommand(Settings.Default.HtmlTidyPath, "-m " + fname + ".htm", Settings.Default.OutputPath, false, 60, out output);
                }
                catch (Exception ex)
                {
                    output += "Error running " + Settings.Default.HtmlTidyPath + ": " + ex.Message + "\r\n";
                }
            }
            if (converter == null)
            {
                rtn = htmlFile;
                // Can't clean up as we may have hrefs to temp files
            }
            else if (converter.IsFileConverter)
            {
                rtn = converter.ConvertHtmlFile(Settings.Default.OutputPath, fname, ref output);
                File.Delete(htmlFile);
                cs.CleanUp();
            }
            else
            {
                if (htmlFile != null) // reload html
                {
                    html8 = Utils.LoadBytes(htmlFile);
                }
                rtn = RunConverter(converter, fname, html8, ref output);
                cs.CleanUp();
            }

            if (output != null && output.Length > 0)
            {
                Log(output);
            }
            if (rtn != null && File.Exists(rtn))
            {
                return rtn;
            }
            return null;
        }

        #endregion

        #region Content Generation

        bool OutputDirectoryExists()
        {
            if (!Directory.Exists(Settings.Default.OutputPath))
            {
                if (errorlogger != null)
                    errorlogger.Log("Output directory " + Settings.Default.OutputPath + " does not exist. Please fix this by creating the directory or changing the setting under Tools - Options menu.");
                return false;
            }
            return true;
        }

        void Go(ContentSourceList sc, bool isAutoUpdate)
        {
            Cursor.Current = Cursors.WaitCursor;
            goButton.Enabled = false;

            if (!isAutoUpdate) errorlogger = new TestForm();

            if (OutputDirectoryExists())
            {
                OpenProgressForm();
                progressForm.Start(sc.Count);
                ArrayList books = MakeBooks(sc, isAutoUpdate);
                if (books != null && books.Count > 0 && !isAutoUpdate && !progressForm.Cancelled)
                    SyncFiles(books, sc.Source.FileNamePrefix);

                CloseProgressForm();
            }

            if (errorlogger != null)
            {
                errorlogger.ShowResults();
                errorlogger = null;
            }

            goButton.Enabled = true;
            Cursor.Current = Cursors.Default;
        }

        string AddSource(ContentSourceList sourceClass, ContentSource source, bool isAutoUpdate)
        {
            Error("Processing " + source.Name); // not really an error but Log is reserved for verbose testing
            StringBuilder log = new StringBuilder();
            string rtn = null;
            try
            {
                ISyncDevice device = null;
                if (!isAutoUpdate) device = GetSyncDevice();
                string h = source.GetHtml(
                                    sourceClass.Source,
                                    ((device == null) ? 0 : device.DisplayWidth), // display res if synching to device, else 0,0
                                    ((device == null) ? 0 : device.DisplayHeight),
                                    ((device == null) ? 0 : device.DisplayDepth),
                                    log);
                Log(log.ToString());
                log.Length = 0;
                if (h != null)
                {
                    bool hasTopLevelTag = (h.IndexOf("<html>", StringComparison.OrdinalIgnoreCase) >= 0);
                    StringBuilder htm = new StringBuilder();
                    if (!hasTopLevelTag)
                    {
                        if (sourceClass.Source.HasVolatileContent)
                        {
                            htm.Append("<html><head>" + source.Name + " (" + DateTime.Now.ToString() + ")</head><body>\r\n");
                        }
                        else
                        {
                            htm.Append("<html><head>" + source.Name + "</head><body>\r\n");
                        }
                    }
                    htm.Append(h);
                    htm.Append("</body></html>\r\n");
                    string fname = sourceClass.Source.FileNamePrefix + "-" + source.Name;
                    if (sourceClass.Source.HasVolatileContent)
                    {
                        fname += "-" + DateTime.Now.ToShortDateString().Replace("/", "-");
                    }
                    rtn = ConvertHtml(htm, fname, source, isAutoUpdate);
                }
                else
                {
                    source.CleanUp();
                }
            }
            catch (Exception ex)
            {
                Error(log.ToString() + "\r\n" + ex.ToString());
            }
            return rtn;
        }

        ArrayList MakeBook(ContentSourceList sourceClass, ContentSource source, bool isAutoUpdate)
        {
            ArrayList books = null;
            try
            {
                books = new ArrayList();
                string fname = AddSource(sourceClass, source, isAutoUpdate);
                if (fname != null) books.Add(fname);
            }
            catch (Exception ex)
            {
                Error(ex.ToString());
            }
            return books;
        }

        ArrayList MakeBooks(ContentSourceList source, bool isAutoUpdate)
        {
            ArrayList books = null;
            try
            {
                books = new ArrayList();
                for (int i = 0; i < source.Count; i++)
                {
                    if (progressForm != null && progressForm.Cancelled)
                        return null;

                    if (source[i].Enabled)
                    {
                        progressForm.Message = source[i].Name;
                        string fname = AddSource(source, source[i], isAutoUpdate);
                        if (fname != null) books.Add(fname);
                    }
                    progressForm.Next();
                }
            }
            catch (Exception ex)
            {
                Error(ex.ToString());
            }
            return books;
        }

        #endregion

        #region Device sync (source independent)

        ISyncDevice GetSyncDevice()
        {
            if (Settings.Default.SyncToDevice)
            {
                foreach (ISyncDevice d in Utils.Devices)
                {
                    if (d.Name == Settings.Default.SyncDevice)
                    {
                        return d;
                    }
                }
            }
            return null;
        }

        void SyncFiles(ArrayList books, string type)
        {
            ISyncDevice device = GetSyncDevice();

            if (device != null)
            {
                try
                {
                    while (!device.Connect())
                    {
                        if (MessageBox.Show(device.ConnectPrompt, "Connect Failure", MessageBoxButtons.RetryCancel) == DialogResult.Cancel)
                            break;
                    }
                    if (device.IsConnected)
                    {
                        if (type != null)
                        {
                            string[] files = device.GetFiles(type);
                            if (Settings.Default.DeleteFiles)
                            {
                                progressForm.Message = "Deleting existing files";
                                progressForm.Start(files.Length);
                                foreach (string f in files)
                                {
                                    try
                                    {
                                        progressForm.Message = "Removing old file " + f;
                                        if (!device.DeleteFile(f))
                                            Error("Failed to remove old file " + f);
                                    }
                                    catch (Exception ex)
                                    {
                                        Error("Exception occurred while trying to remove old file " + f + "\r\n" + ex.ToString());
                                    }
                                    progressForm.Next();
                                    if (progressForm.Cancelled) return;
                                }
                            }

                            progressForm.Message = "Adding new files";
                            progressForm.Start(books.Count);
                            foreach (string f in books)
                            {
                                try
                                {
                                    progressForm.Message = "Adding new file " + f;
                                    string fname = Path.GetFileName(f);
                                    if (!Settings.Default.DeleteFiles)
                                    {
                                        try
                                        {
                                            device.DeleteFile(fname);
                                        }
                                        catch (Exception ex)
                                        {
                                        }
                                    }
                                    if (!device.AddFile(f, fname))
                                    {
                                        Error("Failed to sync " + f);
                                    }
                                    else
                                    {
                                        if (Settings.Default.DeleteAfterSync)
                                        {
                                            File.Delete(f);
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Error("Exception occurred while trying to sync file " + f + "\r\n" + ex.ToString());
                                }
                                progressForm.Next();
                                if (progressForm.Cancelled) return;
                            }
                            device.Disconnect();
                        }
                        else Error("Couldn't connect to device");
                    }
                }
                catch (Exception ex)
                {
                    Error(ex.ToString());
                }
            }
        }

        #endregion

        #region Save/Export Sources

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveSources(xmlSourcesFile);
            Settings.Default.Save();
        }

        private void SaveSources(string fname)
        {
            XmlTextWriter xw = null;
            try
            {
                xw = new XmlTextWriter(fname, Encoding.ASCII);
                xw.WriteStartElement("web2book");
                for (int i = 0; i < sources.Length; i++)
                {
                    sources[i].Write(xw);
                }
                xw.WriteEndElement();
            }
            catch (Exception ex)
            {
            }
            finally
            {
                if (xw != null) xw.Close();
            }
        }

        private void Export(string fname, ContentSource f)
        {
            XmlTextWriter xw = null;
            try
            {
                xw = new XmlTextWriter(fname, Encoding.ASCII);
                xw.WriteStartElement(f.ElementName+"s");
                f.Write(xw);
                xw.WriteEndElement();
            }
            catch (Exception ex)
            {
            }
            finally
            {
                if (xw != null) xw.Close();
            }
        }

        private void exportButton_Click(object sender, EventArgs e)
        {
            autoUpdateTimer.Enabled = false;
            ContentSource cs = CurrentSource();
            if (cs != null)
            {
                string f = Utils.BrowseXmlFileWrite("Export to File", cs.Name);
                if (f != null) Export(f, cs);
            } 
            ScheduleAutoUpdate();
        }

        #endregion

        #region Load/Import Sources

        int ImportSources(XmlTextReader xr)
        {
            int rtn = 0;
            while (xr.Read())
            {
                if (xr.NodeType == XmlNodeType.Element)
                {
                    for (int i = 0; i < sources.Length; i++)
                    {
                        if (xr.LocalName == sources[i].Source.ElementName)
                        {
                            ContentSource s = sources[i].Source.MakeSource();
                            s.Read(xr);
                            sources[i].Add(s);
                            ++rtn;
                        }
                        else if (xr.LocalName == (sources[i].Source.ElementName+"s")) // read group settings
                        {
                            sources[i].Source.Read(xr);
                        }
                    }
                }
            }
            return rtn;
        }

        private int ImportSources(string fname)
        {
            FileStream fs = null;
            XmlTextReader xr = null;
            try
            {
                fs = File.OpenRead(fname);
                xr = new XmlTextReader(fs);
                return ImportSources(xr);
            }
            catch (Exception ex)
            {
                return 0;
            }
            finally
            {
                if (xr != null) xr.Close();
                if (fs != null) fs.Close();
            }
        }

        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {
            autoUpdateTimer.Enabled = false;
            string[] files = Utils.BrowseXmlFileRead("Import From");
            if (files != null)
            {
                foreach (string f in files) ImportSources(f);
            }
            ScheduleAutoUpdate();
        }

        #endregion

        #region Publish

        void Publish(object sender, EventArgs ea)
        {
            Publish(currentContentSource.Source, (ContentSource)sender);
        }

        bool AlreadyPublished(ISource s, ContentSource cs)
        {
            ArrayList f = Utils.GetPublishedSources(Settings.Default.Username, Settings.Default.Password);
            foreach (PublishedSource pcs in f)
            {
                if (pcs.Name == cs.Name)
                {
                    ContentSource pf = Utils.GetSource(pcs.Url, Settings.Default.Username, Settings.Default.Password);
                    if (pf != null)
                    {
                        if (!s.IsType(pf))
                        {
                            MessageBox.Show("An item with the same name but different type already exists", "Mismatched Entry Types", MessageBoxButtons.OK);
                            return true;
                        }
                        string diff = s.DescribeDifferences(pf, cs);
                        Debug.Assert(diff != null);
                        if (diff.Length == 0)
                        {
                            MessageBox.Show("An identical entry already exists", "Duplicate "+s.SourceName, MessageBoxButtons.OK);
                            return false;
                        }
                        else if (MessageBox.Show(diff+"\r\n\r\nDo you still want to publish?", "Duplicate "+s.SourceName, MessageBoxButtons.YesNo) == DialogResult.No)
                        {
                            return false;
                        }
                        return true;
                    }
                }
            }
            return false;
        }

        void Publish(ISource s, ContentSource cs)
        {
            MemoryStream ms = null;
            XmlTextWriter xw = null;

            try
            {
                publishButton.Enabled = false;
                Cursor = Cursors.WaitCursor;
                if (cs.Name == null || cs.Name.Length == 0)
                {
                    MessageBox.Show("Please name the item first!");
                }
                else if (!AlreadyPublished(s, cs))
                {
                    CommentForm f = new CommentForm(cs.Name);
                    if (f.ShowDialog() != DialogResult.Cancel)
                    {
                        cs.Comment = f.Comment;
                        ms = new MemoryStream();
                        xw = new XmlTextWriter(ms, Encoding.ASCII);
                        xw.WriteStartElement(cs.ElementName + "s");
                        cs.Write(xw);
                        xw.WriteEndElement();
                        xw.Flush();
                        if (Utils.PublishOverHttp(ms.ToArray(),
                            cs.Name + "[" + Settings.Default.Username + ":" + DateTime.Now.ToUniversalTime().Ticks.ToString() + "]",
                            Settings.Default.Username, Settings.Default.Password))
                        {
                            MessageBox.Show("Publish successful!");
                        }
                        else
                        {
                            MessageBox.Show("Publish failed");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                if (xw != null) xw.Close();
                publishButton.Enabled = true;
                Cursor = Cursors.Default;
            }
        }

        #endregion

        #region Subscribe

        private void subscribeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            autoUpdateTimer.Enabled = false;
            SubscribeForm sf = new SubscribeForm(currentContentSource);
            sf.ShowDialog();
            ArrayList sourcesToAdd = sf.SourcesToAdd;
            foreach (ContentSource s in sourcesToAdd)
            {
                for (int i = 0; i < sources.Length; i++)
                {
                    if (sources[i].Source.IsType(s))
                    {
                        sources[i].Add(s);
                        break;
                    }
                }
            }
            ScheduleAutoUpdate();
        }

        #endregion

        #region Test Individual Sources

        void Test(ContentSourceList sourceClass, ContentSource source)
        {
            Debug.Assert(source != null);
            logger = errorlogger = new TestForm();
            logger.ShowResults();
            if (OutputDirectoryExists())
            {
                ArrayList books = MakeBook(sourceClass, source, false);
                if (books != null && books.Count > 0)
                {
                    string output;
                    Log("Opening file " + (string)books[0]);
                    Utils.RunExternalCommand((string)books[0], "", ".", true, -1, out output);
                    if (output != null) Log(output);
                }
            }
            logger.CloseIfEmpty();
            logger = errorlogger = null;
        }

        void Test(object sender, EventArgs ea)
        {
            Test(currentContentSource, (ContentSource)sender);
        }

        #endregion

        #region Button Handlers

        void EnableButtons(bool state)
        {
            testButton.Enabled = goButton.Enabled = propertiesButton.Enabled = deleteButton.Enabled = publishButton.Enabled = exportButton.Enabled = state;
        }

        void StartButtonAction()
        {
            autoUpdateTimer.Enabled = false;
            EnableButtons(false);
        }

        void EndButtonAction()
        {
            ScheduleAutoUpdate();
            EnableButtons(true);
        }

        private void goButton_Click(object sender, EventArgs e)
        {
            StartButtonAction();
            if (currentContentSource.Count > 0) Go(currentContentSource, false);
            EndButtonAction();
        }

        private void propertiesButton_Click(object sender, EventArgs e)
        {
            StartButtonAction();
            ContentSource cs = currentContentSource.CurrentItem();
            if (cs != null)
            {
                currentContentSource.Source.CustomizeItem(cs, new EventHandler(Test), new EventHandler(Publish));
            }
            EndButtonAction();
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            StartButtonAction();
            currentContentSource.DeleteCurrent();
            EndButtonAction();
        }

        private void publishButton_Click(object sender, EventArgs e)
        {
            StartButtonAction();
            ContentSource cs = currentContentSource.CurrentItem();
            if (cs != null)
            {
                Publish(currentContentSource.Source, cs);
            }
            EndButtonAction();
        }

        private void testButton_Click(object sender, EventArgs e)
        {
            StartButtonAction();
            ContentSource cs = currentContentSource.CurrentItem();
            if (cs != null)
            {
                Test(currentContentSource, cs);
            }
            EndButtonAction();
        }

        private void donateButton_Click(object sender, EventArgs e)
        {
            Utils.OpenUrl("https://www.paypal.com/cgi-bin/webscr?cmd=_xclick&business=gram%40geekraver%2ecom&item_name=rss2book%20donations&no_shipping=1&no_note=1&tax=0&currency_code=USD&lc=US&bn=PP%2dDonationsBF&charset=UTF%2d8");
        }

        #endregion 

        #region Bulk enable/disable

        void BulkEnable(bool v)
        {
            for (int i = 0; i < currentContentSource.Count; i++)
            {
                currentContentSource[i].Enabled = v;
            }
            currentContentSource.ResetBindings();
        }

        #endregion

        #region Switch source class (tab page)

        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetCurrentSource();
        }

        void SetCurrentSource()
        {
            currentContentSource = CurrentContentSourceClass();
            publishButton.Enabled = currentContentSource.Source.CanPublish;
            propertiesButton.Enabled = currentContentSource.Source.HasItemCustomizeUI;
            sourceGroupOptionsToolStripMenuItem.Enabled = currentContentSource.Source.HasGroupCustomizeUI;
            sourceGroupOptionsToolStripMenuItem.Text = currentContentSource.Source.SourceName + " Source Plugin Options...";
        }

        #endregion

        private void reportBugRequestFeatureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Utils.OpenUrl("http://www.geekraver.com/flyspray/index.php?do=newtask&project=4");
        }

        private void sourceGroupOptionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentContentSource.Source.CustomizeGroup();
        }

        private void forumToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Utils.OpenUrl("http://www.mobileread.com/forums/showthread.php?t=7946");
        }
    }
}