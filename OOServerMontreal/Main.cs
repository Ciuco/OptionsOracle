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

namespace OOServerMontreal
{
    public class Main : WebSite, IServer
    {
        // yahoo exchange suffix
        private const string suffix = ".TO";

        // host
        private IServerHost host = null;
        private WorldInterestRate wir = null;

        // connection status
        private bool connect = false;

        // feature and server array list
        private ArrayList feature_list = new ArrayList();
        private ArrayList server_list = new ArrayList();

        // download xml pages cache
        private Dictionary<string, XmlDocument> page_cache = new Dictionary<string, XmlDocument>();

        // culture
        CultureInfo ci = new CultureInfo("en-US", false);

        public Main()
        {
            wir = new WorldInterestRate(cap);

            // update feature list
            feature_list.Add(FeaturesT.SUPPORTS_DELAYED_OPTIONS_CHAIN);
            feature_list.Add(FeaturesT.SUPPORTS_DELAYED_STOCK_QUOTE);

            // update server list
            server_list.Add(Name);
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
        public string Author { get { return "Shlomo Shachar"; } }
        public string Description { get { return "Delayed Quote for Montreal Exchange"; } }
        public string Name { get { return "PlugIn Server Canada (Montreal)"; } }
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
        //public int ConnectionsRetries { get; set; } // implemented by parent class WebSite
        //public string ProxyAddress { get; set; }    // implemented by parent class WebSite
        //public bool UseProxy { get; set; }          // implemented by parent class WebSite

        // debug log control
        public bool LogEnable { get { return false; } set { } }
        public string DebugLog { get { return null; } }

        // configuration form
        public void ShowConfigForm(object form) { }

        // default symbol
        public string DefaultSymbol { get { return ""; } }

        public string CorrectSymbol(string ticker)
        {
            ticker = ticker.ToUpper().Trim().Replace(suffix, "");

            switch (ticker)
            {
                case "SXO":
                    return "^SXO";
                default:
                    break;
            }

            return ticker;
        }

        public string YahooSymbol(string ticker)
        {
            ticker = ticker.ToUpper().Trim().Replace(suffix, "");

            switch (ticker)
            {
                case "^SXO":
                    return "OSP60.TO";
                default:
                    return ticker + ".TO";
            }
        }

        // get stock latest quote
        public Quote GetQuote(string ticker)
        {
            // correct symbol
            ticker = CorrectSymbol(ticker);

            // clear cache
            page_cache.Clear();

            XmlDocument xml = cap.DownloadXmlPartialWebPage(@"http://www.m-x.ca/nego_cotes_en.php?symbol=" + ticker.TrimStart(new char[] { '^' }), "<body", "</body>", 1, 1);
            if (xml == null) return null;

            // save xml in cache
            page_cache.Add(ticker, xml);

            XmlNode nd, root_nd = prs.FindXmlNodeByName(xml.FirstChild, "table", "id=TsansLeft");
            if (root_nd == null) return null;

            nd = prs.FindXmlNodeByName(root_nd.ParentNode, "div", "id=titre");
            if (nd == null || nd.InnerText == null) return null;

            XmlNode table_nd = prs.FindXmlNodeByName(root_nd, "table", "id=TsansBorder");
            if (table_nd == null) return null;

            try
            {
                Quote quote = new Quote();

                quote.name = System.Web.HttpUtility.HtmlDecode(nd.InnerText).Trim();
                if (quote.name.Contains("–") || quote.name.Contains("-")) 
                    quote.name = quote.name.Split(new char[] { '–', '-' })[1].Trim();
                else 
                    quote.name = quote.name.Trim();

                quote.stock = ticker;
                quote.update_timestamp = DateTime.Now;

                nd = prs.GetXmlNodeByPath(table_nd, @"tr\td(2)\strong");
                quote.price.last = double.NaN;
                if (!double.TryParse(nd.InnerText, NumberStyles.Number, ci, out quote.price.last)) return null;

                nd = prs.GetXmlNodeByPath(table_nd, @"tr\td(3)\strong");
                quote.price.change = double.NaN;
                double.TryParse(nd.InnerText, NumberStyles.Number, ci, out quote.price.change);

                quote.price.open = quote.price.last - quote.price.change;

                nd = prs.GetXmlNodeByPath(table_nd, @"tr\td(4)\strong");
                quote.price.bid = double.NaN;
                double.TryParse(nd.InnerText, NumberStyles.Number, ci, out quote.price.bid);

                nd = prs.GetXmlNodeByPath(table_nd, @"tr\td(5)\strong");
                quote.price.ask = double.NaN;
                double.TryParse(nd.InnerText, NumberStyles.Number, ci, out quote.price.ask);

                quote.price.high = double.NaN;
                quote.price.low = double.NaN;

                quote.volume.total = double.NaN;
                quote.general.dividend_rate = 0;

                // fallback last price to mid-price
                if (quote.price.last == 0 && quote.price.bid > 0 && !double.IsNaN(quote.price.bid) && quote.price.ask > 0 && !double.IsNaN(quote.price.ask))
                    quote.price.last = (quote.price.bid + quote.price.ask) * 0.5;

                return quote;
            }
            catch { }

            return null;
        }

        // get stock latest options chain
        public ArrayList GetOptionsChain(string ticker)
        {
            // correct symbol
            ticker = CorrectSymbol(ticker);

            XmlDocument xml = null;

            if (page_cache.ContainsKey(ticker)) xml = page_cache[ticker];
            else
            {
                xml = cap.DownloadXmlPartialWebPage(@"http://www.m-x.ca/nego_cotes_en.php?symbol=" + ticker.TrimStart(new char[] { '^' }), "<body", "</body>", 1, 1);
                if (xml == null) return null;

                // save xml in cache
                page_cache.Add(ticker, xml);
            }

            List<XmlNode> root_nd_list = new List<XmlNode>();

            for (int i = 1; ; i++)
            {
                XmlNode temp_nd = prs.FindXmlNodeByName(xml.FirstChild, "div", "id=titreElement", i);
                while (temp_nd != null && temp_nd.Name != "table") temp_nd = temp_nd.ParentNode;
                if (temp_nd == null) break;
                else if (!root_nd_list.Contains(temp_nd)) root_nd_list.Add(temp_nd);
            }

            // create options array list
            ArrayList options_list = new ArrayList();
            options_list.Clear();
            options_list.Capacity = 1024;

            foreach (XmlNode root_nd in root_nd_list)
            {
                string[] mon_list = new string[] { "JA", "FE", "MR", "AL", "MA", "JN", "JL", "AU", "SE", "OC", "NV", "DE" };

                for (int j = 1; j <= 2; j++)
                {
                    XmlNode table_nd = prs.GetXmlNodeByPath(root_nd, @"tr\td(" + j.ToString() + @")\table");
                    if (table_nd == null) continue;

                    for (int i = 2; ; i++)
                    {
                        XmlNode cell_nd, row_nd = prs.GetXmlNodeByPath(table_nd, @"tr(" + i.ToString() + @")");
                        if (row_nd == null) break;

                        try
                        {
                            Option option = new Option();

                            // option stock ticker and number of stocks per contract
                            option.stock = ticker;
                            option.stocks_per_contract = 100;
                            option.update_timestamp = DateTime.Now;

                            // option type
                            if (j == 2) option.type = "Put";
                            else if (j == 1) option.type = "Call";
                            else continue;

                            // get option detail link
                            string row_title = null;
                            try
                            {
                                if (row_nd.Attributes != null)
                                {
                                    foreach (XmlAttribute attr in row_nd.Attributes) if (attr.Name == "title")
                                        {
                                            row_title = System.Web.HttpUtility.HtmlDecode(attr.Value).Trim();
                                            break;
                                        }
                                }
                            }
                            catch { }
                            if (row_title == null) continue;

                            string[] title_split = row_title.Split(new char[] { '[', ']' });

                            option.symbol = null;
                            option.open_int = 0;
                            option.strike = double.NaN;

                            for (int k = 1; k < title_split.Length; k++)
                            {
                                try
                                {
                                    switch (title_split[k - 1].Trim())
                                    {
                                        case "header=":
                                            {
                                                string last_item = null;
                                                string[] split = title_split[k].Replace("\u00a0", " ").Split(new char[] { ' ' });

                                                option.symbol = "";
                                                foreach (string item in split)
                                                    if (item != "")
                                                    {
                                                        option.symbol += item;
                                                        last_item = item;
                                                    }
                                                if (option.symbol != "") option.symbol = "." + option.symbol;

                                                if (last_item != null)
                                                {
                                                    split = last_item.Split(new char[] { 'C', 'P' });
                                                    double.TryParse(split[split.Length - 1].Trim(), NumberStyles.Number, ci, out option.strike);
                                                }
                                                else option.strike = double.NaN;
                                            }
                                            break;
                                        case "body=":
                                            {
                                                string[] split = title_split[k].Replace("\u00a0", " ").Split(new char[] { ':' });
                                                int.TryParse(split[split.Length - 1].Trim(), NumberStyles.Number, ci, out option.open_int);
                                            }
                                            break;
                                    }
                                }
                                catch { }
                            }

                            if (option.symbol == null || option.strike == 0 || double.IsNaN(option.strike)) continue;

                            // option expiration
                            cell_nd = prs.GetXmlNodeByPath(row_nd, @"td(1)");
                            if (cell_nd == null || cell_nd.InnerText == null) continue;

                            int exp_month = 0, exp_year = 0;
                            string exp_string = System.Web.HttpUtility.HtmlDecode(cell_nd.InnerText).Trim();
                            string[] exp_split = exp_string.Split(new char[] { exp_string[1] });

                            int.TryParse(exp_split[1], NumberStyles.Number, ci, out exp_year);
                            for (int k = 0; k < mon_list.Length && exp_month == 0; k++)
                                if (exp_split[2].Trim() == mon_list[k]) exp_month = k + 1;
                            if (exp_month == 0 || exp_year == 0) continue;
                            option.expiration = new DateTime(exp_year + 2000, exp_month, 1);

                            // get the day after the 3rd firday
                            int days_to_1st_friday = (int)DayOfWeek.Friday - (int)option.expiration.DayOfWeek;
                            if (days_to_1st_friday < 0) days_to_1st_friday += 7;
                            option.expiration = option.expiration.AddDays(15 + days_to_1st_friday);

                            // option bid price
                            cell_nd = prs.GetXmlNodeByPath(row_nd, @"td(2)");
                            option.price.bid = double.NaN;
                            if (cell_nd != null && cell_nd.InnerText != null)
                                double.TryParse(cell_nd.InnerText, NumberStyles.Number, ci, out option.price.bid);
                            if (option.price.bid <= 0) option.price.bid= double.NaN;

                            // option ask price
                            cell_nd = prs.GetXmlNodeByPath(row_nd, @"td(3)");
                            option.price.ask = double.NaN;
                            if (cell_nd != null && cell_nd.InnerText != null)
                                double.TryParse(cell_nd.InnerText, NumberStyles.Number, ci, out option.price.ask);
                            if (option.price.ask <= 0) option.price.ask = double.NaN;

                            // option last price
                            cell_nd = prs.GetXmlNodeByPath(row_nd, @"td(4)");
                            option.price.last = double.NaN;
                            if (cell_nd != null && cell_nd.InnerText != null)
                                double.TryParse(cell_nd.InnerText, NumberStyles.Number, ci, out option.price.last);

                            if (ticker.StartsWith("^"))
                            {
                                // option last price
                                cell_nd = prs.GetXmlNodeByPath(row_nd, @"td(5)");
                                option.price.change = double.NaN;
                                if (cell_nd != null && cell_nd.InnerText != null)
                                    double.TryParse(cell_nd.InnerText, NumberStyles.Number, ci, out option.price.change);

                                // option volume
                                cell_nd = prs.GetXmlNodeByPath(row_nd, @"td(6)");
                                option.volume.total = 0;
                                if (cell_nd != null && cell_nd.InnerText != null)
                                    double.TryParse(cell_nd.InnerText, NumberStyles.Number, ci, out option.volume.total);
                            }
                            else
                            {
                                // no price change information
                                option.price.change = double.NaN;

                                // option volume
                                cell_nd = prs.GetXmlNodeByPath(row_nd, @"td(5)");
                                option.volume.total = 0;
                                if (cell_nd != null && cell_nd.InnerText != null)
                                    double.TryParse(cell_nd.InnerText, NumberStyles.Number, ci, out option.volume.total);
                            }

                            options_list.Add(option);
                        }
                        catch { }
                    }
                }
            }

            return options_list;
        }

        // get stock/option historical prices 
        public ArrayList GetHistoricalData(string ticker, DateTime start, DateTime end)
        {
            // correct symbol
            ticker = CorrectSymbol(ticker);

            double p_factor = 1.0;

            ArrayList list = new ArrayList();

            string em = (end.Month - 1).ToString();
            string ed = (end.Day).ToString();
            string ey = (end.Year).ToString();
            string sm = (start.Month - 1).ToString();
            string sd = (start.Day).ToString();
            string sy = (start.Year).ToString();

            string page = cap.DownloadHtmlWebPage(@"http://ichart.yahoo.com/table.csv?s=" + YahooSymbol(ticker) + @"&d=" + em + @"&e=" + ed + @"&f=" + ey + @"&g=d&a=" + sm + @"&b=" + sd + @"&c=" + sy + @"&ignore=.csv");

            string[] split1 = page.Split(new char[] { '\r', '\n' });

            for (int i = 1; i < split1.Length; i++)
            {
                History history = new History();
                history.stock = ticker;

                try
                {
                    string[] split2 = split1[i].Split(new char[] { ',' });
                    if (split2.Length < 6) continue;

                    history.date = Convert.ToDateTime(split2[0], ci);
                    history.price.open = Convert.ToDouble(split2[1], ci) * p_factor;
                    history.price.high = Convert.ToDouble(split2[2], ci) * p_factor;
                    history.price.low = Convert.ToDouble(split2[3], ci) * p_factor;
                    history.price.close = Convert.ToDouble(split2[4], ci) * p_factor;
                    history.price.close_adj = Convert.ToDouble(split2[6], ci) * p_factor;
                    history.volume.total = Convert.ToDouble(split2[5], ci);

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
            string lookup_url = @"http://finance.yahoo.com/lookup?s=" + name.Replace(suffix, "") + @"&t=S&m=ALL";

            XmlDocument xml = cap.DownloadXmlWebPage(lookup_url);
            if (xml == null) return null;

            ArrayList symbol_list = new ArrayList();
            symbol_list.Capacity = 256;

            for (int i = 0; i < symbol_list.Capacity; i++)
            {
                string entry = "";

                XmlNode nd, root_node = prs.GetXmlNodeByPath(xml.FirstChild, @"body\div\br\br\table\tr(3)\td\table(2)\tr(3)\td\table\tr\td\table\tr(" + (i + 2).ToString() + @")");
                if (root_node == null) break;

                // stock name
                nd = prs.GetXmlNodeByPath(root_node, @"td(2)");
                if (nd == null) break;
                entry = nd.InnerText.Replace('(', '[').Replace(')', ']');

                // stock ticker
                nd = prs.GetXmlNodeByPath(root_node, @"td(1)\a");
                if (nd == null) break;

                int x = nd.InnerText.IndexOf('.');
                if (x >= 0) entry += nd.InnerText.Substring(0, x);
                else entry += " (" + nd.InnerText + ")";

                // add name + ticker entry
                if (entry.Contains(suffix)) symbol_list.Add(entry.Replace(suffix, ""));
            }

            symbol_list.TrimToSize();
            return symbol_list;
        }

        // get default annual interest rate for specified duration [in years]
        public double GetAnnualInterestRate(double duration)
        {
            return wir.GetAnnualInterestRate("CAD");
        }

        // get default historical volatility for specified duration [in years]
        public double GetHistoricalVolatility(string ticker, double duration)
        {
            // get historical data
            ArrayList list = GetHistoricalData(ticker, DateTime.Now.AddDays(-duration * 365), DateTime.Now);

            // calculate historical value
            return 100.0 * HistoryVolatility.HighLowParkinson(list);
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
