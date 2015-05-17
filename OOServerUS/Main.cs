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

namespace OOServerUS
{
    public class Main : WebSite, IServer
    {
        // configuration file
        private const string CONFIG_FILE = "plugin_us_config.xml";

        // configuration dictionary
        SerializableDictionary<string, string> config = new SerializableDictionary<string, string>();

        // connection status
        private bool connect = false;

        // tda/yahoo interface        
        private Yahoo yho = null;
        private MarketWatch mkw = null;
        private MoneyCentral msn = null;
        private WorldInterestRate wir = null;        
        private MorningStar mst = null;
        private Optionetics opt = null;

        // host
        private IServerHost host = null;

        // feature and server array list
        private ArrayList feature_list = new ArrayList();
        private ArrayList server_list = new ArrayList();

        private WebForm wbf;

        public Main()
        {
            wbf = new WebForm();
            wbf.Show();
            wbf.Hide();

            // create interaces            
            yho = new Yahoo(cap);
            mkw = new MarketWatch(cap);
            msn = new MoneyCentral(cap);
            mst = new MorningStar(cap);
            opt = new Optionetics(cap, wbf);
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
        public string Description { get { return "Delayed Quote for US Market"; } }
        public string Name { get { return "PlugIn Server US (CBOE)"; } }
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
            if (!config.ContainsKey("QuoteDataSource")) config.Add("QuoteDataSource", "Default");
            if (!config.ContainsKey("StockOptionChainDataSource")) config.Add("StockOptionChainDataSource", "Default");
            if (!config.ContainsKey("IndexOptionChainDataSource")) config.Add("IndexOptionChainDataSource", "Default");
            if (!config.ContainsKey("FundOptionChainDataSource")) config.Add("FundOptionChainDataSource", "Default");
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

            configForm.QuoteDataSource = config["QuoteDataSource"];
            configForm.StockOptionChainDataSource = config["StockOptionChainDataSource"];
            configForm.IndexOptionChainDataSource = config["IndexOptionChainDataSource"];
            configForm.FundOptionChainDataSource = config["FundOptionChainDataSource"];

            if (configForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                // update configuration dictionary
                config["QuoteDataSource"] = configForm.QuoteDataSource;
                config["StockOptionChainDataSource"] = configForm.StockOptionChainDataSource;
                config["IndexOptionChainDataSource"] = configForm.IndexOptionChainDataSource;
                config["FundOptionChainDataSource"] = configForm.FundOptionChainDataSource;

                // save configuration to file
                Save();
            }
        }


        // default symbol
        public string DefaultSymbol { get { return ""; } }

        // get stock latest quote
        public Quote GetQuote(string ticker)
        {
            Symbol.Type type = Symbol.Type.Unknown;
            ticker = Symbol.CorrectSymbol(ticker, out type);

            string mode = config["QuoteDataSource"];

            Quote quote = null;

            switch (mode)
            {
                case "A":
                case "Default":
                default:
                    quote = yho.GetQuote(ticker);
                    if (quote == null) quote = msn.GetQuote(ticker);
                    break;
                case "B":
                    quote = msn.GetQuote(ticker);
                    if (quote == null) quote = yho.GetQuote(ticker);
                    break;
            }

            return quote;
        }

        // get stock latest options chain
        public ArrayList GetOptionsChain(string ticker)
        {
            Symbol.Type type = Symbol.Type.Unknown;
            ticker = Symbol.CorrectSymbol(ticker, out type);

            string mode = "Default";

            if (type == Symbol.Type.Stock) mode = config["StockOptionChainDataSource"];
            else if (type == Symbol.Type.Fund) mode = config["FundOptionChainDataSource"];
            else if (type == Symbol.Type.Index) mode = config["IndexOptionChainDataSource"];

            ArrayList list = null;

            switch (mode)
            {
                case "A":
                    {
                        list = mkw.GetOptionsChain(ticker, type);

                        if ((list == null || list.Count == 0) && type == Symbol.Type.Stock)
                            list = mkw.GetOptionsChain(ticker, Symbol.Type.Fund);

                        if ((list == null || list.Count == 0) && type == Symbol.Type.Fund)
                            list = mkw.GetOptionsChain(ticker, Symbol.Type.Index);
                    }
                    break;
                case "B":
                    {
                        list = mst.GetOptionsChain(ticker, type);
                    }
                    break;
                case "C":
                    {
                        list = yho.GetOptionsChain(ticker);
                    }
                    break;
                case "D":
                    {
                        list = opt.GetOptionsChain(ticker, type);
                    }
                    break;
                case "Default":
                default:
                    if (type != Symbol.Type.Index)
                    {
                        list = mkw.GetOptionsChain(ticker, type);

                        if ((list == null || list.Count == 0))
                            list = mkw.GetOptionsChain(ticker, Symbol.Type.Fund);

                        if ((list == null || list.Count == 0))
                            list = mst.GetOptionsChain(ticker, type);

                        if ((list == null || list.Count == 0))
                            list = yho.GetOptionsChain(ticker);

                        if ((list == null || list.Count == 0))
                            list = opt.GetOptionsChain(ticker, type);
                    }
                    else
                    {
                        list = opt.GetOptionsChain(ticker, type);

                        if ((list == null || list.Count == 0))
                            list = mst.GetOptionsChain(ticker, type);

                        if ((list == null || list.Count == 0))
                            list = mkw.GetOptionsChain(ticker, type);

                        if ((list == null || list.Count == 0) && type == Symbol.Type.Fund)
                            list = mkw.GetOptionsChain(ticker, Symbol.Type.Index);

                        if ((list == null || list.Count == 0))
                            list = yho.GetOptionsChain(ticker);
                    }
                    break;
            }

            return list;
        }

        // get stock/option historical prices 
        public ArrayList GetHistoricalData(string ticker, DateTime start, DateTime end)
        {
            Symbol.Type type = Symbol.Type.Unknown;
            ticker = Symbol.CorrectSymbol(ticker, out type);

            return yho.GetHistoricalData(ticker, start, end);
        }

        // get stock name lookup results
        public ArrayList GetStockSymbolLookup(string name)
        {
            return msn.GetStockSymbolLookup(name);
        }

        // get default annual interest rate for specified duration [in years]
        public double GetAnnualInterestRate(double duration)
        {
            return wir.GetAnnualInterestRate("USD");
        }

        // get default historical volatility for specified duration [in years]
        public double GetHistoricalVolatility(string ticker, double duration)
        {
            Symbol.Type type = Symbol.Type.Unknown;
            ticker = Symbol.CorrectSymbol(ticker, out type);

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
            string[] split = name.Split(new char[] { ' ' });

            switch (split[0])
            {
                case "Earning":
                    try
                    {
                        return yho.GetStocksWithEarningDate(DateTime.Parse(split[1]));
                    }
                    catch { }
                    break;
            }

            return null;
        }

        public void SetParameterList(string name, ArrayList value)
        {
        }
    }
}
