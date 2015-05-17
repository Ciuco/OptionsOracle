/*
 * OptionsOracle DataCenter
 * Copyright 2006-2012 SamoaSky
 * 
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 * 
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.IO;
using System.Windows.Forms;
using OptionsOracle.DataCenter.Data;

namespace OptionsOracle.DataCenter
{
    public partial class ImportForm : Form
    {
        private string filename = "";
        private Core.FileFormat fileform = Core.FileFormat.OpoFile;
        private List<char> delimiters = new List<char>();

        public string FileName
        {
            get { return filename; }
            set { filename = value; }
        }

        public Core.FileFormat FileFormat
        {
            get { return fileform; }
        }

        public string CsvParsingRule
        {
            get { if (preConNameComboBox.SelectedIndex != -1) return preConNameComboBox.SelectedItem.ToString(); else return null; }
        }

        private string CsvDelimiters
        {
            get
            {
                string del = "";
                foreach (char c in delimiters) del += c.ToString();
                return del;
            }
            set
            {
                delimiters.Clear();

                foreach (char c in value.ToCharArray())
                {
                    switch (c)
                    {
                        case '\t':
                            tabDelCheckBox.Checked = true;
                            break;
                        case ';':
                            semicolonDelCheckBox.Checked = true;
                            break;
                        case ',':
                            commaDelCheckBox.Checked = true;
                            break;
                        case ' ':
                            spaceDelCheckBox.Checked = true;
                            break;
                        default:
                            otherDelCheckBox.Checked = true;
                            otherDelTextBox.Text = c.ToString();
                            break;
                    }
                }
            }
        }

        public ImportForm()
        {
            InitializeComponent();
        }

        private void ImportForm_Load(object sender, EventArgs e)
        {
            string rule = Config.Local.GetParameter(ConfigSet.PRM_LAST_SELECTED_RULE);
            preConNameComboBox_UpdateList(rule);

            ConfigSet.ParsingTableRow rul = Config.Local.ParsingTable.FindByRule(rule);
            if (rul != null)
            {
                underlyingConCheckBox.Checked = rul.IncludesUnderlying;
                optionConCheckBox.Checked = rul.IncludesOptionsChain;
                CsvDelimiters = rul.Delimiters;
            }

            if (filename.EndsWith(".opo"))
                opoFileTypeRadioButton.Checked = true;
            else if (filename.EndsWith(".opd"))
                opdFileTypeRadioButton.Checked = true;
            else
                csvFileTypeRadioButton.Checked = true;
        }

        private void preConNameComboBox_UpdateList(string select_item)
        {
            int j = 0;

            // update pre-configuration combo-box
            List<string> list = Config.Local.DelimitingRuleList;

            preConNameComboBox.Items.Clear();
            for (int i = 0; i < list.Count; i++)
            {
                preConNameComboBox.Items.Add(list[i]);
                if (!string.IsNullOrEmpty(select_item) && select_item == list[i]) j = i;
            }

            if (preConNameComboBox.Items.Count > 0)
                preConNameComboBox.SelectedIndex = j;
        }

        private void xxxButton_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;

            if (button == nextButton) tabControl.SelectedIndex++;
            else if (button == backButton) tabControl.SelectedIndex--;
        }

        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl.SelectedIndex == 0)
            {
                tabControl.SelectedIndex = 0;
                doneButton.Visible = (fileform != Core.FileFormat.CsvFile);
                nextButton.Visible = !doneButton.Visible;
                backButton.Enabled = false;

            }
            else if (tabControl.SelectedIndex == 1)
            {
                doneButton.Visible = true;
                nextButton.Visible = !doneButton.Visible;
                backButton.Enabled = true;
            }
        }

        private void xxxFileTypeRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (opoFileTypeRadioButton.Checked)
            {
                fileform = Core.FileFormat.OpoFile;
                fileListView_Load(null);
            }
            else if (opdFileTypeRadioButton.Checked)
            {
                fileform = Core.FileFormat.OpdFile;
                fileListView_Load(null);
            }
            else if (csvFileTypeRadioButton.Checked)
            {
                fileform = Core.FileFormat.CsvFile;
                fileListView_Load(delimiters);
            }

            delimitersGroupBox.Enabled = (fileform == Core.FileFormat.CsvFile);
            tabControl_SelectedIndexChanged(null, null); // refresh back/next buttons
        }

        private void xxxDelCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            otherDelTextBox.Enabled = otherDelCheckBox.Checked;

            // create seperator list
            delimiters.Clear();
            if (tabDelCheckBox.Checked) delimiters.Add('\t');
            if (semicolonDelCheckBox.Checked) delimiters.Add(';');
            if (commaDelCheckBox.Checked) delimiters.Add(',');
            if (spaceDelCheckBox.Checked) delimiters.Add(' ');
            if (otherDelCheckBox.Checked && otherDelTextBox.Text.Length > 0) delimiters.Add(otherDelTextBox.Text[0]);

            // update file-list view
            if (csvFileTypeRadioButton.Checked) fileListView_Load(delimiters);
        }

        private void otherDelTextBox_TextChanged(object sender, EventArgs e)
        {
            xxxDelCheckBox_CheckedChanged(otherDelCheckBox, e);
        }

        private void fileListView_Load(List<char> delimiters)
        {
            fileListView.Clear();
            fileListView.Columns.Add("Id", 35, HorizontalAlignment.Right);

            // clear header line
            string[] hdr_split = null;

            try
            {
                FileStream stream = new FileStream(filename, FileMode.Open);
                StreamReader reader = new StreamReader(stream);

                try
                {
                    for (int i = 0; i < 64 && !reader.EndOfStream; i++)
                    {
                        string s = reader.ReadLine();
                        if (s == null) break;

                        fileListView.Items.Add(i.ToString());

                        string[] split;
                        if (delimiters == null || delimiters.Count == 0)
                            split = new string[] { s };
                        else
                            split = s.Split(delimiters.ToArray());

                        // keep the first line for column parsing
                        if (hdr_split == null) hdr_split = split;

                        for (int j = 0; j < split.Length; j++)
                        {
                            if (fileListView.Columns.Count < j + 2)
                            {
                                fileListView.Columns.Add("Data." + j.ToString(), 120, HorizontalAlignment.Left);
                            }

                            fileListView.Items[i].SubItems.Add(split[j]);
                        }
                    }

                    // auto-resize columns           
                    for (int j = 0; j < fileListView.Columns.Count; j++)
                    {
                        fileListView.Columns[j].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
                    }
                }
                catch { }

                reader.Close();
                stream.Close();
            }
            catch { }

            // update parsing-set with columns
            parsingSet.Clear();
            parsingSet.ColumnTable.AddColumnTableRow(-1, "Not-Set");
            parsingSet.ColumnTable.AddColumnTableRow(0, "Fixed-Value");

            int n = 0;

            if (hdr_split != null)
            {
                for (int k = 1; k <= hdr_split.Length; k++)
                {
                    parsingSet.ColumnTable.AddColumnTableRow(k, k.ToString() + " (" + hdr_split[k - 1].Trim() + ")");
                    n++;
                }
            }

            for (int l = n + 1; l < 16; l++ )
            {
                parsingSet.ColumnTable.AddColumnTableRow(l, l.ToString() + " (Invalid)");
            }
        }

        private void newPreConButton_Click(object sender, EventArgs e)
        {
            PreConfigCtrlForm ctrlForm = new PreConfigCtrlForm();

            ctrlForm.Name = "";
            ctrlForm.ReadOnly = false;
            ctrlForm.Operation = "Create";

            if (ctrlForm.ShowDialog() == DialogResult.OK && !string.IsNullOrEmpty(ctrlForm.Value))
                Config.Local.CreateDelimitingRule(ctrlForm.Value, CsvDelimiters);

            preConNameComboBox_UpdateList(ctrlForm.Value);
        }

        private void deletePreConButton_Click(object sender, EventArgs e)
        {
            PreConfigCtrlForm ctrlForm = new PreConfigCtrlForm();

            ctrlForm.Name = preConNameComboBox.SelectedItem.ToString();
            ctrlForm.ReadOnly = true;
            ctrlForm.Operation = "Delete";

            if (ctrlForm.ShowDialog() == DialogResult.OK)
            {
                Config.Local.DeleteDelimitingRule(ctrlForm.Name);
                
                // remove rule from combo-box
                preConNameComboBox.Items.Remove(preConNameComboBox.SelectedItem);
                if (preConNameComboBox.Items.Count > 0) preConNameComboBox.SelectedIndex = 0;
                else
                {
                    preConNameComboBox.SelectedIndex = -1;
                    preConNameComboBox.Text = "";
                }
            }
            preConNameComboBox_UpdateList(null);
        }

        private void preConNameComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            rulesDataGridView_UpdateDataSet(sender, e);
        }

        private void xxxConCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (preConNameComboBox.SelectedIndex != -1)
            {
                string rule = preConNameComboBox.SelectedItem.ToString();

                ConfigSet.ParsingTableRow rul = Config.Local.ParsingTable.FindByRule(rule);
                if (rul != null)
                {
                    rul.IncludesUnderlying = underlyingConCheckBox.Checked;
                    rul.IncludesOptionsChain = optionConCheckBox.Checked;
                }

                rulesDataGridView_UpdateDataSet(sender, e);
            }
        }

        private void rulesDataGridView_UpdateDataSet(object sender, EventArgs e)
        {
            if (preConNameComboBox.SelectedIndex == -1)
            {
                underlyingConCheckBox.Checked = true;
                optionConCheckBox.Checked = true;

                rulesDataGridView.DataSource = null;
            }
            else
            {
                string rule = preConNameComboBox.SelectedItem.ToString();

                ConfigSet.ParsingTableRow rul = Config.Local.ParsingTable.FindByRule(rule);
                if (rul != null)
                {
                    underlyingConCheckBox.Checked = rul.IncludesUnderlying;
                    optionConCheckBox.Checked = rul.IncludesOptionsChain;
                }

                DataView view = new DataView(Config.Local.MappingTable);
                view.RowFilter = "(Rule = '" + rule + "')";
                if (underlyingConCheckBox.Checked && !optionConCheckBox.Checked)
                    view.RowFilter += " AND (Field LIKE 'Underlying.*')";
                else if (!underlyingConCheckBox.Checked && optionConCheckBox.Checked)
                    view.RowFilter += " AND (Field LIKE 'Option.*')";
                rulesDataGridView.DataSource = view;
            }
        }

        private void doneButton_Click(object sender, EventArgs e)
        {
            // save done button
            if (preConNameComboBox.SelectedIndex != -1)
            {
                Config.Local.SetParameter(ConfigSet.PRM_LAST_SELECTED_RULE, preConNameComboBox.SelectedItem.ToString());

                // save rule configuration
                ConfigSet.ParsingTableRow rul = Config.Local.ParsingTable.FindByRule(preConNameComboBox.SelectedItem.ToString());
                rul.IncludesUnderlying = underlyingConCheckBox.Checked;
                rul.IncludesOptionsChain = optionConCheckBox.Checked;
                rul.Delimiters = CsvDelimiters;
            }
            else
                Config.Local.SetParameter(ConfigSet.PRM_LAST_SELECTED_RULE, "");
            
            Config.Local.Save();
        }

        private void rulesDataGridView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            try
            {
                ComboBox cb = (ComboBox)e.Control;

                if (cb != null)
                {
                    // first remove event handler to keep from attaching multiple:                
                    cb.SelectionChangeCommitted -= new EventHandler(rulesDataGridView_SelectionChangeCommitted);

                    // now attach the event handler                
                    cb.SelectionChangeCommitted += new EventHandler(rulesDataGridView_SelectionChangeCommitted);
                }
            }
            catch { }
        }

        private void rulesDataGridView_SelectionChangeCommitted(object sender, EventArgs e)
        {
            try
            {
                ComboBox cb = (ComboBox)sender;

                if (cb != null)
                {
                    // first remove event handler to keep from attaching multiple:                
                    cb.SelectionChangeCommitted -= new EventHandler(rulesDataGridView_SelectionChangeCommitted);
                }
            }
            catch { }

            rulesDataGridView.EndEdit();
        }

        private void rulesDataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {

        }
    }
}