using System.Windows.Forms;

namespace web2book
{
    partial class WebPageCustomizeForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
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
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.webpageUrlTextBox = new System.Windows.Forms.TextBox();
            this.webpageEnabledCheckBox = new System.Windows.Forms.CheckBox();
            this.webpageDaysNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.webpageDepthNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.webpageContentFormatterTextBox = new System.Windows.Forms.TextBox();
            this.webpageContentExtractorTextBox = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.okButton = new System.Windows.Forms.Button();
            this.testButton = new System.Windows.Forms.Button();
            this.publishButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.webpageDaysNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.webpageDepthNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // webpageUrlTextBox
            // 
            this.webpageUrlTextBox.Location = new System.Drawing.Point(156, 26);
            this.webpageUrlTextBox.Name = "webpageUrlTextBox";
            this.webpageUrlTextBox.Size = new System.Drawing.Size(272, 20);
            this.webpageUrlTextBox.TabIndex = 21;
            this.toolTip.SetToolTip(this.webpageUrlTextBox, "URL of the web page");
            // 
            // webpageEnabledCheckBox
            // 
            this.webpageEnabledCheckBox.AutoSize = true;
            this.webpageEnabledCheckBox.Location = new System.Drawing.Point(156, 53);
            this.webpageEnabledCheckBox.Name = "webpageEnabledCheckBox";
            this.webpageEnabledCheckBox.Size = new System.Drawing.Size(65, 17);
            this.webpageEnabledCheckBox.TabIndex = 22;
            this.webpageEnabledCheckBox.Text = "Enabled";
            this.toolTip.SetToolTip(this.webpageEnabledCheckBox, "Check this to enable this web page");
            this.webpageEnabledCheckBox.UseVisualStyleBackColor = true;
            // 
            // webpageDaysNumericUpDown
            // 
            this.webpageDaysNumericUpDown.Location = new System.Drawing.Point(341, 52);
            this.webpageDaysNumericUpDown.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.webpageDaysNumericUpDown.Name = "webpageDaysNumericUpDown";
            this.webpageDaysNumericUpDown.Size = new System.Drawing.Size(87, 20);
            this.webpageDaysNumericUpDown.TabIndex = 24;
            this.toolTip.SetToolTip(this.webpageDaysNumericUpDown, "Number of days content to retrieve (if applicable)");
            // 
            // webpageDepthNumericUpDown
            // 
            this.webpageDepthNumericUpDown.Location = new System.Drawing.Point(156, 87);
            this.webpageDepthNumericUpDown.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.webpageDepthNumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.webpageDepthNumericUpDown.Name = "webpageDepthNumericUpDown";
            this.webpageDepthNumericUpDown.Size = new System.Drawing.Size(87, 20);
            this.webpageDepthNumericUpDown.TabIndex = 29;
            this.toolTip.SetToolTip(this.webpageDepthNumericUpDown, "Number of days content to retrieve (if applicable)");
            this.webpageDepthNumericUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(19, 29);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(29, 13);
            this.label10.TabIndex = 35;
            this.label10.Text = "URL";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(19, 126);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(131, 13);
            this.label12.TabIndex = 33;
            this.label12.Text = "Content Extraction Pattern";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(19, 161);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(102, 13);
            this.label13.TabIndex = 32;
            this.label13.Text = "Content Reformatter";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(304, 54);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(31, 13);
            this.label14.TabIndex = 31;
            this.label14.Text = "Days";
            // 
            // webpageContentFormatterTextBox
            // 
            this.webpageContentFormatterTextBox.Location = new System.Drawing.Point(156, 157);
            this.webpageContentFormatterTextBox.Name = "webpageContentFormatterTextBox";
            this.webpageContentFormatterTextBox.Size = new System.Drawing.Size(272, 20);
            this.webpageContentFormatterTextBox.TabIndex = 25;
            // 
            // webpageContentExtractorTextBox
            // 
            this.webpageContentExtractorTextBox.Location = new System.Drawing.Point(156, 123);
            this.webpageContentExtractorTextBox.Name = "webpageContentExtractorTextBox";
            this.webpageContentExtractorTextBox.Size = new System.Drawing.Size(272, 20);
            this.webpageContentExtractorTextBox.TabIndex = 26;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(19, 89);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(103, 13);
            this.label15.TabIndex = 30;
            this.label15.Text = "Follow links to depth";
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(22, 215);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 36;
            this.okButton.Text = "&OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // testButton
            // 
            this.testButton.Location = new System.Drawing.Point(132, 215);
            this.testButton.Name = "testButton";
            this.testButton.Size = new System.Drawing.Size(75, 23);
            this.testButton.TabIndex = 37;
            this.testButton.Text = "&Test";
            this.testButton.UseVisualStyleBackColor = true;
            this.testButton.Click += new System.EventHandler(this.testButton_Click);
            // 
            // publishButton
            // 
            this.publishButton.Location = new System.Drawing.Point(242, 215);
            this.publishButton.Name = "publishButton";
            this.publishButton.Size = new System.Drawing.Size(75, 23);
            this.publishButton.TabIndex = 38;
            this.publishButton.Text = "&Publish";
            this.publishButton.UseVisualStyleBackColor = true;
            this.publishButton.Click += new System.EventHandler(this.publishButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(352, 215);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 39;
            this.cancelButton.Text = "&Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // WebPageCustomizeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(456, 262);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.publishButton);
            this.Controls.Add(this.testButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.webpageDepthNumericUpDown);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.webpageContentExtractorTextBox);
            this.Controls.Add(this.webpageContentFormatterTextBox);
            this.Controls.Add(this.webpageDaysNumericUpDown);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.webpageEnabledCheckBox);
            this.Controls.Add(this.webpageUrlTextBox);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label10);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "WebPageCustomizeForm";
            ((System.ComponentModel.ISupportInitialize)(this.webpageDaysNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.webpageDepthNumericUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Label label10;
        private Label label12;
        private Label label13;
        private TextBox webpageUrlTextBox;
        private CheckBox webpageEnabledCheckBox;
        private Label label14;
        private NumericUpDown webpageDaysNumericUpDown;
        private TextBox webpageContentFormatterTextBox;
        private TextBox webpageContentExtractorTextBox;
        private Label label15;
        private NumericUpDown webpageDepthNumericUpDown;
        private Button okButton;
        private Button testButton;
        private Button publishButton;
        private Button cancelButton;

    }
}