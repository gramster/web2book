using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace web2book
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
            aboutTextBox.Text = "web2book release "+Utils.RELEASE.ToString()+" by GeekRaver\r\ninfo@geekraver.com\r\n(c) 2006,2007 All Rights Reserved\r\niText Library - Copyright (C) 1999-2005\r\nby Bruno Lowagie and Paulo Soares.\r\nAll Rights Reserved.";
            aboutTextBox.TabStop = false;
        }
    }
}