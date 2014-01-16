using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

namespace web2book
{
    [Serializable]
    public class LitBookSource : LocalContentSource
    {
        public override string ElementName
        {
            get { return "LitBook"; }
        }

        public LitBookSource()
            : this(String.Empty, String.Empty, false, String.Empty)
        { }

        public LitBookSource(string name, string comment, bool enabled, string path)
            : base(name, comment, enabled, path)
        {
        }

        public override string GetHtml(ISource mySourceGroup, int displayWidth, int displayHeight, int displayDepth, StringBuilder log)
        {
            ReadLit rl = (ReadLit)mySourceGroup;
            string clitPath = rl.ClitPath;
            string tmpPath = System.IO.Path.GetTempPath() + "\\" + System.IO.Path.GetFileNameWithoutExtension(Path);
            if (!Directory.Exists(tmpPath)) 
                Directory.CreateDirectory(tmpPath);
            string output;
            Utils.RunExternalCommand(clitPath, "-d " + Path + " " + tmpPath, tmpPath, false, 60, out output);
            if (output != null) log.Append(output);
            string fname = tmpPath + @"\xml2lit_book";
            if (File.Exists(fname))
            {
                StreamReader sr = File.OpenText(fname);
                string rtn = sr.ReadToEnd();
                sr.Close();
                // strip xml stuff
                int p = rtn.IndexOf("<html>", StringComparison.OrdinalIgnoreCase);
                if (p > 0) rtn = rtn.Substring(p);

                return Utils.PatchLocalRefs(rtn, tmpPath);
            }
            return null;
        }
    }

    public class ReadLit : ISource
    {
        public string HtmlHelp { get { return null; } }

        public string ClitPath
        {
            get 
            {
                if (Settings.Default.ClitPath == null || Settings.Default.ClitPath.Length == 0)
                    Settings.Default.ClitPath = @"c:\Program Files\clit18\clit.exe";
                return Settings.Default.ClitPath; 
            }
            set { Settings.Default.ClitPath = value; }
        }

        public IContentSourceList DataSource
        {
            get { return new ContentSourceBindingList<LitBookSource>(this); }
        }

        public bool IsType(ContentSource cs) // is a source of this type?
        {
            return (cs is LitBookSource);
        }

        public string[] PrimaryFields
        {
            get { return new string[] { "Enabled", "Name", "Path" }; } // which fields should be displayed in the main view?
        }

        public bool HasVolatileContent
        {
            get { return false; } // Does the content change frequently?
        }

        public bool HasItemCustomizeUI
        {
            get { return true; }
        }

        public bool HasGroupCustomizeUI
        {
            get { return true; }
        }

        public bool CanPublish
        {
            get { return false; } // Can this source type be published to www.geekraver.com?
        }

        public string SourceName
        {
            get { return "Lit Book"; } // Friendly name to display in UI
        }

        public string ElementName
        {
            get { return "LitBook"; } // Xml element name to use in config files
        }

        public ContentSource MakeSource()// Factory method for source type
        {
            return new LitBookSource();
        }

        public string FileNamePrefix
        {
            get { return "lit"; }
        }

        public void CustomizeItem(ContentSource cs, EventHandler test, EventHandler publish)
        {
            new ReadLitItemConfigForm(cs).ShowDialog();
        }

        public void CustomizeGroup()
        {
            new ReadLitConfigForm(this).ShowDialog();
        }

        public string DescribeDifferences(ContentSource publishedSource, ContentSource mySource)
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
