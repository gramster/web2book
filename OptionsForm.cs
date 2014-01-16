using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace web2book
{
    public partial class OptionsForm : Form
    {
        public string HtmlDocPath;

        enum PageSizes
        {
            Custom, A4, A5, A6, A7, Legal, Letter, HalfLetter
        }

        static int[,] PageDimensions = 
            {
                { 0, 0},
                { 210, 297 },
                { 148, 210 },
                { 105, 148 },
                { 74, 105 },
                { 216, 356 },
                { 216, 279 },
                { 148, 216 },
                { 90, 120 },
            };

        public OptionsForm()
        {
            InitializeComponent();

            // Populate and set the output format combo box

            outputFormatComboBox.Items.Add("HTML");
            bool foundCurrent = (Settings.Default.OutputFormat == "HTML");
            foreach (IHtmlConverter converter in Utils.Converters)
            {
                if (Settings.Default.OutputFormat == converter.Name)
                    foundCurrent = true;
                outputFormatComboBox.Items.Add(converter.Name);
            }
            if (!foundCurrent)
            {
                Settings.Default.OutputFormat = "HTML";
            }

            outputFormatComboBox.SelectedItem = Settings.Default.OutputFormat;

            // Populate and set the sync device combo box

            foundCurrent = false;
            foreach (ISyncDevice device in Utils.Devices)
            {
                if (Settings.Default.SyncDevice == device.Name)
                    foundCurrent = true;
                outputDeviceComboBox.Items.Add(device.Name);
            }
            if (foundCurrent)
            {
                outputDeviceComboBox.SelectedItem = Settings.Default.SyncDevice;
            }

            SetPageSizeValue();

            this.autoUpdateDateTimePicker.Value = DateTime.Today.AddSeconds(global::web2book.Settings.Default.AutoUpdateTime);
            EnableSyncDeviceControls(syncToDeviceCheckBox.Checked);

            typefaceComboBox.SelectedIndex = Settings.Default.Typeface;
            toolTip.SetToolTip(this.topMarginNumericUpDown, "Top page margin in millimeters");
            toolTip.SetToolTip(this.bottomMarginNumericUpDown, "Bottom page margin in millimeters");
            toolTip.SetToolTip(this.leftMarginNumericUpDown, "Left page margin in millimeters");
            toolTip.SetToolTip(this.rightMarginNumericUpDown, "Right page margin in millimeters");
            toolTip.SetToolTip(this.pageHeightNumericUpDown, "Page height in millimeters");
            toolTip.SetToolTip(this.pageWidthNumericUpDown, "Page width in millimeters");
            toolTip.SetToolTip(this.fontSizeNumericUpDown, "Font size (in points) of normal text");
            toolTip.SetToolTip(this.pageSizeComboBox, "Page size (select custom to specify your own width/height)");
            toolTip.SetToolTip(this.typefaceComboBox, "Default typeface to use in output document");
            toolTip.SetToolTip(this.outputPathTextBox, "PC path for output documents");
            toolTip.SetToolTip(this.syncToDeviceCheckBox, "Check this to sync output documents to an attached device such as a Sony Reader");
            toolTip.SetToolTip(this.deleteAfterSyncCheckBox, "Check this to delete output documents from PC after synching to Sony Reader");
            toolTip.SetToolTip(this.outputFormatComboBox, "Select the format for the output document(s)");
            toolTip.SetToolTip(this.outputConverterOptionsButton, "Click here to set options for the selected output format converter");
            toolTip.SetToolTip(this.outputDeviceComboBox, "Select the device to which you want to sync output document(s)");
            toolTip.SetToolTip(this.outputDeviceOptionsButton, "Click here to set options for the selected sync device");
            toolTip.SetToolTip(this.usernameTextBox, "Username to use for publishing your rss2book feeds");
            toolTip.SetToolTip(this.passwordTextBox, "Password to use when publishing your rss2book feeds");
            toolTip.SetToolTip(this.syncPage, "Sync options");
            toolTip.SetToolTip(this.pageOptionsPage, "Page layout options");
            toolTip.SetToolTip(this.publishingPage, "Feed publishing options");
            toolTip.SetToolTip(this.autoUpdateDateTimePicker, "Check this and set a time to have daily documents generated on your PC");
            toolTip.SetToolTip(this.htmlTidyPathTextBox, "Optionally set the path to htmltidy.exe here, if you have htmltidy installed");
        }

        void SetPageSizeValue()
        {
            for (int i = 0; i < PageDimensions.Length / 2; i++)
            {
                if ((int)pageWidthNumericUpDown.Value == PageDimensions[i, 0] &&
                    (int)pageHeightNumericUpDown.Value == PageDimensions[i, 1])
                {
                    pageSizeComboBox.SelectedIndex = i;
                    return;
                }
            }
            pageSizeComboBox.SelectedIndex = 0;
        }

        private void browse2Button_Click(object sender, EventArgs e)
        {
            string f = Utils.BrowsePath("Output to directory");
            if (f != null) outputPathTextBox.Text = f;
        }

        private void pageWidthNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            SetPageSizeValue();
        }

        private void pageHeightNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            SetPageSizeValue();
        }

        private void pageSizeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (pageSizeComboBox.SelectedIndex != 0)
            {
                pageWidthNumericUpDown.Value = (decimal)PageDimensions[pageSizeComboBox.SelectedIndex, 0];
                pageHeightNumericUpDown.Value = (decimal)PageDimensions[pageSizeComboBox.SelectedIndex, 1];
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            Settings.Default.Typeface = typefaceComboBox.SelectedIndex;
            Settings.Default.OutputFormat = (string)outputFormatComboBox.SelectedItem;
            Settings.Default.SyncDevice = (string)outputDeviceComboBox.SelectedItem; // TODO: what if none selected? Will this throw?
            long v = autoUpdateDateTimePicker.Value.Ticks - DateTime.Today.Ticks;
            if (v >= 0) v %= TimeSpan.TicksPerDay;
            else v = TimeSpan.TicksPerDay - ((-v)%TimeSpan.TicksPerDay);
            Settings.Default.AutoUpdateTime = (int)(v/TimeSpan.TicksPerSecond);
            Settings.Default.Save();
            this.DialogResult = DialogResult.OK;
        }

        private void outputConverterOptionsButton_Click(object sender, EventArgs e)
        {
            bool foundCurrent = (Settings.Default.OutputFormat == "HTML");
            foreach (IHtmlConverter converter in Utils.Converters)
            {
                if (((string)outputFormatComboBox.SelectedItem) == converter.Name)
                {
                    converter.Configure();
                    break;
                }
            }
        }

        private void outputFormatComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string cvt = (string)outputFormatComboBox.SelectedItem;
            outputConverterOptionsButton.Enabled = false;
            foreach (IHtmlConverter converter in Utils.Converters)
            {
                if (cvt == converter.Name)
                {
                    outputConverterOptionsButton.Enabled = converter.HasConfigurationUI;
                    foreach (Control c in pageOptionsPage.Controls)
                    {
                        c.Enabled = converter.HasConfigurablePageSettings;
                    }
                    break;
                }
            }
        }

        private void outputDeviceOptionsButton_Click(object sender, EventArgs e)
        {
            foreach (ISyncDevice device in Utils.Devices)
            {
                if (((string)outputDeviceComboBox.SelectedItem) == device.Name)
                {
                    device.Configure();
                    break;
                }
            }
        }

        private void outputDeviceComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string dev = (string)outputDeviceComboBox.SelectedItem;
            outputDeviceOptionsButton.Enabled = false;
            foreach (ISyncDevice device in Utils.Devices)
            {
                if (dev == device.Name)
                {
                    outputDeviceOptionsButton.Enabled = device.HasConfigurationUI;
                    break;
                }
            }
        }

        private void syncToDeviceCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            EnableSyncDeviceControls(syncToDeviceCheckBox.Checked);
        }

        void EnableSyncDeviceControls(bool v)
        {
            outputDeviceComboBox.Enabled = outputDeviceOptionsButton.Enabled = deleteAfterSyncCheckBox.Enabled = deleteAfterSyncCheckBox.Enabled = v;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Settings.Default.Reload();
            this.DialogResult = DialogResult.Cancel;
        }

        private void htmlTidyPathBrowseButton_Click(object sender, EventArgs e)
        {
            string[] f = Utils.BrowseFiles("Html Tidy Application", "Exe files (*.exe)|*.exe", true, false);
            if (f != null && f.Length == 1)
                htmlTidyPathTextBox.Text = f[0];
        }
    }
}