namespace OOServerUS
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
            this.indexOptionChainDataSourceComboBox = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.fundOptionChainDataSourceComboBox = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.stockOptionChainDataSourceComboBox = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.quoteDataSourceComboBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
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
            // indexOptionChainDataSourceComboBox
            // 
            this.indexOptionChainDataSourceComboBox.FormattingEnabled = true;
            this.indexOptionChainDataSourceComboBox.Items.AddRange(new object[] {
            "Default",
            "A",
            "B",
            "C",
            "D"});
            this.indexOptionChainDataSourceComboBox.Location = new System.Drawing.Point(271, 93);
            this.indexOptionChainDataSourceComboBox.Name = "indexOptionChainDataSourceComboBox";
            this.indexOptionChainDataSourceComboBox.Size = new System.Drawing.Size(121, 21);
            this.indexOptionChainDataSourceComboBox.TabIndex = 35;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 96);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(195, 13);
            this.label4.TabIndex = 34;
            this.label4.Text = "Data Source for Option-Chain (Indexes):";
            // 
            // fundOptionChainDataSourceComboBox
            // 
            this.fundOptionChainDataSourceComboBox.FormattingEnabled = true;
            this.fundOptionChainDataSourceComboBox.Items.AddRange(new object[] {
            "Default",
            "A",
            "B",
            "C",
            "D"});
            this.fundOptionChainDataSourceComboBox.Location = new System.Drawing.Point(271, 66);
            this.fundOptionChainDataSourceComboBox.Name = "fundOptionChainDataSourceComboBox";
            this.fundOptionChainDataSourceComboBox.Size = new System.Drawing.Size(121, 21);
            this.fundOptionChainDataSourceComboBox.TabIndex = 33;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 69);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(187, 13);
            this.label3.TabIndex = 32;
            this.label3.Text = "Data Source for Option-Chain (Funds):";
            // 
            // stockOptionChainDataSourceComboBox
            // 
            this.stockOptionChainDataSourceComboBox.FormattingEnabled = true;
            this.stockOptionChainDataSourceComboBox.Items.AddRange(new object[] {
            "Default",
            "A",
            "B",
            "C",
            "D"});
            this.stockOptionChainDataSourceComboBox.Location = new System.Drawing.Point(271, 39);
            this.stockOptionChainDataSourceComboBox.Name = "stockOptionChainDataSourceComboBox";
            this.stockOptionChainDataSourceComboBox.Size = new System.Drawing.Size(121, 21);
            this.stockOptionChainDataSourceComboBox.TabIndex = 31;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(191, 13);
            this.label2.TabIndex = 30;
            this.label2.Text = "Data Source for Option-Chain (Stocks):";
            // 
            // quoteDataSourceComboBox
            // 
            this.quoteDataSourceComboBox.FormattingEnabled = true;
            this.quoteDataSourceComboBox.Items.AddRange(new object[] {
            "Default",
            "A",
            "B"});
            this.quoteDataSourceComboBox.Location = new System.Drawing.Point(271, 12);
            this.quoteDataSourceComboBox.Name = "quoteDataSourceComboBox";
            this.quoteDataSourceComboBox.Size = new System.Drawing.Size(121, 21);
            this.quoteDataSourceComboBox.TabIndex = 29;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(122, 13);
            this.label1.TabIndex = 28;
            this.label1.Text = "Data Source for Quotes:";
            // 
            // ConfigForm
            // 
            this.AcceptButton = this.saveButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(404, 173);
            this.Controls.Add(this.indexOptionChainDataSourceComboBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.fundOptionChainDataSourceComboBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.stockOptionChainDataSourceComboBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.quoteDataSourceComboBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.aboutButton);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.cancelButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ConfigForm";
            this.ShowInTaskbar = false;
            this.Text = "TD Ameritrade Config";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button aboutButton;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.ComboBox indexOptionChainDataSourceComboBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox fundOptionChainDataSourceComboBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox stockOptionChainDataSourceComboBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox quoteDataSourceComboBox;
        private System.Windows.Forms.Label label1;
    }
}