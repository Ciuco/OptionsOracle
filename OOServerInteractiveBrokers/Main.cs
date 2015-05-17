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

namespace OOServerInteractiveBrokers
{
    public class Main : WebSite, IServer
    {
        // configuration file
        private const string CONFIG_FILE = "plugin_interactivebrokers_config.xml";      
        private const int MAX_CACHE_SIZE = 32;

        // configuration dictionary
        SerializableDictionary<string, string> config = new SerializableDictionary<string, string>();

        // quote cache
        Dictionary<string, Quote> quote_cache = new Dictionary<string, Quote>();

        // tws/yahoo interface
        private Tws tws = null;
        private Yahoo yho = null;
        private NameLookup nlk = null;
        private WorldInterestRate wir = null;

        // host
        private IServerHost host = null;

        // feature and server array list
        private ArrayList feature_list = new ArrayList();
        private ArrayList server_list  = new ArrayList();

        public Main()
        {
            // create interaces
            tws = new Tws();
            yho = new Yahoo(cap);
            nlk = new NameLookup(cap);
            wir = new WorldInterestRate(cap);

            // update feature list
            feature_list.Add(FeaturesT.SUPPORTS_DELAYED_OPTIONS_CHAIN);
            feature_list.Add(FeaturesT.SUPPORTS_DELAYED_STOCK_QUOTE);
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
        public string Description { get { return "Real-Time Quote from InteractiveBrokers TWS"; } }
        public string Name { get { return "PlugIn Server Interactive-Brokers (TWS)"; } }
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
        public bool Connect { get { return tws.Connect; } set { tws.Connect = value; } }

        // connection settings
        //public int ConnectionsRetries { get; set; } // implemented by parent class WebSite
        //public string ProxyAddress { get; set; }    // implemented by parent class WebSite
        //public bool UseProxy { get; set; }          // implemented by parent class WebSite

        // debug log control
        public bool LogEnable { get { return false; } set { } }
        public string DebugLog { get { return null; } }

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
            }
            catch { }

            // default config
            if (!config.ContainsKey("Exchange.Underlying")) config.Add("Exchange.Underlying", "SMART");
            if (!config.ContainsKey("Exchange.Option")) config.Add("Exchange.Option", "CBOE");
            if (!config.ContainsKey("Currency")) config.Add("Currency", "USD");

            if (!config.ContainsKey("QuoteTimeout")) config.Add("QuoteTimeout", "5");
            if (!config.ContainsKey("BackOffTime")) config.Add("BackOffTime", "0");
            if (!config.ContainsKey("MaxParallelTickers")) config.Add("MaxParallelTickers", "100");

            if (!config.ContainsKey("DownloadMode")) config.Add("DownloadMode", "0");
            if (!config.ContainsKey("MaxExpirationLimit")) config.Add("MaxExpirationLimit", DateTime.MaxValue.Date.ToShortDateString());
            if (!config.ContainsKey("MinExpirationLimit")) config.Add("MinExpirationLimit", DateTime.MinValue.Date.ToShortDateString());
            if (!config.ContainsKey("MaxStrikeLimit")) config.Add("MaxStrikeLimit", Double.NaN.ToString());
            if (!config.ContainsKey("MinStrikeLimit")) config.Add("MinStrikeLimit", Double.NaN.ToString());
            if (!config.ContainsKey("DownloadOpenInt")) config.Add("DownloadOpenInt", bool.TrueString);
            if (!config.ContainsKey("FilterNoPriceOptions")) config.Add("FilterNoPriceOptions", bool.TrueString);

            if (!config.ContainsKey("DownloadHistoryFromYahoo")) config.Add("DownloadHistoryFromYahoo", bool.TrueString);
            if (!config.ContainsKey("YahooExchangeSuffix")) config.Add("YahooExchangeSuffix", "");

            try
            {
                tws.DownloadOpenInt = bool.Parse(config["DownloadOpenInt"]);
                tws.QuoteTimeout = int.Parse(config["QuoteTimeout"]);
                tws.BackOffTime = int.Parse(config["BackOffTime"]);
                yho.YahooExchangeSuffix = config["YahooExchangeSuffix"];
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

            configForm.ExchangeUnderlying = config["Exchange.Underlying"];
            configForm.ExchangeOption = config["Exchange.Option"];
            configForm.Currency = config["Currency"];

            configForm.QuoteTimeout = int.Parse(config["QuoteTimeout"]);
            configForm.BackOffTime = int.Parse(config["BackOffTime"]);
            configForm.MaxParallelTickers = int.Parse(config["MaxParallelTickers"]);
         
            configForm.DownloadMode = int.Parse(config["DownloadMode"]);
            configForm.MaxExpirationLimit = DateTime.Parse(config["MaxExpirationLimit"]);
            configForm.MinExpirationLimit = DateTime.Parse(config["MinExpirationLimit"]);
            if (config["MaxStrikeLimit"] == double.NaN.ToString()) configForm.MaxStrikeLimit = double.NaN;
            else configForm.MaxStrikeLimit = double.Parse(config["MaxStrikeLimit"]);
            if (config["MinStrikeLimit"] == double.NaN.ToString()) configForm.MinStrikeLimit = double.NaN;
            else configForm.MinStrikeLimit = double.Parse(config["MinStrikeLimit"]);
            configForm.DownloadOpenInt = bool.Parse(config["DownloadOpenInt"]);
            configForm.FilterNoPriceOptions = bool.Parse(config["FilterNoPriceOptions"]);

            configForm.DownloadHistoryFromYahoo = bool.Parse(config["DownloadHistoryFromYahoo"]);
            configForm.YahooExchangeSuffix = config["YahooExchangeSuffix"];

            if (configForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                // update configuration dictionary
                config["Exchange.Underlying"] = configForm.ExchangeUnderlying;
                config["Exchange.Option"] = configForm.ExchangeOption;
                config["Currency"] = configForm.Currency;

                config["QuoteTimeout"] = configForm.QuoteTimeout.ToString();
                config["BackOffTime"] = configForm.BackOffTime.ToString();
                config["MaxParallelTickers"] = configForm.MaxParallelTickers.ToString(); 

                config["DownloadMode"] = configForm.DownloadMode.ToString();
                config["MaxExpirationLimit"] = configForm.MaxExpirationLimit.Date.ToShortDateString();
                config["MinExpirationLimit"] = configForm.MinExpirationLimit.Date.ToShortDateString();
                config["MaxStrikeLimit"] = configForm.MaxStrikeLimit.ToString();
                config["MinStrikeLimit"] = configForm.MinStrikeLimit.ToString();
                config["DownloadOpenInt"] = configForm.DownloadOpenInt.ToString();
                config["FilterNoPriceOptions"] = configForm.FilterNoPriceOptions.ToString();

                config["DownloadHistoryFromYahoo"] = configForm.DownloadHistoryFromYahoo.ToString();
                config["YahooExchangeSuffix"] = configForm.YahooExchangeSuffix;

                // save configuration to file
                Save();
            }

            try
            {
                tws.DownloadOpenInt = bool.Parse(config["DownloadOpenInt"]);
                tws.QuoteTimeout = int.Parse(config["QuoteTimeout"]);
                tws.BackOffTime = int.Parse(config["BackOffTime"]);
                yho.YahooExchangeSuffix = config["YahooExchangeSuffix"];
            }
            catch { }
        }


        // default symbol
        public string DefaultSymbol { get { return ""; } }

        public string CorrectSymbol(string ticker)
        {
            ticker = ticker.ToUpper().Trim();

            switch (ticker)
            {
                default:
                    break;
            }

            return ticker;
        }

        // get stock latest quote
        public Quote GetQuote(string ticker)
        {
            ticker = CorrectSymbol(ticker);

            string symbol;
            Krs.Ats.IBNet.SecurityType type;

            if (ticker.StartsWith("^"))
            {
                symbol = ticker.TrimStart(new char[] { '^' });
                type = Krs.Ats.IBNet.SecurityType.Index;
            }
            else if (ticker.StartsWith("~"))
            {
                symbol = ticker.TrimStart(new char[] { '~' });
                type = Krs.Ats.IBNet.SecurityType.Future;
            }
            else
            {
                symbol = ticker;
                type = Krs.Ats.IBNet.SecurityType.Stock;
            }

            List<Tws.TickData> tick_list;
            if (type == Krs.Ats.IBNet.SecurityType.Index) tick_list = tws.RequestQuote(symbol, config["Exchange.Underlying"], config["Exchange.Option"], type, config["Currency"], null);
            else tick_list = tws.RequestQuote(symbol, config["Exchange.Underlying"], null, type, config["Currency"], null);
            if (tick_list == null || tick_list.Count == 0) return null;

            Tws.TickData tick = tick_list[0];

            Quote quote = new Quote();
            quote.stock = ticker;

            // resolve name & last closing price
            string name = nlk.GetNameBySymbol(symbol, config["Exchange.Underlying"], config["Currency"], type);
            if (!string.IsNullOrEmpty(name)) quote.name = name;
            else if (!string.IsNullOrEmpty(tick.name)) quote.name = tick.name;
            else quote.name = quote.stock;

            // if price is not available try to get it from the lookup
            if (tick.price.ask <= 0 && tick.price.bid <= 0 && tick.price.last <= 0)
                tick.price.last = nlk.GetLastPriceBySymbol(symbol, config["Exchange.Underlying"], config["Currency"], type);

            // copy prices
            quote.price.last = tick.price.last;
            if (quote.price.last <= 0) quote.price.last = double.NaN;            
            
            quote.price.ask = tick.price.ask;
            if (quote.price.ask <= 0) quote.price.ask = double.NaN;             
            
            quote.price.bid = tick.price.bid;
            if (quote.price.bid <= 0) quote.price.bid = double.NaN;             
            
            quote.price.low = tick.price.low;
            if (quote.price.low <= 0) quote.price.low = double.NaN;             
            
            quote.price.high = tick.price.high;
            if (quote.price.high <= 0) quote.price.high = double.NaN;             
            
            quote.price.open = tick.price.close;
            if (quote.price.open <= 0) quote.price.open = double.NaN;
            
            if (quote.price.last > 0 && quote.price.open > 0)
                quote.price.change = quote.price.last - quote.price.open;
            else
                quote.price.change = double.NaN;

            // copy volume
            quote.volume.total = tick.size.volume;

            quote.general.dividend_rate = 0;
            quote.update_timestamp = TimeZone.CurrentTimeZone.ToLocalTime(tick.timestamp);

            // save in cache
            if (quote_cache.ContainsKey(ticker)) quote_cache[ticker] = quote;
            else
            {
                if (quote_cache.Count >= MAX_CACHE_SIZE)
                    quote_cache.Remove(quote_cache.Keys.GetEnumerator().Current);

                quote_cache.Add(ticker, quote);
            }

            return quote;
        }

        // get stock latest options chain
        public ArrayList GetOptionsChain(string ticker)
        {
            ticker = CorrectSymbol(ticker);

            string symbol;
            Krs.Ats.IBNet.SecurityType type;

            if (ticker.StartsWith("^"))
            {
                symbol = ticker.TrimStart(new char[] { '^' });
                type = Krs.Ats.IBNet.SecurityType.Option;
            }
            else if (ticker.StartsWith("~"))
            {
                symbol = ticker.TrimStart(new char[] { '~' });
                type = Krs.Ats.IBNet.SecurityType.FutureOption;
            }
            else
            {
                symbol = ticker;
                type = Krs.Ats.IBNet.SecurityType.Option;
            }

            Tws.ContData cont = tws.RequestDetails(symbol, null, config["Exchange.Option"], type, config["Currency"], true);
            if (cont == null) return null;

            ArrayList list = new ArrayList();

            // progress report
            int current, last = 0, n = 0;
            BackgroundWorker bw = null;
            if (host != null) bw = host.BackgroundWorker;

            List<Krs.Ats.IBNet.ContractDetails> cont_list = new List<Krs.Ats.IBNet.ContractDetails>();

            int download_mode = int.Parse(config["DownloadMode"]);

            DateTime max_expdate_filter = DateTime.Parse(config["MaxExpirationLimit"]);
            DateTime min_expdate_filter = DateTime.Parse(config["MinExpirationLimit"]);

            double max_strike_filter = double.NaN;
            double min_strike_filter = double.NaN;
            
            if (quote_cache.ContainsKey(ticker))
            {
                Quote quote = quote_cache[ticker];

                max_strike_filter = (1 + double.Parse(config["MaxStrikeLimit"]) * 0.01) * quote.price.last;
                min_strike_filter = (1 + double.Parse(config["MinStrikeLimit"]) * 0.01) * quote.price.last;
            }

            MappingSet mapping = null;
            
            if (download_mode == 1)
            {
                mapping = new MappingSet();

                // update type list table
                mapping.TypeTable.AddTypeTableRow("Call", "Call", false);
                mapping.TypeTable.AddTypeTableRow("Put", "Put", false);

                // update expiration/strike list table
                foreach (Krs.Ats.IBNet.ContractDetailsEventArgs d in cont.contracts)
                {
                    try
                    {
                        if (d.ContractDetails.Summary.SecurityType != Krs.Ats.IBNet.SecurityType.Option &&
                            d.ContractDetails.Summary.Right == Krs.Ats.IBNet.RightType.Undefined) continue;

                        // expiration date
                        DateTime expdate = Tws.TwsDateStringToDateTime(d.ContractDetails.Summary.Expiry);
                        if (mapping.ExpirationTable.FindByExpiration(expdate) == null) mapping.ExpirationTable.AddExpirationTableRow(expdate, expdate.ToString("d-MMM-yyyy"), false);

                        // strike
                        double strike = d.ContractDetails.Summary.Strike;
                        if (mapping.StrikeTable.FindByStrike(strike) == null) mapping.StrikeTable.AddStrikeTableRow(strike, strike.ToString("N2"), false);
                    }
                    catch { }
                }
                
                mapping.AcceptChanges();

                // open dialog to select filter
                OptionsFilterForm optionsFilterForm = new OptionsFilterForm(mapping);
                optionsFilterForm.ShowDialog();
            }

            foreach (Krs.Ats.IBNet.ContractDetailsEventArgs d in cont.contracts)
            {
                try
                {
                    if (d.ContractDetails.Summary.SecurityType != Krs.Ats.IBNet.SecurityType.Option &&
                        d.ContractDetails.Summary.Right == Krs.Ats.IBNet.RightType.Undefined) continue;

                    if (download_mode != 0)
                    {
                        // expiration date
                        DateTime expdate = Tws.TwsDateStringToDateTime(d.ContractDetails.Summary.Expiry);

                        // strike
                        double strike = d.ContractDetails.Summary.Strike;

                        // type
                        string optype = d.ContractDetails.Summary.Right == Krs.Ats.IBNet.RightType.Call ? "Call" : "Put";

                        if (download_mode == 1 && mapping != null) // dynamic filter
                        {
                            if (!mapping.ExpirationTable.FindByExpiration(expdate).Enabled ||
                                !mapping.StrikeTable.FindByStrike(strike).Enabled ||
                                !mapping.TypeTable.FindByType(optype).Enabled) continue;
                        }
                        else if (download_mode == 2) // predefined filter
                        {
                            if ((expdate > max_expdate_filter) ||
                                (expdate < min_expdate_filter) ||
                                (!double.IsNaN(max_strike_filter) && strike > max_strike_filter) ||
                                (!double.IsNaN(min_strike_filter) && strike < min_strike_filter)) continue;
                        }
                    }

                    cont_list.Add(d.ContractDetails);
                }
                catch { }
            }

            bool filter_no_price_options = bool.Parse(config["FilterNoPriceOptions"]);
            int  max_parallel_tickers = int.Parse(config["MaxParallelTickers"]);

            for (int i = 0; i < cont_list.Count; i += max_parallel_tickers)
            {
                try
                {
                    List<Krs.Ats.IBNet.ContractDetails> cont_sub_list = cont_list.GetRange(i, (int)Math.Min(max_parallel_tickers, cont_list.Count - i));
                    List<Tws.TickData> tick_list = tws.RequestQuote(symbol, config["Exchange.Option"], null, type, null, cont_sub_list);
                    if (tick_list == null || tick_list.Count != cont_sub_list.Count) continue;

                    // keep rate down
                    if (i % 500 == 0 && i != 0) Thread.Sleep(5000);

                    for (int j = 0; j < tick_list.Count; j++)
                    {
                        try
                        {
                            Tws.TickData tick = tick_list[j];

                            Option option = new Option();

                            option.symbol = "." + cont_sub_list[j].Summary.LocalSymbol.Replace(" ","").Trim();
                            option.stock = ticker.Trim(); // cont_sub_list[j].Summary.Symbol;

                            option.type = cont_sub_list[j].Summary.Right == Krs.Ats.IBNet.RightType.Call ? "Call" : "Put";
                            option.expiration = Tws.TwsDateStringToDateTime(cont_sub_list[j].Summary.Expiry);
                            
                            option.strike = cont_sub_list[j].Summary.Strike;
                            option.stocks_per_contract = (double)decimal.Parse(cont_sub_list[j].Summary.Multiplier);                            
                            
                            option.price.ask = tick.price.ask;
                            if (option.price.ask <= 0) option.price.ask = double.NaN;                            

                            option.price.bid = tick.price.bid;                            
                            if (option.price.bid <= 0) option.price.bid = double.NaN;                            
                            
                            option.price.last = tick.price.last;
                            if (option.price.last <= 0) option.price.last = double.NaN;
                            
                            if (tick.price.last > 0 && tick.price.close > 0)
                                option.price.change = tick.price.last - tick.price.close;
                            else
                                option.price.change = double.NaN;

                            option.volume.total = tick.size.volume;
                            option.open_int = tick.size.open_int;
                            
                            try
                            {
                                option.update_timestamp = TimeZone.CurrentTimeZone.ToLocalTime(tick.timestamp);
                            }
                            catch { }

                            if (!filter_no_price_options || 
                                !double.IsNaN(option.price.last) || 
                                !double.IsNaN(option.price.ask)  || 
                                !double.IsNaN(option.price.bid))
                            {
                                list.Add(option);
                            }
                        }
                        catch { }

                        // update progress bar
                        n++;
                        if (bw != null)
                        {
                            current = n * 95 / cont_list.Count;
                            if (current != last)
                            {
                                bw.ReportProgress(current);
                                last = current;
                            }
                        }
                    }
                }
                catch { }
            }

            return list;
        }

        // get stock/option historical prices 
        public ArrayList GetHistoricalData(string ticker, DateTime start, DateTime end)
        {
            ticker = CorrectSymbol(ticker);

            // return from yahoo if configured as default
            if (bool.Parse(config["DownloadHistoryFromYahoo"]))
                return yho.GetHistoricalData(ticker, start, end);

            string symbol;
            Krs.Ats.IBNet.SecurityType type;

            if (ticker.StartsWith("^"))
            {
                symbol = ticker.TrimEnd(new char[] { '^' });
                type = Krs.Ats.IBNet.SecurityType.Index;
            }
            else if (ticker.StartsWith("~"))
            {
                symbol = ticker.TrimEnd(new char[] { '~' });
                type = Krs.Ats.IBNet.SecurityType.Future;
            }
            else
            {
                symbol = ticker;
                type = Krs.Ats.IBNet.SecurityType.Stock;
            }

            Tws.HistData hist = tws.RequestHistoricalData(symbol, config["Exchange.Underlying"], type, config["Currency"], start, true);
            if (hist == null) return null;

            ArrayList list = new ArrayList();

            foreach (Krs.Ats.IBNet.HistoricalDataEventArgs h in hist.history)
            {
                if (h.Date < start || h.Date > end) continue;

                History history = new History();

                history.stock = ticker;

                history.date = h.Date;
                history.price.close = (double)h.Close;
                history.price.open = (double)h.Open;
                history.price.low = (double)h.Low;
                history.price.high = (double)h.High;
                history.price.close_adj = (double)h.Close;
                history.volume.total = (double)h.Volume;

                list.Add(history);
            }

            return list;
        }

        // get stock name lookup results
        public ArrayList GetStockSymbolLookup(string name)
        {
            // do not lookup if tws is not already connected
            if (!tws.Connect) return null;

            return nlk.GetStockSymbolLookup(name, config["Exchange.Underlying"], config["Currency"], Krs.Ats.IBNet.SecurityType.Undefined);
        }

        // get default annual interest rate for specified duration [in years]
        public double GetAnnualInterestRate(double duration)
        {
            return wir.GetAnnualInterestRate(config["Currency"]);
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
