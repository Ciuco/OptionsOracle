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
using System.Net;
using System.ComponentModel;
using System.Reflection;
using OOServerLib.Web;
using OOServerLib.Interface;
using OOServerLib.Global;
using OOServerLib.Config;

namespace OOServerBovespaTradeZone
{
    public class Main : WebSite, IServer
    {
        // configuration file
        private const string CONFIG_FILE = "plugin_bovespa_tradezone_config.xml";

        // configuration dictionary
        SerializableDictionary<string, string> config = new SerializableDictionary<string, string>();

        // trade-zone data-center web-client        
        TradeZone data_center = new TradeZone();

        // dictionary
        Dictionary<string, Quote> quote_dict = new Dictionary<string, Quote>();

        // end-of-day bovespa
        //OOServerBovespa.Bovespa Bovespa = new OOServerBovespa.Bovespa();

        // host
        private IServerHost host = null;

        // feature and server array list
        private ArrayList feature_list = new ArrayList();
        private ArrayList server_list = new ArrayList();

        public Main()
        {
            // update feature list
            feature_list.Add(FeaturesT.SUPPORTS_REALTIME_OPTIONS_CHAIN);
            feature_list.Add(FeaturesT.SUPPORTS_REALTIME_STOCK_QUOTE);
            feature_list.Add(FeaturesT.SUPPORTS_CONFIG_FORM);

            // update server list
            server_list.Add(Name);

            // load configuration from file
            Load();
        }

        public void Initialize(string config)
        {
        }

        public void Dispose()
        {
            // disconnect
            Connect = false;
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
        public string Author { get { return "Ori Kop"; } }
        public string Description { get { return "TradeZone Quote for Bovespa Brazil"; } }
        public string Name { get { return "PlugIn Server TradeZone Brazil (Bovespa)"; } }
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
        public string Mode { get { return null; } set { } }

        // set/get callback host
        public IServerHost Host { get { return host; } set { host = value; } }

        // connect/disconnect to server
        public bool Connect
        { 
            get 
            {
                return !string.IsNullOrEmpty(data_center.LoginId); 
            } 
        
            set 
            {
                if (value && string.IsNullOrEmpty(data_center.LoginId))
                {
                    // connect
                    data_center.Login();
                }
                else if (!value && !string.IsNullOrEmpty(data_center.LoginId))
                {
                    // disconnect
                    data_center.Logout();
                }
            } 
        }

        //// connection settings
        //public int ConnectionsRetries { get { return Bovespa.ConnectionsRetries; } set { Bovespa.ConnectionsRetries = value; } }
        //public string ProxyAddress { get { return Bovespa.ProxyAddress; } set { Bovespa.ProxyAddress = value; } }
        //public bool UseProxy { get { return Bovespa.UseProxy; } set { Bovespa.UseProxy = value; } }

        // debug log control
        public bool LogEnable { get { return data_center.LogEnable; } set { data_center.LogEnable = value; } }
        public string DebugLog { get { return data_center.DebugLog; } }

        private void Load()
        {
            // get config directory path
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\OptionsOracle\";
            string conf = path + CONFIG_FILE;

            // check if config directory exist, if not create it
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            // load / create-new configuration
            try
            {
                if (File.Exists(conf))
                {
                    // load configuration
                    XmlTextReader reader = new XmlTextReader(conf);
                    config.ReadXml(reader);
                }
                else
                {
                    // default config
                    config.Add("MonthsCount", "4");
                    config.Add("StrikeLowerLimit", "80");
                    config.Add("StrikeUpperLimit", "120");
                    config.Add("HideOptionsWithNoMarket", true.ToString());
                }
            }
            catch { }
        }

        private void Save()
        {
            // get config directory path
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\OptionsOracle\";
            string conf = path + CONFIG_FILE;

            // check if config directory exist, if not create it
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            // load / create-new configuration
            try
            {
                // load configuration
                XmlTextWriter writer = new XmlTextWriter(conf, null);
                config.WriteXml(writer);
                writer.Close();
            }
            catch { }
        }

        // configuration form
        public void ShowConfigForm(object form)
        {
            ConfigForm configForm = new ConfigForm();

            configForm.MonthsCount = int.Parse(config["MonthsCount"]);
            configForm.StrikeLowerLimit = int.Parse(config["StrikeLowerLimit"]);
            configForm.StrikeUpperLimit = int.Parse(config["StrikeUpperLimit"]);
            configForm.HideOptionsWithNoMarket = bool.Parse(config["HideOptionsWithNoMarket"]);

            if (configForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                // update configuration dictionary
                config["MonthsCount"] = configForm.MonthsCount.ToString();
                config["StrikeLowerLimit"] = configForm.StrikeLowerLimit.ToString();
                config["StrikeUpperLimit"] = configForm.StrikeUpperLimit.ToString();
                config["HideOptionsWithNoMarket"] = configForm.HideOptionsWithNoMarket.ToString();

                // save configuration to file
                Save();
            }
        }

        // default symbol
        public string DefaultSymbol { get { return ""; } }
               
        public string CorrectSymbol(string ticker)
        {
            ticker = ticker.ToUpper().Trim();

            switch (ticker)
            {
                case "IBOVESPA":
                case "IBOV":
                case "^IBOV":
                case "^IBOVESPA":
                    return "^IBOV";
                default:
                    break;
            }

            return ticker;
        }

        // get stock latest quote
        public Quote GetQuote(string ticker)
        {
            // make sure we are connected
            if (!Connect)
            {
                Connect = true;
                if (!Connect) return null;
            }

            // correct symbol
            ticker = CorrectSymbol(ticker);

            // trade-zone ticker
            string ticker_tz = ticker.TrimStart(new char[] { '^' });

            // create one symbol list
            List<string> sym_list = new List<string>();
            sym_list.Add(ticker_tz);

            // get tick-list and mini-book list            
            Dictionary<string, TradeZone.Tick> ord_list = data_center.GetQuoteTick(sym_list);
            Dictionary<string, TradeZone.Minibook> mnb_list = data_center.GetQuoteMiniBook(sym_list);
            if (ord_list == null || mnb_list == null) return null;

            // get symbol lookup
            string sym_lookup = data_center.GetSymbolsStream(ticker_tz);

            if (ord_list.ContainsKey(ticker_tz))
            {
                TradeZone.Tick tick = ord_list[ticker_tz];

                Quote quote = new Quote();

                quote.name = tick.Stock;
                quote.stock = ticker;

                if (sym_lookup != null)
                {
                    string[] sym_split = sym_lookup.Split(new char[] { ';' });
                    if (sym_split.Length > 1) quote.name = sym_split[1].Substring(0, 1).ToUpper() + sym_split[1].Substring(1).ToLower();
                }

                quote.price.open = tick.Open;
                quote.price.last = tick.Last;
                quote.price.change = quote.price.last - quote.price.open;
                quote.price.high = tick.High;
                quote.price.low = tick.Low;

                if (mnb_list.ContainsKey(ticker_tz))
                {
                    TradeZone.Minibook minibook = mnb_list[ticker_tz];

                    if (minibook != null && minibook.Items.Count > 0)
                    {
                        quote.price.ask = minibook.Items[0].BestAskPrice;
                        quote.price.bid = minibook.Items[0].BestBidPrice;
                    }
                }

                quote.update_timestamp = tick.Timestamp;
                quote.volume.total = (double)tick.TradeVolume;

                // save quote in quote dict
                quote_dict[quote.stock] = quote;
                return quote;
            }

            return null;
        }

        //private ArrayList GetEndOfDayOptionChain(string ticker)
        //{
        //    CultureInfo current_culture = Thread.CurrentThread.CurrentCulture;

        //    ArrayList list = null;

        //    try
        //    {
        //        Bovespa.GetQuote(ticker);
        //        list = Bovespa.GetOptionsChain(ticker);
        //    }
        //    catch { }

        //    Thread.CurrentThread.CurrentCulture = current_culture;

        //    return list;
        //}

        private ArrayList GetToMonthOptionChain(string ticker)
        {
            string sym_string = data_center.GetSymbolsStream(ticker.Substring(0, 4));
            if (string.IsNullOrEmpty(sym_string)) return null;

            // get quote
            if (!quote_dict.ContainsKey(ticker)) return null;
            Quote quote = quote_dict[ticker];

            // strike factor
            double strike_factor = 1;
            try { 
                strike_factor = Math.Pow(10, Math.Truncate(Math.Log10(quote.price.last)) - 1); 
            }
            catch { }

            // list of options
            ArrayList list = new ArrayList();

            // make valid month list
            string mon_list = "";

            // get parameters from configuration
            int month_count = int.Parse(config["MonthsCount"]);
            double strike_lower_limit = double.Parse(config["StrikeLowerLimit"]) * 0.01;
            double strike_upper_limit = double.Parse(config["StrikeUpperLimit"]) * 0.01;

            for (int i = 0; i < month_count; i++)
            {
                int j = DateTime.Now.AddMonths(i).Month - 1;
                mon_list += ((char)('M' + j)).ToString() + ((char)('A' + j)).ToString();
            }

            foreach (string sym in sym_string.Split(new char[] { '\n' }))
            {
                string[] sym_split = sym.Split(new char[] { ';' });

                if (sym_split.Length >= 2 && sym_split[2] == "OPCAO")
                {
                    // PETRA26;PETR;OPCAO;False;OPCAO;BOVESPA;Tradezone

                    char mon_suffix = sym_split[0][4];

                    if (mon_list.Contains(mon_suffix.ToString()))
                    {

                        Option option = new Option();

                        option.stock = ticker;
                        option.symbol = sym_split[0];
                        option.stocks_per_contract = 1;

                        if ((char)mon_suffix < (char)'M')
                        {
                            option.type = "Call";
                            option.expiration = new DateTime(DateTime.Now.Year, (int)mon_suffix - (int)'A' + 1, 1);
                            if (option.expiration.Month < DateTime.Now.Month) option.expiration = option.expiration.AddYears(1);
                        }
                        else
                        {
                            option.type = "Put";
                            option.expiration = new DateTime(DateTime.Now.Year, (int)mon_suffix - (int)'M' + 1, 1);
                            if (option.expiration.Month < DateTime.Now.Month) option.expiration = option.expiration.AddYears(1);
                        }

                        // get the day after the 3rd monday
                        int days_to_1st_monday = (int)DayOfWeek.Monday - (int)option.expiration.DayOfWeek;
                        if (days_to_1st_monday < 0) days_to_1st_monday += 7;
                        option.expiration = option.expiration.AddDays(15 + days_to_1st_monday);

                        try
                        {
                            option.strike = double.Parse(sym_split[0].Substring(5)) * strike_factor;
                        }
                        catch { continue; }

                        if ((option.strike >= quote.price.last * strike_lower_limit) &&
                            (option.strike <= quote.price.last * strike_upper_limit))
                        {
                            // add options to list
                            list.Add(option);
                        }
                    }
                }
            }

            return list;
        }

        // get stock latest options chain
        public ArrayList GetOptionsChain(string ticker)
        {
            // make sure we are connected
            if (!Connect)
            {
                Connect = true;
                if (!Connect) return null;
            }

            // correct symbol
            ticker = CorrectSymbol(ticker);

            // get end-of-day option chain
            //ArrayList list = GetEndOfDayOptionChain(ticker);

            // get four-months option chain
            ArrayList list = GetToMonthOptionChain(ticker);

            // construct list of all options
            List<string> sym_list = new List<string>();
            foreach (Option option in list) sym_list.Add(option.symbol.Replace(".", ""));

            // get tick-list and mini-book list
            Dictionary<string, TradeZone.Tick> ord_list = data_center.GetQuoteTick(sym_list);
            Dictionary<string, TradeZone.Minibook> mnb_list = data_center.GetQuoteMiniBook(sym_list);
            if (ord_list == null || mnb_list == null) return null;

            // options with no market data list
            ArrayList sym_withnodata = new ArrayList();

            foreach (Option option in list)
            {
                try
                {
                    bool with_data = false;

                    if (mnb_list.ContainsKey(option.symbol))
                    {
                        TradeZone.Minibook minibook = mnb_list[option.symbol];

                        option.price.ask = minibook.Items[0].BestAskPrice;
                        option.price.bid = minibook.Items[0].BestAskPrice;
                        with_data = true;
                    }
                    else
                    {
                        option.price.ask = double.NaN;
                        option.price.bid = double.NaN;
                    }

                    if (ord_list.ContainsKey(option.symbol))
                    {
                        TradeZone.Tick tick = ord_list[option.symbol];

                        option.price.last = tick.Last;
                        option.price.change = tick.Last - tick.Open;

                        option.update_timestamp = tick.Timestamp;
                        option.volume.total = (double)tick.TradeVolume;
                        option.open_int = (int)tick.TotalVolume;
                        with_data = true;
                    }
                    else
                    {
                        option.price.last = double.NaN;
                        option.price.change = double.NaN;
                        option.update_timestamp = DateTime.Now;
                        option.volume.total = 0;
                    }

                    if (!with_data) sym_withnodata.Add(option);                    
                }
                catch { }
            }

            try
            {
                if (!config.ContainsKey("HideOptionsWithNoMarket") || bool.Parse(config["HideOptionsWithNoMarket"]))
                {
                    foreach (Option option in sym_withnodata) list.Remove(option);
                }
            }
            catch { }

            return list;
        }

        // get stock/option historical prices 
        public ArrayList GetHistoricalData(string ticker, DateTime start, DateTime end)
        {
            // make sure we are connected
            if (!Connect)
            {
                Connect = true;
                if (!Connect) return null;
            }

            // correct symbol
            ticker = CorrectSymbol(ticker);

            string his_string = data_center.GetHistoryStream(ticker, start, end);
            if (string.IsNullOrEmpty(his_string)) return null;

            // history list
            ArrayList history_list = new ArrayList();

            // used for date conversion
            CultureInfo ci = new CultureInfo("pt-BR", false);

            foreach (string his in his_string.Split(new char[] {'\r'}))
            {
                string[] his_split = his.Split(new char[] { ';' });

                // Date, Time, Open, High, Low, Close, Q tt, volume, Business, Status

                History history = new History();

                history.stock = ticker;

                try
                {
                    history.date = DateTime.Parse(his_split[0], ci);
                    history.price.open = double.Parse(his_split[1], ci);
                    history.price.high = double.Parse(his_split[2], ci);
                    history.price.low = double.Parse(his_split[3], ci);
                    history.price.close = double.Parse(his_split[4], ci);
                    history.price.close_adj = double.Parse(his_split[4], ci);
                    history.volume.total = double.Parse(his_split[6], ci);

                    history_list.Add(history);
                }
                catch { }
            }

            // update open values
            for (int i = 0; i < history_list.Count - 1; i++) ((History)history_list[i]).price.open = ((History)history_list[i + 1]).price.close;
            ((History)history_list[history_list.Count - 1]).price.open = ((History)history_list[history_list.Count - 1]).price.close;

            return history_list;
        }

        // get stock name lookup results
        public ArrayList GetStockSymbolLookup(string name)
        {
            // make sure we are connected
            if (!Connect)
            {
                Connect = true;
                if (!Connect) return null;
            }

            string sym_string = data_center.GetSymbolsStream("");
            if (string.IsNullOrEmpty(sym_string)) return null;

            ArrayList symbol_list = new ArrayList();
            name = name.ToUpper();

            foreach (string sym in sym_string.Split(new char[] { '\n' }))
            {
                if (!sym.ToUpper().Contains(name)) continue;

                string[] sym_split = sym.Split(new char[] { ';' });

                if (sym_split.Length >= 2 && sym_split[2] != "OPCAO")
                {
                    symbol_list.Add(sym_split[1].Substring(0, 1).ToUpper() + sym_split[1].Substring(1).ToLower() + " (" + sym_split[0] + ")");
                }
            }

            return symbol_list;
        }

        // get default annual interest rate for specified duration [in years]
        public double GetAnnualInterestRate(double duration)
        {
            XmlDocument xml = cap.DownloadXmlPartialWebPage(@"http://www.bcb.gov.br/?english", "<body", "</body>", 1, 1);
            if (xml == null) return double.NaN;

            XmlNode nd = prs.GetXmlNodeByPath(xml.FirstChild, @"body\table\tr\td(3)\table\tr\td(3)\table(2)\tr(5)\td\table\tr(4)\td(2)");
            if (nd == null) return double.NaN;

            try
            {
                return double.Parse(nd.InnerText.Replace("%", "").Trim());
            }
            catch { }

            return double.NaN;
        }

        // get default historical volatility for specified duration [in years]
        public double GetHistoricalVolatility(string ticker, double duration)
        {
            // get historical data
            ArrayList list = GetHistoricalData(ticker, DateTime.Now.AddDays(-duration*365), DateTime.Now);

            // calculate historical value
            return 100.0*HistoryVolatility.HighLowParkinson(list);
        }

        // get and set generic parameters
        public string GetParameter(string name)
        {
            if (name == "Download Limit") return "Unlimited";
            else if (name == "Download Delay") return "0";

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
