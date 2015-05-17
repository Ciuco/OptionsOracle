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

using OOServerLib.Web;
using OOServerLib.Interface;
using OOServerLib.Global;
using OOServerLib.Config;

namespace OOServerPhiladelphia
{
    public class Main : WebSite, IServer
    {
        // host
        private IServerHost host = null;
        private WorldInterestRate wir = null;

        // connection status
        private bool connect = false;

        // feature and server array list
        private ArrayList feature_list = new ArrayList();
        private ArrayList server_list = new ArrayList();

        // symbol convert
        private SymbolSet symbol_lut = null;

        // dictionary
        private Dictionary<string, HtmlDocument> cache_dict = new Dictionary<string, HtmlDocument>();

        // culture
        CultureInfo ci = new CultureInfo("en-US", false);

        private WebForm wbf;

        public Main()
        {
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
        public string Author { get { return "Shlomo Shachar"; } }
        public string Description { get { return "Delayed Quote for Philadelphia Exchange"; } }
        public string Name { get { return "PlugIn Server PHLX (Philadelphia)"; } }
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

        private Quote GetQuoteFromDocument(HtmlDocument doc, SymbolSet.SymbolTableRow tck)
        {
            // locate table of "Last Sale" element
            HtmlElement table_elem = WebForm.LocateParentElement(doc, "Last Sale", 0, "TABLE");
            if (table_elem == null) return null;

            // get quote table
            XmlDocument xml = cap.ConvertHtmlToXml(table_elem.OuterHtml);
            if (xml == null) return null;

            XmlNode nd, row_nd = prs.GetXmlNodeByPath(xml.FirstChild, @"TBODY\TR(2)");
            if (row_nd == null) return null;

            // create qutote
            Quote quote = new Quote();

            // get date and time
            quote.update_timestamp = DateTime.Now;

            // get underlying symbol
            quote.stock = tck.Symbol;

            // get underlying name
            nd = prs.GetXmlNodeByPath(row_nd, @"TD(1)");
            if (nd != null && !string.IsNullOrEmpty(nd.InnerText))
                quote.name = System.Web.HttpUtility.HtmlDecode(nd.InnerText).Trim().Trim();
            else
                quote.name = tck.Name;

            // get underlying last price
            nd = prs.GetXmlNodeByPath(row_nd, @"TD(4)");
            quote.price.last = double.NaN;
            if (nd != null && !string.IsNullOrEmpty(nd.InnerText))
                try
                {
                    quote.price.last = Convert.ToDouble(System.Web.HttpUtility.HtmlDecode(nd.InnerText).Trim(), ci);
                }
                catch { }

            // get underlying price change
            nd = prs.GetXmlNodeByPath(row_nd, @"TD(5)\SPAN\#text");
            quote.price.change = double.NaN;
            if (nd != null && !string.IsNullOrEmpty(nd.InnerText))
                try
                {
                    quote.price.change = Convert.ToDouble(System.Web.HttpUtility.HtmlDecode(nd.InnerText).Trim(), ci);
                }
                catch { }

            // get open price
            quote.price.open = quote.price.last - quote.price.change;

            // set unsupported data to NaN
            quote.price.low = double.NaN;
            quote.price.high = double.NaN;
            quote.price.bid = double.NaN;
            quote.price.ask = double.NaN;
            quote.volume.total = double.NaN;

            return quote;
        }

        // get stock latest quote
        public Quote GetQuote(string ticker)
        {
            // correct symbol
            SymbolSet.SymbolTableRow tck = CorrectSymbolRow(ticker);
            if (tck == null) return null;

            // clear global cache
            cache_dict.Clear();

            if (tck.Symbol.StartsWith("^"))
            {
                // get url for quote page
                string url = @"http://www.nasdaq.com/asp/currency-options.asp?currency=" + tck.Symbol;
                
                // read first page (try twice before giving up)
                HtmlDocument doc = wbf.GetHtmlDocumentWithWebBrowser(url, null, null, null, 40);
                if (doc == null || doc.Body == null || string.IsNullOrEmpty(doc.Body.InnerText)) return null;
                cache_dict.Add(tck.Symbol, doc);

                return GetQuoteFromDocument(doc, tck);
            }
            else
            {
                // get url for quote page
                string url = @"http://www.nasdaq.com/aspxcontent/options.aspx?symbol=" + tck.Symbol + "&selected=" + tck.Symbol;

                // read first page (try twice before giving up)
                HtmlDocument doc = wbf.GetHtmlDocumentWithWebBrowser(url, null, null, null, 40);
                if (doc == null || doc.Body == null || string.IsNullOrEmpty(doc.Body.InnerText)) return null;
                cache_dict.Add(tck.Symbol, doc);

                return GetQuoteFromDocument(doc, tck);
            }
        }

        private ArrayList GetOptionChainFromDocument(HtmlDocument doc, SymbolSet.SymbolTableRow tck, DateTime expdate)
        {
            // locate table of "Open Int" element
            HtmlElement table_elem = WebForm.LocateParentElement(doc, "Open Int", 0, "TABLE");
            if (table_elem == null) return null;

            // get quote table
            XmlDocument xml = cap.ConvertHtmlToXml(table_elem.OuterHtml);
            if (xml == null) return null;

            // create options array list
            ArrayList options_list = new ArrayList();

            for (int i = 2; ; i++)
            {
                XmlNode nd, row_nd = prs.GetXmlNodeByPath(xml.FirstChild, @"TBODY\TR(" + i.ToString() + ")");
                if (row_nd == null) break;

                // get strike
                nd = prs.GetXmlNodeByPath(row_nd, @"TD(9)");
                double strike = double.NaN;
                if (nd != null && !string.IsNullOrEmpty(nd.InnerText))
                    try
                    {
                        strike = Convert.ToDouble(System.Web.HttpUtility.HtmlDecode(nd.InnerText).Trim(), ci);
                    }
                    catch { }
                if (double.IsNaN(strike)) continue;

                for (int j = 0; j <= 9; j+=9)
                { 
                    // create new option
                    Option option = new Option();

                    // type
                    option.type = (j == 0) ? "Call" : "Put";
                    
                    // strike
                    option.strike = strike;

                    // underlying
                    option.stock = tck.Symbol;

                    // update time stamp
                    option.update_timestamp = DateTime.Now;

                    // exipration date
                    nd = prs.GetXmlNodeByPath(row_nd, @"TD(" + (j + 1).ToString() + ")");
                    if (nd == null || string.IsNullOrEmpty(nd.InnerText)) break;                    
                    string[] split = nd.InnerText.Replace(",", "").Split(' ');
                    option.expiration = expdate; 
                    DateTime.TryParse(split[1] + "-" + split[0] + "-" + split[2], ci, DateTimeStyles.None, out option.expiration);

                    // symbol
                    option.symbol = string.Format(".{0}{1:yyMMdd}{2}{3:00000000}", tck.Symbol.TrimStart(new char[] { '^' }), option.expiration, option.type[0], option.strike * 1000);

                    // get last price
                    nd = prs.GetXmlNodeByPath(row_nd, @"TD(" + (j + 2).ToString() + ")");
                    option.price.last = double.NaN;
                    if (nd != null && !string.IsNullOrEmpty(nd.InnerText))
                        try
                        {
                            string stmp = System.Web.HttpUtility.HtmlDecode(nd.InnerText).Trim();
                            if (!string.IsNullOrEmpty(stmp)) option.price.last = Convert.ToDouble(stmp, ci);
                        }
                        catch { }

                    // get price change
                    nd = prs.GetXmlNodeByPath(row_nd, @"TD(" + (j + 3).ToString() + ")");
                    option.price.change = double.NaN;
                    if (nd != null && !string.IsNullOrEmpty(nd.InnerText))
                        try
                        {
                            string stmp = System.Web.HttpUtility.HtmlDecode(nd.InnerText).Trim();
                            if (!string.IsNullOrEmpty(stmp)) option.price.change = Convert.ToDouble(stmp, ci);
                        }
                        catch { }

                    // get bid price
                    nd = prs.GetXmlNodeByPath(row_nd, @"TD(" + (j + 4).ToString() + ")");
                    option.price.bid = double.NaN;
                    if (nd != null && !string.IsNullOrEmpty(nd.InnerText))
                        try
                        {
                            string stmp = System.Web.HttpUtility.HtmlDecode(nd.InnerText).Trim();
                            if (!string.IsNullOrEmpty(stmp)) option.price.bid = Convert.ToDouble(stmp, ci);
                        }
                        catch { }

                    // get ask price
                    nd = prs.GetXmlNodeByPath(row_nd, @"TD(" + (j + 5).ToString() + ")");
                    option.price.ask = double.NaN;
                    if (nd != null && !string.IsNullOrEmpty(nd.InnerText))
                        try
                        {
                            string stmp = System.Web.HttpUtility.HtmlDecode(nd.InnerText).Trim();
                            if (!string.IsNullOrEmpty(stmp)) option.price.ask = Convert.ToDouble(stmp, ci);
                        }
                        catch { }

                    // get volume
                    nd = prs.GetXmlNodeByPath(row_nd, @"TD(" + (j + 6).ToString() + ")");
                    option.volume.total = 0;
                    if (nd != null && !string.IsNullOrEmpty(nd.InnerText))
                        try
                        {
                            string stmp = System.Web.HttpUtility.HtmlDecode(nd.InnerText).Trim();
                            if (!string.IsNullOrEmpty(stmp)) option.volume.total = Convert.ToInt32(stmp, ci);
                        }
                        catch { }

                    // get open interest
                    nd = prs.GetXmlNodeByPath(row_nd, @"TD(" + (j + 7).ToString() + ")");
                    option.open_int = 0;
                    if (nd != null && !string.IsNullOrEmpty(nd.InnerText))
                        try
                        {
                            string stmp = System.Web.HttpUtility.HtmlDecode(nd.InnerText).Trim();
                            if (!string.IsNullOrEmpty(stmp)) option.open_int = Convert.ToInt32(stmp, ci);
                        }
                        catch { }

                    // add option to option-list
                    options_list.Add(option);
                }
            }

            return options_list;
        }

        // get stock latest options chain
        public ArrayList GetOptionsChain(string ticker)
        {
            // date prefix list
            const string DatePrefixList = "Jan Feb Mar Apr May Jun Jul Aug Sep Oct Nov Dec ";

            // correct symbol
            SymbolSet.SymbolTableRow tck = CorrectSymbolRow(ticker);
            if (tck == null || !cache_dict.ContainsKey(tck.Symbol)) return null;

            // get first document
            HtmlDocument doc = cache_dict[tck.Symbol];

            // global arraylist
            ArrayList option_list = new ArrayList();

            // progress status
            int current, last = 0, total = 0, count = 0;
            BackgroundWorker bw = null;
            if (host != null) bw = host.BackgroundWorker;

            // rotate 
            while (true)
            {
                DateTime exp_date = DateTime.MinValue;

                // locate table of "All" element
                HtmlElement table_elem = WebForm.LocateParentElement(doc, "All", 1, "TABLE");                
                if (table_elem == null) break;

                HtmlElementCollection list_elem = table_elem.GetElementsByTagName("SPAN");
                if (list_elem == null || list_elem.Count == 0) break;

                HtmlElement date_elem = null;

                foreach (HtmlElement elem in list_elem)
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(elem.InnerText) &&
                            elem.InnerText.Trim().Length == 6 &&
                            DatePrefixList.Contains(elem.InnerText.Trim().Substring(0, 4)))
                        {
                            date_elem = elem;
                            exp_date = Convert.ToDateTime("1 " + date_elem.InnerText, ci); // option expiration
                            break;
                        }
                    }
                    catch { }
                }

                if (date_elem == null) break;

                // get the day after the 3rd firday
                int days_to_1st_friday = (int)DayOfWeek.Friday - (int)exp_date.DayOfWeek;
                if (days_to_1st_friday < 0) days_to_1st_friday += 7;
                exp_date = exp_date.AddDays(15 + days_to_1st_friday);

                while (true)
                {
                    // parse page and add to global result
                    ArrayList page_list = GetOptionChainFromDocument(cache_dict[tck.Symbol], tck, exp_date);
                    if (page_list != null && page_list.Count > 0) option_list.AddRange(page_list);

                    // locate text element
                    List<HtmlElement> click_elem = new List<HtmlElement>();
                    WebForm.LocateTextByWebBrowser(doc.Body, "Next Page >>", click_elem);
                    if (click_elem.Count == 0) break;

                    // get "Next Page >>" document
                    if (click_elem != null && click_elem.Count > 0)
                        doc = wbf.GetNavigatedHtmlDocumentWithWebBrowser(click_elem[0], null, null, null, 20);
                    else
                        break;
                }

                // locate table of "All" element
                table_elem = WebForm.LocateParentElement(doc, "All", 1, "TABLE");
                if (table_elem == null) break;

                list_elem = table_elem.GetElementsByTagName("A");
                if (list_elem == null || list_elem.Count == 0) break;

                // next element
                HtmlElement next_elem = null;

                total = 1;
                foreach (HtmlElement elem in list_elem)
                {
                    if (!string.IsNullOrEmpty(elem.InnerText) &&
                        elem.InnerText.Trim().Length == 6 &&
                        DatePrefixList.Contains(elem.InnerText.Trim().Substring(0, 4)))
                    {
                        try
                        {
                            if (next_elem == null &&
                                Convert.ToDateTime("1 " + elem.InnerText, ci) > exp_date) next_elem = elem;

                            total++;
                        }
                        catch { }
                    }
                }

                if (next_elem != null && next_elem.InnerText != "All")
                    doc = wbf.GetNavigatedHtmlDocumentWithWebBrowser(next_elem, null, null, null, 60);
                else
                    break;

                // update progress bar
                count++;

                // update progress bar
                if (bw != null && total > 0)
                {
                    current = count * 100 / total;
                    if (current != last)
                    {
                        bw.ReportProgress(current);
                        last = current;
                    }
                }
            }

            return option_list;
        }

        // get stock/option historical prices 
        public ArrayList GetHistoricalData(string ticker, DateTime start, DateTime end)
        {
            // correct symbol
            SymbolSet.SymbolTableRow tck = CorrectSymbolRow(ticker);
            if (tck == null) return null;

            double p_factor = 1.0;

            ArrayList list = new ArrayList();

            string em = (end.Month - 1).ToString();
            string ed = (end.Day).ToString();
            string ey = (end.Year).ToString();
            string sm = (start.Month - 1).ToString();
            string sd = (start.Day).ToString();
            string sy = (start.Year).ToString();

            string page = cap.DownloadHtmlWebPage(@"http://ichart.yahoo.com/table.csv?s=" + tck.Symbol + @"&d=" + em + @"&e=" + ed + @"&f=" + ey + @"&g=d&a=" + sm + @"&b=" + sd + @"&c=" + sy + @"&ignore=.csv");

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
            ArrayList symbol_list = new ArrayList();
            symbol_list.Capacity = 256;

            string name_uc = name.ToUpper();

            foreach (SymbolSet.SymbolTableRow row in symbol_lut.SymbolTable.Rows)
            {
                if (row.Name.ToUpper().Contains(name_uc) ||
                    row.Symbol.ToUpper().Contains(name_uc))
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
            return wir.GetAnnualInterestRate("USD");
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
