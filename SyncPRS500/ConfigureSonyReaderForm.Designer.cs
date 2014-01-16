namespace web2book
{
    partial class ConfigureSonyReaderForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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
            this.syncToSDCardRadioButton = new System.Windows.Forms.RadioButton();
            this.syncToMemoryStickRadioButton = new System.Windows.Forms.RadioButton();
            this.browse3Button = new System.Windows.Forms.Button();
            this.syncToMainMemoryRadioButton = new System.Windows.Forms.RadioButton();
            this.label5 = new System.Windows.Forms.Label();
            this.connectPathTextBox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // syncToSDCardRadioButton
            // 
            this.syncToSDCardRadioButton.AutoSize = true;
            this.syncToSDCardRadioButton.Location = new System.Drawing.Point(104, 68);
            this.syncToSDCardRadioButton.Name = "syncToSDCardRadioButton";
            this.syncToSDCardRadioButton.Size = new System.Drawing.Size(65, 17);
            this.syncToSDCardRadioButton.TabIndex = 24;
            this.syncToSDCardRadioButton.TabStop = true;
            this.syncToSDCardRadioButton.Text = "SD Card";
            this.syncToSDCardRadioButton.UseVisualStyleBackColor = true;
            this.syncToSDCardRadioButton.Click += new System.EventHandler(this.syncToSDCardRadioButton_Click);
            // 
            // syncToMemoryStickRadioButton
            // 
            this.syncToMemoryStickRadioButton.AutoSize = true;
            this.syncToMemoryStickRadioButton.Location = new System.Drawing.Point(104, 45);
            this.syncToMemoryStickRadioButton.Name = "syncToMemoryStickRadioButton";
            this.syncToMemoryStickRadioButton.Size = new System.Drawing.Size(89, 17);
            this.syncToMemoryStickRadioButton.TabIndex = 23;
            this.syncToMemoryStickRadioButton.TabStop = true;
            this.syncToMemoryStickRadioButton.Text = "Memory Stick";
            this.syncToMemoryStickRadioButton.UseVisualStyleBackColor = true;
            this.syncToMemoryStickRadioButton.Click += new System.EventHandler(this.syncToMemoryStickRadioButton_Click);
            // 
            // browse3Button
            // 
            this.browse3Button.Location = new System.Drawing.Point(372, 96);
            this.browse3Button.Name = "browse3Button";
            this.browse3Button.Size = new System.Drawing.Size(62, 23);
            this.browse3Button.TabIndex = 26;
            this.browse3Button.Text = "Browse";
            this.browse3Button.UseVisualStyleBackColor = true;
            this.browse3Button.Click += new System.EventHandler(this.browse3Button_Click);
            // 
            // syncToMainMemoryRadioButton
            // 
            this.syncToMainMemoryRadioButton.AutoSize = true;
            this.syncToMainMemoryRadioButton.Location = new System.Drawing.Point(104, 22);
            this.syncToMainMemoryRadioButton.Name = "syncToMainMemoryRadioButton";
            this.syncToMainMemoryRadioButton.Size = new System.Drawing.Size(88, 17);
            this.syncToMainMemoryRadioButton.TabIndex = 22;
            this.syncToMainMemoryRadioButton.TabStop = true;
            this.syncToMainMemoryRadioButton.Text = "Main Memory";
            this.syncToMainMemoryRadioButton.UseVisualStyleBackColor = true;
            this.syncToMainMemoryRadioButton.Click += new System.EventHandler(this.syncToMainMemoryRadioButton_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(16, 24);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(71, 13);
            this.label5.TabIndex = 27;
            this.label5.Text = "Sync Files To";
            // 
            // connectPathTextBox
            // 
            this.connectPathTextBox.Location = new System.Drawing.Point(198, 98);
            this.connectPathTextBox.MaxLength = 256;
            this.connectPathTextBox.Name = "connectPathTextBox";
            this.connectPathTextBox.Size = new System.Drawing.Size(168, 20);
            this.connectPathTextBox.TabIndex = 25;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(17, 101);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(176, 13);
            this.label6.TabIndex = 28;
            this.label6.Text = "Sony CONNECT Software Directory";
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(104, 146);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 29;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(291, 146);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 30;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // ConfigureSonyReaderForm
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(468, 187);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.syncToSDCardRadioButton);
            this.Controls.Add(this.syncToMemoryStickRadioButton);
            this.Controls.Add(this.browse3Button);
            this.Controls.Add(this.syncToMainMemoryRadioButton);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.connectPathTextBox);
            this.Controls.Add(this.label6);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "ConfigureSonyReaderForm";
            this.Text = "Sony Reader Options";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton syncToSDCardRadioButton;
        private System.Windows.Forms.RadioButton syncToMemoryStickRadioButton;
        private System.Windows.Forms.Button browse3Button;
        private System.Windows.Forms.RadioButton syncToMainMemoryRadioButton;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox connectPathTextBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.ToolTip toolTip;
    }
}