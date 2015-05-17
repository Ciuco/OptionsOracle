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

namespace OptionsOracle.DataCenter
{
    public partial class PreConfigCtrlForm : Form
    {
        public bool ReadOnly
        {
            set { preConNameText.ReadOnly = value; }
        }

        public string Operation
        {
            set { okButton.Text = value; }
        }

        public string Value
        {
            get { return preConNameText.Text; }
        }

        public PreConfigCtrlForm()
        {
            InitializeComponent();
        }

        private void preConNameText_TextChanged(object sender, EventArgs e)
        {
            okButton.Enabled = preConNameText.Text != "";
        }
    }
}