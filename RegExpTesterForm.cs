using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace web2book
{
    public partial class RegExpTesterForm : Form
    {
        public RegExpTesterForm()
        {
            InitializeComponent();
        }

        private void testButton_Click(object sender, EventArgs e)
        {
            try
            {
                matchTextBox.Text = "No match";
                groupTextBox.Text = "No match";
                Regex re = new Regex(regExpTextBox.Text, RegexOptions.Singleline);
                MatchCollection matches = re.Matches(inputTextBox.Text);
                if (matches.Count > 0)
                {
                    matchTextBox.Text = matches[0].Value;
                    if (matches[0].Groups.Count > 1)
                    {
                        groupTextBox.Text = matches[0].Groups[1].Value;
                    }
                }
            }
            catch (Exception ex)
            {
                matchTextBox.Text = ex.ToString();
            }
        }
    }
}