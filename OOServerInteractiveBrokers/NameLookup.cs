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
using OOServerLib.Web;
using OOServerLib.Interface;
using OOServerLib.Global;
using OOServerLib.Config;
using Krs.Ats.IBNet;

namespace OOServerInteractiveBrokers
{
    public class NameLookup
    {
        private const string LOOKUP_FILE = "plugin_interactivebrokers_lookup.xml";

        // web capture client
        private WebCapture cap = null;

        // cache dictionary
        private SerializableDictionary<string, string> name_cache = new SerializableDictionary<string, string>();

        public NameLookup(WebCapture cap)
        {
            this.cap = cap;
            Load();
        }

        private void Load()
        {
            // get config directory path
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\OptionsOracle\";
            string conf = path + LOOKUP_FILE;

            // check if config directory exist, if not create it
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            // load / create-new configuration
            try
            {
                if (File.Exists(conf))
                {
                    // load configuration
                    XmlTextReader reader = new XmlTextReader(conf);
                    name_cache.ReadXml(reader);
                }
            }
            catch { }
        }

        private void Save()
        {
            // get config directory path
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\OptionsOracle\";
            string conf = path + LOOKUP_FILE;

            // check if config directory exist, if not create it
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            // load / create-new configuration
            try
            {
                // load configuration
                XmlTextWriter writer = new XmlTextWriter(conf, null);
                name_cache.WriteXml(writer);
                writer.Close();
            }
            catch { }
        }


        public string GetNameBySymbol(string symbol, string exchange, string currency, SecurityType type)
        {
            string type_id = "";

            if (symbol.StartsWith("^"))
            {
                symbol = symbol.TrimStart(new char[] { '^' });
                type = Krs.Ats.IBNet.SecurityType.Index;
            }
            else if (symbol.StartsWith("~"))
            {
                symbol = symbol.TrimStart(new char[] { '~' });
                type = Krs.Ats.IBNet.SecurityType.Future;
            }

            if (type == SecurityType.Stock) type_id = "STK";
            else if (type == SecurityType.Index) type_id = "IND";
            else if (type == SecurityType.Future) type_id = "FUTGRP";
            else if (type == SecurityType.Option) type_id = "OPT";
            else if (type == SecurityType.FutureOption) type_id = "FOP";

            string cache_key = symbol + "," + exchange + "," + currency + "," + type_id;

            // check cache
            if (name_cache.ContainsKey(cache_key)) return name_cache[cache_key];

            // constract url
            string url = @"http://www.interactivebrokers.co.uk/contract_info/index.php?action=Advanced Search";
            if (!string.IsNullOrEmpty(exchange) && exchange != "SMART") url += "&exchange=" + exchange;
            if (!string.IsNullOrEmpty(type_id)) url += "&contractType=" + type_id;
            if (!string.IsNullOrEmpty(currency)) url += "&currency=" + currency;
            if (!string.IsNullOrEmpty(symbol)) url += "&symbol=" + symbol;

            // get html page in xml format
            XmlDocument xml = cap.DownloadXmlPartialWebPage(url, "<body", "</body", 1, 1);
            if (xml == null) return null;

            // parse html page and extract name
            XmlParser prs = new XmlParser();

            XmlNode table_nd = prs.GetXmlNodeByPath(xml.FirstChild, @"body\br\center\form\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\table(2)");
            if (table_nd == null) return null;

            // parse html page and extract name
            XmlNode nd = prs.GetXmlNodeByPath(table_nd, @"tr(3)\td(2)\div\b");
            if (nd == null || string.IsNullOrEmpty(nd.InnerText)) return null;

            // get name
            string name = null;
            try
            {
                name = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(System.Web.HttpUtility.HtmlDecode(nd.InnerText).Trim().ToLower());
            }
            catch { }
            if (string.IsNullOrEmpty(name)) return null;

            // add to cache and save it
            name_cache.Add(cache_key, name);
            Save();

            return name;
        }

        public double GetLastPriceBySymbol(string symbol, string exchange, string currency, SecurityType type)
        {
            string type_id = "";

            if (symbol.StartsWith("^"))
            {
                symbol = symbol.TrimStart(new char[] { '^' });
                type = Krs.Ats.IBNet.SecurityType.Index;
            }
            else if (symbol.StartsWith("~"))
            {
                symbol = symbol.TrimStart(new char[] { '~' });
                type = Krs.Ats.IBNet.SecurityType.Future;
            }

            if (type == SecurityType.Stock) type_id = "STK";
            else if (type == SecurityType.Index) type_id = "IND";
            else if (type == SecurityType.Future) type_id = "FUTGRP";
            else if (type == SecurityType.Option) type_id = "OPT";
            else if (type == SecurityType.FutureOption) type_id = "FOP";

            // constract url
            string url = @"http://www.interactivebrokers.co.uk/contract_info/index.php?action=Advanced Search";
            if (!string.IsNullOrEmpty(exchange) && exchange != "SMART") url += "&exchange=" + exchange;
            if (!string.IsNullOrEmpty(type_id)) url += "&contractType=" + type_id;
            if (!string.IsNullOrEmpty(currency)) url += "&currency=" + currency;
            if (!string.IsNullOrEmpty(symbol)) url += "&symbol=" + symbol;

            // get html page in xml format
            XmlDocument xml = cap.DownloadXmlPartialWebPage(url, "<body", "</body", 1, 1);
            if (xml == null) return double.NaN;

            // parse html page and extract name
            XmlParser prs = new XmlParser();

            XmlNode table_nd = prs.GetXmlNodeByPath(xml.FirstChild, @"body\br\center\form\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\table(2)");
            if (table_nd == null) return double.NaN;

            // parse html page and extract name
            XmlNode nd = prs.GetXmlNodeByPath(table_nd, @"tr(3)\td(6)");
            if (nd == null || string.IsNullOrEmpty(nd.InnerText)) return double.NaN;

            // get name
            double last = double.NaN;
            try
            {
                last = (double)Convert.ToDecimal(System.Web.HttpUtility.HtmlDecode(nd.InnerText).Trim());
            }
            catch { }

            return last;
        }

        public ArrayList GetStockSymbolLookup(string lookup, string exchange, string currency, SecurityType type)
        {
            string type_id = "";

            if (lookup.StartsWith("^"))
            {
                lookup = lookup.TrimStart(new char[] { '^' });
                type = Krs.Ats.IBNet.SecurityType.Index;
            }
            else if (lookup.StartsWith("~"))
            {
                lookup = lookup.TrimStart(new char[] { '~' });
                type = Krs.Ats.IBNet.SecurityType.Future;
            }

            if (type == SecurityType.Stock) type_id = "STK";
            else if (type == SecurityType.Index) type_id = "IND";
            else if (type == SecurityType.Future) type_id = "FUTGRP";
            else if (type == SecurityType.Option) type_id = "OPT";
            else if (type == SecurityType.FutureOption) type_id = "FOP";

            // constract url
            string base_url = @"http://www.interactivebrokers.co.uk/contract_info/index.php?action=Advanced Search";
            if (!string.IsNullOrEmpty(exchange) && exchange != "SMART") base_url += "&exchange=" + exchange;
            if (!string.IsNullOrEmpty(type_id)) base_url += "&contractType=" + type_id;
            if (!string.IsNullOrEmpty(currency)) base_url += "&currency=" + currency;

            // create list
            ArrayList list = new ArrayList();
            XmlParser prs = new XmlParser();

            foreach (string item in new string[] { "description", "symbol" })
            {
                string url = base_url;
                if (!string.IsNullOrEmpty(lookup)) url += "&" + item + "=" + lookup;

                // get html page in xml format
                XmlDocument xml = cap.DownloadXmlPartialWebPage(url, "<body", "</body", 1, 1);
                if (xml == null) return null;

                XmlNode table_nd = prs.GetXmlNodeByPath(xml.FirstChild, @"body\br\center\form\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\input\table(2)");
                if (table_nd == null) continue;

                for (int j = 3; j < 32; j++)
                {
                    // parse html page and extract name
                    XmlNode nd = prs.GetXmlNodeByPath(table_nd, @"tr(" + j.ToString() + @")\td(2)\div\b");
                    if (nd == null || string.IsNullOrEmpty(nd.InnerText)) break;

                    // get name
                    string name = null;
                    try
                    {
                        name = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(System.Web.HttpUtility.HtmlDecode(nd.InnerText).Trim().ToLower());
                    }
                    catch { }
                    if (string.IsNullOrEmpty(name)) continue;

                    // get type                    
                    nd = prs.GetXmlNodeByPath(table_nd, @"tr(" + j.ToString() + @")\td(3)");
                    if (nd == null || string.IsNullOrEmpty(nd.InnerText)) continue;
                   
                    string type_prefix = "";
                    if (nd.InnerText.Contains("Index")) type_prefix = "^";
                    else if (nd.InnerText.Contains("Future")) type_prefix = "~";

                    // get exchange
                    nd = prs.GetXmlNodeByPath(table_nd, @"tr(" + j.ToString() + @")\td(5)");
                    if (nd == null || string.IsNullOrEmpty(nd.InnerText)) continue;

                    string exchange_code = nd.InnerText.Trim().ToUpper();

                    // get symbol
                    nd = prs.GetXmlNodeByPath(table_nd, @"tr(" + j.ToString() + @")\td(4)");
                    if (nd == null || string.IsNullOrEmpty(nd.InnerText)) continue;

                    string symbol = nd.InnerText.Trim().ToUpper();

                    // generate list entry
                    string entry = name.Replace('(', '[').Replace(')', ']') + " @ " + exchange_code + " (" + type_prefix + symbol + ")";
                    if (!list.Contains(entry)) list.Add(entry);
                }

                if (lookup == null) break;
            }

            return list;
        }
    }
}
