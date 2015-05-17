using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace OOServerTDAmeritrade
{
    public partial class ConfigForm : Form
    {
        public bool DownloadHistoryFromYahoo
        {
            get { return downloadHistoryFromYahooCheckBox.Checked; }
            set { downloadHistoryFromYahooCheckBox.Checked = value; }
        }

        public string YahooExchangeSuffix
        {
            get { return yahooExchangeSuffixTextBox.Text; }
            set { yahooExchangeSuffixTextBox.Text = value; }
        }

        public string DefaultUsername
        {
            get { return defaultUsernameTextBox.Text; }
            set { defaultUsernameTextBox.Text = value; }
        }

        public bool LoginOnStartUp
        {
            get { return loginOnStartUpCheckBox.Checked; }
            set { loginOnStartUpCheckBox.Checked = value; }
        }

        public ConfigForm()
        {
            InitializeComponent();
        }

        private void aboutButton_Click(object sender, EventArgs e)
        {
            AboutBox aboutBox = new AboutBox();
            aboutBox.ShowDialog();
        }

        private void downloadHistoryFromYahooCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            yahooExchangeSuffixTextBox.Enabled = downloadHistoryFromYahooCheckBox.Checked;
        }
    }
}