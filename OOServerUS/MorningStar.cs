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
using System.Text;
using System.Globalization;
using System.Xml;
using System.Diagnostics;
using System.Collections;
using System.Threading;
using System.IO;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;

using Microsoft.Win32;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Security;

using OOServerLib.Web;
using OOServerLib.Interface;
using OOServerLib.Global;
using OOServerLib.Config;

namespace OOServerUS
{
    public class MorningStar
    {
        // web capture client
        private WebCapture cap = null;

        // parse html page
        private XmlParser prs = new XmlParser();

        // culture info
        private CultureInfo ci = new CultureInfo("en-US", false);

        public MorningStar(WebCapture cap)
        {
            this.cap = cap;
        }

        private string GetJavaScriptVariable(string text, string varname)
        {
            int i1, i2;

            try
            {
                i1 = text.IndexOf(varname);

                if (i1 >= 0)
                {
                    i1 = text.IndexOf("\"", i1);
                    if (i1 >= 0)
                    {
                        i2 = text.IndexOf("\"", i1 + 1);
                        if (i1 >= 0 && i2 > i1)
                        {
                            return text.Substring(i1 + 1, i2 - i1 - 1).Trim().TrimEnd(new char[] { '\"' }).TrimStart(new char[] { '\"' }).Trim();
                        }
                    }
                }
            }
            catch { }

            return null;
        }

        // get stock latest options chain
        public ArrayList GetOptionsChain(string ticker, Symbol.Type type)
        {
            // create options array list
            ArrayList options_list = new ArrayList();
            options_list.Clear();
            options_list.Capacity = 1024;

            string symbol = ticker.TrimStart(new char[] { '^', '~' }).ToLower();
            string url = @"http://quote.morningstar.com/Option/Options.aspx?Ticker=" + symbol + @"&chnExpireDate[0]=";

            string page = cap.DownloadHtmlWebPage(url);

            string exp_list = GetJavaScriptVariable(page, "strExpireDateShort");
            string opt_list = GetJavaScriptVariable(page, "strOptions");
            if (exp_list == null || opt_list == null) return null;

            string[] opt_list_split = opt_list.Split('~');
            string[] exp_list_split = exp_list.Split(',');

            for (int i = 0; i < exp_list_split.Length && i < opt_list_split.Length; i++)
            {
                string exp_str = exp_list_split[i].Trim();
                string opt_str = opt_list_split[i].Trim();

                if (exp_str == "" || opt_str == "") continue;

                try
                {
                    int ed_d = int.Parse(exp_str.Substring(2, 2), ci);
                    int ed_m = int.Parse(exp_str.Substring(0, 2), ci);
                    int ed_y = int.Parse(exp_str.Substring(4, 4), ci);

                    DateTime exp_date = new DateTime(ed_y, ed_m, ed_d);

                    foreach (string opt_line in opt_str.Split('|'))
                    {
                        string[] split = opt_line.Trim().Split(',');

                        for (int j = 0; j < 2; j++)
                        {
                            int ofst = j * 10;

                            try
                            {
                                Option option = new Option();

                                option.expiration = exp_date;
                                if (!double.TryParse(split[0], NumberStyles.Number, ci, out option.strike)) continue;

                                option.stock = ticker;
                                option.stocks_per_contract = 100;
                                option.type = (j == 0) ? "Call" : "Put";
                                option.update_timestamp = DateTime.Now;

                                option.symbol = "." + split[2 + ofst];

                                if (!double.TryParse(split[3 + ofst], NumberStyles.Number, ci, out option.price.last))
                                    option.price.last = double.NaN;

                                if (!double.TryParse(split[4 + ofst], NumberStyles.Number, ci, out option.price.bid))
                                    option.price.bid = double.NaN;

                                if (!double.TryParse(split[5 + ofst], NumberStyles.Number, ci, out option.price.ask))
                                    option.price.ask = double.NaN;

                                if (!double.TryParse(split[8 + ofst], NumberStyles.Number, ci, out option.volume.total))
                                    option.volume.total = double.NaN;

                                if (!int.TryParse(split[9 + ofst], NumberStyles.Number, ci, out option.open_int))
                                    option.open_int = (int)option.volume.total;

                                options_list.Add(option);
                            }
                            catch { }
                        }
                    }
                }
                catch { }
            }

            return options_list;
        }
    }
}
