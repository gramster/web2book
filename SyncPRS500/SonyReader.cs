using System;
using System.Collections;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace web2book
{
    public class SonyReader : ISyncDevice
    {
        public string Name
        {
            get { return "Sony PRS-500"; }
        }

        public string HtmlHelp { get { return null; } }

        public int DisplayWidth
        {
            get { return 600; }
        }

        public int DisplayHeight 
        {
            get { return 800; }
        }

        public int DisplayDepth
        {
            get { return 2; } // 2bpp
        }

        internal class NativeMethods
        {
            [DllImport("UsbShim.dll")]
            static public extern int Connect([MarshalAs(UnmanagedType.LPStr)]string path);

            [DllImport("UsbShim.dll")]
            static public extern int GetNext([MarshalAs(UnmanagedType.LPStr)]string path, StringBuilder filename, int space);
    
            [DllImport("UsbShim.dll")]
            static public extern int Delete([MarshalAs(UnmanagedType.LPStr)]string path);
    
            [DllImport("UsbShim.dll")]
            static public extern int Copy([MarshalAs(UnmanagedType.LPStr)]string src, [MarshalAs(UnmanagedType.LPStr)] string dest);

            [DllImport("UsbShim.dll")]
            static public extern void Disconnect();
        }

        public string ConnectPrompt
        {
            get { return "Please ensure your Reader is attached and Connect software is NOT running"; }
        }

        bool connected;

        public bool IsConnected
        {
            get { return connected; }
        }

        public bool HasConfigurationUI
        {
            get { return true; }
        }

        public void Configure()
        {
            new ConfigureSonyReaderForm(this).ShowDialog();
        }

        public SonyReader()
        {}

        public string ConnectPath
        {
            get
            {
                if (Settings.Default.ConnectPath == null || Settings.Default.ConnectPath.Length == 0)
                    Settings.Default.ConnectPath = @"c:\Program Files\Sony\CONNECT Reader\Data\bin";
                return Settings.Default.ConnectPath;
            }
            set
            {
                Settings.Default.ConnectPath = value;
            }
        }

        public string TargetPath
        {
            get
            {
                if (Settings.Default.ReaderTargetPath == null || Settings.Default.ReaderTargetPath.Length == 0)
                    Settings.Default.ReaderTargetPath = @"/Data/media/books/";
                return Settings.Default.ReaderTargetPath;
            }
            set
            {
                Settings.Default.ReaderTargetPath = value;
            }
        }

        public bool Connect()
        {
            if (!connected)
            {
                connected = (NativeMethods.Connect(ConnectPath) >= 0);
            }
            return connected;
        }

        string[] GetFiles(string path, string pat)
        {
            ArrayList rtn = new ArrayList();
            bool mustDisconnect = false;
            if (!connected && Connect()) mustDisconnect = true;
            if (connected)
            {
                StringBuilder fname = new StringBuilder(256);
                while (NativeMethods.GetNext(path, fname, 256) >= 0)
                {
                    string f = fname.ToString();
                    if (pat == null || f.StartsWith(pat))
                    {
                        rtn.Add(f);
                    }
                }
                if (mustDisconnect) Disconnect();
            }
            return (string[])rtn.ToArray(typeof(string));
        }

        public string[] GetFiles(string prefix)
        {
            return GetFiles(TargetPath, prefix);
        }

        string FullPath(string path, string name)
        {
            string f = path;
            if (!f.EndsWith("/")) f = f + "/";
            f = f + name;
            return f;
        }

        bool DeleteFile(string path, string name)
        {
            bool rtn = false;
            bool mustDisconnect = false;
            if (!connected && Connect()) mustDisconnect = true;
            if (connected)
            {
                rtn = (NativeMethods.Delete(FullPath(path, name)) >= 0);
                Thread.Sleep(100);
                if (mustDisconnect) Disconnect();
            }
            return rtn;
        }

        public bool DeleteFile(string name)
        {
            return DeleteFile(TargetPath, name);
        }

        public bool AddFile(string path, string name)
        {
            bool rtn = false;
            bool mustDisconnect = false;
            if (!connected && Connect()) mustDisconnect = true;
            if (connected)
            {
                rtn = (NativeMethods.Copy(path, FullPath(TargetPath, name)) >= 0);
                Thread.Sleep(100);
                if (mustDisconnect) Disconnect();
            }
            return rtn;
        }

        public bool Disconnect()
        {
            if (connected) NativeMethods.Disconnect();
            connected = false;
            return true;
        }

    }
}
