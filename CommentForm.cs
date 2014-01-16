using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace web2book
{
    public partial class CommentForm : Form
    {
        public string Comment
        {
            get { return commentTextBox.Text; }
        }

        public CommentForm()
        {
            InitializeComponent();
        }

        public CommentForm(string feedName)
            : this()
        {
            feedNameLabel.Text = feedName;
        }
    }
}