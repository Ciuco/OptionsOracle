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
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace OOServerInteractiveBrokers
{
    public partial class OptionsFilterForm : Form
    {
        private MappingSet mapping = null;

        public OptionsFilterForm(MappingSet mappingSet)
        {
            // local mapping set for list of strikes/expirations
            mapping = mappingSet;

            InitializeComponent();

            // link expiration and strike lists to data source            
            typeListBox.DisplayMember = "TypeString";
            mapping.TypeTable.DefaultView.Sort = "Type";
            typeListBox.DataSource = mapping.TypeTable;

            
            strikeListBox.DisplayMember = "StrikeString";
            mapping.StrikeTable.DefaultView.Sort = "Strike";
            strikeListBox.DataSource = mapping.StrikeTable;

            expirationListBox.DisplayMember = "ExpirationString";
            mapping.ExpirationTable.DefaultView.Sort = "Expiration";
            expirationListBox.DataSource = mapping.ExpirationTable;

            for (int i = 0; i < typeListBox.Items.Count; i++) typeListBox.SetSelected(i, true);
            for (int i = 0; i < strikeListBox.Items.Count; i++) strikeListBox.SetSelected(i, true);
            for (int i = 0; i < expirationListBox.Items.Count; i++) expirationListBox.SetSelected(i, true);
        }

        private void xxxButton_Click(object sender, EventArgs e)
        {
            Button bt = (Button)sender;

            if (bt == typeNoneButton || bt == typeAllButton)
            {
                for (int i = 0; i < typeListBox.Items.Count; i++) typeListBox.SetSelected(i, bt == typeAllButton);
            }
            else if (bt == strikeNoneButton || bt == strikeAllButton)
            {
                for (int i = 0; i < strikeListBox.Items.Count; i++) strikeListBox.SetSelected(i, bt == strikeAllButton);
            }
            else if (bt == expirationNoneButton || bt == expirationAllButton)
            {
                for (int i = 0; i < expirationListBox.Items.Count; i++) expirationListBox.SetSelected(i, bt == expirationAllButton);
            }
        }

        private bool IsAllSelected(ListBox lsb)
        {
            return (lsb.SelectedItems.Count == lsb.Items.Count);
        }

        private bool IsNoneSelected(ListBox lsb)
        {
            return (lsb.SelectedItems.Count == 0);
        }

        private ArrayList GetPartialItems(ListBox lsb)
        {
            ArrayList part_list = new ArrayList();
            if (IsNoneSelected(lsb)) return part_list;

            foreach (object item in lsb.SelectedItems)
            {
                if (item.GetType().ToString() == "System.Data.DataRowView")
                    part_list.Add(((System.Data.DataRowView)item).Row[0]);
                else
                    part_list.Add(item.ToString());
            }

            return part_list;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            ArrayList list;

            list = GetPartialItems(typeListBox);
            foreach (MappingSet.TypeTableRow row in mapping.TypeTable)
            {
                if (list.Contains(row.TypeString)) row.Enabled = true;
            }

            list = GetPartialItems(strikeListBox);
            foreach (MappingSet.StrikeTableRow row in mapping.StrikeTable)
            {
                if (list.Contains(row.Strike)) row.Enabled = true;
            }

            list = GetPartialItems(expirationListBox);
            foreach (MappingSet.ExpirationTableRow row in mapping.ExpirationTable)
            {
                if (list.Contains(row.Expiration)) row.Enabled = true;
            }

            mapping.AcceptChanges();
        }
    }
}