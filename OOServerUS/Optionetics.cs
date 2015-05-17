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
    public class Optionetics
    {
        // web capture client
        private WebCapture cap = null;
        private WebForm wbf = null;

        // parse html page
        private XmlParser prs = new XmlParser();

        // culture info
        private CultureInfo ci = new CultureInfo("en-US", false);

        public Optionetics(WebCapture cap, WebForm wbf)
        {
            this.cap = cap;
            this.wbf = wbf;
        }

#if (false)
 
#endif

        // get stock latest options chain
        public ArrayList GetOptionsChain(string ticker, Symbol.Type type)
        {
            // create options array list
            ArrayList options_list = new ArrayList();
            options_list.Clear();
            options_list.Capacity = 1024;

            string symbol = ticker.TrimStart(new char[] { '^', '~' }).ToLower();
            string url = @"http://www.optionetics.com/marketdata/details.asp?symb=" + symbol.ToUpper() + "&page=chain";

            //XmlDocument doc = cap.DownloadXmlPartialWebPage(url, "<body", "</body>", 1, 1);
            //if (doc == null) return null;

            HtmlDocument html = wbf.GetHtmlDocumentWithWebBrowser(url, null, null, null, 60); 
            if (html == null) return null;

            XmlDocument doc = cap.ConvertHtmlToXml("<html>" + cap.GetPartialWebPage(html.Body.OuterHtml, "Options Expiration:", "</div>", 1, 1) + "</html>");
            if (doc == null) return null;

            List<DateTime> exp_date_list = new List<DateTime>();
            List<XmlNode> table_nd_list = new List<XmlNode>();

            XmlNode nd, table_nd;

            for (int i = 1; ; i++)
            {
                table_nd = prs.FindXmlNodeByName(doc.FirstChild, "TABLE", "class=chaintable", i);

                if (table_nd == null) break;

                if (table_nd.PreviousSibling != null)
                {
                    DateTime exp_date;

                    if (DateTime.TryParse(System.Web.HttpUtility.HtmlDecode(table_nd.PreviousSibling.InnerText.Replace("Options Expiration:","").Trim()), out exp_date))
                    {
                        exp_date_list.Add(exp_date.AddDays(1));
                        table_nd_list.Add(table_nd);
                    }
                }
            }

            for (int e = 0; e < table_nd_list.Count; e++)
            {
                // expiration
                DateTime exp_date = exp_date_list[e];

                for (int r = 3; ; r++)
                {
                    XmlNode row_nd = prs.GetXmlNodeByPath(table_nd_list[e], @"TBODY\TR(" + r.ToString() + @")");
                    if (row_nd == null) break;

                    // strike
                    double strike;
                    nd = prs.GetXmlNodeByPath(row_nd, @"TD(7)");
                    if (nd == null || nd.InnerText == null ||
                        !double.TryParse(System.Web.HttpUtility.HtmlDecode(nd.InnerText.Trim()), out strike)) continue;

                    for (int i = 0; i < 2; i++)
                    {
                        try
                        {
                            Option option = new Option();

                            option.expiration = exp_date;
                            option.strike = strike;

                            option.stock = ticker;
                            option.stocks_per_contract = 100;
                            option.type = (i == 0) ? "Call" : "Put";
                            option.update_timestamp = DateTime.Now;

                            // symbol
                            option.symbol = null;
                            nd = prs.GetXmlNodeByPath(row_nd, (i == 0) ? @"TD(1)\A" : @"TD(13)\A");
                            if (nd != null)
                            {
                                string[] split = System.Web.HttpUtility.HtmlDecode(nd.Attributes["href"].InnerText).Trim().Split(new char[] { '=', '&', '?' });
                                for (int k = 0; k < split.Length; k++)
                                {
                                    if (split[k] == "symbol" && (k + 1) < split.Length)
                                    {
                                        option.symbol = "." + split[k + 1].Replace("^","");
                                        break;
                                    }
                                }
                            }
                            if (option.symbol == null) continue;

                            // bid
                            option.price.bid = double.NaN;
                            nd = prs.GetXmlNodeByPath(row_nd, (i == 0) ? @"TD(5)" : @"TD(8)");
                            if (nd != null && nd.InnerText != null)
                                double.TryParse(System.Web.HttpUtility.HtmlDecode(nd.InnerText.Trim()), out option.price.bid);
                            if (option.price.bid == 0) option.price.bid = double.NaN;

                            // ask
                            option.price.ask = double.NaN;
                            nd = prs.GetXmlNodeByPath(row_nd, (i == 0) ? @"TD(6)" : @"TD(9)");
                            if (nd != null && nd.InnerText != null)
                                double.TryParse(System.Web.HttpUtility.HtmlDecode(nd.InnerText.Trim()), out option.price.ask);
                            if (option.price.ask == 0) option.price.ask = double.NaN;

                            // last
                            option.price.last = double.NaN;
                            nd = prs.GetXmlNodeByPath(row_nd, (i == 0) ? @"TD(4)" : @"TD(10)");
                            if (nd != null && nd.InnerText != null)
                                double.TryParse(System.Web.HttpUtility.HtmlDecode(nd.InnerText.Trim()), out option.price.last);
                            if (option.price.last == 0) option.price.last = double.NaN;

                            option.price.change = double.NaN;

                            // volume
                            option.volume.total = double.NaN;
                            nd = prs.GetXmlNodeByPath(row_nd, (i == 0) ? @"TD(3)" : @"TD(11)");
                            if (nd != null && nd.InnerText != null)
                                double.TryParse(System.Web.HttpUtility.HtmlDecode(nd.InnerText.Trim()), out option.volume.total);

                            // open-int
                            option.open_int = 0;
                            nd = prs.GetXmlNodeByPath(row_nd, (i == 0) ? @"TD(2)" : @"TD(12)");
                            if (nd != null && nd.InnerText != null)
                                int.TryParse(System.Web.HttpUtility.HtmlDecode(nd.InnerText.Trim()), out option.open_int);

                            options_list.Add(option);
                        }
                        catch { }
                    }
                }
            }

            return options_list;
        }
    }
}
