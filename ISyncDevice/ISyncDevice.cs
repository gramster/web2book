using System;
using System.Collections.Generic;
using System.Text;

namespace web2book
{
    public interface ISyncDevice
    {
        string Name { get; }
        bool HasConfigurationUI { get; }
        bool IsConnected { get; }
        void Configure();
        string ConnectPrompt { get; }
        bool Connect();
        string[] GetFiles(string namePrefix);
        bool DeleteFile(string fname);
        bool AddFile(string sourcePath, string targetName);
        bool Disconnect();
        int DisplayWidth { get; }
        int DisplayHeight { get; }
        int DisplayDepth { get; }
        string HtmlHelp { get; }
    }
}
