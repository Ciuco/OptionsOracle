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
using System.Windows.Forms;
using System.Threading;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Messaging;
using OptionsOracle.DataCenter.Data;
using OptionsOracle.DataCenter.Remote;

namespace OptionsOracle.DataCenter
{
    public partial class MainForm : Form
    {
        // delegates
        private delegate bool LoadDelegate(string filename, string rule, MarketSet data);
        private delegate void SetProgressDelegate(int progress);

        // local variables
        private string   underlying = "";
        private string   last_filename = "";
        private WaitForm wait = null;

        private string SelectedUnderlying
        {
            get 
            { 
                return underlying; 
            }

            set
            {
                if (!string.IsNullOrEmpty(value))
                    underlying = value;
                else
                    underlying = "";

                optionTableBindingSource.Filter = "Underlying = '" + underlying + "'";

                optionChainDataGridView.ClearSelection();
                optionChainDataGridView.Refresh();
            }
        }

        public MainForm()
        {
            InitializeComponent();
            wait = new WaitForm(this);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            serverWorker.RunWorkerAsync();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // get my-documents directory path
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\OptionsOracle\";
            string filename = null;

            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.InitialDirectory = path;
            openFileDialog.Filter = @"OptionsOracle DataCenter files (*.opd)|*.opd|OptionsOracle files (*.opo)|*.opo|csv files (*.csv)|*.csv|All files (*.*)|*.*";

            try
            {
                openFileDialog.FilterIndex = int.Parse(Config.Local.GetParameter(ConfigSet.PRM_DEFAULT_IMPORT_FILTER_INDEX));
                openFileDialog.FileName = Config.Local.GetParameter(ConfigSet.PRM_DEFAULT_IMPORT_FILENAME);
                
                if (openFileDialog.FilterIndex == 1) 
                    openFileDialog.FileName = openFileDialog.FileName.Replace(".opd", "");
            }
            catch { }

            openFileDialog.RestoreDirectory = true;
            openFileDialog.AddExtension = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK) filename = openFileDialog.FileName;

            if (filename != null)
            {
                ImportForm importForm = new ImportForm();
                importForm.FileName = filename;
                
                // save default import filter
                Config.Local.SetParameter(ConfigSet.PRM_DEFAULT_IMPORT_FILTER_INDEX, openFileDialog.FilterIndex.ToString());

                // save default filename
                Config.Local.SetParameter(ConfigSet.PRM_DEFAULT_IMPORT_FILENAME, filename);
                
                if (importForm.ShowDialog() == DialogResult.OK)
                {
                    switch (importForm.FileFormat)
                    {
                        case Core.FileFormat.CsvFile:
                            CsvLoad(filename, importForm.CsvParsingRule, marketSet);
                            break;
                        case Core.FileFormat.OpoFile:
                            OpoLoad(filename, marketSet); 
                            break;
                        case Core.FileFormat.OpdFile:
                            OpdLoad(filename, marketSet);
                            last_filename = filename;
                            break;
                    }
                }
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // get my-documents directory path
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\OptionsOracle\";
            string filename = null;

            if (string.IsNullOrEmpty(last_filename) || (ToolStripMenuItem)sender == saveAsToolStripMenuItem)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();

                saveFileDialog.InitialDirectory = path;
                saveFileDialog.Filter = @"opd files (*.opd)|*.opd|All files (*.*)|*.*";
                saveFileDialog.FilterIndex = 1;
                saveFileDialog.RestoreDirectory = true;
                saveFileDialog.AddExtension = true;

                if (last_filename != null) saveFileDialog.FileName = last_filename;
                else saveFileDialog.FileName = "";

                if (saveFileDialog.FilterIndex == 1) 
                    saveFileDialog.FileName = saveFileDialog.FileName.Replace(".opd", "");

                if (saveFileDialog.ShowDialog() == DialogResult.OK) 
                    filename = saveFileDialog.FileName;
            }

            if (filename != null)
            {
                try
                {
                    marketSet.WriteXml(filename);
                    last_filename = filename;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error! Could not save file", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }

        public void SetProgress(int progress)
        {
            if (this.InvokeRequired)
            {
                SetProgressDelegate d = new SetProgressDelegate(SetProgress);
                this.Invoke(d, new object[] { progress });
            }
            else
            {
                if (progress == 100)
                {
                    wait.Hide();
                    Cursor = quoteDataGridView.Cursor = optionChainDataGridView.Cursor = System.Windows.Forms.Cursors.Default;
                    quoteDataGridView.DataSource = marketSet.QuoteTable;
                }
                else if (progress == 0)
                {
                    wait.Show("Please wait while loading data...");
                    quoteDataGridView.DataSource = null;
                    optionChainDataGridView.DataSource = null;
                    Cursor = quoteDataGridView.Cursor = optionChainDataGridView.Cursor = System.Windows.Forms.Cursors.WaitCursor;
                }
            }
        }

        public void CsvLoad(string filename, string rule, MarketSet data)
        {
            // set progress
            SetProgress(0);

            // delegate async csv load
            LoadDelegate ld = new LoadDelegate(Core.Csv.Load);
            ld.BeginInvoke(filename, rule, data, new AsyncCallback(this.LoadComplete), null);            
        }

        public void OpoLoad(string filename, MarketSet data)
        {
            // set progress
            SetProgress(0);

            // delegate async csv load
            LoadDelegate ld = new LoadDelegate(Core.Opo.Load);
            ld.BeginInvoke(filename, null, data, new AsyncCallback(this.LoadComplete), null);
        }

        public void OpdLoad(string filename, MarketSet data)
        {
            // set progress
            SetProgress(0);

            // delegate async csv load
            LoadDelegate ld = new LoadDelegate(Core.Opd.Load);
            ld.BeginInvoke(filename, null, data, new AsyncCallback(this.LoadComplete), null);
        }

        public void LoadComplete(IAsyncResult ar)
        {
            // extract the delegate from the AsyncResult, and obtain result
            LoadDelegate ld = (LoadDelegate)((AsyncResult)ar).AsyncDelegate;
            ld.EndInvoke(ar);

            // set progress
            SetProgress(100);
        }

        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            WindowState = FormWindowState.Normal;
            ShowInTaskbar = true;
            notifyIcon.Visible = false;
        }

        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {                
                notifyIcon.Visible = true;
                ShowInTaskbar = false;
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show(this, "Exit from OptionsOracle DataCenter?", "Exit?", MessageBoxButtons.YesNo) == DialogResult.No) e.Cancel = true;
        }

        private void notifyExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void notifyOpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            notifyIcon_MouseDoubleClick(null, null);
        }

        private void quoteDataGridView_SelectionChanged(object sender, EventArgs e)
        {
            if (quoteDataGridView.SelectedCells.Count > 0)
            {
                object und = quoteDataGridView.Rows[quoteDataGridView.SelectedCells[0].RowIndex].Cells[0].Value;
                if (und != null) SelectedUnderlying = und.ToString();
                else SelectedUnderlying = null;
            }
            else SelectedUnderlying = null;
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            marketSet.Clear();
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            // current version
            string current_version = Config.Local.CurrentVersion;

            // check latest version
            string lastest_version = Config.Remote.GetRemoteGlobalVersion("lastest");

            if (lastest_version != null && current_version != null)
            {
                string lastest_version_checked = Config.Local.GetParameter("Last Version Check");
                if (lastest_version_checked == null || lastest_version_checked == "")
                {
                    lastest_version_checked = Config.Local.CurrentVersion;
                    Config.Local.SetParameter("Last Version Check", lastest_version);
                    Config.Local.Save();
                }

                if (RemoteConfig.CompareVersions(lastest_version, lastest_version_checked) == 1)
                {
                    // keep new version in config file
                    Config.Local.SetParameter("Last Version Check", lastest_version);
                    Config.Local.Save();

                    if (RemoteConfig.CompareVersions(lastest_version, Config.Local.CurrentVersion) == 1)
                    {
                        string message = "A newer version (" + lastest_version + ") of OptionsOracle is available. Download?       ";
                        string caption = "New Version is Available";

                        if (MessageBox.Show(message, caption, MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                        {
                            Global.OpenExternalBrowser(Config.Remote.GetRemoteGlobalUrl("download"));
                            Close();
                        }
                    }
                }
            }
        }

        private void serverWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            new Server().Start(marketSet);
        }

        private void serverWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            notifyIcon.ShowBalloonTip(1000, "Error!", "Server Disconnected", ToolTipIcon.Error);
        }

        private void symbolListStripMenuItem_Click(object sender, EventArgs e)
        {
            ListForm listForm = new ListForm();
            listForm.Text = "Symbol List";
            listForm.List = marketSet.QuoteTable.GetSymbolList();
            listForm.ShowDialog();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutForm aboutBox = new AboutForm();
            aboutBox.ShowDialog();
        }
    }
}