using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace web2book
{
    public partial class ReadLitItemConfigForm : Form
    {
        LitBookSource litBook;

        public ReadLitItemConfigForm(ContentSource cs)
        {
            litBook = (LitBookSource)cs;
            InitializeComponent();
            if (litBook.Name.Length > 0)
                this.Text = litBook.Name + " " + Text;
            pathTextBox.Text = litBook.Path;
        }

        private void browseButton_Click(object sender, EventArgs e)
        {
            string[] f = Utils.BrowseFiles("Lit File", "Lit files (*.lit)|*.lit", true, false);
            if (f != null && f.Length == 1)
            {
                pathTextBox.Text = f[0];
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            litBook.Path = pathTextBox.Text;
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}