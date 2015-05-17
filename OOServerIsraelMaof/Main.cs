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
using System.ComponentModel;
using System.Reflection;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

using Microsoft.Win32;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Security;

using OOServerLib.Web;
using OOServerLib.Interface;
using OOServerLib.Global;
using OOServerLib.Config;
using OOServerLib.Forms;

namespace OOServerIsraelMaof
{
    public class Main : OOServerIsraelMaof.WebSite, IServer
    {
        private enum ModeT
        {
            MODE_WITHOUT_ASK_BID_PRICES,
            MODE_WITH_ASK_BID_PRICES
        };

        // connection status
        private bool connect = false;

        // feature and server array list
        private ArrayList feature_list = new ArrayList();
        private ArrayList server_list = new ArrayList();

        // host
        private IServerHost host = null;

        // mode status
        private ModeT mode = ModeT.MODE_WITHOUT_ASK_BID_PRICES;
        private string[] mode_list = new string[] { "Without Ask/Bid Prices [Short Download]", "With Ask/Bid Prices [Long Download]" };

        // debugging
        private bool debug_en = false;
        private string debug_log = "";

        // culture
        CultureInfo ci = new CultureInfo("en-US", false);

        public Main()
        {
            // update feature list
            feature_list.Add(FeaturesT.SUPPORTS_DELAYED_OPTIONS_CHAIN);
            feature_list.Add(FeaturesT.SUPPORTS_DELAYED_STOCK_QUOTE);

            // update server list
            server_list.Add(Name);

            cap.UserAgent = null;
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
        public ArrayList ModeList
        {
            get
            {
                ArrayList list = new ArrayList();
                foreach (string s in mode_list) list.Add(s);
                return list;
            }
        }

        // get display accuracy
        public int DisplayAccuracy { get { return 2; } }

        // get server assembly data
        public string Author { get { return "Shlomo Shachar"; } }
        public string Description { get { return "Delayed Quote for Israel Tel-Aviv 25 Index"; } }
        public string Name { get { return "PlugIn Server Israel (TA25, USDILS)"; } }
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
        // set/get operation mode
        public string Mode
        {
            get
            {
                return mode_list[(int)mode];
            }

            set
            {
                for (int i = 0; i < mode_list.Length; i++)
                {
                    if (mode_list[i] == value)
                    {
                        mode = (ModeT)i;
                        return;
                    }
                }
                mode = ModeT.MODE_WITHOUT_ASK_BID_PRICES;
            }
        }

        // set/get callback host
        public IServerHost Host { get { return host; } set { host = value; } }

        // connect/disconnect to server
        public bool Connect { get { return connect; } set { connect = value; } }

        // connection settings
        //public int ConnectionsRetries { get; set; } // implemented by parent class WebSite
        //public string ProxyAddress { get; set; }    // implemented by parent class WebSite
        //public bool UseProxy { get; set; }          // implemented by parent class WebSite

        // debug log control
        public bool LogEnable { get { return debug_en; } set { debug_en = value; } }
        public string DebugLog { get { return debug_log; } }

        // configuration form
        public void ShowConfigForm(object form) { }

        // default symbol
        public string DefaultSymbol { get { return "^TA25"; } }

        private string CorrectSymbol(string ticker)
        {
            ticker = ticker.ToUpper().Trim();

            switch (ticker)
            {
                case "מעוף":
                case "מעו\"ף":
                case "ת\"א":
                case "ת\"א25":
                case "ת\"א 25":
                case "TA25":
                case "^TA25":
                    return "^TA25";
                case "בנקים":
                case "BANKS":
                case "^BANKS":
                case "BK":
                case "^BK":
                    return "^BK";
                case "דולר":
                case "USD":
                case "USDILS":
                    return "USDILS";
                case "יורו":
                case "EUR":
                case "EURILS":
                    return "EURILS";
                case "הפועלים":
                case "HAPOALIM":
                    return "HAPOALIM";
                case "טבע":
                case "TEVA":
                    return "TEVA";
                case "לאומי":
                case "LEUMI":
                    return "LEUMI";
                case "כיל":
                case "כי\"ל":
                case "ICL":
                    return "ICL";
                default:
                    break;
            }

            return ticker;
        }

        private string GetUnderlyingName(string ticker)
        {
            switch (ticker)
            {
                case "^TA25":
                    return "Tel-Aviv 25 Index";
                case "^BK":
                    return "Israel Banks Index";
                case "USDILS":
                    return "USD/ILS";
                case "EURILS":
                    return "EUR/ILS";
                case "HAPOALIM":
                    return "Bank Hapoalim";
                case "TEVA":
                    return "Teva Pharmaceutical Industries";
                case "LEUMI":
                    return "Bank Leumi";
                case "ICL":
                    return "Israel Chemicals";
                default:
                    return null;
            }
        }

        private enum UnderlyingType { Unknown, Index, Stock, Currency };

        private string GetOptionChainId(string ticker, out UnderlyingType type)
        {
            switch (ticker)
            {
                case "^TA25":
                    type = UnderlyingType.Index; 
                    return "41";
                case "^BK":
                    type = UnderlyingType.Index; 
                    return "44";
                case "USDILS":
                    type = UnderlyingType.Currency; 
                    return "42";
                case "EURILS":
                    type = UnderlyingType.Currency; 
                    return "46";
                case "HAPOALIM":
                    type = UnderlyingType.Stock; 
                    return "49,662577";
                case "TEVA":
                    type = UnderlyingType.Stock; 
                    return "49,629014";
                case "LEUMI":
                    type = UnderlyingType.Stock; 
                    return "49,604611";
                case "ICL":
                    type = UnderlyingType.Stock; 
                    return "49,281014";
                default:
                    type = UnderlyingType.Unknown; 
                    return null;
            }
        }

        private string GetOptionChainUrl(string ticker, out UnderlyingType type)
        {
            string optionchain_id = GetOptionChainId(ticker, out type);

            if (optionchain_id == null)
                return null;
            else
                return @"http://www.bizportal.co.il/shukhahon/sh_maof.shtml?Sug=" + optionchain_id + "&mbase=0&stage=tr";
        }

        private string GetQuoteId(string ticker, out UnderlyingType type)
        {
            switch (ticker)
            {
                case "^TA25":
                    type = UnderlyingType.Index;
                    return "198";
                case "^BK":
                    type = UnderlyingType.Index;
                    return "13";                   
                case "USDILS":
                    type = UnderlyingType.Currency;
                    return "1002";
                case "EURILS":
                    type = UnderlyingType.Currency;
                    return "1030";
                case "HAPOALIM":
                    type = UnderlyingType.Stock;
                    return "662577";
                case "TEVA":
                    type = UnderlyingType.Stock;
                    return "629014";
                case "LEUMI":
                    type = UnderlyingType.Stock;
                    return "604611";
                case "ICL":
                    type = UnderlyingType.Stock;
                    return "281014";
                default:
                    type = UnderlyingType.Unknown;
                    return null;
            }
        }

        private string GetQuoteUrl(string ticker, out UnderlyingType type)
        {
            string quote_id = GetQuoteId(ticker, out type);

            if (quote_id == null || type == UnderlyingType.Unknown) 
                return null;
            else
                return @"http://www.bizportal.co.il/shukhahon/bizcompquote.shtml?p_id=" + quote_id;
        }

        public Quote GetQuote(string ticker)
        {
            // get corrected ticker
            ticker = CorrectSymbol(ticker);

            string url = null;
            UnderlyingType type = UnderlyingType.Unknown;

            // get url
            url = GetQuoteUrl(ticker, out type);
            if (url == null || type == UnderlyingType.Unknown) return null;
         
            XmlDocument xml = cap.DownloadXmlPartialWebPage(url, "<body", "</body>", 1, 1);
            if (xml == null) return null;

            if (debug_en) debug_log = "Getting Quote for '" + ticker + "'\r\n";

            switch (type)
            {
                case UnderlyingType.Index:
                case UnderlyingType.Currency:
                    try
                    {
                        Quote quote = new Quote();
                        XmlNode root_nd, nd;

                        quote.stock = ticker;
                        quote.name = GetUnderlyingName(ticker);

                        if (debug_en) debug_log += "Got HTML Page for '" + ticker + "'\nHTML.InnerText: " + xml.InnerText + "\r\n";

                        root_nd = prs.FindXmlNodeByName(xml.FirstChild, "div", "id=midCol");
                        if (root_nd == null) return null;
                        root_nd = prs.GetXmlNodeByPath(root_nd, "table");
                        if (root_nd == null) return null;

                        // get last price
                        nd = prs.GetXmlNodeByPath(root_nd, @"tr(2)\td(2)");
                        if (nd == null) return null;
                        quote.price.last = double.NaN;
                        if (nd != null && nd.InnerText != null)
                            double.TryParse(System.Web.HttpUtility.HtmlDecode(nd.InnerText).Trim(), NumberStyles.Number, ci, out quote.price.last);
                        if (debug_en) debug_log += "quote.price.last = '" + quote.price.last.ToString() + "'\r\n";

                        if (type == UnderlyingType.Index)
                        {
                            // get open price
                            nd = prs.GetXmlNodeByPath(root_nd, @"tr(3)\td(2)");
                            if (nd == null) return null;
                            quote.price.open = double.NaN;
                            if (nd != null && nd.InnerText != null)
                                double.TryParse(System.Web.HttpUtility.HtmlDecode(nd.InnerText).Trim(), NumberStyles.Number, ci, out quote.price.open);
                            if (debug_en) debug_log += "quote.price.open = '" + quote.price.open.ToString() + "'\r\n";

                            // calculate price change
                            try
                            {
                                quote.price.change = Math.Round(quote.price.last - quote.price.open, 2);
                            }
                            catch { quote.price.change = double.NaN; }
                            if (debug_en) debug_log += "quote.price.change = '" + quote.price.change.ToString() + "'\r\n";
                        }
                        else
                        {
                            // get price change
                            nd = prs.GetXmlNodeByPath(root_nd, @"tr(2)\td(3)");
                            if (nd == null) return null;
                            quote.price.change = double.NaN;
                            if (nd != null && nd.InnerText != null)
                                double.TryParse(System.Web.HttpUtility.HtmlDecode(nd.InnerText).Trim(), NumberStyles.Number, ci, out quote.price.change);
                            if (debug_en) debug_log += "quote.price.change = '" + quote.price.change.ToString() + "'\r\n";

                            // calculate open price
                            try
                            {
                                quote.price.open = Math.Round(quote.price.last - quote.price.change, 2);
                            }
                            catch { quote.price.open = double.NaN; }
                            if (debug_en) debug_log += "quote.price.open = '" + quote.price.open.ToString() + "'\r\n";
                        }

                        // get low price
                        if (type == UnderlyingType.Index)
                            nd = prs.GetXmlNodeByPath(root_nd, @"tr(6)\td(2)");
                        else 
                            nd = prs.GetXmlNodeByPath(root_nd, @"tr(5)\td(2)");
                        quote.price.low = double.NaN;
                        if (nd != null && nd.InnerText != null)
                            double.TryParse(System.Web.HttpUtility.HtmlDecode(nd.InnerText).Trim(), NumberStyles.Number, ci, out quote.price.low);
                        if (debug_en) debug_log += "quote.price.low = '" + quote.price.low.ToString() + "'\r\n";

                        // get high price
                        if (type == UnderlyingType.Index)
                            nd = prs.GetXmlNodeByPath(root_nd, @"tr(5)\td(2)");
                        else 
                            nd = prs.GetXmlNodeByPath(root_nd, @"tr(4)\td(2)");
                        quote.price.high = double.NaN;
                        if (nd != null && nd.InnerText != null)
                            double.TryParse(System.Web.HttpUtility.HtmlDecode(nd.InnerText).Trim(), NumberStyles.Number, ci, out quote.price.high);
                        if (debug_en) debug_log += "quote.price.high = '" + quote.price.high.ToString() + "'\r\n";

                        // set N/A values to NaN
                        quote.volume.total = double.NaN;
                        quote.price.ask = double.NaN;
                        quote.price.bid = double.NaN;

                        // update timestamp
                        quote.update_timestamp = DateTime.Now;

                        return quote;
                    }
                    catch { }
                    break;

                case UnderlyingType.Stock:
                    try
                    {
                        Quote quote = new Quote();
                        XmlNode root_nd, nd;

                        quote.stock = ticker;
                        quote.name = GetUnderlyingName(ticker);

                        if (debug_en) debug_log += "Got HTML Page for '" + ticker + "'\nHTML.InnerText: " + xml.InnerText + "\r\n";
                        
                        root_nd = prs.FindXmlNodeByName(xml.FirstChild, "table", "id=bcc_main_data_table");
                        if (root_nd == null) return null;
                        root_nd = root_nd.ParentNode;

                        // get last price
                        nd = prs.GetXmlNodeByPath(root_nd, @"table(1)\tr(2)\td(2)");
                        if (nd == null) return null;
                        quote.price.last = double.NaN;
                        if (nd != null && nd.InnerText != null)
                            double.TryParse(System.Web.HttpUtility.HtmlDecode(nd.InnerText).Trim(), NumberStyles.Number, ci, out quote.price.last);
                        if (debug_en) debug_log += "quote.price.last = '" + quote.price.last.ToString() + "'\r\n";

                        // get open price
                        nd = prs.GetXmlNodeByPath(root_nd, @"table(2)\tr(1)\td(2)");
                        if (nd == null) return null;
                        quote.price.open = double.NaN;
                        if (nd != null && nd.InnerText != null)
                            double.TryParse(System.Web.HttpUtility.HtmlDecode(nd.InnerText).Trim(), NumberStyles.Number, ci, out quote.price.open);
                        if (debug_en) debug_log += "quote.price.open = '" + quote.price.open.ToString() + "'\r\n";

                        // calculate price change
                        try
                        {
                            quote.price.change = Math.Round(quote.price.last - quote.price.open, 2);
                        }
                        catch { quote.price.change = double.NaN; }
                        if (debug_en) debug_log += "quote.price.change = '" + quote.price.change.ToString() + "'\r\n";

                        // get low price
                        nd = prs.GetXmlNodeByPath(root_nd, @"table(3)\tr(3)\td(2)");
                        quote.price.low = double.NaN;
                        if (nd != null && nd.InnerText != null)
                            double.TryParse(System.Web.HttpUtility.HtmlDecode(nd.InnerText).Trim(), NumberStyles.Number, ci, out quote.price.low);
                        if (debug_en) debug_log += "quote.price.low = '" + quote.price.low.ToString() + "'\r\n";

                        // get high price
                        nd = prs.GetXmlNodeByPath(root_nd, @"table(3)\tr(2)\td(2)");
                        quote.price.high = double.NaN;
                        if (nd != null && nd.InnerText != null)
                            double.TryParse(System.Web.HttpUtility.HtmlDecode(nd.InnerText).Trim(), NumberStyles.Number, ci, out quote.price.high);
                        if (debug_en) debug_log += "quote.price.high = '" + quote.price.high.ToString() + "'\r\n";

                        // get volume
                        nd = prs.GetXmlNodeByPath(root_nd, @"table(2)\tr(5)\td(2)");
                        quote.volume.total = double.NaN;
                        if (nd != null && nd.InnerText != null)
                            double.TryParse(System.Web.HttpUtility.HtmlDecode(nd.InnerText).Trim(), NumberStyles.Number, ci, out quote.volume.total);
                        if (debug_en) debug_log += "quote.volume.total = '" + quote.volume.total.ToString() + "'\r\n";

                        // get div node
                        root_nd = prs.GetXmlNodeByPath(xml.FirstChild, @"body\div(2)\div\div(6)\div(2)\div(1)\div(3)");
                        if (root_nd == null) root_nd = prs.GetXmlNodeByPath(xml.FirstChild, @"body\div(2)\div\div(6)\div(2)\div(2)\div(3)");

                        // get ask price
                        nd = prs.GetXmlNodeByPath(root_nd, @"table(1)\tr(2)\td(5)");
                        quote.price.ask = double.NaN;
                        if (nd != null && nd.InnerText != null)
                            double.TryParse(System.Web.HttpUtility.HtmlDecode(nd.InnerText).Trim(), NumberStyles.Number, ci, out quote.price.ask);
                        if (debug_en) debug_log += "quote.price.ask = '" + quote.price.ask.ToString() + "'\r\n";

                        // get bid price
                        nd = prs.GetXmlNodeByPath(root_nd, @"table(2)\tr(2)\td(1)");
                        quote.price.bid = double.NaN;
                        if (nd != null && nd.InnerText != null)
                            double.TryParse(System.Web.HttpUtility.HtmlDecode(nd.InnerText).Trim(), NumberStyles.Number, ci, out quote.price.bid);
                        if (debug_en) debug_log += "quote.price.bid = '" + quote.price.bid.ToString() + "'\r\n";

                        return quote;
                    }
                    catch { }                   
                    break;

                default:
                    return null;
            }

            return null;
        }

        public ArrayList GetOptionsChain(string ticker)
        {
            // get corrected ticker
            ticker = CorrectSymbol(ticker);

            string url = null;
            UnderlyingType type = UnderlyingType.Unknown;

            // get url
            url = GetOptionChainUrl(ticker, out type);
            if (url == null || type == UnderlyingType.Unknown) return null;

            XmlDocument xml = cap.DownloadXmlPartialWebPage(url, "<tbody", "</tbody>", 1, 1);
            if (xml == null) return null;

            ArrayList options_list = new ArrayList();
            options_list.Clear();

            double s_factor, o_factor;
            switch (type)
            {
                case UnderlyingType.Currency:
                    s_factor = 0.001;
                    o_factor = 0.0001;
                    break;
                default:
                    s_factor = 1;
                    o_factor = 0.01;
                    break;
            }

            for (int i = 1; ; i++)
            {
                XmlNode nd, row_nd = prs.GetXmlNodeByPath(xml.FirstChild, @"tbody\tr(" + i.ToString() + @")");
                if (row_nd == null) break;

                Option option = new Option();

                option.stock = ticker;
                option.stocks_per_contract = 100;
                option.update_timestamp = DateTime.Now;

                // type, strike, and symbol
                try
                {
                    // get symbol

                    nd = prs.GetXmlNodeByPath(row_nd, @"td(2)");
                    if (nd == null || nd.InnerText == null) continue;

                    option.symbol = System.Web.HttpUtility.HtmlDecode(nd.InnerText).Trim().Trim();
                    if (debug_en) debug_log += "option.symbol = '" + option.symbol.ToString() + "'\r\n";

                    // get type and strike

                    nd = prs.GetXmlNodeByPath(row_nd, @"td(11)");
                    if (nd == null || nd.InnerText == null) continue;

                    string[] split = System.Web.HttpUtility.HtmlDecode(nd.InnerText).Trim().Trim().Split(new char[] { ' ' });

                    int j, n;
                    for (n = 0, j = -1; j == -1 && n < split.Length; n++)
                        if (split[n] == "C" || split[n] == "P") j = n;
                    if (j == -1) continue;

                    option.type = (split[j] == "C") ? "Call" : "Put";
                    if (debug_en) debug_log += "option.type = '" + option.type.ToString() + "'\r\n";

                    option.strike = double.Parse(split[j + 1]) * s_factor;
                    if (debug_en) debug_log += "option.strike = '" + option.strike.ToString() + "'\r\n";

                    // get expiration date
                    DateTime date = DateTime.Parse("1-" + split[j + 2] + "-" + DateTime.Now.Year.ToString()).AddMonths(1).AddDays(-2);
                    if (date < DateTime.Now) date = date.AddYears(1);

                    while (date.DayOfWeek != DayOfWeek.Thursday)
                        date = date.AddDays(-1);

                    option.expiration = date.AddDays(1);
                    if (debug_en) debug_log += "option.expiration = '" + option.expiration.ToString() + "'\r\n";
                }
                catch { continue; }

                try
                {
                    // open int
                    nd = prs.GetXmlNodeByPath(row_nd, @"td(5)");
                    option.open_int = 0;
                    if (nd != null && nd.InnerText != null)
                        int.TryParse(System.Web.HttpUtility.HtmlDecode(nd.InnerText).Trim().Trim(), NumberStyles.Number, ci, out option.open_int);
                    if (debug_en) debug_log += "option.open_int = '" + option.open_int.ToString() + "'\r\n";

                    // ask / bid prices
                    option.price.ask = double.NaN;
                    option.price.bid = double.NaN;

                    // last price
                    nd = prs.GetXmlNodeByPath(row_nd, @"td(10)");
                    option.price.last = double.NaN;
                    if (nd != null && nd.InnerText != null)
                        double.TryParse(System.Web.HttpUtility.HtmlDecode(nd.InnerText).Trim().Trim(), NumberStyles.Number, ci, out option.price.last);
                    if (debug_en) debug_log += "option.price.last = '" + option.price.last.ToString() + "'\r\n";

                    // price change
                    nd = prs.GetXmlNodeByPath(row_nd, @"td(9)");
                    option.price.change = 0;
                    if (nd != null && nd.InnerText != null)
                    {
                        double d = 0;
                        double.TryParse(System.Web.HttpUtility.HtmlDecode(nd.InnerText).Trim().Trim(), NumberStyles.Number, ci, out d);
                        option.price.change = option.price.last * d / (1.0 + d);
                    }
                    if (debug_en) debug_log += "option.price.change = '" + option.price.change.ToString() + "'\r\n";

                    // volume
                    nd = prs.GetXmlNodeByPath(xml.FirstChild, @"td(6)");
                    option.volume.total = 0;
                    if (nd != null && nd.InnerText != null)
                        double.TryParse(System.Web.HttpUtility.HtmlDecode(nd.InnerText).Trim().Trim(), NumberStyles.Number, ci, out option.volume.total);
                    if (debug_en) debug_log += "option.volume.total = '" + option.volume.total.ToString() + "'\r\n";
                }
                catch { }

                options_list.Add(option);
            }

            if (mode == ModeT.MODE_WITH_ASK_BID_PRICES)
            {
                if (host != null)
                {
                    ThreadPoolT.bw = host.BackgroundWorker;
                    ThreadPoolT.current = 0;
                    ThreadPoolT.last = 0;
                    ThreadPoolT.count = 0;
                    ThreadPoolT.total = options_list.Count;
                }

                List<ManualResetEvent> event_list = new List<ManualResetEvent>();
                ThreadPool.SetMaxThreads(16, 16);

                for (int i = 0; i < options_list.Count; i++)
                {
                    // update option
                    Option option = (Option)options_list[i];

                    // create operation argument
                    ThreadPoolT arg = new ThreadPoolT(option, new ManualResetEvent(false));
                    event_list.Add(arg.done_event);

                    // queue work
                    ThreadPool.QueueUserWorkItem(ThreadPoolCallback, arg);

                    // we wait every 64 since the WaitAll is limited by 64 decsriptors
                    if (event_list.Count == 64)
                    {
                        // wait for work to complete
                        WaitHandle.WaitAll(event_list.ToArray());
                        event_list.Clear();
                    }
                }

                // wait for work to complete
                if (event_list.Count > 0)
                    WaitHandle.WaitAll(event_list.ToArray());

                // reset progress bar
                if (ThreadPoolT.bw != null) ThreadPoolT.bw.ReportProgress(0);
            }

            // update 
            for (int i = 0; i < options_list.Count; i++)
            {
                Option option = (Option)options_list[i];

                option.price.last *= o_factor;
                option.price.change *= o_factor;
                option.price.ask *= o_factor;
                option.price.bid *= o_factor;
            }

            return options_list;
        }

        private class ThreadPoolT
        {
            public static int current = 0;
            public static int last = 0;
            public static int count = 0, total = 0;
            public static BackgroundWorker bw = null;

            public Option option = null;
            public ManualResetEvent done_event = null;

            public ThreadPoolT(Option option, ManualResetEvent done_event) { this.option = option; this.done_event = done_event; }
        }

        private void ThreadPoolCallback(Object obj)
        {
            ThreadPoolT arg = (ThreadPoolT)obj;

            // execute option update
            UpdateOptionDetails(arg.option, false);

            // signal done event
            arg.done_event.Set();

            // update progress bar
            if (ThreadPoolT.bw != null)
            {
                lock (ThreadPoolT.bw)
                {
                    try
                    {
                        ThreadPoolT.count++;

                        ThreadPoolT.current = ThreadPoolT.count * 100 / ThreadPoolT.total;
                        if (ThreadPoolT.current != ThreadPoolT.last)
                        {
                            ThreadPoolT.bw.ReportProgress(ThreadPoolT.current);
                            ThreadPoolT.last = ThreadPoolT.current;
                        }
                    }
                    catch { }
                }
            }
        }

        private void UpdateOptionDetails(Option option, bool use_global_cap)
        {
            if (option.open_int == 0) return;

            XmlDocument xml;

            string url = @"http://www.bizportal.co.il/shukhahon/bizcompquote.shtml?p_id=" + option.symbol;

            if (use_global_cap)
                xml = cap.DownloadXmlPartialWebPage(url, "<body", "</body>", 1, 1);
            else
            {
                OOServerIsraelMaof.WebCapture lcp = new OOServerIsraelMaof.WebCapture();

                lcp.ProxyAddress = cap.ProxyAddress;
                lcp.UseProxy = cap.UseProxy;
                lcp.Encoding = cap.Encoding;
                lcp.Credentials = cap.Credentials;
                lcp.ConnectionsRetries = cap.ConnectionsRetries;
                lcp.UserAgent = null;

                xml = lcp.DownloadXmlPartialWebPage(url, "<body", "</body>", 1, 1);
            }

            // get div node            
            XmlNode root_nd = prs.FindXmlNodeByName(xml.FirstChild, "table", "class=book_table");
            if (root_nd == null) return;
            root_nd = root_nd.ParentNode;

            // ask price
            XmlNode nd = prs.GetXmlNodeByPath(root_nd, @"table(1)\tr(2)\td(5)");
            option.price.ask = double.NaN;
            if (nd != null && nd.InnerText != null)
                double.TryParse(System.Web.HttpUtility.HtmlDecode(nd.InnerText).Trim().Trim(), NumberStyles.Number, ci, out option.price.ask);
            if (option.price.ask <= 0) option.price.ask = double.NaN;
            if (debug_en) debug_log += "option.price.ask = '" + option.price.last.ToString() + "'\r\n";

            // bid price
            nd = prs.GetXmlNodeByPath(root_nd, @"table(2)\tr(2)\td(1)");
            option.price.bid = double.NaN;
            if (nd != null && nd.InnerText != null)
                double.TryParse(System.Web.HttpUtility.HtmlDecode(nd.InnerText).Trim().Trim(), NumberStyles.Number, ci, out option.price.bid);
            if (option.price.bid <= 0) option.price.bid = double.NaN;
            if (debug_en) debug_log += "option.price.bid = '" + option.price.last.ToString() + "'\r\n";
        }

        // get stock/option historical prices 
        public ArrayList GetHistoricalData(string ticker, DateTime start, DateTime end)
        {
            // force en-US culture
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US", false);

            double p_factor;

            // get corrected ticker
            ticker = CorrectSymbol(ticker);

            switch (ticker)
            {
                case "^TA25":
                    p_factor = 0.01;
                    break;

                default:
                    return null;
            }

            ArrayList list = new ArrayList();

            string em = (end.Month - 1).ToString();
            string ed = (end.Day).ToString();
            string ey = (end.Year).ToString();
            string sm = (start.Month - 1).ToString();
            string sd = (start.Day).ToString();
            string sy = (start.Year).ToString();

            string page = cap.DownloadHtmlWebPage(@"http://ichart.yahoo.com/table.csv?s=" + ticker + @"&amp;d=" + em + @"&amp;e=" + ed + @"&amp;f=" + ey + @"&amp;g=d&amp;a=" + sm + @"&amp;b=" + sd + @"&amp;c=" + sy + @"&amp;ignore=.csv");

            string[] split1 = page.Split(new char[] { '\r', '\n' });
            
            for (int i=1; i<split1.Length; i++)
            {
                History history = new History();
                history.stock = ticker;

                try
                {
                    string[] split2 = split1[i].Split(new char[] { ',' });
                    if (split2.Length < 6) continue;

                    history.date = DateTime.Parse(split2[0]);
                    history.price.open = double.Parse(split2[1]) * p_factor;
                    history.price.high = double.Parse(split2[2]) * p_factor;
                    history.price.low = double.Parse(split2[3]) * p_factor;
                    history.price.close = double.Parse(split2[4]) * p_factor;
                    history.price.close_adj = double.Parse(split2[6]) * p_factor;
                    history.volume.total = double.Parse(split2[5]);

                    list.Add(history);
                }
                catch { }
            }

            // update open values
            for (int i = 0; i < list.Count - 1; i++)
                ((History)list[i]).price.open = ((History)list[i + 1]).price.close;
            if (list.Count > 0)
                ((History)list[list.Count - 1]).price.open = ((History)list[list.Count - 1]).price.close;

            return list;
        }

        // get stock name lookup results
        public ArrayList GetStockSymbolLookup(string name)
        {
            return null;
        }

        // get default annual interest rate for specified duration [in years]
        public double GetAnnualInterestRate(double duration)
        {
            return double.NaN;
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
