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
    public class Main : WebSite, IServer
    {
        // configuration file
        private const string CONFIG_FILE = "plugin_tdameritrade_config.xml";
        private const int MAX_CACHE_SIZE = 32;

        // configuration dictionary
        SerializableDictionary<string, string> config = new SerializableDictionary<string, string>();

        // quote cache
        Dictionary<string, Quote> quote_cache = new Dictionary<string, Quote>();

        // tda/yahoo interface
        private Tda tda = null;
        private Yahoo yho = null;
        private NameLookup nlk = null;
        private WorldInterestRate wir = null;

        // host
        private IServerHost host = null;

        // feature and server array list
        private ArrayList feature_list = new ArrayList();
        private ArrayList server_list = new ArrayList();

        public Main()
        {
            // create interaces
            tda = new Tda();
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
        public string Description { get { return "Real-Time Quote from TD-Ameritrade"; } }
        public string Name { get { return "PlugIn Server TD-Ameritrade"; } }
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
        public bool Connect
        { 
            get { return tda.Connect; } 
            set { if (!value || bool.Parse(config["LoginOnStartUp"])) tda.Connect = value; } 
        }

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
            if (!config.ContainsKey("DownloadHistoryFromYahoo")) config.Add("DownloadHistoryFromYahoo", bool.FalseString);
            if (!config.ContainsKey("LoginOnStartUp")) config.Add("LoginOnStartUp", bool.FalseString);            
            if (!config.ContainsKey("YahooExchangeSuffix")) config.Add("YahooExchangeSuffix", "");
            if (!config.ContainsKey("DefaultUsername")) config.Add("DefaultUsername", "");

            try
            {
                yho.YahooExchangeSuffix = config["YahooExchangeSuffix"];
                tda.Username = config["DefaultUsername"];
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

            configForm.DownloadHistoryFromYahoo = bool.Parse(config["DownloadHistoryFromYahoo"]);
            configForm.LoginOnStartUp = bool.Parse(config["LoginOnStartUp"]);
            configForm.YahooExchangeSuffix = config["YahooExchangeSuffix"];
            configForm.DefaultUsername = config["DefaultUsername"];

            if (configForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                // update configuration dictionary
                config["DownloadHistoryFromYahoo"] = configForm.DownloadHistoryFromYahoo.ToString();
                config["LoginOnStartUp"] = configForm.LoginOnStartUp.ToString();
                config["YahooExchangeSuffix"] = configForm.YahooExchangeSuffix;
                config["DefaultUsername"] = configForm.DefaultUsername;

                // save configuration to file
                Save();
            }

            try
            {
                yho.YahooExchangeSuffix = config["YahooExchangeSuffix"];
                tda.Username = config["DefaultUsername"];
            }
            catch { }
        }


        // default symbol
        public string DefaultSymbol { get { return ""; } }

        public string CorrectSymbol(string ticker)
        {
            ticker = ticker.ToUpper().Trim();

            // convert TD Ameritrade symbols to OptionsOracle symbols
            if (ticker.StartsWith("$") && 
                ticker.EndsWith(".X"))
            {
                ticker = "^" + ticker.TrimStart(new char[] { '$' }).TrimStart(new char[] { '.', 'X' });
            }

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

            if (ticker.StartsWith("^"))
            {
                // index-option
                symbol = "$" + ticker.TrimStart(new char[] { '^' }) + ".X";
            }
            else if (ticker.StartsWith("~"))
            {
                // future-option
                symbol = ticker.TrimStart(new char[] { '~' });
            }
            else
            {
                // stock-option
                symbol = ticker;
            }

            Tda.TDQuote tda_quote = tda.GetQuote(symbol);
            if (tda_quote == null) return null;

            Quote quote = new Quote();

            quote.stock = ticker;
            quote.name = tda_quote.description;

            if (!double.TryParse(tda_quote.last, out quote.price.last)) return null;            
            if (!double.TryParse(tda_quote.ask, out quote.price.ask)) quote.price.ask = double.NaN;
            if (!double.TryParse(tda_quote.bid, out quote.price.bid)) quote.price.bid = double.NaN;
            if (!double.TryParse(tda_quote.high, out quote.price.high)) quote.price.high = double.NaN;
            if (!double.TryParse(tda_quote.low, out quote.price.low)) quote.price.low = double.NaN;
            if (!double.TryParse(tda_quote.open, out quote.price.open)) quote.price.open = double.NaN;
            if (!double.TryParse(tda_quote.change, out quote.price.change)) quote.price.change = double.NaN;
            if (!double.TryParse(tda_quote.volume, out quote.volume.total)) quote.volume.total = 0;

            quote.general.dividend_rate = 0;
            if (!DateTime.TryParse(tda_quote.last_trade_date, out quote.update_timestamp)) quote.update_timestamp = DateTime.Now;

            return quote;
        }

        // get stock latest options chain
        public ArrayList GetOptionsChain(string ticker)
        {
            ticker = CorrectSymbol(ticker);

            string symbol;

            if (ticker.StartsWith("^"))
            {
                // index-option
                symbol = "$" + ticker.TrimStart(new char[] { '^' }) + ".X";
            }
            else if (ticker.StartsWith("~"))
            {
                // future-option
                symbol = ticker.TrimStart(new char[] { '~' });
            }
            else
            {
                // stock-option
                symbol = ticker;
            }

            List<Tda.TDOptionQuote> tda_option_list = tda.GetOptionChain(symbol, "");
            if (tda_option_list == null) return null;

            ArrayList option_list = new ArrayList();

            foreach (Tda.TDOptionQuote tda_option in tda_option_list)
            {
                // only standard options
                if (tda_option.standard_option.ToLower() != "true") continue;

                Option option = new Option();
                option.type = tda_option.type;

                DateTime.TryParseExact(tda_option.date, "yyyyMMdd", null, System.Globalization.DateTimeStyles.AdjustToUniversal, out option.expiration);
                option.expiration = option.expiration.AddDays(-1);
                option.update_timestamp = DateTime.Now;

                option.stock = ticker;
                option.symbol = "." + tda_option.option_symbol.TrimStart(new char[] { '+' }); ;

                if (!double.TryParse(tda_option.strike_price, out option.strike)) continue;
                if (!double.TryParse(tda_option.last, out option.price.last)) option.price.last = double.NaN;
                if (!double.TryParse(tda_option.change, out option.price.change)) option.price.change = double.NaN;
                if (!double.TryParse(tda_option.bid, out option.price.bid)) option.price.bid = double.NaN;
                if (!double.TryParse(tda_option.ask, out option.price.ask)) option.price.ask = double.NaN;
                if (!double.TryParse(tda_option.volume, out option.volume.total)) option.volume.total = 0;
                if (!int.TryParse(tda_option.open_intereset, out option.open_int)) option.open_int = 0;

                option_list.Add(option);
            }

            return option_list;
        }

        // get stock/option historical prices 
        public ArrayList GetHistoricalData(string ticker, DateTime start, DateTime end)
        {
            ticker = CorrectSymbol(ticker);

            // return from yahoo if configured as default
            if (bool.Parse(config["DownloadHistoryFromYahoo"]))
                return yho.GetHistoricalData(ticker, start, end);

            string symbol;

            if (ticker.StartsWith("^"))
            {
                // index-option
                symbol = "$" + ticker.TrimStart(new char[] { '^' }) + ".X";
            }
            else if (ticker.StartsWith("~"))
            {
                // future-option
                symbol = ticker.TrimStart(new char[] { '~' });
            }
            else
            {
                // stock-option
                symbol = ticker;
            }

            List<Tda.TDPriceHistory> tda_history_list = tda.GetHistoricalData(symbol, start, end);
            if (tda_history_list == null) return null;

            // reverse array
            tda_history_list.Reverse();

            ArrayList history_list = new ArrayList();

            foreach (Tda.TDPriceHistory tda_history in tda_history_list)
            {
                History history = new History();

                history.stock = ticker;
                history.date = tda_history.timestamp;

                history.price.close = tda_history.close;
                history.price.close_adj = tda_history.close;
                history.price.open = tda_history.open;
                history.price.high = tda_history.high;
                history.price.low = tda_history.low;
                history.volume.total = tda_history.volume;

                history_list.Add(history);
            }

            return history_list;
        }

        // get stock name lookup results
        public ArrayList GetStockSymbolLookup(string name)
        {
            // do not lookup if tws is not already connected
            if (!tda.Connect) return null;

            return nlk.GetStockSymbolLookup(name);
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
