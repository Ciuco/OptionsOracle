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
using System.Text.RegularExpressions;
using System.Globalization;
using System.Xml;
using System.Diagnostics;
using System.Collections;
using System.Threading;
using System.Reflection;
using OOServerLib.Web;
using OOServerLib.Interface;
using OOServerLib.Global;
using OOServerLib.Config;

namespace OOServerUS
{
    public class MoneyCentral
    {
        // web capture client
        private WebCapture cap = null;

        // parse html page
        private XmlParser prs = new XmlParser();

        // culture info
        private CultureInfo ci = new CultureInfo("en-US", false);

        public MoneyCentral(WebCapture cap)
        {
            this.cap = cap;
        }

        // get stock latest quote
        public Quote GetQuote(string ticker)
        {
            string url = @"http://moneycentral.msn.com/detail/stock_quote?Symbol=" + ticker.Replace("^", "$");

            XmlNode nd;
            XmlDocument xml = DownloadXmlPartialWebPage(url, "<body", "</body>", 1, 1);
            if (xml == null) return null;

            Quote quote = new Quote();

            quote.name = ticker;
            quote.stock = ticker;
            
            XmlNode root_node = prs.FindXmlNodeByName(xml.FirstChild, @"div", @"id=area1");
            if (root_node == null) return null;

            XmlNode head_node = prs.FindXmlNodeByName(xml.FirstChild, @"div", @"id=quickquoteb");
            if (head_node == null) return null;

            XmlNode tbdy_node = prs.FindXmlNodeByName(root_node, @"tbody", "");
            if (tbdy_node == null) return null;

            // underlying name
            nd = prs.FindXmlNodeByName(head_node, @"h1", @"class=cn");
            if (nd != null && nd.InnerText != null)
                quote.name = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(System.Web.HttpUtility.HtmlDecode(nd.InnerText).Split('(')[0].Trim().ToLower());
            else
                return null;

            // last price
            nd = prs.FindXmlNodeByName(head_node, @"span", @"class=lp");
            quote.price.last = double.NaN;
            if (nd != null && !string.IsNullOrEmpty(nd.InnerText))
                double.TryParse(nd.InnerText, NumberStyles.Number, ci, out quote.price.last);

            // price change
            nd = prs.FindXmlNodeByName(head_node, @"span", @"class=chg");
            quote.price.change = 0;
            if (nd != null && !string.IsNullOrEmpty(nd.InnerText))
                double.TryParse(nd.InnerText, NumberStyles.Number, ci, out quote.price.change);

            // bid price
            nd = prs.GetXmlNodeByPath(tbdy_node, @"tr(7)\td(2)");
            quote.price.bid = double.NaN;
            if (nd != null && !string.IsNullOrEmpty(nd.InnerText))
                double.TryParse(nd.InnerText.Trim(), NumberStyles.Number, ci, out quote.price.bid);

            // ask price
            nd = prs.GetXmlNodeByPath(tbdy_node, @"tr(9)\td(2)");
            quote.price.ask = double.NaN;
            if (nd != null && !string.IsNullOrEmpty(nd.InnerText))
                double.TryParse(nd.InnerText.Trim(), NumberStyles.Number, ci, out quote.price.ask);

            // open price
            nd = prs.GetXmlNodeByPath(tbdy_node, @"tr(2)\td(2)");
            quote.price.open = double.NaN;
            if (nd != null && !string.IsNullOrEmpty(nd.InnerText))
                double.TryParse(nd.InnerText.Trim(), NumberStyles.Number, ci, out quote.price.open);

            // high price
            nd = prs.GetXmlNodeByPath(tbdy_node, @"tr(3)\td(2)");
            quote.price.high = double.NaN;
            if (nd != null && !string.IsNullOrEmpty(nd.InnerText))
                double.TryParse(nd.InnerText.Trim(), NumberStyles.Number, ci, out quote.price.high);

            // low price
            nd = prs.GetXmlNodeByPath(tbdy_node, @"tr(4)\td(2)");
            quote.price.low = double.NaN;
            if (nd != null && !string.IsNullOrEmpty(nd.InnerText))
                double.TryParse(nd.InnerText.Trim(), NumberStyles.Number, ci, out quote.price.low);

            // volume
            nd = prs.GetXmlNodeByPath(tbdy_node, @"tr(5)\td(2)");
            quote.volume.total = 0;
            if (nd != null && !string.IsNullOrEmpty(nd.InnerText))
                double.TryParse(nd.InnerText.Trim(), NumberStyles.Number, ci, out quote.volume.total);

            // timestamp
            quote.update_timestamp = DateTime.Now;

            return quote;
        }

        // get stock name lookup results
        public ArrayList GetStockSymbolLookup(string name)
        {
            string lookup_url = @"http://moneycentral.msn.com/investor/common/findsymbol.asp?Company=" + name;

            XmlDocument xml = DownloadXmlPartialWebPage(lookup_url, "<body", "</body>", 1, 1);
            if (xml == null) return null;

            XmlNode root_node = prs.FindXmlNodeByName(xml.FirstChild, @"div", @"id=area1");
            if (root_node == null) return null;

            XmlNode tbdy_node = prs.FindXmlNodeByName(root_node, @"tbody", "");
            if (tbdy_node == null) return null;

            ArrayList symbol_list = new ArrayList();
            symbol_list.Capacity = 256;

            for (int i = 1; i < symbol_list.Capacity; i++)
            {
                string entry = "";

                XmlNode nd, row_nd = prs.GetXmlNodeByPath(tbdy_node, @"tr(" + i.ToString() + @")");
                if (row_nd == null) break;

                try
                {
                    // name
                    nd = prs.GetXmlNodeByPath(row_nd, @"td(2)");
                    entry = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(System.Web.HttpUtility.HtmlDecode(nd.InnerText).Trim().ToLower()).Replace('(', '[').Replace(')', ']');

                    // ticker
                    nd = prs.GetXmlNodeByPath(row_nd, @"td(1)\a");
                    entry += " (" + nd.InnerText.ToUpper().Split('.')[0] + ")";

                    // add name + ticker entry
                    symbol_list.Add(entry);
                }
                catch { }
            }

            symbol_list.TrimToSize();
            return symbol_list;
        }

        private XmlDocument DownloadXmlPartialWebPage(string url, string sta_str, string end_str, int sta_count, int end_count)
        {
            try
            {
                string page = Regex.Replace(cap.DownloadHtmlWebPage(url), "<script.*?</script>", "", RegexOptions.Singleline | RegexOptions.IgnoreCase).Replace("sys:","sys_");
                return cap.ConvertHtmlToXml(@"<html>" + cap.GetPartialWebPage(page, sta_str, end_str, sta_count, end_count) + @"</html>");
            }
            catch { return null; }
        }
    }
}
