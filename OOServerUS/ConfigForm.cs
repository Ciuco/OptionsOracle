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

namespace OOServerUS
{
    public partial class ConfigForm : Form
    {
        public string QuoteDataSource
        {
            get { return quoteDataSourceComboBox.SelectedItem.ToString(); }
            set { quoteDataSourceComboBox.SelectedItem = value; }
        }

        public string StockOptionChainDataSource
        {
            get { return stockOptionChainDataSourceComboBox.SelectedItem.ToString(); }
            set { stockOptionChainDataSourceComboBox.SelectedItem = value; }
        }

        public string IndexOptionChainDataSource
        {
            get { return indexOptionChainDataSourceComboBox.SelectedItem.ToString(); }
            set { indexOptionChainDataSourceComboBox.SelectedItem = value; }
        }

        public string FundOptionChainDataSource
        {
            get { return fundOptionChainDataSourceComboBox.SelectedItem.ToString(); }
            set { fundOptionChainDataSourceComboBox.SelectedItem = value; }
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
    }
}