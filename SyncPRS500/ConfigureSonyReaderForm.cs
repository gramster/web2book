using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace web2book
{
    public partial class ConfigureSonyReaderForm : Form
    {
        SonyReader reader;

        public ConfigureSonyReaderForm(SonyReader reader)
        {
            this.reader = reader;
            InitializeComponent();

            //connectPathTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::web2book.Settings.Default, "ConnectPath", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            connectPathTextBox.Text = reader.ConnectPath;

            // Set the radio buttons for the Reader sync dest

            if (reader.TargetPath == "a:/")
                syncToMemoryStickRadioButton.Checked = true;
            else if (reader.TargetPath == "b:/")
                syncToSDCardRadioButton.Checked = true;
            else
                syncToMainMemoryRadioButton.Checked = true;

            toolTip.SetToolTip(this.syncToMainMemoryRadioButton, "Select this to have files sync to the Sony Reader's internal memory");
            toolTip.SetToolTip(this.syncToMemoryStickRadioButton, "Select this to have files sync to the Memory Stick on the Sony Reader");
            toolTip.SetToolTip(this.syncToSDCardRadioButton, "Select this to have files sync to the SD card on the Sony Reader");
            toolTip.SetToolTip(this.connectPathTextBox, "Directory on PC where Sony Connect software is installed");
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            //Settings.Default.Reload();
            this.DialogResult = DialogResult.Cancel;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            reader.ConnectPath = connectPathTextBox.Text;

            if (syncToMemoryStickRadioButton.Checked)
            {
                reader.TargetPath = "a:/";
            }
            else if (syncToSDCardRadioButton.Checked)
            {
                reader.TargetPath = "b:/";
            }
            else
            {
                reader.TargetPath = "/Data/media/books/";
            }
            Settings.Default.Save();
            this.DialogResult = DialogResult.OK;
        }


        private void syncToMainMemoryRadioButton_Click(object sender, EventArgs e)
        {
            syncToMainMemoryRadioButton.Checked = true;
        }

        private void syncToMemoryStickRadioButton_Click(object sender, EventArgs e)
        {
            syncToMemoryStickRadioButton.Checked = true;
        }

        private void syncToSDCardRadioButton_Click(object sender, EventArgs e)
        {
            syncToSDCardRadioButton.Checked = true;
        }

        private void browse3Button_Click(object sender, EventArgs e)
        {
            string f = Utils.BrowsePath("Sony CONNECT software directory");
            if (f != null) connectPathTextBox.Text = f;
        }
    }
}