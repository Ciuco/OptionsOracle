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

using Microsoft.Win32;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Security;

using OOServerLib.Web;
using OOServerLib.Interface;
using OOServerLib.Global;
using OOServerLib.Config;
using OOServerLib.Forms;

namespace OOServerNordicEx
{
    public class Main : WebSite, IServer
    {
        // host
        private IServerHost host = null;

        // connection status
        private bool connect = false;

        // wir interface        
        private WorldInterestRate wir = null;

        // feature and server array list
        private ArrayList feature_list = new ArrayList();
        private ArrayList server_list = new ArrayList();

        // symbol convert
        private SymbolSet symbol_lut = null;

        // dictionary
        private Dictionary<string, XmlDocument> cache_dict = new Dictionary<string, XmlDocument>();

        // culture
        CultureInfo ci = new CultureInfo("en-US", false);

        private WebForm wbf;

        public Main()
        {
            // create interaces            
            wir = new WorldInterestRate(cap);

            // update feature list
            feature_list.Add(FeaturesT.SUPPORTS_DELAYED_OPTIONS_CHAIN);
            feature_list.Add(FeaturesT.SUPPORTS_DELAYED_STOCK_QUOTE);
            feature_list.Add(FeaturesT.SUPPORTS_CONFIG_FORM);

            // update server list
            server_list.Add(Name);

            wbf = new WebForm();
            wbf.Show();
            wbf.Hide();
        }

        public void Initialize(string config)
        {
        }

        public void Dispose()
        {
        }

        // get server feature list
        public ArrayList FeatureList { get { return feature_list; } }

        // get list of servers 
        public ArrayList ServerList { get { return server_list; } }

        // get server operation mode list
        public ArrayList ModeList { get { return null; } }

        // get display accuracy
        public int DisplayAccuracy { get { return 2; } }

        // get server assembly data
        public string Author { get { return ""; } }
        public string Description { get { return "Delayed Quote for Nordic Market"; } }
        public string Name { get { return "PlugIn Server OMX (Nordic)"; } }
        public string Version { get { return Assembly.GetExecutingAssembly().GetName().Version.ToString(); } }

        // plugin authentication
        public string Authentication(string oo_version, string phrase)
        {
            try
            {
                Crypto crypto = new Crypto(@"<RSAKeyValue><Modulus>otysuKHd8wjQn9Oe9m3zAJ1oXtgs9ukfvBOeEjgM/xIMpAk3pFbyT6lGBjGjBvdMTP4kyMRgBYT1SXUXKU85VulcJjvTVH6kCfq04fktoZrKswahz7XCs5tmt7E1yxnavfZddSdhwOWyjgYyCVjXMpOKIZc04XeSJO6COYptQV8=</Modulus><Exponent>AQAB</Exponent><P>0TRDDBI6gZvxDZokegiocMKejl5RINKSEGc7kHARB3G0MwZ1ZvrOaHMsDeS+feHZlX1MGIJUcP0oM0UdmWXuIw==</P><Q>x0q0fPbhLbM06hNiSCIWDxwC5yNprrLEuyJlqTKQFPTd1xZJ6wLf0c/Zr93KeTaepR7nMBdSsABm16ivo+StlQ==</Q><DP>Rpdd8FrORyG5ix9yI4N8YuAo5F1K/spO4x4SaUCHXn2tknIhd2g18eS6/s0qwgtNgjXPUY3YtG+X+wTdYf+VBQ==</DP><DQ>PxMPyLVCU3pydtsnsfjHzoRpDsqQejAuP6QFVOWh4GAXjimJv42rVPZZyWWC3ZZB47TCKuBW1UlrQzoqTM7leQ==</DQ><InverseQ>Pu9T/OTeCLirNvs/pc4CS3fGfPlNA0K9SpaNyWQMi8FIW9q8ggCCoyVxc3Ij3Ote6cl1xTXa7LRyn3kbtJOiIw==</InverseQ><D>DB1UL8vCodB3DFyGh5g4KkSLPfrgpWFD/g6LhJlsxhCGpjEVVYEuNyTFU7KfiOYeY9/HxrNs3Rw9zsAKAAWnoyQHv/CGwGET1H4xLuTRrykShGACPeu+hsfjj0dHyCjVWmsRiTUdY5IjEsUoniknMd9pm393ZoiINvod0UyPljk=</D></RSAKeyValue>");
                return crypto.Decrypt(phrase);
            }
            catch { return ""; }
        }

        // username and password
        public string Username { set { } } // not-required
        public string Password { set { } } // not-required

        // get configuration string
        public string Configuration { get { return null; } }

        // set/get server
        public string Server { get { return Name; } set { } } // not-supported

        // set/get operation mode
        public string Mode { get { return null; } set { } } // not-supported

        // set/get callback host
        public IServerHost Host { get { return host; } set { host = value; } }

        // connect/disconnect to server
        public bool Connect { get { return connect; } set { connect = value; } }

        // connection settings
        //public int    ConnectionsRetries { get; set; } // implemented by parent class WebSite
        //public string ProxyAddress { get; set; }       // implemented by parent class WebSite
        //public bool   UseProxy { get; set; }           // implemented by parent class WebSite

        // debug log control
        public bool LogEnable { get { return false; } set { } }
        public string DebugLog { get { return null; } }

        // configuration form
        public void ShowConfigForm(object form) { (new AboutBox()).ShowDialog(); }

        // default symbol
        public string DefaultSymbol { get { return ""; } }

        public SymbolSet.SymbolTableRow CorrectSymbolRow(string ticker)
        {
            if (symbol_lut == null)
            {
                symbol_lut = new SymbolSet();
                symbol_lut.Load();
            }


            return symbol_lut.SymbolTable.FindByTicker(ticker);
        }

        public string StripHtmlPage(string page, string sta_str, string end_str)
        {
            string stmp = "";

            int idx_0 = 0, idx_1 = 0, end_len = end_str.Length;

            while (true)
            {
                idx_1 = page.IndexOf(sta_str, idx_0);
                if (idx_1 <= idx_0) return stmp + page.Substring(idx_0);

                stmp += page.Substring(idx_0, idx_1 - idx_0);

                idx_0 = page.IndexOf(end_str, idx_1) + end_len;
                if (idx_0 == -1) return stmp;
            }
        }

        // get stock latest quote
        public Quote GetQuote(string ticker)
        {
            XmlDocument xml = null;
            System.Windows.Forms.HtmlElement elem = null;

            // correct symbol
            SymbolSet.SymbolTableRow tck = CorrectSymbolRow(ticker);
            if (tck == null) return null;

            // is index ?
            bool is_index = tck.Symbol.StartsWith("^");

            // clear global cache
            cache_dict.Clear();

            XmlNode nd, row_nd = null;
            System.Windows.Forms.HtmlDocument doc = null;

            // @"http://www.nasdaqomxnordic.com/shares/optionsandfutures/?Instrument={0}"
            // @"http://www.nasdaqomxnordic.com/optionsandfutures/microsite?Instrument={0}"
 
            // get url to quote+option-chain page
            string url = string.Format(@"http://www.nasdaqomxnordic.com/shares/optionsandfutures/?Instrument={0}", is_index ? tck.Isin : tck.Id);

            // read first page (try twice before giving up)
            doc = wbf.GetHtmlDocumentWithWebBrowser(url, null, "optionsAndFuturesTable", "TABLE", 60);
            if (doc == null || doc.Body == null || string.IsNullOrEmpty(doc.Body.InnerText)) return null;

            // get quote table
            elem = doc.GetElementById("avistaTable");
            if (elem == null) return null;

            xml = cap.ConvertHtmlToXml(elem.OuterHtml);
            if (xml == null) return null;

            row_nd = prs.GetXmlNodeByPath(xml.FirstChild, @"TBODY\TR");
            if (row_nd == null) return null;

            // create quote
            Quote quote = new Quote();

            // get stock symbol / name            
            quote.stock = tck.Symbol;

            // get name
            quote.name = quote.stock;
            nd = prs.GetXmlNodeByPath(row_nd, @"TD(1)");
            if (nd != null && nd.InnerText != "")
            {
                try { quote.name = nd.InnerText.Trim(); }
                catch { }
            }

            // get last price
            quote.price.last = double.NaN;
            nd = prs.GetXmlNodeByPath(row_nd, @"TD(2)");
            if (nd != null && nd.InnerText != "")
            {
                try { quote.price.last = Convert.ToDouble(nd.InnerText); }
                catch { }
            }

            // get price change
            quote.price.change = double.NaN;
            nd = prs.GetXmlNodeByPath(row_nd, @"TD(3)");
            if (nd != null && nd.InnerText != "")
            {
                try { quote.price.change = Convert.ToDouble(nd.InnerText); }
                catch { }
            }

            // get open price
            quote.price.open = quote.price.last - quote.price.change;

            // get bid price
            quote.price.bid = double.NaN;
            nd = prs.GetXmlNodeByPath(row_nd, @"TD(5)");
            if (nd != null && nd.InnerText != "")
            {
                try { quote.price.bid = Convert.ToDouble(nd.InnerText); }
                catch { }
            }

            // get ask price
            quote.price.ask = double.NaN;
            nd = prs.GetXmlNodeByPath(row_nd, @"TD(6)");
            if (nd != null && nd.InnerText != "")
            {
                try { quote.price.ask = Convert.ToDouble(nd.InnerText); }
                catch { }
            }

            // get high price
            quote.price.high = double.NaN;
            nd = prs.GetXmlNodeByPath(row_nd, @"TD(7)");
            if (nd != null && nd.InnerText != "")
            {
                try { quote.price.high = Convert.ToDouble(nd.InnerText); }
                catch { }
            }

            // get low price
            quote.price.low = double.NaN;
            nd = prs.GetXmlNodeByPath(row_nd, @"TD(8)");
            if (nd != null && nd.InnerText != "")
            {
                try { quote.price.low = Convert.ToDouble(nd.InnerText); }
                catch { }
            }

            // get total volume
            quote.volume.total = double.NaN;
            elem = doc.GetElementById("tradingInformationTable");
            if (elem != null)
            {
                xml = cap.ConvertHtmlToXml(elem.OuterHtml);
                if (xml != null)
                {
                    nd = prs.GetXmlNodeByPath(xml.FirstChild, @"TBODY\TR(3)\TD(2)");
                    if (nd != null && nd.InnerText != "")
                    {
                        try
                        {
                            quote.volume.total = Convert.ToDouble(nd.InnerText) * quote.price.last;
                        }
                        catch { }
                    }
                }
            }

            // time-stamp
            quote.update_timestamp = DateTime.Now;

            // get and cache option table
            elem = doc.GetElementById("optionsAndFuturesTable");
            if (elem != null)
            {
                xml = cap.ConvertHtmlToXml(elem.OuterHtml);
                if (xml != null) cache_dict.Add(tck.Id, xml);
            }

            return quote;
        }

        // get stock latest options chain
        public ArrayList GetOptionsChain(string ticker)
        {
            XmlDocument xml = null;
            System.Windows.Forms.HtmlElement elem = null;

            // correct symbol
            SymbolSet.SymbolTableRow tck = CorrectSymbolRow(ticker);
            if (tck == null) return null;

            // check cache for option data
            if (!cache_dict.ContainsKey(tck.Id))
            {
                // get url for quote page
                string url = @"http://www.nasdaqomxnordic.com/shares/optionsandfutures/?Instrument=" + tck.Id;

                // read first page (try twice before giving up)
                System.Windows.Forms.HtmlDocument doc = wbf.GetHtmlDocumentWithWebBrowser(url, null, "optionsAndFuturesTable", "TABLE", 30);
                if (doc == null || doc.Body == null || string.IsNullOrEmpty(doc.Body.InnerText)) return null;

                elem = doc.GetElementById("optionsAndFuturesTable");
                if (elem == null) return null;

                xml = cap.ConvertHtmlToXml(elem.OuterHtml);
            }
            else
            {
                xml = cache_dict[tck.Id];
            }
            if (xml == null) return null;

            // create options array list
            ArrayList options_list = new ArrayList();
            options_list.Clear();

            for (int i = 1; ; i++)
            {
                XmlNode nd, row_nd = prs.GetXmlNodeByPath(xml.FirstChild, @"TBODY\TR(" + i.ToString() + ")");
                if (row_nd == null) break;

                try
                {
                    // get title
                    string title = null;
                    foreach (XmlAttribute attr in row_nd.Attributes) if (attr.Name.ToUpper() == "TITLE")
                        {
                            title = attr.Value;
                            break;
                        }
                    if (title == null) continue;

                    string[] split = title.Split('-');
                    if (split.Length == 1 || (!split[1].Contains("CALL OPTION") && !split[1].Contains("PUT OPTION"))) continue;

                    Option option = new Option();

                    // set option stock
                    option.stock = tck.Symbol;

                    // set option symbol
                    option.symbol = "." + split[0].Trim();

                    // get option type by parsing the option symbol
                    string stmp = option.symbol.TrimEnd(new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' }).ToUpper();
                    char ctmp = stmp[stmp.Length - 1];

                    if (ctmp >= 'A' && ctmp <= 'L') option.type = "Call";
                    else if (ctmp >= 'M' && ctmp <= 'X') option.type = "Put";
                    else continue;

                    // get expiration date
                    nd = prs.GetXmlNodeByPath(row_nd, @"TD(2)");
                    if (nd == null || nd.InnerText.Trim() == "") continue;
                    split = nd.InnerText.Trim().Split(new char[] { '.', '-' });
                    option.expiration = Convert.ToDateTime(split[1] + "/" + split[2] + "/" + split[0], ci).AddDays(1);

                    // get strike price
                    nd = prs.GetXmlNodeByPath(row_nd, @"TD(11)");
                    if (nd == null || nd.InnerText.Trim() == "") continue;
                    option.strike = Convert.ToDouble(nd.InnerText);

                    // get stocks per contract
                    nd = prs.GetXmlNodeByPath(row_nd, @"TD(10)");
                    try { option.stocks_per_contract = (int)Convert.ToDecimal(nd.InnerText); }
                    catch { option.stocks_per_contract = 100; }

                    // get bid price
                    nd = prs.GetXmlNodeByPath(row_nd, @"TD(3)");
                    if (nd == null || nd.InnerText == "") option.price.bid = double.NaN;
                    else
                    {
                        try { option.price.bid = Convert.ToDouble(nd.InnerText); }
                        catch { option.price.bid = double.NaN; }
                    }

                    // get ask price
                    nd = prs.GetXmlNodeByPath(row_nd, @"TD(4)");
                    if (nd == null || nd.InnerText == "") option.price.ask = double.NaN;
                    else
                    {
                        try { option.price.ask = Convert.ToDouble(nd.InnerText); }
                        catch { option.price.ask = double.NaN; }
                    }

                    // get last price
                    nd = prs.GetXmlNodeByPath(row_nd, @"TD(5)");
                    if (nd == null || nd.InnerText == "") option.price.last = double.NaN;
                    else
                    {
                        try { option.price.last = Convert.ToDouble(nd.InnerText); }
                        catch { option.price.last = double.NaN; }
                    }

                    // set price change
                    option.price.change = double.NaN;

                    // get option open-int
                    nd = prs.GetXmlNodeByPath(row_nd, @"TD(8)");
                    if (nd == null || nd.InnerText == "") option.open_int = 0;
                    else
                    {
                        try { option.open_int = (int)Convert.ToDecimal(nd.InnerText); }
                        catch { option.open_int = 0; }
                    }

                    // get option volume
                    nd = prs.GetXmlNodeByPath(row_nd, @"TD(9)");
                    if (nd == null || nd.InnerText == "") option.volume.total = 0;
                    else
                    {
                        try { option.volume.total = (int)Convert.ToDecimal(nd.InnerText); }
                        catch { option.volume.total = 0; }
                    }

                    // update time stamp
                    option.update_timestamp = DateTime.Now;

                    // add option to list
                    options_list.Add(option);
                }
                catch { }
            }

            return options_list;
        }

        // get stock/option historical prices 
        public ArrayList GetHistoricalData(string ticker, DateTime start, DateTime end)
        {
            return null;
        }

        // get stock name lookup results
        public ArrayList GetStockSymbolLookup(string name)
        {
            ArrayList symbol_list = new ArrayList();
            symbol_list.Capacity = 256;

            string name_uc = name.ToUpper();

            foreach (SymbolSet.SymbolTableRow row in symbol_lut.SymbolTable.Rows)
            {
                if (row.Name.ToUpper().Contains(name_uc) ||
                    row.Symbol.ToUpper().Contains(name_uc) ||
                    row.Isin.ToUpper().Contains(name_uc) ||
                    row.Id.ToUpper().Contains(name_uc))
                {
                    symbol_list.Add(row.Name.Trim().Replace('(', '[').Replace(')', ']') + " (" + row.Symbol + ")");
                }
            }

            symbol_list.TrimToSize();
            return symbol_list;
        }

        // get default annual interest rate for specified duration [in years]
        public double GetAnnualInterestRate(double duration)
        {
            return wir.GetAnnualInterestRate("EUR");
        }

        // get default historical volatility for specified duration [in years]
        public double GetHistoricalVolatility(string ticker, double duration)
        {
            return double.NaN;
        }

        // get and set generic parameters
        public string GetParameter(string name)
        {
            return null;
        }

        public void SetParameter(string name, string value)
        {
        }

        // get and set generic parameters list
        public ArrayList GetParameterList(string name)
        {
            return null;
        }

        public void SetParameterList(string name, ArrayList value)
        {
        }
    }
}
