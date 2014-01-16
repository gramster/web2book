using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace web2book
{
    public partial class HtmlDocConfigForm : Form
    {
        WriteHtmlDoc writer;

        public HtmlDocConfigForm(WriteHtmlDoc writer)
        {
            this.writer = writer;
            InitializeComponent();
            //this.htmldocPathTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::web2book.Settings.Default, "HtmlDocPath", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            //this.optionsTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::web2book.Settings.Default, "HtmlDocOptions", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            htmldocPathTextBox.Text = writer.HtmlDocPath;
            optionsTextBox.Text = writer.HtmlDocOptions;
            toolTip.SetToolTip(this.htmldocPathTextBox, "Path on PC where htmldoc software is installed");
            toolTip.SetToolTip(this.optionsTextBox, "Additional options to pass to htmldoc on command line");
        }

        private void browseButton_Click(object sender, EventArgs e)
        {
            string f = Utils.BrowsePath("Htmldoc directory");
            if (f != null) htmldocPathTextBox.Text = f;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            //Settings.Default.Reload();
            this.DialogResult = DialogResult.Cancel;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            writer.HtmlDocPath = htmldocPathTextBox.Text;
            writer.HtmlDocOptions = optionsTextBox.Text;
            Settings.Default.Save();
            this.DialogResult = DialogResult.OK;
        }

   }
}