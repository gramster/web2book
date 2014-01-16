using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace web2book
{
    public partial class RssFeedCustomizeForm : Form
    {
        public RssFeedCustomizeForm()
        {
            InitializeComponent();
        }

        Feed f;
        EventHandler test, publish;

        public RssFeedCustomizeForm(ContentSource cs, EventHandler test, EventHandler publish)
            : this()
        {
            this.Text = cs.Name + " Properties";
            this.f = (Feed)cs; 
            this.publish = publish;
            this.test = test;
            feedUrlTextBox.Text = f.Url;
            feedEnabledCheckBox.Checked = f.Enabled;
            if (f.NumberOfDays < 0) f.NumberOfDays = 0;
            else if (f.NumberOfDays > 100) f.NumberOfDays = 100;
            feedDaysNumericUpDown.Value = (Decimal)f.NumberOfDays;
            feedItemElementTextBox.Text = f.ItemElement;
            feedTitleElementTextBox.Text = f.TitleElement;
            feedDateElementTextBox.Text = f.DateElement;
            feedContentElementTextBox.Text = f.ContentElement;
            feedLinkElementTextBox.Text = f.LinkElement;
            feedLinkExtractorPatternTextBox.Text = f.LinkExtractorPattern;
            feedExtractFromContentCheckBox.Checked = f.ExtractLinkFromContent;
            feedLinkReformatterTextBox.Text = f.LinkReformatter;
            feedContentExtractorTextBox.Text = f.ContentExtractor;
            feedContentFormatterTextBox.Text = f.ContentFormatter;
            reverseOderCheckBox.Checked = f.ReverseOrder;
            if (publish == null) publishButton.Visible = false;
            if (test == null) testButton.Visible = false;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        void SaveProperties()
        {
            f.Url = feedUrlTextBox.Text;
            f.Enabled = feedEnabledCheckBox.Checked;
            f.NumberOfDays = (int)feedDaysNumericUpDown.Value;
            f.ItemElement = feedItemElementTextBox.Text;
            f.TitleElement = feedTitleElementTextBox.Text;
            f.DateElement = feedDateElementTextBox.Text;
            f.ContentElement = feedContentElementTextBox.Text;
            f.LinkElement = feedLinkElementTextBox.Text;
            f.LinkExtractorPattern = feedLinkExtractorPatternTextBox.Text;
            f.ExtractLinkFromContent = feedExtractFromContentCheckBox.Checked;
            f.LinkReformatter = feedLinkReformatterTextBox.Text;
            f.ContentExtractor = feedContentExtractorTextBox.Text;
            f.ContentFormatter = feedContentFormatterTextBox.Text;
            f.ReverseOrder = reverseOderCheckBox.Checked;
        }

        private void testButton_Click(object sender, EventArgs e)
        {
            publishButton.Enabled = testButton.Enabled = okButton.Enabled = cancelButton.Enabled = false;
            SaveProperties();
            test(f, null);
            publishButton.Enabled = testButton.Enabled = okButton.Enabled = cancelButton.Enabled = true;
        }

        private void publishButton_Click(object sender, EventArgs e)
        {
            publishButton.Enabled = testButton.Enabled = okButton.Enabled = cancelButton.Enabled = false;
            SaveProperties();
            publish(f, null);
            publishButton.Enabled = testButton.Enabled = okButton.Enabled = cancelButton.Enabled = true;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            SaveProperties();
            this.DialogResult = DialogResult.OK;
        }

        private void cancelButton_Click_1(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void helpButton_Click(object sender, EventArgs e)
        {
            Utils.ShowHelp(this, "web2book.chm", null);
        }
    }
}