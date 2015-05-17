namespace OOServerTDAmeritrade
{
    partial class ConfigForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigForm));
            this.aboutButton = new System.Windows.Forms.Button();
            this.saveButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.defaultUsernameTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.loginOnStartUpCheckBox = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.downloadHistoryFromYahooCheckBox = new System.Windows.Forms.CheckBox();
            this.yahooExchangeSuffixTextBox = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // aboutButton
            // 
            this.aboutButton.Location = new System.Drawing.Point(5, 141);
            this.aboutButton.Name = "aboutButton";
            this.aboutButton.Size = new System.Drawing.Size(73, 23);
            this.aboutButton.TabIndex = 19;
            this.aboutButton.Text = "About...";
            this.aboutButton.UseVisualStyleBackColor = true;
            this.aboutButton.Click += new System.EventHandler(this.aboutButton_Click);
            // 
            // saveButton
            // 
            this.saveButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.saveButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.saveButton.Location = new System.Drawing.Point(321, 141);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(74, 24);
            this.saveButton.TabIndex = 18;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = true;
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.cancelButton.Location = new System.Drawing.Point(241, 141);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(74, 24);
            this.cancelButton.TabIndex = 17;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // defaultUsernameTextBox
            // 
            this.defaultUsernameTextBox.Location = new System.Drawing.Point(150, 17);
            this.defaultUsernameTextBox.Name = "defaultUsernameTextBox";
            this.defaultUsernameTextBox.Size = new System.Drawing.Size(140, 20);
            this.defaultUsernameTextBox.TabIndex = 11;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(138, 13);
            this.label1.TabIndex = 12;
            this.label1.Text = "Default Account Username:";
            // 
            // loginOnStartUpCheckBox
            // 
            this.loginOnStartUpCheckBox.AutoSize = true;
            this.loginOnStartUpCheckBox.Checked = true;
            this.loginOnStartUpCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.loginOnStartUpCheckBox.Location = new System.Drawing.Point(9, 43);
            this.loginOnStartUpCheckBox.Name = "loginOnStartUpCheckBox";
            this.loginOnStartUpCheckBox.Size = new System.Drawing.Size(173, 17);
            this.loginOnStartUpCheckBox.TabIndex = 13;
            this.loginOnStartUpCheckBox.Text = "Automatically Login On StartUp";
            this.loginOnStartUpCheckBox.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.loginOnStartUpCheckBox);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.defaultUsernameTextBox);
            this.groupBox1.Location = new System.Drawing.Point(5, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(393, 68);
            this.groupBox1.TabIndex = 21;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Login";
            // 
            // downloadHistoryFromYahooCheckBox
            // 
            this.downloadHistoryFromYahooCheckBox.AutoSize = true;
            this.downloadHistoryFromYahooCheckBox.Checked = true;
            this.downloadHistoryFromYahooCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.downloadHistoryFromYahooCheckBox.Location = new System.Drawing.Point(9, 20);
            this.downloadHistoryFromYahooCheckBox.Name = "downloadHistoryFromYahooCheckBox";
            this.downloadHistoryFromYahooCheckBox.Size = new System.Drawing.Size(299, 17);
            this.downloadHistoryFromYahooCheckBox.TabIndex = 10;
            this.downloadHistoryFromYahooCheckBox.Text = "Download Historical Data from Yahoo! (Exchange:           )";
            this.downloadHistoryFromYahooCheckBox.UseVisualStyleBackColor = true;
            this.downloadHistoryFromYahooCheckBox.CheckedChanged += new System.EventHandler(this.downloadHistoryFromYahooCheckBox_CheckedChanged);
            // 
            // yahooExchangeSuffixTextBox
            // 
            this.yahooExchangeSuffixTextBox.Location = new System.Drawing.Point(268, 18);
            this.yahooExchangeSuffixTextBox.Name = "yahooExchangeSuffixTextBox";
            this.yahooExchangeSuffixTextBox.Size = new System.Drawing.Size(26, 20);
            this.yahooExchangeSuffixTextBox.TabIndex = 11;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.yahooExchangeSuffixTextBox);
            this.groupBox2.Controls.Add(this.downloadHistoryFromYahooCheckBox);
            this.groupBox2.Location = new System.Drawing.Point(5, 86);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(393, 49);
            this.groupBox2.TabIndex = 20;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Historical-Data Download";
            // 
            // ConfigForm
            // 
            this.AcceptButton = this.saveButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(404, 173);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.aboutButton);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.cancelButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ConfigForm";
            this.ShowInTaskbar = false;
            this.Text = "TD Ameritrade Config";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button aboutButton;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.TextBox defaultUsernameTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox loginOnStartUpCheckBox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox downloadHistoryFromYahooCheckBox;
        private System.Windows.Forms.TextBox yahooExchangeSuffixTextBox;
        private System.Windows.Forms.GroupBox groupBox2;
    }
}