namespace OptionsOracle.DataCenter
{
    partial class ImportForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImportForm));
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.delimitersGroupBox = new System.Windows.Forms.GroupBox();
            this.otherDelTextBox = new System.Windows.Forms.TextBox();
            this.otherDelCheckBox = new System.Windows.Forms.CheckBox();
            this.spaceDelCheckBox = new System.Windows.Forms.CheckBox();
            this.commaDelCheckBox = new System.Windows.Forms.CheckBox();
            this.semicolonDelCheckBox = new System.Windows.Forms.CheckBox();
            this.tabDelCheckBox = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.fileListView = new System.Windows.Forms.ListView();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.csvFileTypeRadioButton = new System.Windows.Forms.RadioButton();
            this.opdFileTypeRadioButton = new System.Windows.Forms.RadioButton();
            this.opoFileTypeRadioButton = new System.Windows.Forms.RadioButton();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.newPreConButton = new System.Windows.Forms.Button();
            this.deletePreConButton = new System.Windows.Forms.Button();
            this.preConNameComboBox = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rulesDataGridView = new System.Windows.Forms.DataGridView();
            this.mappingRuleColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.mappingFieldColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.mappingColumnColumn = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.columnTableBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.parsingSetBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.parsingSet = new OptionsOracle.DataCenter.Data.ParsingSet();
            this.mappingValueColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.mappingCultureColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.mappingTableBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.configSet = new OptionsOracle.DataCenter.Data.ConfigSet();
            this.csvContentGroupBox = new System.Windows.Forms.GroupBox();
            this.optionConCheckBox = new System.Windows.Forms.CheckBox();
            this.underlyingConCheckBox = new System.Windows.Forms.CheckBox();
            this.backButton = new System.Windows.Forms.Button();
            this.nextButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.doneButton = new System.Windows.Forms.Button();
            this.tabControl.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.delimitersGroupBox.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rulesDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.columnTableBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.parsingSetBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.parsingSet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mappingTableBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.configSet)).BeginInit();
            this.csvContentGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.tabControl.Controls.Add(this.tabPage1);
            this.tabControl.Controls.Add(this.tabPage2);
            this.tabControl.Location = new System.Drawing.Point(6, -21);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(570, 433);
            this.tabControl.TabIndex = 0;
            this.tabControl.SelectedIndexChanged += new System.EventHandler(this.tabControl_SelectedIndexChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.delimitersGroupBox);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.fileListView);
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Location = new System.Drawing.Point(4, 25);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(562, 404);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // delimitersGroupBox
            // 
            this.delimitersGroupBox.Controls.Add(this.otherDelTextBox);
            this.delimitersGroupBox.Controls.Add(this.otherDelCheckBox);
            this.delimitersGroupBox.Controls.Add(this.spaceDelCheckBox);
            this.delimitersGroupBox.Controls.Add(this.commaDelCheckBox);
            this.delimitersGroupBox.Controls.Add(this.semicolonDelCheckBox);
            this.delimitersGroupBox.Controls.Add(this.tabDelCheckBox);
            this.delimitersGroupBox.Enabled = false;
            this.delimitersGroupBox.Location = new System.Drawing.Point(336, 10);
            this.delimitersGroupBox.Name = "delimitersGroupBox";
            this.delimitersGroupBox.Size = new System.Drawing.Size(209, 88);
            this.delimitersGroupBox.TabIndex = 22;
            this.delimitersGroupBox.TabStop = false;
            this.delimitersGroupBox.Text = "Delimiters";
            // 
            // otherDelTextBox
            // 
            this.otherDelTextBox.Location = new System.Drawing.Point(167, 42);
            this.otherDelTextBox.MaxLength = 1;
            this.otherDelTextBox.Name = "otherDelTextBox";
            this.otherDelTextBox.Size = new System.Drawing.Size(27, 20);
            this.otherDelTextBox.TabIndex = 8;
            this.otherDelTextBox.TextChanged += new System.EventHandler(this.otherDelTextBox_TextChanged);
            // 
            // otherDelCheckBox
            // 
            this.otherDelCheckBox.AutoSize = true;
            this.otherDelCheckBox.Location = new System.Drawing.Point(106, 44);
            this.otherDelCheckBox.Name = "otherDelCheckBox";
            this.otherDelCheckBox.Size = new System.Drawing.Size(55, 17);
            this.otherDelCheckBox.TabIndex = 7;
            this.otherDelCheckBox.Text = "&Other:";
            this.otherDelCheckBox.UseVisualStyleBackColor = true;
            this.otherDelCheckBox.CheckedChanged += new System.EventHandler(this.xxxDelCheckBox_CheckedChanged);
            // 
            // spaceDelCheckBox
            // 
            this.spaceDelCheckBox.AutoSize = true;
            this.spaceDelCheckBox.Location = new System.Drawing.Point(106, 21);
            this.spaceDelCheckBox.Name = "spaceDelCheckBox";
            this.spaceDelCheckBox.Size = new System.Drawing.Size(57, 17);
            this.spaceDelCheckBox.TabIndex = 6;
            this.spaceDelCheckBox.Text = "&Space";
            this.spaceDelCheckBox.UseVisualStyleBackColor = true;
            this.spaceDelCheckBox.CheckedChanged += new System.EventHandler(this.xxxDelCheckBox_CheckedChanged);
            // 
            // commaDelCheckBox
            // 
            this.commaDelCheckBox.AutoSize = true;
            this.commaDelCheckBox.Location = new System.Drawing.Point(15, 67);
            this.commaDelCheckBox.Name = "commaDelCheckBox";
            this.commaDelCheckBox.Size = new System.Drawing.Size(61, 17);
            this.commaDelCheckBox.TabIndex = 5;
            this.commaDelCheckBox.Text = "&Comma";
            this.commaDelCheckBox.UseVisualStyleBackColor = true;
            this.commaDelCheckBox.CheckedChanged += new System.EventHandler(this.xxxDelCheckBox_CheckedChanged);
            // 
            // semicolonDelCheckBox
            // 
            this.semicolonDelCheckBox.AutoSize = true;
            this.semicolonDelCheckBox.Location = new System.Drawing.Point(15, 44);
            this.semicolonDelCheckBox.Name = "semicolonDelCheckBox";
            this.semicolonDelCheckBox.Size = new System.Drawing.Size(75, 17);
            this.semicolonDelCheckBox.TabIndex = 4;
            this.semicolonDelCheckBox.Text = "Se&micolon";
            this.semicolonDelCheckBox.UseVisualStyleBackColor = true;
            this.semicolonDelCheckBox.CheckedChanged += new System.EventHandler(this.xxxDelCheckBox_CheckedChanged);
            // 
            // tabDelCheckBox
            // 
            this.tabDelCheckBox.AutoSize = true;
            this.tabDelCheckBox.Location = new System.Drawing.Point(15, 21);
            this.tabDelCheckBox.Name = "tabDelCheckBox";
            this.tabDelCheckBox.Size = new System.Drawing.Size(45, 17);
            this.tabDelCheckBox.TabIndex = 3;
            this.tabDelCheckBox.Text = "&Tab";
            this.tabDelCheckBox.UseVisualStyleBackColor = true;
            this.tabDelCheckBox.CheckedChanged += new System.EventHandler(this.xxxDelCheckBox_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 112);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 13);
            this.label1.TabIndex = 21;
            this.label1.Text = "File Preview:";
            // 
            // fileListView
            // 
            this.fileListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.fileListView.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.fileListView.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.fileListView.GridLines = true;
            this.fileListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.fileListView.Location = new System.Drawing.Point(6, 128);
            this.fileListView.Name = "fileListView";
            this.fileListView.Size = new System.Drawing.Size(550, 265);
            this.fileListView.TabIndex = 20;
            this.fileListView.UseCompatibleStateImageBehavior = false;
            this.fileListView.View = System.Windows.Forms.View.Details;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.csvFileTypeRadioButton);
            this.groupBox1.Controls.Add(this.opdFileTypeRadioButton);
            this.groupBox1.Controls.Add(this.opoFileTypeRadioButton);
            this.groupBox1.Location = new System.Drawing.Point(6, 10);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(326, 88);
            this.groupBox1.TabIndex = 19;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Original Data Type";
            // 
            // csvFileTypeRadioButton
            // 
            this.csvFileTypeRadioButton.AutoSize = true;
            this.csvFileTypeRadioButton.Location = new System.Drawing.Point(7, 66);
            this.csvFileTypeRadioButton.Name = "csvFileTypeRadioButton";
            this.csvFileTypeRadioButton.Size = new System.Drawing.Size(179, 17);
            this.csvFileTypeRadioButton.TabIndex = 2;
            this.csvFileTypeRadioButton.Text = "Delimited File (*.CSV / *.TXT file)";
            this.csvFileTypeRadioButton.UseVisualStyleBackColor = true;
            this.csvFileTypeRadioButton.CheckedChanged += new System.EventHandler(this.xxxFileTypeRadioButton_CheckedChanged);
            // 
            // opdFileTypeRadioButton
            // 
            this.opdFileTypeRadioButton.AutoSize = true;
            this.opdFileTypeRadioButton.Location = new System.Drawing.Point(7, 43);
            this.opdFileTypeRadioButton.Name = "opdFileTypeRadioButton";
            this.opdFileTypeRadioButton.Size = new System.Drawing.Size(244, 17);
            this.opdFileTypeRadioButton.TabIndex = 1;
            this.opdFileTypeRadioButton.Text = "OptionsOracle DataCenter data file (*.OPD file)";
            this.opdFileTypeRadioButton.UseVisualStyleBackColor = true;
            this.opdFileTypeRadioButton.CheckedChanged += new System.EventHandler(this.xxxFileTypeRadioButton_CheckedChanged);
            // 
            // opoFileTypeRadioButton
            // 
            this.opoFileTypeRadioButton.AutoSize = true;
            this.opoFileTypeRadioButton.Location = new System.Drawing.Point(7, 20);
            this.opoFileTypeRadioButton.Name = "opoFileTypeRadioButton";
            this.opoFileTypeRadioButton.Size = new System.Drawing.Size(187, 17);
            this.opoFileTypeRadioButton.TabIndex = 0;
            this.opoFileTypeRadioButton.Text = "OptionsOracle data file (*.OPO file)";
            this.opoFileTypeRadioButton.UseVisualStyleBackColor = true;
            this.opoFileTypeRadioButton.CheckedChanged += new System.EventHandler(this.xxxFileTypeRadioButton_CheckedChanged);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.groupBox3);
            this.tabPage2.Controls.Add(this.groupBox2);
            this.tabPage2.Controls.Add(this.csvContentGroupBox);
            this.tabPage2.Location = new System.Drawing.Point(4, 25);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(562, 412);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.newPreConButton);
            this.groupBox3.Controls.Add(this.deletePreConButton);
            this.groupBox3.Controls.Add(this.preConNameComboBox);
            this.groupBox3.Location = new System.Drawing.Point(167, 10);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(389, 70);
            this.groupBox3.TabIndex = 22;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Pre-Configured Settings";
            // 
            // newPreConButton
            // 
            this.newPreConButton.Location = new System.Drawing.Point(326, 13);
            this.newPreConButton.Name = "newPreConButton";
            this.newPreConButton.Size = new System.Drawing.Size(55, 24);
            this.newPreConButton.TabIndex = 12;
            this.newPreConButton.Text = "New";
            this.newPreConButton.UseVisualStyleBackColor = true;
            this.newPreConButton.Click += new System.EventHandler(this.newPreConButton_Click);
            // 
            // deletePreConButton
            // 
            this.deletePreConButton.Location = new System.Drawing.Point(326, 38);
            this.deletePreConButton.Name = "deletePreConButton";
            this.deletePreConButton.Size = new System.Drawing.Size(55, 24);
            this.deletePreConButton.TabIndex = 4;
            this.deletePreConButton.Text = "Delete";
            this.deletePreConButton.UseVisualStyleBackColor = true;
            this.deletePreConButton.Click += new System.EventHandler(this.deletePreConButton_Click);
            // 
            // preConNameComboBox
            // 
            this.preConNameComboBox.FormattingEnabled = true;
            this.preConNameComboBox.Location = new System.Drawing.Point(6, 27);
            this.preConNameComboBox.Name = "preConNameComboBox";
            this.preConNameComboBox.Size = new System.Drawing.Size(314, 21);
            this.preConNameComboBox.TabIndex = 0;
            this.preConNameComboBox.SelectedIndexChanged += new System.EventHandler(this.preConNameComboBox_SelectedIndexChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.rulesDataGridView);
            this.groupBox2.Location = new System.Drawing.Point(6, 86);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(550, 317);
            this.groupBox2.TabIndex = 21;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Delimited File Columns Mapping";
            // 
            // rulesDataGridView
            // 
            this.rulesDataGridView.AllowUserToAddRows = false;
            this.rulesDataGridView.AllowUserToDeleteRows = false;
            this.rulesDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.rulesDataGridView.AutoGenerateColumns = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.rulesDataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.rulesDataGridView.ColumnHeadersHeight = 22;
            this.rulesDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.mappingRuleColumn,
            this.mappingFieldColumn,
            this.mappingColumnColumn,
            this.mappingValueColumn,
            this.mappingCultureColumn});
            this.rulesDataGridView.DataSource = this.mappingTableBindingSource;
            this.rulesDataGridView.Location = new System.Drawing.Point(7, 19);
            this.rulesDataGridView.Name = "rulesDataGridView";
            this.rulesDataGridView.RowHeadersVisible = false;
            this.rulesDataGridView.RowHeadersWidth = 16;
            this.rulesDataGridView.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rulesDataGridView.RowTemplate.Height = 20;
            this.rulesDataGridView.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.rulesDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.rulesDataGridView.Size = new System.Drawing.Size(537, 292);
            this.rulesDataGridView.TabIndex = 5;
            this.rulesDataGridView.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.rulesDataGridView_EditingControlShowing);
            this.rulesDataGridView.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.rulesDataGridView_DataError);
            // 
            // mappingRuleColumn
            // 
            this.mappingRuleColumn.DataPropertyName = "Rule";
            this.mappingRuleColumn.HeaderText = "Rule";
            this.mappingRuleColumn.Name = "mappingRuleColumn";
            this.mappingRuleColumn.ReadOnly = true;
            this.mappingRuleColumn.Visible = false;
            // 
            // mappingFieldColumn
            // 
            this.mappingFieldColumn.DataPropertyName = "Field";
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.Thistle;
            this.mappingFieldColumn.DefaultCellStyle = dataGridViewCellStyle2;
            this.mappingFieldColumn.HeaderText = "Data Field";
            this.mappingFieldColumn.Name = "mappingFieldColumn";
            this.mappingFieldColumn.ReadOnly = true;
            this.mappingFieldColumn.Width = 170;
            // 
            // mappingColumnColumn
            // 
            this.mappingColumnColumn.DataPropertyName = "Column";
            this.mappingColumnColumn.DataSource = this.columnTableBindingSource;
            this.mappingColumnColumn.DisplayMember = "ColumnName";
            this.mappingColumnColumn.HeaderText = "Column in File";
            this.mappingColumnColumn.Name = "mappingColumnColumn";
            this.mappingColumnColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.mappingColumnColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.mappingColumnColumn.ValueMember = "Column";
            this.mappingColumnColumn.Width = 170;
            // 
            // columnTableBindingSource
            // 
            this.columnTableBindingSource.DataMember = "ColumnTable";
            this.columnTableBindingSource.DataSource = this.parsingSetBindingSource;
            // 
            // parsingSetBindingSource
            // 
            this.parsingSetBindingSource.DataSource = this.parsingSet;
            this.parsingSetBindingSource.Position = 0;
            // 
            // parsingSet
            // 
            this.parsingSet.DataSetName = "ParsingSet";
            this.parsingSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // mappingValueColumn
            // 
            this.mappingValueColumn.DataPropertyName = "Value";
            this.mappingValueColumn.HeaderText = "Fixed Value";
            this.mappingValueColumn.Name = "mappingValueColumn";
            // 
            // mappingCultureColumn
            // 
            this.mappingCultureColumn.DataPropertyName = "Culture";
            this.mappingCultureColumn.HeaderText = "Culture";
            this.mappingCultureColumn.Name = "mappingCultureColumn";
            this.mappingCultureColumn.Width = 70;
            // 
            // mappingTableBindingSource
            // 
            this.mappingTableBindingSource.DataMember = "MappingTable";
            this.mappingTableBindingSource.DataSource = this.configSet;
            // 
            // configSet
            // 
            this.configSet.DataSetName = "ConfigSet";
            this.configSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // csvContentGroupBox
            // 
            this.csvContentGroupBox.Controls.Add(this.optionConCheckBox);
            this.csvContentGroupBox.Controls.Add(this.underlyingConCheckBox);
            this.csvContentGroupBox.Location = new System.Drawing.Point(6, 10);
            this.csvContentGroupBox.Name = "csvContentGroupBox";
            this.csvContentGroupBox.Size = new System.Drawing.Size(157, 70);
            this.csvContentGroupBox.TabIndex = 20;
            this.csvContentGroupBox.TabStop = false;
            this.csvContentGroupBox.Text = "Delimited File Content";
            // 
            // optionConCheckBox
            // 
            this.optionConCheckBox.AutoSize = true;
            this.optionConCheckBox.Checked = true;
            this.optionConCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.optionConCheckBox.Location = new System.Drawing.Point(7, 43);
            this.optionConCheckBox.Name = "optionConCheckBox";
            this.optionConCheckBox.Size = new System.Drawing.Size(87, 17);
            this.optionConCheckBox.TabIndex = 6;
            this.optionConCheckBox.Text = "&Option-Chain";
            this.optionConCheckBox.UseVisualStyleBackColor = true;
            this.optionConCheckBox.CheckedChanged += new System.EventHandler(this.xxxConCheckBox_CheckedChanged);
            // 
            // underlyingConCheckBox
            // 
            this.underlyingConCheckBox.AutoSize = true;
            this.underlyingConCheckBox.Checked = true;
            this.underlyingConCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.underlyingConCheckBox.Location = new System.Drawing.Point(7, 20);
            this.underlyingConCheckBox.Name = "underlyingConCheckBox";
            this.underlyingConCheckBox.Size = new System.Drawing.Size(102, 17);
            this.underlyingConCheckBox.TabIndex = 5;
            this.underlyingConCheckBox.Text = "&Underlying Data";
            this.underlyingConCheckBox.UseVisualStyleBackColor = true;
            this.underlyingConCheckBox.CheckedChanged += new System.EventHandler(this.xxxConCheckBox_CheckedChanged);
            // 
            // backButton
            // 
            this.backButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.backButton.Enabled = false;
            this.backButton.Location = new System.Drawing.Point(374, 418);
            this.backButton.Name = "backButton";
            this.backButton.Size = new System.Drawing.Size(90, 24);
            this.backButton.TabIndex = 9;
            this.backButton.Text = "<< Back";
            this.backButton.UseVisualStyleBackColor = true;
            this.backButton.Click += new System.EventHandler(this.xxxButton_Click);
            // 
            // nextButton
            // 
            this.nextButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.nextButton.Location = new System.Drawing.Point(470, 418);
            this.nextButton.Name = "nextButton";
            this.nextButton.Size = new System.Drawing.Size(90, 24);
            this.nextButton.TabIndex = 8;
            this.nextButton.Text = "Next >>";
            this.nextButton.UseVisualStyleBackColor = true;
            this.nextButton.Visible = false;
            this.nextButton.Click += new System.EventHandler(this.xxxButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(278, 418);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(90, 24);
            this.cancelButton.TabIndex = 10;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // doneButton
            // 
            this.doneButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.doneButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.doneButton.Location = new System.Drawing.Point(470, 418);
            this.doneButton.Name = "doneButton";
            this.doneButton.Size = new System.Drawing.Size(90, 24);
            this.doneButton.TabIndex = 11;
            this.doneButton.Text = "Finish";
            this.doneButton.UseVisualStyleBackColor = true;
            this.doneButton.Click += new System.EventHandler(this.doneButton_Click);
            // 
            // ImportForm
            // 
            this.AcceptButton = this.doneButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(582, 454);
            this.Controls.Add(this.doneButton);
            this.Controls.Add(this.backButton);
            this.Controls.Add(this.nextButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.tabControl);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ImportForm";
            this.Text = "Import...";
            this.Load += new System.EventHandler(this.ImportForm_Load);
            this.tabControl.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.delimitersGroupBox.ResumeLayout(false);
            this.delimitersGroupBox.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.rulesDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.columnTableBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.parsingSetBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.parsingSet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mappingTableBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.configSet)).EndInit();
            this.csvContentGroupBox.ResumeLayout(false);
            this.csvContentGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button backButton;
        private System.Windows.Forms.Button nextButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton csvFileTypeRadioButton;
        private System.Windows.Forms.RadioButton opdFileTypeRadioButton;
        private System.Windows.Forms.RadioButton opoFileTypeRadioButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListView fileListView;
        private System.Windows.Forms.GroupBox delimitersGroupBox;
        private System.Windows.Forms.CheckBox tabDelCheckBox;
        private System.Windows.Forms.TextBox otherDelTextBox;
        private System.Windows.Forms.CheckBox otherDelCheckBox;
        private System.Windows.Forms.CheckBox spaceDelCheckBox;
        private System.Windows.Forms.CheckBox commaDelCheckBox;
        private System.Windows.Forms.CheckBox semicolonDelCheckBox;
        private System.Windows.Forms.Button doneButton;
        private System.Windows.Forms.GroupBox csvContentGroupBox;
        private System.Windows.Forms.CheckBox optionConCheckBox;
        private System.Windows.Forms.CheckBox underlyingConCheckBox;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ComboBox preConNameComboBox;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button deletePreConButton;
        private OptionsOracle.DataCenter.Data.ConfigSet configSet;
        private System.Windows.Forms.DataGridView rulesDataGridView;
        private System.Windows.Forms.Button newPreConButton;
        private OptionsOracle.DataCenter.Data.ParsingSet parsingSet;
        private System.Windows.Forms.BindingSource parsingSetBindingSource;
        private System.Windows.Forms.BindingSource mappingTableBindingSource;
        private System.Windows.Forms.BindingSource columnTableBindingSource;
        private System.Windows.Forms.DataGridViewTextBoxColumn mappingRuleColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn mappingFieldColumn;
        private System.Windows.Forms.DataGridViewComboBoxColumn mappingColumnColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn mappingValueColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn mappingCultureColumn;

    }
}