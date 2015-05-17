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

namespace OOServerTDAmeritrade
{
    public class NameLookup
    {
        private const string LOOKUP_FILE = "plugin_tdameritrade_lookup.xml";

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

        public ArrayList GetStockSymbolLookup(string lookup)
        {
            if (lookup.StartsWith("^"))
            {
                lookup = lookup.TrimStart(new char[] { '^' });
            }
            else if (lookup.StartsWith("~"))
            {
                lookup = lookup.TrimStart(new char[] { '~' });
            }

            // search url
            string url = @"http://research.tdameritrade.com/public/symbollookup/symbollookup.asp?text=" + lookup;

            ArrayList list = new ArrayList();

            // get html page in xml format
            XmlDocument xml = cap.DownloadXmlPartialWebPage(url, "<body", "</body", 1, 1);
            if (xml == null) return list;

            // parse html page
            XmlParser prs = new XmlParser();

            XmlNode table_nd = prs.GetXmlNodeByPath(xml.FirstChild, @"body\div\div\table\tr\td\div\div(4)\table\tbody");
            if (table_nd == null) return list;

            for (int j = 1; ; j++)
            {
                // extract name
                XmlNode nd = prs.GetXmlNodeByPath(table_nd, @"tr(" + j.ToString() + @")\td(2)");
                if (nd == null || string.IsNullOrEmpty(nd.InnerText)) break;

                // get symbol
                nd = prs.GetXmlNodeByPath(table_nd, @"tr(" + j.ToString() + @")\td(1)");
                if (nd == null || string.IsNullOrEmpty(nd.InnerText)) continue;

                string symbol = nd.InnerText.Trim().ToUpper();

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

                // get exchange
                nd = prs.GetXmlNodeByPath(table_nd, @"tr(" + j.ToString() + @")\td(4)");
                if (nd == null || string.IsNullOrEmpty(nd.InnerText)) continue;

                string exchange_code = nd.InnerText.Trim().ToUpper();

                // generate list entry
                string entry = name.Replace('(', '[').Replace(')', ']') + " @ " + exchange_code + " (" + symbol + ")";
                if (!list.Contains(entry)) list.Add(entry);
            }

            return list;
        }
    }
}
