namespace web2book
{
    partial class RegExpTesterForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.regExpTextBox = new System.Windows.Forms.TextBox();
            this.inputTextBox = new System.Windows.Forms.TextBox();
            this.matchTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.testButton = new System.Windows.Forms.Button();
            this.groupTextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "RegExp";
            // 
            // regExpTextBox
            // 
            this.regExpTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.regExpTextBox.Location = new System.Drawing.Point(70, 33);
            this.regExpTextBox.Name = "regExpTextBox";
            this.regExpTextBox.Size = new System.Drawing.Size(212, 20);
            this.regExpTextBox.TabIndex = 1;
            // 
            // inputTextBox
            // 
            this.inputTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.inputTextBox.Location = new System.Drawing.Point(70, 62);
            this.inputTextBox.MaxLength = 327670;
            this.inputTextBox.Multiline = true;
            this.inputTextBox.Name = "inputTextBox";
            this.inputTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.inputTextBox.Size = new System.Drawing.Size(286, 123);
            this.inputTextBox.TabIndex = 2;
            // 
            // matchTextBox
            // 
            this.matchTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.matchTextBox.Location = new System.Drawing.Point(70, 191);
            this.matchTextBox.MaxLength = 327670;
            this.matchTextBox.Multiline = true;
            this.matchTextBox.Name = "matchTextBox";
            this.matchTextBox.ReadOnly = true;
            this.matchTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.matchTextBox.Size = new System.Drawing.Size(286, 115);
            this.matchTextBox.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 65);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(31, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Input";
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 191);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(37, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Match";
            // 
            // testButton
            // 
            this.testButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.testButton.Location = new System.Drawing.Point(289, 33);
            this.testButton.Name = "testButton";
            this.testButton.Size = new System.Drawing.Size(75, 23);
            this.testButton.TabIndex = 6;
            this.testButton.Text = "Test";
            this.testButton.UseVisualStyleBackColor = true;
            this.testButton.Click += new System.EventHandler(this.testButton_Click);
            // 
            // groupTextBox
            // 
            this.groupTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupTextBox.Location = new System.Drawing.Point(70, 312);
            this.groupTextBox.MaxLength = 327670;
            this.groupTextBox.Multiline = true;
            this.groupTextBox.Name = "groupTextBox";
            this.groupTextBox.ReadOnly = true;
            this.groupTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.groupTextBox.Size = new System.Drawing.Size(286, 126);
            this.groupTextBox.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 312);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(36, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Group";
            // 
            // RegExpTesterForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(382, 457);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.groupTextBox);
            this.Controls.Add(this.testButton);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.matchTextBox);
            this.Controls.Add(this.inputTextBox);
            this.Controls.Add(this.regExpTextBox);
            this.Controls.Add(this.label1);
            this.MaximumSize = new System.Drawing.Size(1000, 491);
            this.MinimumSize = new System.Drawing.Size(390, 491);
            this.Name = "RegExpTesterForm";
            this.Text = "Regular Expression Tester";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox regExpTextBox;
        private System.Windows.Forms.TextBox inputTextBox;
        private System.Windows.Forms.TextBox matchTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button testButton;
        private System.Windows.Forms.TextBox groupTextBox;
        private System.Windows.Forms.Label label4;
    }
}