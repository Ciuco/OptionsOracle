/*
 * OptionsOracle Interface Class Library
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

namespace OOServerInteractiveBrokers
{
    public partial class ConfigForm : Form
    {
        public string ExchangeUnderlying
        {
            get { return exchangeUnderlyingComboBox.SelectedValue.ToString(); }
            set { exchangeUnderlyingComboBox.SelectedValue = value; }
        }

        public string ExchangeOption
        {
            get { return exchangeOptionComboBox.SelectedValue.ToString(); }
            set { exchangeOptionComboBox.SelectedValue = value; }
        }

        public string Currency
        {
            get { return currencyComboBox.SelectedValue.ToString(); }
            set { currencyComboBox.SelectedValue = value; }
        }

        public int QuoteTimeout
        {
            get { return (int)quoteTimeoutNumericUpDown.Value; }
            set { quoteTimeoutNumericUpDown.Value = value; }
        }

        public int BackOffTime
        {
            get { return (int)backoffTimeNumericUpDown.Value; }
            set { backoffTimeNumericUpDown.Value = value; }
        }

        public int MaxParallelTickers
        {
            get { return (int)maxParallelTickersNumericUpDown.Value; }
            set { maxParallelTickersNumericUpDown.Value = value; }
        }

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

        public bool DownloadOpenInt
        {
            get { return downloadOpenIntCheckBox.Checked; }
            set { downloadOpenIntCheckBox.Checked = value; }
        }

        public bool FilterNoPriceOptions
        {
            get { return filterNoPriceOptionsCheckBox.Checked; }
            set { filterNoPriceOptionsCheckBox.Checked = value; }
        }

        public int DownloadMode
        {
            get
            {
                if (downloadDynamicRadioButton.Checked) return 1;
                else if (downloadPredefinedRadioButton.Checked) return 2;
                else return 0;
            }
            set
            {
                if (value == 1) downloadDynamicRadioButton.Checked = true;
                else if (value == 2) downloadPredefinedRadioButton.Checked = true;
                else downloadAllRadioButton.Checked = true;
            }
        }

        public DateTime MaxExpirationLimit
        {
            get
            {
                if (enableExpDateFilterCheckBox.Checked) return maxExpDateFilterTimePicker.Value;
                else return DateTime.MaxValue;
            }

            set
            {
                if (value.Date < DateTime.MaxValue.Date)
                    maxExpDateFilterTimePicker.Value = value.Date;
                else
                    maxExpDateFilterTimePicker.Value = DateTime.Now.Date;

                enableExpDateFilterCheckBox.Checked = (maxExpDateFilterTimePicker.Value.Date != DateTime.Now.Date ||
                                                       minExpDateFilterTimePicker.Value.Date != DateTime.Now.Date);
            }
        }

        public DateTime MinExpirationLimit
        {
            get
            {
                if (enableExpDateFilterCheckBox.Checked) return minExpDateFilterTimePicker.Value;
                else return DateTime.MinValue;
            }

            set
            {
                if (value.Date > DateTime.MinValue.Date)
                    minExpDateFilterTimePicker.Value = value.Date;
                else
                    minExpDateFilterTimePicker.Value = DateTime.Now.Date;

                enableExpDateFilterCheckBox.Checked = (maxExpDateFilterTimePicker.Value.Date != DateTime.Now.Date ||
                                                       minExpDateFilterTimePicker.Value.Date != DateTime.Now.Date);
            }
        }

        public double MaxStrikeLimit
        {
            get
            {
                if (enableStrikeFilterCheckBox.Checked) return (double)maxStrikeFilterNumericUpDown.Tag;
                else return double.NaN;
            }

            set
            {
                if (!double.IsNaN(value)) 
                    maxStrikeFilterNumericUpDown.Value = (decimal)value;

                enableStrikeFilterCheckBox.Checked = (!double.IsNaN((double)maxStrikeFilterNumericUpDown.Tag) ||
                                                      !double.IsNaN((double)minStrikeFilterNumericUpDown.Tag));
            }
        }

        public double MinStrikeLimit
        {
            get
            {
                if (enableStrikeFilterCheckBox.Checked) return (double)minStrikeFilterNumericUpDown.Tag;
                else return double.NaN;
            }

            set
            {
                if (!double.IsNaN(value)) 
                    minStrikeFilterNumericUpDown.Value = (decimal)value;

                enableStrikeFilterCheckBox.Checked = (!double.IsNaN((double)maxStrikeFilterNumericUpDown.Tag) ||
                                                      !double.IsNaN((double)minStrikeFilterNumericUpDown.Tag));
            }
        }

        public ConfigForm()
        {
            InitializeComponent();
            
            minStrikeFilterNumericUpDown.Tag = double.NaN;
            maxStrikeFilterNumericUpDown.Tag = double.NaN;

            mappingSet.Load();
        }

        private void downloadHistoryFromYahooCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            yahooExchangeSuffixTextBox.Enabled = downloadHistoryFromYahooCheckBox.Checked;
        }

        private void aboutButton_Click(object sender, EventArgs e)
        {
            AboutBox aboutBox = new AboutBox();
            aboutBox.ShowDialog();
        }

        private void downloadXXXRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (!downloadPredefinedRadioButton.Checked)
            {
                enableExpDateFilterCheckBox.Checked = false;
                enableExpDateFilterCheckBox.Enabled = false;
                enableStrikeFilterCheckBox.Checked = false;
                enableStrikeFilterCheckBox.Enabled = false;
            }
            else
            {
                enableExpDateFilterCheckBox.Enabled = true;
                enableStrikeFilterCheckBox.Enabled = true;
            }
        }

        private void enableXXXFilterCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            minExpDateFilterTimePicker.Enabled = enableExpDateFilterCheckBox.Checked;
            maxExpDateFilterTimePicker.Enabled = enableExpDateFilterCheckBox.Checked;
            minStrikeFilterNumericUpDown.Enabled = enableStrikeFilterCheckBox.Checked;
            maxStrikeFilterNumericUpDown.Enabled = enableStrikeFilterCheckBox.Checked;
        }

        private void xxxStrikeFilterNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDown nud = (NumericUpDown)sender;
            nud.Tag = (double)nud.Value;
        }
    }
}