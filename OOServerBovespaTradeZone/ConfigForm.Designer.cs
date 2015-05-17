namespace OOServerBovespaTradeZone
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
            this.cancelButton = new System.Windows.Forms.Button();
            this.saveButton = new System.Windows.Forms.Button();
            this.optionChainDownloadGroupBox = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.strikeUpperLimitNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.strikeLowerLimitNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.monthsCountNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.hideOptionsWithNoMarketDataCheckBox = new System.Windows.Forms.CheckBox();
            this.optionChainDownloadGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.strikeUpperLimitNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.strikeLowerLimitNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.monthsCountNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.cancelButton.Location = new System.Drawing.Point(181, 143);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(74, 24);
            this.cancelButton.TabIndex = 10;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // saveButton
            // 
            this.saveButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.saveButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.saveButton.Location = new System.Drawing.Point(261, 143);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(74, 24);
            this.saveButton.TabIndex = 11;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = true;
            // 
            // optionChainDownloadGroupBox
            // 
            this.optionChainDownloadGroupBox.Controls.Add(this.hideOptionsWithNoMarketDataCheckBox);
            this.optionChainDownloadGroupBox.Controls.Add(this.label4);
            this.optionChainDownloadGroupBox.Controls.Add(this.label3);
            this.optionChainDownloadGroupBox.Controls.Add(this.strikeUpperLimitNumericUpDown);
            this.optionChainDownloadGroupBox.Controls.Add(this.strikeLowerLimitNumericUpDown);
            this.optionChainDownloadGroupBox.Controls.Add(this.monthsCountNumericUpDown);
            this.optionChainDownloadGroupBox.Controls.Add(this.label2);
            this.optionChainDownloadGroupBox.Controls.Add(this.label1);
            this.optionChainDownloadGroupBox.Location = new System.Drawing.Point(8, 8);
            this.optionChainDownloadGroupBox.Name = "optionChainDownloadGroupBox";
            this.optionChainDownloadGroupBox.Size = new System.Drawing.Size(327, 129);
            this.optionChainDownloadGroupBox.TabIndex = 12;
            this.optionChainDownloadGroupBox.TabStop = false;
            this.optionChainDownloadGroupBox.Text = "Option Chain Download";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(161, 59);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(10, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "-";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(241, 58);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(80, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "[ % Underlying ]";
            // 
            // strikeUpperLimitNumericUpDown
            // 
            this.strikeUpperLimitNumericUpDown.Increment = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.strikeUpperLimitNumericUpDown.Location = new System.Drawing.Point(179, 56);
            this.strikeUpperLimitNumericUpDown.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.strikeUpperLimitNumericUpDown.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.strikeUpperLimitNumericUpDown.Name = "strikeUpperLimitNumericUpDown";
            this.strikeUpperLimitNumericUpDown.Size = new System.Drawing.Size(56, 20);
            this.strikeUpperLimitNumericUpDown.TabIndex = 4;
            this.strikeUpperLimitNumericUpDown.Value = new decimal(new int[] {
            130,
            0,
            0,
            0});
            // 
            // strikeLowerLimitNumericUpDown
            // 
            this.strikeLowerLimitNumericUpDown.Increment = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.strikeLowerLimitNumericUpDown.Location = new System.Drawing.Point(97, 56);
            this.strikeLowerLimitNumericUpDown.Name = "strikeLowerLimitNumericUpDown";
            this.strikeLowerLimitNumericUpDown.Size = new System.Drawing.Size(56, 20);
            this.strikeLowerLimitNumericUpDown.TabIndex = 3;
            this.strikeLowerLimitNumericUpDown.Value = new decimal(new int[] {
            70,
            0,
            0,
            0});
            // 
            // monthsCountNumericUpDown
            // 
            this.monthsCountNumericUpDown.Location = new System.Drawing.Point(97, 27);
            this.monthsCountNumericUpDown.Maximum = new decimal(new int[] {
            12,
            0,
            0,
            0});
            this.monthsCountNumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.monthsCountNumericUpDown.Name = "monthsCountNumericUpDown";
            this.monthsCountNumericUpDown.Size = new System.Drawing.Size(56, 20);
            this.monthsCountNumericUpDown.TabIndex = 2;
            this.monthsCountNumericUpDown.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 58);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Strike Range:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Num Of Months:";
            // 
            // hideOptionsWithNoMarketDataCheckBox
            // 
            this.hideOptionsWithNoMarketDataCheckBox.AutoSize = true;
            this.hideOptionsWithNoMarketDataCheckBox.Location = new System.Drawing.Point(10, 91);
            this.hideOptionsWithNoMarketDataCheckBox.Name = "hideOptionsWithNoMarketDataCheckBox";
            this.hideOptionsWithNoMarketDataCheckBox.Size = new System.Drawing.Size(293, 17);
            this.hideOptionsWithNoMarketDataCheckBox.TabIndex = 7;
            this.hideOptionsWithNoMarketDataCheckBox.Text = "Hide options with no market data (no ask/bid/last prices)";
            this.hideOptionsWithNoMarketDataCheckBox.UseVisualStyleBackColor = true;
            // 
            // ConfigForm
            // 
            this.AcceptButton = this.saveButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(343, 170);
            this.Controls.Add(this.optionChainDownloadGroupBox);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.cancelButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ConfigForm";
            this.Text = "Bovespa TradeZone Config";
            this.optionChainDownloadGroupBox.ResumeLayout(false);
            this.optionChainDownloadGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.strikeUpperLimitNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.strikeLowerLimitNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.monthsCountNumericUpDown)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.GroupBox optionChainDownloadGroupBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown strikeUpperLimitNumericUpDown;
        private System.Windows.Forms.NumericUpDown strikeLowerLimitNumericUpDown;
        private System.Windows.Forms.NumericUpDown monthsCountNumericUpDown;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox hideOptionsWithNoMarketDataCheckBox;
    }
}