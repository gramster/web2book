using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace web2book
{
    public partial class ReadLitConfigForm : Form
    {
        ReadLit source;

        public ReadLitConfigForm(ReadLit source)
        {
            this.source = source;
            InitializeComponent();
            clitPathTextBox.Text = source.ClitPath;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            source.ClitPath = clitPathTextBox.Text;
            Settings.Default.Save();
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void browseButton_Click(object sender, EventArgs e)
        {
            string[] f = Utils.BrowseFiles("ConvertLit Executable", "Exe files (*.exe)|*.exe", true, false);
            if (f != null && f.Length == 1)
            {
                clitPathTextBox.Text = f[0];
            }
        }
    }
}