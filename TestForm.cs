using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace web2book
{
    public partial class TestForm : Form
    {
        StringBuilder log = new StringBuilder();

        public void Log(string txt)
        {
            log.AppendLine(txt);
            logTextBox.Text = log.ToString();
            Application.DoEvents();
        }

        public TestForm()
        {
            InitializeComponent();
        }

        public void ShowResults()
        {
            Show();
        }

        public void CloseIfEmpty()
        {
            if (logTextBox.Text == String.Empty) Close();
        }
        
        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}