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
using System.Data;
using OOServerLib.Web;
using OOServerLib.Interface;
using OOServerLib.Global;
using OOServerLib.Config;

namespace OOServerASX
{
    public class Main : WebSite, IServer
    {
        // feature and server array list
        private ArrayList feature_list = new ArrayList();
        private ArrayList server_list = new ArrayList();

        // host
        private IServerHost host = null;

        // connection status
        private bool connect = false;

        // wir interface   
        private Yahoo yho = null;
        private WorldInterestRate wir = null;

        // symbol convert
        private SymbolSet symbol_lut = null;

        // culture
        CultureInfo ci = new CultureInfo("en-AU", false);

        public Main()
        {
            // create interaces
            yho = new Yahoo(cap);
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
        public string Description { get { return "Delayed Quote for ASX Exchange"; } }
        public string Name { get { return "PlugIn Server Australia (ASX)"; } }
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
        public string Mode { get { return Name; } set { } } // not-supported

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
        public void ShowConfigForm(object form) { }

        // default symbol
        public string DefaultSymbol { get { return ""; } }

        public SymbolSet.SymbolTableRow CorrectSymbolRow(string ticker)
        {
            // format ticker
            ticker = ticker.ToUpper().Trim();

            if (symbol_lut == null)
            {
                symbol_lut = new SymbolSet();
                symbol_lut.ConnectionsRetries = cap.ConnectionsRetries;
                symbol_lut.ProxyAddress = cap.ProxyAddress;
                symbol_lut.UseProxy = cap.UseProxy;
                symbol_lut.Load();
            }

            return symbol_lut.SymbolTable.FindByTicker(ticker);
        }

        public string CorrectSymbol(string ticker)
        {
            switch (ticker)
            {
                case "XJO":
                case "ASX200":
                case "ASX 200":
                case "S&P/ASX 200":
                    return "^XJO";
                default:
                    break;
            }

            return ticker;
        }

        // get stock latest quote
        public Quote GetQuote(string ticker)
        {
            SymbolSet.SymbolTableRow tck = CorrectSymbolRow(ticker);            

            // correct symbol
            ticker = CorrectSymbol(ticker);

            // create qutote
            Quote quote = new Quote();

            // get stock symbol / name
            quote.stock = ticker;
            quote.name = ticker;

            // time-stamp
            quote.update_timestamp = DateTime.Now;

            if (ticker.StartsWith("^"))
            {
                // quote / option-chain url
                string url = @"http://www.asx.com.au/asx/statistics/indexInfo.do";

                // get quote page
                XmlDocument xml = cap.DownloadXmlPartialWebPage(url, "<body", "</body>", 1, 1);
                if (xml == null) return null;

                // get quote table
                XmlNode nd, row_nd = null, table_nd = prs.FindXmlNodeByName(xml.FirstChild, "table", "class=datatable indices", 1);
                if (table_nd == null) return null;

                string symbol = ticker.TrimStart(new char[] { '^' });

                for (int i = 1; ; i++)
                {
                    row_nd = prs.GetXmlNodeByPath(table_nd, @"tr(" + i.ToString() + ")");
                    if (row_nd == null) return null;

                    nd = prs.GetXmlNodeByPath(row_nd, @"td(3)");
                    if (nd != null && nd.InnerText == symbol) break;
                }

                // get name          
                nd = prs.GetXmlNodeByPath(row_nd, @"td(2)");
                if (nd != null && nd.InnerText != null)
                    quote.name = System.Web.HttpUtility.HtmlDecode(nd.InnerText).Trim();

                // get last price            
                nd = prs.GetXmlNodeByPath(row_nd, @"td(4)");
                if (nd != null && nd.InnerText != null)
                {
                    quote.price.last = double.NaN;
                    double.TryParse(System.Web.HttpUtility.HtmlDecode(nd.InnerText).Trim(), NumberStyles.Number, ci, out quote.price.last);
                }
                else return null;

                // get price change & open
                nd = prs.GetXmlNodeByPath(row_nd, @"td(5)");
                if (nd != null && nd.InnerText != null)
                {
                    quote.price.change = 0;
                    double.TryParse(System.Web.HttpUtility.HtmlDecode(nd.InnerText).Trim(), NumberStyles.Number, ci, out quote.price.change);
                    quote.price.open = Math.Round(quote.price.last - quote.price.change, 4);
                }

                // set ask/bid/low/high price to none
                quote.price.bid = double.NaN;
                quote.price.ask = double.NaN;           
                quote.price.low = double.NaN;        
                quote.price.high = double.NaN;

                // set volume to none
                quote.volume.total = double.NaN;
            }
            else
            {
                if (tck != null)
                    quote.name = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(tck.Name.ToLower());

                // quote / option-chain url
                string url = @"http://www.asx.com.au/asx/markets/equityPrices.do?by=asxCodes&asxCodes=" + ticker;

                // get quote page
                XmlDocument xml = cap.DownloadXmlPartialWebPage(url, "<body", "</body>", 1, 1);
                if (xml == null) return null;

                // get quote table
                XmlNode nd, table_nd = prs.FindXmlNodeByName(xml.FirstChild, "table", "class=datatable", 1);
                if (table_nd == null) return null;

                // get last price            
                nd = prs.GetXmlNodeByPath(table_nd, @"tr(2)\td(1)");
                if (nd != null && nd.InnerText != null)
                {
                    quote.price.last = double.NaN;
                    double.TryParse(System.Web.HttpUtility.HtmlDecode(nd.InnerText).Trim(), NumberStyles.Number, ci, out quote.price.last);
                }
                else return null;

                // get price change & open
                nd = prs.GetXmlNodeByPath(table_nd, @"tr(2)\td(2)");
                if (nd != null && nd.InnerText != null)
                {
                    quote.price.change = 0;
                    double.TryParse(System.Web.HttpUtility.HtmlDecode(nd.InnerText).Trim(), NumberStyles.Number, ci, out quote.price.change);
                    quote.price.open = Math.Round(quote.price.last - quote.price.change, 4);
                }

                // get bid price            
                nd = prs.GetXmlNodeByPath(table_nd, @"tr(2)\td(3)");
                quote.price.bid = double.NaN;
                if (nd != null && nd.InnerText != null)
                    double.TryParse(System.Web.HttpUtility.HtmlDecode(nd.InnerText).Trim(), NumberStyles.Number, ci, out quote.price.bid);

                // get ask price            
                nd = prs.GetXmlNodeByPath(table_nd, @"tr(2)\td(4)");
                quote.price.ask = double.NaN;
                if (nd != null && nd.InnerText != null)
                    double.TryParse(System.Web.HttpUtility.HtmlDecode(nd.InnerText).Trim(), NumberStyles.Number, ci, out quote.price.ask);

                // get low price            
                nd = prs.GetXmlNodeByPath(table_nd, @"tr(2)\td(7)");
                quote.price.low = double.NaN;
                if (nd != null && nd.InnerText != null)
                    double.TryParse(System.Web.HttpUtility.HtmlDecode(nd.InnerText).Trim(), NumberStyles.Number, ci, out quote.price.low);

                // get high price            
                nd = prs.GetXmlNodeByPath(table_nd, @"tr(2)\td(6)");
                quote.price.high = double.NaN;
                if (nd != null && nd.InnerText != null)
                    double.TryParse(System.Web.HttpUtility.HtmlDecode(nd.InnerText).Trim(), NumberStyles.Number, ci, out quote.price.high);

                // get volume                     
                nd = prs.GetXmlNodeByPath(table_nd, @"tr(2)\td(8)");
                quote.volume.total = double.NaN;
                if (nd != null && nd.InnerText != null)
                    double.TryParse(System.Web.HttpUtility.HtmlDecode(nd.InnerText).Trim(), NumberStyles.Number, ci, out quote.volume.total);
            }

            return quote;
        }

        // get stock latest options chain
        public ArrayList GetOptionsChain(string ticker)
        {
            // correct symbol
            ticker = CorrectSymbol(ticker);

            string text, symbol = ticker.TrimStart(new char[] { '^' });

            // quote / option-chain url
            string url = @"http://www.asx.com.au/asx/markets/optionPrices.do?by=underlyingCode&underlyingCode=" + symbol + @"&expiryDate=&optionType=";

            // get quote page
            XmlDocument xml = cap.DownloadXmlPartialWebPage(url, "<body", "</body>", 1, 1);
            if (xml == null) return null;

            // get quote table
            XmlNode nd, row_nd, table_nd;
            
            table_nd = prs.FindXmlNodeByName(xml.FirstChild, "table", "class=datatable options", 2);
            if (table_nd == null) table_nd = prs.FindXmlNodeByName(xml.FirstChild, "table", "class=datatable options", 1);
            if (table_nd == null) return null;

            // create options array list
            ArrayList options_list = new ArrayList();

            for (int i = 2; ; i++)
            {
                row_nd = prs.GetXmlNodeByPath(table_nd, @"tbody\tr(" + i.ToString() + ")");
                if (row_nd == null) break;

                try
                {
                    Option option = new Option();

                    // set option stock
                    option.stock = ticker;

                    // get option type            
                    nd = prs.GetXmlNodeByPath(row_nd, @"td(2)");
                    text = (nd != null && nd.InnerText != null) ? System.Web.HttpUtility.HtmlDecode(nd.InnerText).Trim() : null;
                    if (text == null) continue;
                    option.type = text;
                    if (option.type != "Call" && option.type != "Put") continue;

                    // get option symbol            
                    nd = prs.GetXmlNodeByPath(row_nd, @"th(1)");
                    text = (nd != null && nd.InnerText != null) ? System.Web.HttpUtility.HtmlDecode(nd.InnerText).Trim() : null;
                    if (text == null) continue;
                    option.symbol = "." + text.ToUpper();

                    // get option expiration date
                    nd = prs.GetXmlNodeByPath(row_nd, @"td(1)");
                    text = (nd != null && nd.InnerText != null) ? System.Web.HttpUtility.HtmlDecode(nd.InnerText).Trim() : null;
                    if (text == null) continue;
                    try
                    {
                        DateTime exp_date = DateTime.Parse(text, ci, DateTimeStyles.None);
                        option.expiration = exp_date;
                    }
                    catch { continue; }

                    // get strike price
                    nd = prs.GetXmlNodeByPath(row_nd, @"td(3)");
                    text = (nd != null && nd.InnerText != null) ? System.Web.HttpUtility.HtmlDecode(nd.InnerText).Trim() : null;
                    if (text == null) continue;
                    option.strike = double.Parse(text, NumberStyles.Number, ci);

                    // get stocks per contract
                    option.stocks_per_contract = 100;

                    // get bid price
                    nd = prs.GetXmlNodeByPath(row_nd, @"td(4)");
                    text = (nd != null && nd.InnerText != null) ? System.Web.HttpUtility.HtmlDecode(nd.InnerText).Trim() : null;
                    if (text == null || text == "" || text == "-") option.price.bid = double.NaN;
                    else
                    {
                        try { option.price.bid = double.Parse(text, NumberStyles.Number, ci); }
                        catch { option.price.bid = double.NaN; }
                    }

                    // get ask price
                    nd = prs.GetXmlNodeByPath(row_nd, @"td(5)");
                    text = (nd != null && nd.InnerText != null) ? System.Web.HttpUtility.HtmlDecode(nd.InnerText).Trim() : null;
                    if (text == null || text == "" || text == "-") option.price.ask = double.NaN;
                    else
                    {
                        try { option.price.ask = double.Parse(text, NumberStyles.Number, ci); }
                        catch { option.price.ask = double.NaN; }
                    }

                    // get last price
                    nd = prs.GetXmlNodeByPath(row_nd, @"td(6)");
                    text = (nd != null && nd.InnerText != null) ? System.Web.HttpUtility.HtmlDecode(nd.InnerText).Trim() : null;
                    if (text == null || text == "" || text == "-") option.price.ask = double.NaN;
                    else
                    {
                        try { option.price.last = double.Parse(text, NumberStyles.Number, ci); }
                        catch { option.price.last = double.NaN; }
                    }

                    // get price change
                    option.price.change = double.NaN;

                    // get open int
                    nd = prs.GetXmlNodeByPath(row_nd, @"td(8)");
                    text = (nd != null && nd.InnerText != null) ? System.Web.HttpUtility.HtmlDecode(nd.InnerText).Trim() : null;
                    if (text == null || text == "" || text == "-") option.open_int = 0;
                    else
                    {
                        try { option.open_int = (int)decimal.Parse(text, NumberStyles.Number, ci); }
                        catch { option.open_int = 0; }
                    }

                    // get volume
                    nd = prs.GetXmlNodeByPath(row_nd, @"td(7)");
                    text = (nd != null && nd.InnerText != null) ? System.Web.HttpUtility.HtmlDecode(nd.InnerText).Trim() : null;
                    if (text == null || text == "" || text == "-") option.volume.total = 0;
                    else
                    {
                        try { option.volume.total = double.Parse(text, NumberStyles.Number, ci); }
                        catch { option.volume.total = double.NaN; }
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
            // correct symbol
            ticker = CorrectSymbol(ticker);

            return yho.GetHistoricalData(ticker, start, end);
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
                    row.Symbol.ToUpper().Contains(name_uc.TrimStart(new char[] { '~', '^' })))
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
            return wir.GetAnnualInterestRate("AUD");
        }

        // get default historical volatility for specified duration [in years]
        public double GetHistoricalVolatility(string ticker, double duration)
        {
            // correct symbol
            ticker = CorrectSymbol(ticker);

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
