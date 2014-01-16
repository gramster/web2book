using System;
using System.Collections.Generic;
using System.Text;

namespace web2book
{
    [Flags]
    public enum FontStyle
    {
        Normal = 0,
        Italic = 1,
        Bold = 2,
        Underline = 4
    }

    public enum TypeFace
    {
        Courier,
        Helvetica,
        TimesRoman
    }

    public interface IParserBasedHtmlConverter
    {
        TypeFace DefaultFont { get; }
        void HandleText(string text, TypeFace face, FontStyle style);
        void FlushParagraph();
        void LineBreak();
        void EnterHeader(int level);
        void ExitHeader();
        void StartUnorderedList();
        void StartOrderedList();
        void FlushListItem();
        void EndList();
        void StartTable();
        void StartRow();
        void StartCell(int span);
        void EndCell();
        void EndRow();
        void EndTable();
        void AddImage(string fname);
        byte[] GetBytes();
    }

    public interface IHtmlConverter
    {
        string Name
        {
            get;
        }
        string Extension
        {
            get;
        }
        bool IsFileConverter
        {
            get;
        }
        IParserBasedHtmlConverter Converter
        {
            get;
        }
        bool HasConfigurationUI
        {
            get;
        }
        bool HasConfigurablePageSettings
        {
            get;
        }
        bool MustScrubHtml
        {
            get;
        }

        void Configure();

        void Initialize(string author,
                        string title,
                        int leftMargin,
                        int rightMargin,
                        int topMargin,
                        int bottomMargin,
                        int pageWidth,
                        int pageHeight,
                        TypeFace font,
                        int fontSize,
                        int displayWidth, // display res if synching to device, else 0,0
                        int displayHeight,
                        int displayDepth,
                        bool isStructured
                        );

        string ConvertHtmlFile(string path, string basename, ref string log);

        string HtmlHelp { get; }

    }
}
