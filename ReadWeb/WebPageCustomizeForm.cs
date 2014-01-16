using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace web2book
{
    public partial class WebPageCustomizeForm : Form
    {
        public WebPageCustomizeForm()
        {
            InitializeComponent();
        }

        WebPage w;
        EventHandler test, publish;

        public WebPageCustomizeForm(ContentSource cs, EventHandler test, EventHandler publish)
            : this()
        {
            this.Text = cs.Name + " Properties";
            this.w = (WebPage)cs; 
            this.publish = publish;
            this.test = test;
            webpageUrlTextBox.Text = w.Url;
            webpageEnabledCheckBox.Checked = w.Enabled;
            if (w.NumberOfDays < 0) w.NumberOfDays = 1;
            else if (w.NumberOfDays > 100) w.NumberOfDays = 100;
            webpageDaysNumericUpDown.Value = (Decimal)w.NumberOfDays;
            if (w.Depth < 1) w.Depth = 1;
            else if (w.Depth > 10) w.Depth = 10;
            webpageDepthNumericUpDown.Value = (Decimal)w.Depth;
            webpageContentExtractorTextBox.Text = w.ContentExtractor;
            webpageContentFormatterTextBox.Text = w.ContentFormatter;
            if (publish == null) publishButton.Visible = false;
            if (test == null) testButton.Visible = false;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        void SaveProperties()
        {
            w.Url = webpageUrlTextBox.Text;
            w.Enabled = webpageEnabledCheckBox.Checked;
            w.NumberOfDays = (int)webpageDaysNumericUpDown.Value;
            w.Depth = (int)webpageDepthNumericUpDown.Value;
            w.ContentExtractor = webpageContentExtractorTextBox.Text;
            w.ContentFormatter = webpageContentFormatterTextBox.Text;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            SaveProperties();
            this.DialogResult = DialogResult.OK;
        }

        private void testButton_Click(object sender, EventArgs e)
        {
            publishButton.Enabled = testButton.Enabled = okButton.Enabled = cancelButton.Enabled = false;
            SaveProperties();
            test(w, null);
            publishButton.Enabled = testButton.Enabled = okButton.Enabled = cancelButton.Enabled = true;
        }

        private void publishButton_Click(object sender, EventArgs e)
        {
            publishButton.Enabled = testButton.Enabled = okButton.Enabled = cancelButton.Enabled = false;
            SaveProperties();
            publish(w, null);
            publishButton.Enabled = testButton.Enabled = okButton.Enabled = cancelButton.Enabled = true;
        }
    }
}