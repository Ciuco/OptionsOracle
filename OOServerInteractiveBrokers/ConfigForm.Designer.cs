namespace OOServerInteractiveBrokers
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigForm));
            this.cancelButton = new System.Windows.Forms.Button();
            this.saveButton = new System.Windows.Forms.Button();
            this.optionChainDownloadGroupBox = new System.Windows.Forms.GroupBox();
            this.exchangeOptionComboBox = new System.Windows.Forms.ComboBox();
            this.optionExchangeTableBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.mappingSet = new OOServerInteractiveBrokers.MappingSet();
            this.label3 = new System.Windows.Forms.Label();
            this.currencyComboBox = new System.Windows.Forms.ComboBox();
            this.currencyTableBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.exchangeUnderlyingComboBox = new System.Windows.Forms.ComboBox();
            this.stockExchangeTableBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.downloadOpenIntCheckBox = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label11 = new System.Windows.Forms.Label();
            this.maxParallelTickersNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.backoffTimeNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.quoteTimeoutNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.yahooExchangeSuffixTextBox = new System.Windows.Forms.TextBox();
            this.downloadHistoryFromYahooCheckBox = new System.Windows.Forms.CheckBox();
            this.filterNoPriceOptionsCheckBox = new System.Windows.Forms.CheckBox();
            this.maxExpDateFilterTimePicker = new System.Windows.Forms.DateTimePicker();
            this.enableExpDateFilterCheckBox = new System.Windows.Forms.CheckBox();
            this.aboutButton = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.maxStrikeFilterNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.minStrikeFilterNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.enableStrikeFilterCheckBox = new System.Windows.Forms.CheckBox();
            this.minExpDateFilterTimePicker = new System.Windows.Forms.DateTimePicker();
            this.downloadPredefinedRadioButton = new System.Windows.Forms.RadioButton();
            this.downloadDynamicRadioButton = new System.Windows.Forms.RadioButton();
            this.downloadAllRadioButton = new System.Windows.Forms.RadioButton();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.optionChainDownloadGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.optionExchangeTableBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mappingSet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.currencyTableBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.stockExchangeTableBindingSource)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.maxParallelTickersNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.backoffTimeNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.quoteTimeoutNumericUpDown)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.maxStrikeFilterNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.minStrikeFilterNumericUpDown)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.cancelButton.Location = new System.Drawing.Point(246, 476);
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
            this.saveButton.Location = new System.Drawing.Point(326, 476);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(74, 24);
            this.saveButton.TabIndex = 11;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = true;
            // 
            // optionChainDownloadGroupBox
            // 
            this.optionChainDownloadGroupBox.Controls.Add(this.exchangeOptionComboBox);
            this.optionChainDownloadGroupBox.Controls.Add(this.label3);
            this.optionChainDownloadGroupBox.Controls.Add(this.currencyComboBox);
            this.optionChainDownloadGroupBox.Controls.Add(this.exchangeUnderlyingComboBox);
            this.optionChainDownloadGroupBox.Controls.Add(this.label2);
            this.optionChainDownloadGroupBox.Controls.Add(this.label1);
            this.optionChainDownloadGroupBox.Location = new System.Drawing.Point(10, 8);
            this.optionChainDownloadGroupBox.Name = "optionChainDownloadGroupBox";
            this.optionChainDownloadGroupBox.Size = new System.Drawing.Size(393, 114);
            this.optionChainDownloadGroupBox.TabIndex = 12;
            this.optionChainDownloadGroupBox.TabStop = false;
            this.optionChainDownloadGroupBox.Text = "Market Selection";
            // 
            // exchangeOptionComboBox
            // 
            this.exchangeOptionComboBox.DataSource = this.optionExchangeTableBindingSource;
            this.exchangeOptionComboBox.DisplayMember = "Description";
            this.exchangeOptionComboBox.FormattingEnabled = true;
            this.exchangeOptionComboBox.Location = new System.Drawing.Point(139, 53);
            this.exchangeOptionComboBox.Name = "exchangeOptionComboBox";
            this.exchangeOptionComboBox.Size = new System.Drawing.Size(244, 21);
            this.exchangeOptionComboBox.TabIndex = 5;
            this.exchangeOptionComboBox.ValueMember = "Code";
            // 
            // optionExchangeTableBindingSource
            // 
            this.optionExchangeTableBindingSource.DataMember = "ExchangeTable";
            this.optionExchangeTableBindingSource.DataSource = this.mappingSet;
            // 
            // mappingSet
            // 
            this.mappingSet.DataSetName = "MappingSet";
            this.mappingSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 56);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(92, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Option Exchange:";
            // 
            // currencyComboBox
            // 
            this.currencyComboBox.DataSource = this.currencyTableBindingSource;
            this.currencyComboBox.DisplayMember = "Description";
            this.currencyComboBox.FormattingEnabled = true;
            this.currencyComboBox.Location = new System.Drawing.Point(139, 80);
            this.currencyComboBox.Name = "currencyComboBox";
            this.currencyComboBox.Size = new System.Drawing.Size(244, 21);
            this.currencyComboBox.TabIndex = 3;
            this.currencyComboBox.ValueMember = "Code";
            // 
            // currencyTableBindingSource
            // 
            this.currencyTableBindingSource.DataMember = "CurrencyTable";
            this.currencyTableBindingSource.DataSource = this.mappingSet;
            // 
            // exchangeUnderlyingComboBox
            // 
            this.exchangeUnderlyingComboBox.DataSource = this.stockExchangeTableBindingSource;
            this.exchangeUnderlyingComboBox.DisplayMember = "Description";
            this.exchangeUnderlyingComboBox.FormattingEnabled = true;
            this.exchangeUnderlyingComboBox.Location = new System.Drawing.Point(139, 26);
            this.exchangeUnderlyingComboBox.Name = "exchangeUnderlyingComboBox";
            this.exchangeUnderlyingComboBox.Size = new System.Drawing.Size(244, 21);
            this.exchangeUnderlyingComboBox.TabIndex = 2;
            this.exchangeUnderlyingComboBox.ValueMember = "Code";
            // 
            // stockExchangeTableBindingSource
            // 
            this.stockExchangeTableBindingSource.DataMember = "ExchangeTable";
            this.stockExchangeTableBindingSource.DataSource = this.mappingSet;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 83);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Currency:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(111, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Underlying Exchange:";
            // 
            // downloadOpenIntCheckBox
            // 
            this.downloadOpenIntCheckBox.AutoSize = true;
            this.downloadOpenIntCheckBox.Checked = true;
            this.downloadOpenIntCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.downloadOpenIntCheckBox.Location = new System.Drawing.Point(11, 128);
            this.downloadOpenIntCheckBox.Name = "downloadOpenIntCheckBox";
            this.downloadOpenIntCheckBox.Size = new System.Drawing.Size(180, 17);
            this.downloadOpenIntCheckBox.TabIndex = 6;
            this.downloadOpenIntCheckBox.Text = "Download Options Open Interest";
            this.downloadOpenIntCheckBox.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.maxParallelTickersNumericUpDown);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.backoffTimeNumericUpDown);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.quoteTimeoutNumericUpDown);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Location = new System.Drawing.Point(10, 128);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(393, 102);
            this.groupBox1.TabIndex = 13;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "TWS Communication";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(208, 73);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(179, 13);
            this.label11.TabIndex = 28;
            this.label11.Text = "(Decrease if you get TWS error msg)";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // maxParallelTickersNumericUpDown
            // 
            this.maxParallelTickersNumericUpDown.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.maxParallelTickersNumericUpDown.Location = new System.Drawing.Point(139, 71);
            this.maxParallelTickersNumericUpDown.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.maxParallelTickersNumericUpDown.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.maxParallelTickersNumericUpDown.Name = "maxParallelTickersNumericUpDown";
            this.maxParallelTickersNumericUpDown.Size = new System.Drawing.Size(63, 20);
            this.maxParallelTickersNumericUpDown.TabIndex = 27;
            this.toolTip.SetToolTip(this.maxParallelTickersNumericUpDown, resources.GetString("maxParallelTickersNumericUpDown.ToolTip"));
            this.maxParallelTickersNumericUpDown.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(8, 73);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(105, 13);
            this.label10.TabIndex = 26;
            this.label10.Text = "Max Parallel Tickers:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(208, 48);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(168, 13);
            this.label9.TabIndex = 25;
            this.label9.Text = "(Increase to slow down download)";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // backoffTimeNumericUpDown
            // 
            this.backoffTimeNumericUpDown.Location = new System.Drawing.Point(139, 45);
            this.backoffTimeNumericUpDown.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.backoffTimeNumericUpDown.Name = "backoffTimeNumericUpDown";
            this.backoffTimeNumericUpDown.Size = new System.Drawing.Size(63, 20);
            this.backoffTimeNumericUpDown.TabIndex = 13;
            this.toolTip.SetToolTip(this.backoffTimeNumericUpDown, "Increase this number to slow down the download process.\r\n\r\n[default value = 0]");
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(8, 47);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(122, 13);
            this.label8.TabIndex = 12;
            this.label8.Text = "Additional Backoff Time:";
            // 
            // quoteTimeoutNumericUpDown
            // 
            this.quoteTimeoutNumericUpDown.Location = new System.Drawing.Point(139, 19);
            this.quoteTimeoutNumericUpDown.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.quoteTimeoutNumericUpDown.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.quoteTimeoutNumericUpDown.Name = "quoteTimeoutNumericUpDown";
            this.quoteTimeoutNumericUpDown.Size = new System.Drawing.Size(63, 20);
            this.quoteTimeoutNumericUpDown.TabIndex = 9;
            this.toolTip.SetToolTip(this.quoteTimeoutNumericUpDown, "Quote timeout. Increase this number if some of the\r\nquote/option-chain data is mi" +
                    "ssing.\r\n\r\n[default value = 5]");
            this.quoteTimeoutNumericUpDown.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 22);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(80, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Quote Timeout:";
            // 
            // yahooExchangeSuffixTextBox
            // 
            this.yahooExchangeSuffixTextBox.Location = new System.Drawing.Point(265, 18);
            this.yahooExchangeSuffixTextBox.Name = "yahooExchangeSuffixTextBox";
            this.yahooExchangeSuffixTextBox.Size = new System.Drawing.Size(26, 20);
            this.yahooExchangeSuffixTextBox.TabIndex = 11;
            this.toolTip.SetToolTip(this.yahooExchangeSuffixTextBox, "The two charaters Yahoo! exchange code (e.g. MI for Milano).\r\nUse blank for US ex" +
                    "changes.\r\n\r\n[default value = BLANK]");
            // 
            // downloadHistoryFromYahooCheckBox
            // 
            this.downloadHistoryFromYahooCheckBox.AutoSize = true;
            this.downloadHistoryFromYahooCheckBox.Checked = true;
            this.downloadHistoryFromYahooCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.downloadHistoryFromYahooCheckBox.Location = new System.Drawing.Point(6, 20);
            this.downloadHistoryFromYahooCheckBox.Name = "downloadHistoryFromYahooCheckBox";
            this.downloadHistoryFromYahooCheckBox.Size = new System.Drawing.Size(299, 17);
            this.downloadHistoryFromYahooCheckBox.TabIndex = 10;
            this.downloadHistoryFromYahooCheckBox.Text = "Download Historical Data from Yahoo! (Exchange:           )";
            this.downloadHistoryFromYahooCheckBox.UseVisualStyleBackColor = true;
            this.downloadHistoryFromYahooCheckBox.CheckedChanged += new System.EventHandler(this.downloadHistoryFromYahooCheckBox_CheckedChanged);
            // 
            // filterNoPriceOptionsCheckBox
            // 
            this.filterNoPriceOptionsCheckBox.AutoSize = true;
            this.filterNoPriceOptionsCheckBox.Checked = true;
            this.filterNoPriceOptionsCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.filterNoPriceOptionsCheckBox.Location = new System.Drawing.Point(11, 151);
            this.filterNoPriceOptionsCheckBox.Name = "filterNoPriceOptionsCheckBox";
            this.filterNoPriceOptionsCheckBox.Size = new System.Drawing.Size(234, 17);
            this.filterNoPriceOptionsCheckBox.TabIndex = 17;
            this.filterNoPriceOptionsCheckBox.Text = "Filter Out Options without Pricing Information";
            this.filterNoPriceOptionsCheckBox.UseVisualStyleBackColor = true;
            // 
            // maxExpDateFilterTimePicker
            // 
            this.maxExpDateFilterTimePicker.Enabled = false;
            this.maxExpDateFilterTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.maxExpDateFilterTimePicker.Location = new System.Drawing.Point(295, 66);
            this.maxExpDateFilterTimePicker.Name = "maxExpDateFilterTimePicker";
            this.maxExpDateFilterTimePicker.Size = new System.Drawing.Size(86, 20);
            this.maxExpDateFilterTimePicker.TabIndex = 16;
            this.toolTip.SetToolTip(this.maxExpDateFilterTimePicker, "Maximum expiration date");
            // 
            // enableExpDateFilterCheckBox
            // 
            this.enableExpDateFilterCheckBox.AutoSize = true;
            this.enableExpDateFilterCheckBox.Enabled = false;
            this.enableExpDateFilterCheckBox.Location = new System.Drawing.Point(119, 69);
            this.enableExpDateFilterCheckBox.Name = "enableExpDateFilterCheckBox";
            this.enableExpDateFilterCheckBox.Size = new System.Drawing.Size(75, 17);
            this.enableExpDateFilterCheckBox.TabIndex = 15;
            this.enableExpDateFilterCheckBox.Text = "Expiration:";
            this.enableExpDateFilterCheckBox.UseVisualStyleBackColor = true;
            this.enableExpDateFilterCheckBox.CheckedChanged += new System.EventHandler(this.enableXXXFilterCheckBox_CheckedChanged);
            // 
            // aboutButton
            // 
            this.aboutButton.Location = new System.Drawing.Point(10, 476);
            this.aboutButton.Name = "aboutButton";
            this.aboutButton.Size = new System.Drawing.Size(73, 23);
            this.aboutButton.TabIndex = 14;
            this.aboutButton.Text = "About...";
            this.aboutButton.UseVisualStyleBackColor = true;
            this.aboutButton.Click += new System.EventHandler(this.aboutButton_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.maxStrikeFilterNumericUpDown);
            this.groupBox2.Controls.Add(this.minStrikeFilterNumericUpDown);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.enableStrikeFilterCheckBox);
            this.groupBox2.Controls.Add(this.minExpDateFilterTimePicker);
            this.groupBox2.Controls.Add(this.filterNoPriceOptionsCheckBox);
            this.groupBox2.Controls.Add(this.downloadPredefinedRadioButton);
            this.groupBox2.Controls.Add(this.maxExpDateFilterTimePicker);
            this.groupBox2.Controls.Add(this.downloadOpenIntCheckBox);
            this.groupBox2.Controls.Add(this.downloadDynamicRadioButton);
            this.groupBox2.Controls.Add(this.enableExpDateFilterCheckBox);
            this.groupBox2.Controls.Add(this.downloadAllRadioButton);
            this.groupBox2.Location = new System.Drawing.Point(10, 236);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(393, 179);
            this.groupBox2.TabIndex = 15;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Option-Chain Download";
            // 
            // maxStrikeFilterNumericUpDown
            // 
            this.maxStrikeFilterNumericUpDown.Enabled = false;
            this.maxStrikeFilterNumericUpDown.Location = new System.Drawing.Point(295, 92);
            this.maxStrikeFilterNumericUpDown.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.maxStrikeFilterNumericUpDown.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.maxStrikeFilterNumericUpDown.Name = "maxStrikeFilterNumericUpDown";
            this.maxStrikeFilterNumericUpDown.Size = new System.Drawing.Size(86, 20);
            this.maxStrikeFilterNumericUpDown.TabIndex = 28;
            this.toolTip.SetToolTip(this.maxStrikeFilterNumericUpDown, "Maximum strike range in % of the underlying");
            this.maxStrikeFilterNumericUpDown.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.maxStrikeFilterNumericUpDown.ValueChanged += new System.EventHandler(this.xxxStrikeFilterNumericUpDown_ValueChanged);
            // 
            // minStrikeFilterNumericUpDown
            // 
            this.minStrikeFilterNumericUpDown.Enabled = false;
            this.minStrikeFilterNumericUpDown.Location = new System.Drawing.Point(193, 93);
            this.minStrikeFilterNumericUpDown.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.minStrikeFilterNumericUpDown.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.minStrikeFilterNumericUpDown.Name = "minStrikeFilterNumericUpDown";
            this.minStrikeFilterNumericUpDown.Size = new System.Drawing.Size(86, 20);
            this.minStrikeFilterNumericUpDown.TabIndex = 27;
            this.toolTip.SetToolTip(this.minStrikeFilterNumericUpDown, "Minimum strike range in % of the underlying");
            this.minStrikeFilterNumericUpDown.Value = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.minStrikeFilterNumericUpDown.ValueChanged += new System.EventHandler(this.xxxStrikeFilterNumericUpDown_ValueChanged);
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(295, 113);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(86, 17);
            this.label7.TabIndex = 24;
            this.label7.Text = "(% of Underlying)";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(282, 93);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(10, 13);
            this.label6.TabIndex = 23;
            this.label6.Text = "-";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(282, 69);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(10, 13);
            this.label5.TabIndex = 22;
            this.label5.Text = "-";
            // 
            // enableStrikeFilterCheckBox
            // 
            this.enableStrikeFilterCheckBox.AutoSize = true;
            this.enableStrikeFilterCheckBox.Enabled = false;
            this.enableStrikeFilterCheckBox.Location = new System.Drawing.Point(119, 93);
            this.enableStrikeFilterCheckBox.Name = "enableStrikeFilterCheckBox";
            this.enableStrikeFilterCheckBox.Size = new System.Drawing.Size(56, 17);
            this.enableStrikeFilterCheckBox.TabIndex = 19;
            this.enableStrikeFilterCheckBox.Text = "Strike:";
            this.enableStrikeFilterCheckBox.UseVisualStyleBackColor = true;
            this.enableStrikeFilterCheckBox.CheckedChanged += new System.EventHandler(this.enableXXXFilterCheckBox_CheckedChanged);
            // 
            // minExpDateFilterTimePicker
            // 
            this.minExpDateFilterTimePicker.Enabled = false;
            this.minExpDateFilterTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.minExpDateFilterTimePicker.Location = new System.Drawing.Point(193, 66);
            this.minExpDateFilterTimePicker.Name = "minExpDateFilterTimePicker";
            this.minExpDateFilterTimePicker.Size = new System.Drawing.Size(86, 20);
            this.minExpDateFilterTimePicker.TabIndex = 18;
            this.toolTip.SetToolTip(this.minExpDateFilterTimePicker, "Minimum expiration date");
            // 
            // downloadPredefinedRadioButton
            // 
            this.downloadPredefinedRadioButton.AutoSize = true;
            this.downloadPredefinedRadioButton.Location = new System.Drawing.Point(11, 68);
            this.downloadPredefinedRadioButton.Name = "downloadPredefinedRadioButton";
            this.downloadPredefinedRadioButton.Size = new System.Drawing.Size(104, 17);
            this.downloadPredefinedRadioButton.TabIndex = 2;
            this.downloadPredefinedRadioButton.Text = "Predefined Filter:";
            this.downloadPredefinedRadioButton.UseVisualStyleBackColor = true;
            this.downloadPredefinedRadioButton.CheckedChanged += new System.EventHandler(this.downloadXXXRadioButton_CheckedChanged);
            // 
            // downloadDynamicRadioButton
            // 
            this.downloadDynamicRadioButton.AutoSize = true;
            this.downloadDynamicRadioButton.Location = new System.Drawing.Point(11, 45);
            this.downloadDynamicRadioButton.Name = "downloadDynamicRadioButton";
            this.downloadDynamicRadioButton.Size = new System.Drawing.Size(227, 17);
            this.downloadDynamicRadioButton.TabIndex = 1;
            this.downloadDynamicRadioButton.Text = "Dynamic Filter (ask me for every download)";
            this.downloadDynamicRadioButton.UseVisualStyleBackColor = true;
            // 
            // downloadAllRadioButton
            // 
            this.downloadAllRadioButton.AutoSize = true;
            this.downloadAllRadioButton.Checked = true;
            this.downloadAllRadioButton.Location = new System.Drawing.Point(11, 22);
            this.downloadAllRadioButton.Name = "downloadAllRadioButton";
            this.downloadAllRadioButton.Size = new System.Drawing.Size(87, 17);
            this.downloadAllRadioButton.TabIndex = 0;
            this.downloadAllRadioButton.TabStop = true;
            this.downloadAllRadioButton.Text = "Download All";
            this.downloadAllRadioButton.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.yahooExchangeSuffixTextBox);
            this.groupBox3.Controls.Add(this.downloadHistoryFromYahooCheckBox);
            this.groupBox3.Location = new System.Drawing.Point(10, 421);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(393, 49);
            this.groupBox3.TabIndex = 16;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Historical-Data Download";
            // 
            // ConfigForm
            // 
            this.AcceptButton = this.saveButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(414, 506);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.aboutButton);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.optionChainDownloadGroupBox);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.cancelButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ConfigForm";
            this.Text = "InteractiveBrokers Config";
            this.optionChainDownloadGroupBox.ResumeLayout(false);
            this.optionChainDownloadGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.optionExchangeTableBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mappingSet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.currencyTableBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.stockExchangeTableBindingSource)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.maxParallelTickersNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.backoffTimeNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.quoteTimeoutNumericUpDown)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.maxStrikeFilterNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.minStrikeFilterNumericUpDown)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.GroupBox optionChainDownloadGroupBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox currencyComboBox;
        private System.Windows.Forms.ComboBox exchangeUnderlyingComboBox;
        private System.Windows.Forms.ComboBox exchangeOptionComboBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox downloadOpenIntCheckBox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown quoteTimeoutNumericUpDown;
        private System.Windows.Forms.CheckBox downloadHistoryFromYahooCheckBox;
        private System.Windows.Forms.TextBox yahooExchangeSuffixTextBox;
        private System.Windows.Forms.BindingSource stockExchangeTableBindingSource;
        private MappingSet mappingSet;
        private System.Windows.Forms.BindingSource currencyTableBindingSource;
        private System.Windows.Forms.BindingSource optionExchangeTableBindingSource;
        private System.Windows.Forms.Button aboutButton;
        private System.Windows.Forms.CheckBox enableExpDateFilterCheckBox;
        private System.Windows.Forms.DateTimePicker maxExpDateFilterTimePicker;
        private System.Windows.Forms.CheckBox filterNoPriceOptionsCheckBox;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton downloadDynamicRadioButton;
        private System.Windows.Forms.RadioButton downloadAllRadioButton;
        private System.Windows.Forms.DateTimePicker minExpDateFilterTimePicker;
        private System.Windows.Forms.RadioButton downloadPredefinedRadioButton;
        private System.Windows.Forms.CheckBox enableStrikeFilterCheckBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown maxStrikeFilterNumericUpDown;
        private System.Windows.Forms.NumericUpDown minStrikeFilterNumericUpDown;
        private System.Windows.Forms.NumericUpDown backoffTimeNumericUpDown;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.NumericUpDown maxParallelTickersNumericUpDown;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ToolTip toolTip;
    }
}