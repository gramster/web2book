using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace web2book
{
    public partial class ProgressForm : Form
    {
        bool cancelled;

        public string Message
        {
            set { 
                messageLabel.Text = value;
                Application.DoEvents();
            }
        }

        public void Start(int count)
        {
            progressBar.Value = 0;
            progressBar.Maximum = count;
            Application.DoEvents();
        }

        public void Next()
        {
            if (progressBar.Value < progressBar.Maximum)
                progressBar.Value++;
            Application.DoEvents();
        }

        public bool Cancelled
        {
            get { return cancelled; }
        }

        public ProgressForm()
        {
            InitializeComponent();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            cancelled = true;
        }
    }
}