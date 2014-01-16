using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace web2book
{
    [Serializable]
    public class ImageBookSource : LocalContentSource
    {
        int Width = 600;
        int Height = 800; // needs to be configureable

        public override string ElementName
        {
            get { return "ImageBook"; }
        }

        public ImageBookSource()
            : this(String.Empty, String.Empty, false, String.Empty)
        { }

        public ImageBookSource(string name, string comment, bool enabled, string path)
            : base(name, comment, enabled, path)
        {
        }

        ArrayList cleanImages = new ArrayList();

        int ParseIndex(string fname)
        {
            int start = -1; ;
            for (int i = 0; i < fname.Length; i++)
            {
                if (Char.IsDigit(fname[i]))
                {
                    if (start < 0) start = i;
                }
                else if (start >= 0)
                {
                    return int.Parse(fname.Substring(start, i - start));
                }
            }
            return 0;
        }

        public override string GetHtml(ISource mySourceGroup, int displayWidth, int displayHeight, int displayDepth, StringBuilder log)
        {
            string[] files = Directory.GetFiles(Path);
            int[] indices = new int[files.Length];
            ImageCleaner ic = new ImageCleaner();
            int p = 0;

            // Clean the files

            for (int i = 0; i < files.Length; i++)
            {
                try
                {
                    Bitmap b = new Bitmap(files[i]);
                    Bitmap cb = ic.Clean(b, Width, Height);
                    indices[p] = ParseIndex(System.IO.Path.GetFileNameWithoutExtension(files[i]));
                    files[p] = System.IO.Path.GetDirectoryName(files[i]) + "\\clean_" + System.IO.Path.GetFileName(files[i]);
                    cb.Save(files[i]);
                    b.Dispose();
                    cb.Dispose();
                    cleanImages.Add(files[p]);
                    p++;
                }
                catch (Exception ex)
                {
                }
            }

            // Sort the files

            for (int i = 0; i < (p - 1); i++)
            {
                for (int j = i + 1; j < p; j++)
                {
                    if (indices[i] > indices[j])
                    {
                        int t = indices[i];
                        string s = files[i];
                        indices[i] = indices[j];
                        files[i] = files[j];
                        indices[j] = t;
                        files[j] = s;
                    }
                }
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("<html><head>");
            sb.Append(Name);
            sb.Append("</head><body><h1>");
            sb.Append(Name);
            sb.Append("</h1><p>\r\n");
            for (int i = 0; i < p; i++)
            {
                sb.Append("<img src=\"file://");
                sb.Append(files[i].Replace('\\', '/'));
                sb.Append("\"><br>\r\n");
            }
            //sb.Append("</body></html>\r\n");
            return sb.ToString();
        }

        public override void CleanUp()
        {
            foreach (string s in cleanImages)
            {
                if (File.Exists(s)) File.Delete(s);
            }
            cleanImages.Clear();
        }
    }

    public class ReadImage : ISource
    {
        public string HtmlHelp { get { return null; } }

        public IContentSourceList DataSource
        {
            get { return new ContentSourceBindingList<ImageBookSource>(this); }
        }

        public bool IsType(ContentSource cs) // is a source of this type?
        {
            return (cs is ImageBookSource);
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
            get { return false; }
        }

        public bool HasGroupCustomizeUI
        {
            get { return false; }
        }

        public bool CanPublish
        {
            get { return false; } // Can this source type be published to www.geekraver.com?
        }

        public string SourceName
        {
            get { return "Image Book"; } // Friendly name to display in UI
        }

        public string ElementName
        {
            get { return "ImageBook"; } // Xml element name to use in config files
        }

        public ContentSource MakeSource()// Factory method for source type
        {
            return new ImageBookSource();
        }

        public string FileNamePrefix
        {
            get { return "img"; } 
        }

        public void CustomizeItem(ContentSource cs, EventHandler test, EventHandler publish)
        {
        }

        public void CustomizeGroup()
        {
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
