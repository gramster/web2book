using System.Windows.Forms;


namespace web2book
{
    partial class RssFeedCustomizeForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TextBox feedContentFormatterTextBox;
        private System.Windows.Forms.TextBox feedContentElementTextBox;
        private System.Windows.Forms.CheckBox feedEnabledCheckBox;
        private System.Windows.Forms.CheckBox feedExtractFromContentCheckBox;
        private System.Windows.Forms.TextBox feedContentExtractorTextBox;
        private System.Windows.Forms.TextBox feedLinkReformatterTextBox;
        private System.Windows.Forms.TextBox feedLinkExtractorPatternTextBox;
        private System.Windows.Forms.TextBox feedLinkElementTextBox;
        private System.Windows.Forms.TextBox feedUrlTextBox;
        NumericUpDown feedDaysNumericUpDown;
        ToolTip toolTip;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.feedContentFormatterTextBox = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.feedContentElementTextBox = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.feedDaysNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.feedEnabledCheckBox = new System.Windows.Forms.CheckBox();
            this.feedExtractFromContentCheckBox = new System.Windows.Forms.CheckBox();
            this.feedContentExtractorTextBox = new System.Windows.Forms.TextBox();
            this.feedLinkReformatterTextBox = new System.Windows.Forms.TextBox();
            this.feedLinkExtractorPatternTextBox = new System.Windows.Forms.TextBox();
            this.feedLinkElementTextBox = new System.Windows.Forms.TextBox();
            this.feedUrlTextBox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.reverseOderCheckBox = new System.Windows.Forms.CheckBox();
            this.cancelButton = new System.Windows.Forms.Button();
            this.publishButton = new System.Windows.Forms.Button();
            this.testButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.feedItemElementTextBox = new System.Windows.Forms.TextBox();
            this.feedTitleElementTextBox = new System.Windows.Forms.TextBox();
            this.feedDateElementTextBox = new System.Windows.Forms.TextBox();
            this.helpButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.feedDaysNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // feedContentFormatterTextBox
            // 
            this.feedContentFormatterTextBox.Location = new System.Drawing.Point(156, 353);
            this.feedContentFormatterTextBox.Name = "feedContentFormatterTextBox";
            this.feedContentFormatterTextBox.Size = new System.Drawing.Size(272, 20);
            this.feedContentFormatterTextBox.TabIndex = 18;
            this.toolTip.SetToolTip(this.feedContentFormatterTextBox, "Format string to reformat the parsed article");
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(19, 356);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(102, 13);
            this.label9.TabIndex = 19;
            this.label9.Text = "Content Reformatter";
            // 
            // feedContentElementTextBox
            // 
            this.feedContentElementTextBox.Location = new System.Drawing.Point(156, 176);
            this.feedContentElementTextBox.Name = "feedContentElementTextBox";
            this.feedContentElementTextBox.Size = new System.Drawing.Size(272, 20);
            this.feedContentElementTextBox.TabIndex = 11;
            this.toolTip.SetToolTip(this.feedContentElementTextBox, "Name of XML element that contains the actual RSS item body text");
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(19, 179);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(85, 13);
            this.label8.TabIndex = 20;
            this.label8.Text = "Content Element";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(316, 58);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(31, 13);
            this.label7.TabIndex = 21;
            this.label7.Text = "Days";
            // 
            // feedDaysNumericUpDown
            // 
            this.feedDaysNumericUpDown.Location = new System.Drawing.Point(353, 56);
            this.feedDaysNumericUpDown.Name = "feedDaysNumericUpDown";
            this.feedDaysNumericUpDown.Size = new System.Drawing.Size(74, 20);
            this.feedDaysNumericUpDown.TabIndex = 10;
            this.toolTip.SetToolTip(this.feedDaysNumericUpDown, "Specifies the number of days worth of RSS entries to fetch");
            this.feedDaysNumericUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // feedEnabledCheckBox
            // 
            this.feedEnabledCheckBox.AutoSize = true;
            this.feedEnabledCheckBox.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.feedEnabledCheckBox.Location = new System.Drawing.Point(19, 57);
            this.feedEnabledCheckBox.Name = "feedEnabledCheckBox";
            this.feedEnabledCheckBox.Size = new System.Drawing.Size(65, 17);
            this.feedEnabledCheckBox.TabIndex = 8;
            this.feedEnabledCheckBox.Text = "Enabled";
            this.toolTip.SetToolTip(this.feedEnabledCheckBox, "Clear this to exclude this entry from being synched when you click Go!");
            this.feedEnabledCheckBox.UseVisualStyleBackColor = true;
            // 
            // feedExtractFromContentCheckBox
            // 
            this.feedExtractFromContentCheckBox.AutoSize = true;
            this.feedExtractFromContentCheckBox.Location = new System.Drawing.Point(156, 266);
            this.feedExtractFromContentCheckBox.Name = "feedExtractFromContentCheckBox";
            this.feedExtractFromContentCheckBox.Size = new System.Drawing.Size(266, 17);
            this.feedExtractFromContentCheckBox.TabIndex = 14;
            this.feedExtractFromContentCheckBox.Text = "Apply extractor to linked content instead of link text";
            this.toolTip.SetToolTip(this.feedExtractFromContentCheckBox, "Check this if the link extractor should be applied to the full article instead of" +
                    " the RSS item");
            this.feedExtractFromContentCheckBox.UseVisualStyleBackColor = true;
            // 
            // feedContentExtractorTextBox
            // 
            this.feedContentExtractorTextBox.Location = new System.Drawing.Point(156, 323);
            this.feedContentExtractorTextBox.Name = "feedContentExtractorTextBox";
            this.feedContentExtractorTextBox.Size = new System.Drawing.Size(272, 20);
            this.feedContentExtractorTextBox.TabIndex = 17;
            this.toolTip.SetToolTip(this.feedContentExtractorTextBox, "Regular expression to parse the full article");
            // 
            // feedLinkReformatterTextBox
            // 
            this.feedLinkReformatterTextBox.Location = new System.Drawing.Point(156, 293);
            this.feedLinkReformatterTextBox.Name = "feedLinkReformatterTextBox";
            this.feedLinkReformatterTextBox.Size = new System.Drawing.Size(272, 20);
            this.feedLinkReformatterTextBox.TabIndex = 15;
            this.toolTip.SetToolTip(this.feedLinkReformatterTextBox, "Format string for reformatting the link element");
            // 
            // feedLinkExtractorPatternTextBox
            // 
            this.feedLinkExtractorPatternTextBox.Location = new System.Drawing.Point(156, 236);
            this.feedLinkExtractorPatternTextBox.Name = "feedLinkExtractorPatternTextBox";
            this.feedLinkExtractorPatternTextBox.Size = new System.Drawing.Size(272, 20);
            this.feedLinkExtractorPatternTextBox.TabIndex = 13;
            this.toolTip.SetToolTip(this.feedLinkExtractorPatternTextBox, "Regular expression to apply to parse the link element if it needs reformatting");
            // 
            // feedLinkElementTextBox
            // 
            this.feedLinkElementTextBox.Location = new System.Drawing.Point(156, 206);
            this.feedLinkElementTextBox.Name = "feedLinkElementTextBox";
            this.feedLinkElementTextBox.Size = new System.Drawing.Size(272, 20);
            this.feedLinkElementTextBox.TabIndex = 12;
            this.toolTip.SetToolTip(this.feedLinkElementTextBox, "Name of XML element that contains the permalink or other URL for getting full art" +
                    "icle text");
            // 
            // feedUrlTextBox
            // 
            this.feedUrlTextBox.Location = new System.Drawing.Point(156, 26);
            this.feedUrlTextBox.Name = "feedUrlTextBox";
            this.feedUrlTextBox.Size = new System.Drawing.Size(272, 20);
            this.feedUrlTextBox.TabIndex = 7;
            this.toolTip.SetToolTip(this.feedUrlTextBox, "URL for the RSS feed or HTML content");
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(19, 326);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(131, 13);
            this.label6.TabIndex = 22;
            this.label6.Text = "Content Extraction Pattern";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(19, 296);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(85, 13);
            this.label4.TabIndex = 24;
            this.label4.Text = "Link Reformatter";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(19, 239);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(109, 13);
            this.label3.TabIndex = 25;
            this.label3.Text = "Link Extractor Pattern";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(19, 209);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 13);
            this.label2.TabIndex = 26;
            this.label2.Text = "Link Element";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(19, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 13);
            this.label1.TabIndex = 27;
            this.label1.Text = "URL";
            // 
            // reverseOderCheckBox
            // 
            this.reverseOderCheckBox.AutoSize = true;
            this.reverseOderCheckBox.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.reverseOderCheckBox.Location = new System.Drawing.Point(156, 57);
            this.reverseOderCheckBox.Name = "reverseOderCheckBox";
            this.reverseOderCheckBox.Size = new System.Drawing.Size(118, 17);
            this.reverseOderCheckBox.TabIndex = 44;
            this.reverseOderCheckBox.Text = "Reverse Item Order";
            this.toolTip.SetToolTip(this.reverseOderCheckBox, "Check this to reverse the order of the feed entries before outputting them");
            this.reverseOderCheckBox.UseVisualStyleBackColor = true;
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(268, 391);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 43;
            this.cancelButton.Text = "&Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // publishButton
            // 
            this.publishButton.Location = new System.Drawing.Point(185, 391);
            this.publishButton.Name = "publishButton";
            this.publishButton.Size = new System.Drawing.Size(75, 23);
            this.publishButton.TabIndex = 42;
            this.publishButton.Text = "&Publish";
            this.publishButton.UseVisualStyleBackColor = true;
            this.publishButton.Click += new System.EventHandler(this.publishButton_Click);
            // 
            // testButton
            // 
            this.testButton.Location = new System.Drawing.Point(102, 391);
            this.testButton.Name = "testButton";
            this.testButton.Size = new System.Drawing.Size(75, 23);
            this.testButton.TabIndex = 41;
            this.testButton.Text = "&Test";
            this.testButton.UseVisualStyleBackColor = true;
            this.testButton.Click += new System.EventHandler(this.testButton_Click);
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(19, 391);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 40;
            this.okButton.Text = "&OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(19, 89);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(68, 13);
            this.label5.TabIndex = 45;
            this.label5.Text = "Item Element";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(19, 119);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(68, 13);
            this.label10.TabIndex = 46;
            this.label10.Text = "Title Element";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(19, 149);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(71, 13);
            this.label11.TabIndex = 47;
            this.label11.Text = "Date Element";
            // 
            // feedItemElementTextBox
            // 
            this.feedItemElementTextBox.Location = new System.Drawing.Point(156, 86);
            this.feedItemElementTextBox.Name = "feedItemElementTextBox";
            this.feedItemElementTextBox.Size = new System.Drawing.Size(271, 20);
            this.feedItemElementTextBox.TabIndex = 48;
            this.toolTip.SetToolTip(this.feedItemElementTextBox, "Name of XML element that identifies the start of an RSS feed item");
            // 
            // feedTitleElementTextBox
            // 
            this.feedTitleElementTextBox.Location = new System.Drawing.Point(156, 116);
            this.feedTitleElementTextBox.Name = "feedTitleElementTextBox";
            this.feedTitleElementTextBox.Size = new System.Drawing.Size(272, 20);
            this.feedTitleElementTextBox.TabIndex = 49;
            this.toolTip.SetToolTip(this.feedTitleElementTextBox, "Name of XML element that contains the actual RSS item title");
            // 
            // feedDateElementTextBox
            // 
            this.feedDateElementTextBox.Location = new System.Drawing.Point(156, 146);
            this.feedDateElementTextBox.Name = "feedDateElementTextBox";
            this.feedDateElementTextBox.Size = new System.Drawing.Size(271, 20);
            this.feedDateElementTextBox.TabIndex = 50;
            this.toolTip.SetToolTip(this.feedDateElementTextBox, "Name of XML element that contains the actual RSS item publication date");
            // 
            // helpButton
            // 
            this.helpButton.Location = new System.Drawing.Point(351, 391);
            this.helpButton.Name = "helpButton";
            this.helpButton.Size = new System.Drawing.Size(75, 23);
            this.helpButton.TabIndex = 51;
            this.helpButton.Text = "&Help";
            this.helpButton.UseVisualStyleBackColor = true;
            this.helpButton.Click += new System.EventHandler(this.helpButton_Click);
            // 
            // RssFeedCustomizeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(456, 434);
            this.Controls.Add(this.helpButton);
            this.Controls.Add(this.feedDateElementTextBox);
            this.Controls.Add(this.feedTitleElementTextBox);
            this.Controls.Add(this.feedItemElementTextBox);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.reverseOderCheckBox);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.publishButton);
            this.Controls.Add(this.testButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.feedContentFormatterTextBox);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.feedContentElementTextBox);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.feedDaysNumericUpDown);
            this.Controls.Add(this.feedEnabledCheckBox);
            this.Controls.Add(this.feedExtractFromContentCheckBox);
            this.Controls.Add(this.feedContentExtractorTextBox);
            this.Controls.Add(this.feedLinkReformatterTextBox);
            this.Controls.Add(this.feedLinkExtractorPatternTextBox);
            this.Controls.Add(this.feedLinkElementTextBox);
            this.Controls.Add(this.feedUrlTextBox);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "RssFeedCustomizeForm";
            ((System.ComponentModel.ISupportInitialize)(this.feedDaysNumericUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Label label9;
        private Label label8;
        private Label label7;
        private Label label6;
        private Label label4;
        private Label label3;
        private Label label2;
        private Label label1;
        private Button cancelButton;
        private Button publishButton;
        private Button testButton;
        private Button okButton;
        private CheckBox reverseOderCheckBox;
        private Label label5;
        private Label label10;
        private Label label11;
        private TextBox feedItemElementTextBox;
        private TextBox feedTitleElementTextBox;
        private TextBox feedDateElementTextBox;
        private Button helpButton;
    }
}