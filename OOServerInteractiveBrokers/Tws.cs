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
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows.Forms;
using OOServerLib.Web;
using OOServerLib.Interface;
using OOServerLib.Global;
using OOServerLib.Config;
using Krs.Ats.IBNet;

namespace OOServerInteractiveBrokers
{
    public class Tws
    {
        // consts
        private const int MAX_CACHE_SIZE = 32;

        // tws interface class and status
        private IBClient tws = new IBClient();
        private bool tws_connection = false;

        // configuration
        private bool tws_download_openint = false;
        public  bool DownloadOpenInt { set { tws_download_openint = value; } }

        private int tws_quote_timeout = 5;
        public int QuoteTimeout { set { tws_quote_timeout = value; } }

        private int tws_backoff_time = 0;
        public int BackOffTime { set { tws_backoff_time = value * 1000; } }

        // error messages
        private bool tws_err_max_tickers_reached = false;
        private bool tws_err_not_connected       = false;

        // list of ids
        private Queue<int> id_queue = new Queue<int>();

        // query holding dictionary
        private Dictionary<int, TickData> tick_list = new Dictionary<int, TickData>();
        private Dictionary<int, ContData> cont_list = new Dictionary<int, ContData>();
        private Dictionary<int, HistData> hist_list = new Dictionary<int, HistData>();

        // cache dictionary
        private Dictionary<string, ContData> cont_cache = new Dictionary<string, ContData>();
        private Dictionary<string, HistData> hist_cache = new Dictionary<string, HistData>();

        public int Port = 7496;
        public int ClientId = 0;

        public class TickData
        {
            public struct PriceT
            {
                public double ask;
                public double bid;
                public double low;
                public double high;
                public double last;
                public double open;
                public double close;
            }

            public struct SizeT
            {
                public int ask;
                public int bid;
                public int last;
                public int volume;
                public int open_int;
            }

            public int id;

            public string name;
            public string symbol;
            public string exchange;
            public string currency;

            public PriceT   price;
            public SizeT    size;
            public DateTime timestamp;

            public ErrorMessage error_code = 0;
            public List<TickType> pending_requests = new List<TickType>();
            public ManualResetEvent ready_event = new ManualResetEvent(false);

            public TickData(int id) { this.id = id; }
        }

        public class ContData
        {
            public int id;

            public List<ContractDetailsEventArgs> contracts = new List<ContractDetailsEventArgs>();
            public DateTime timestamp;

            public ErrorMessage error_code = 0;
            public ManualResetEvent ready_event = new ManualResetEvent(false);

            public ContData(int id) { this.id = id; }
        }

        public class HistData
        {
            public int id;

            public List<HistoricalDataEventArgs> history = new List<HistoricalDataEventArgs>();
            public DateTime timestamp;

            public ErrorMessage error_code = 0;
            public ManualResetEvent ready_event = new ManualResetEvent(false);

            public HistData(int id) { this.id = id; }
        }

        public Tws()
        {
            // add ids to be used
            for (int i = 1; i < 512; i++) id_queue.Enqueue(i);

            tws.ThrowExceptions = true;

            // connection managment
            tws.ConnectionClosed += new EventHandler<ConnectionClosedEventArgs>(tws_ConnectionClosed);
            tws.Error += new EventHandler<Krs.Ats.IBNet.ErrorEventArgs>(tws_Error);

            //// event callbacks of reqMktDataEx(...)
            tws.TickPrice += new EventHandler<TickPriceEventArgs>(tws_TickPrice);            
            tws.TickSize += new EventHandler<TickSizeEventArgs>(tws_TickSize);
            tws.TickGeneric += new EventHandler<TickGenericEventArgs>(tws_TickGeneric);
            tws.TickString += new EventHandler<TickStringEventArgs>(tws_TickString);
            tws.TickEfp += new EventHandler<TickEfpEventArgs>(tws_TickEfp);
            tws.TickOptionComputation += new EventHandler<TickOptionComputationEventArgs>(tws_TickOptionComputation);
            tws.NextValidId += new EventHandler<NextValidIdEventArgs>(tws_NextValidId);
            tws.ContractDetails += new EventHandler<ContractDetailsEventArgs>(tws_ContractDetails);
            tws.ContractDetailsEnd += new EventHandler<ContractDetailsEndEventArgs>(tws_ContractDetailsEnd);
            tws.HistoricalData += new EventHandler<HistoricalDataEventArgs>(tws_HistoricalData);
        }

        public bool Connect
        {
            get
            {
                return tws_connection;
            }
            set
            {
                if (value)
                {
                    try
                    {
                        tws.Connect("", Port, ClientId);
                        tws_connection = true;

                        //foreach (int id in id_queue)
                        //{
                        //    tws.CancelMarketData(id);
                        //    tws.CancelHistoricalData(id);
                        //}

                    }
                    catch { tws_connection = false; }

                    if (!tws_connection)
                    {
                        if (!tws_err_not_connected)
                        {
                            MessageBox.Show("Failed to connect to TWS.\nPlease check that TWS is running and that the Socket API interface is enabled.    ", "Interactive Brokers Plug-In - TWS Error Message", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            tws_err_not_connected = true;
                        }
                    }
                    else
                    {
                        // error messages
                        tws_err_max_tickers_reached = false;
                        tws_err_not_connected = false;
                    }
                }
                else
                {
                    try
                    {
                        tws.Disconnect();
                        tws_connection = false;
                    }
                    catch { }
                }
            }
        }

        public static DateTime TwsDateStringToDateTime(string s)
        {
            DateTime expdate = new DateTime(int.Parse(s.Substring(0, 4)), int.Parse(s.Substring(4, 2)), int.Parse(s.Substring(6, 2))).Date;
            if (expdate.DayOfWeek == DayOfWeek.Friday) return expdate.AddDays(1);
            else if (expdate.DayOfWeek == DayOfWeek.Sunday) return expdate.AddDays(-1);
            else return expdate;
        }

        public List<TickData> RequestQuote(string symbol, string exchange, string primary_exchange, SecurityType type, string currency, List<ContractDetails> details)
        {
            // make sure we are connected
            if (!Connect)
            {
                Connect = true;
                if (!Connect) return null;
            }

            // create returned list
            List<TickData> list = new List<TickData>();

            // get quotes count
            int count = 1;
            if (details != null && details.Count > 1) count = details.Count;

            for (int i = 0; i < count; i++)
            {
                // get id from id queue
                if (id_queue.Count == 0) break;
                int id = id_queue.Dequeue();

                // get tick object
                if (!tick_list.ContainsKey(id)) tick_list[id] = new TickData(id);
                TickData t = tick_list[id];

                t.name = symbol;
                t.symbol = symbol;
                t.exchange = exchange;
                t.currency = currency;

                // keep request rate
                if (i % 24 == 0 && i != 0) Thread.Sleep(500 + tws_backoff_time / 2);

                t.pending_requests.AddRange(new TickType[] 
                { 
                    TickType.BidPrice,
                    TickType.AskPrice,
                    TickType.BidSize,
                    TickType.AskSize,
                    TickType.Volume
                });

                if (type == SecurityType.Index)
                {
                    t.pending_requests.Add(TickType.ClosePrice);
                    t.pending_requests.Add(TickType.OpenPrice);
                    t.pending_requests.Add(TickType.HighPrice);
                    t.pending_requests.Add(TickType.LowPrice);
                    t.pending_requests.Add(TickType.LastPrice);
                    t.pending_requests.Add(TickType.LastSize);
                    t.pending_requests.Add(TickType.LastTimestamp);

                    Krs.Ats.IBNet.Contracts.Index contract = new Krs.Ats.IBNet.Contracts.Index(symbol, exchange);
                    contract.PrimaryExchange = primary_exchange;
                    contract.Currency = currency;
                    tws.RequestMarketData(id, contract, null, false);
                }
                else if (type == SecurityType.Stock)
                {
                    t.pending_requests.Add(TickType.ClosePrice);
                    t.pending_requests.Add(TickType.OpenPrice);
                    t.pending_requests.Add(TickType.HighPrice);
                    t.pending_requests.Add(TickType.LowPrice);
                    t.pending_requests.Add(TickType.LastPrice);
                    t.pending_requests.Add(TickType.LastSize);
                    t.pending_requests.Add(TickType.LastTimestamp);

                    Krs.Ats.IBNet.Contracts.Equity contract = new Krs.Ats.IBNet.Contracts.Equity(symbol, exchange);
                    contract.PrimaryExchange = primary_exchange;
                    contract.Currency = currency;
                    tws.RequestMarketData(id, contract, null, false);
                }
                else if (type == SecurityType.Future)
                {
                    t.pending_requests.Add(TickType.ClosePrice);
                    t.pending_requests.Add(TickType.OpenPrice);
                    t.pending_requests.Add(TickType.HighPrice);
                    t.pending_requests.Add(TickType.LowPrice);
                    t.pending_requests.Add(TickType.LastPrice);
                    t.pending_requests.Add(TickType.LastSize);
                    t.pending_requests.Add(TickType.LastTimestamp);

                    Krs.Ats.IBNet.Contracts.Future contract = new Krs.Ats.IBNet.Contracts.Future(symbol, exchange, details[i].Summary.Expiry);
                    contract.PrimaryExchange = primary_exchange;
                    contract.Currency = currency;
                    tws.RequestMarketData(id, contract, null, false);
                }
                else if (type == SecurityType.Option || type == SecurityType.FutureOption)
                {
                    Collection<GenericTickType> g = null;

                    if (tws_download_openint)
                    {
                        t.pending_requests.Add(TickType.OpenInterest);

                        g = new Collection<GenericTickType>();
                        g.Add(GenericTickType.OptionOpenInterest);
                        g.Add(GenericTickType.OptionVolume);
                    }

                    // Krs.Ats.IBNet.Contracts.Option contract = new Krs.Ats.IBNet.Contracts.Option(details[i].Summary.Symbol, details[i].Summary.LocalSymbol, details[i].Summary.Expiry, details[i].Summary.Right, (decimal)details[i].Summary.Strike);
                    // contract.PrimaryExchange = primary_exchange;
                    // contract.Currency = details[i].Summary.Currency;
                    // tws.RequestMarketData(id, contract, g, false);

                    tws.RequestMarketData(id, details[i].Summary, g, false);
                }

                // add tick to return list
                list.Add(t);
            }
            
            // setup timeout
            DateTime to = DateTime.Now.AddSeconds(tws_quote_timeout);

            foreach (TickData t in list)
            {
                if (to > DateTime.Now)
                {
                    // wait for result
                    t.ready_event.WaitOne((TimeSpan)(to - DateTime.Now), false);
                }

                // remove tick from list
                tick_list.Remove(t.id);

                // cancel market data return id to id queue
                tws.CancelMarketData(t.id);
                id_queue.Enqueue(t.id);
            }

            return list;
        }

        public ContData RequestDetails(string symbol, string exchange, string primary_exchange, SecurityType type, string currency, bool cache_ok)
        {
            // make sure we are connected
            if (!Connect)
            {
                Connect = true;
                if (!Connect) return null;
            }

            // check cache
            string cache_key = symbol + "," + exchange + "," + primary_exchange + "," + type.ToString() + "," + currency;
            if (cont_cache.ContainsKey(cache_key) && cache_ok)
            {
                ContData c_cache = cont_cache[cache_key];
                if (c_cache.contracts.Count > 0 &&
                    c_cache.timestamp.Date == DateTime.Now.Date) return c_cache;
            }

            // get id from id queue
            if (id_queue.Count == 0) return null;
            int id = id_queue.Dequeue();

            // get tick object
            if (!cont_list.ContainsKey(id)) cont_list[id] = new ContData(id);
            ContData c = cont_list[id];
            c.timestamp = DateTime.Now;

            // request underlying contract
            Contract contract = new Contract(symbol, exchange, type, currency);
            contract.PrimaryExchange = primary_exchange;
            tws.RequestContractDetails(id, contract);

            // wait for result
            c.ready_event.WaitOne(new TimeSpan(0, 0, tws_quote_timeout * 2), false);
            
            // remove tick from list
            cont_list.Remove(id);

            // return id to id queue
            id_queue.Enqueue(id);

            // keep in cache
            if (cont_cache.ContainsKey(cache_key)) cont_cache[cache_key] = c;
            else
            {
                if (cont_cache.Count >= MAX_CACHE_SIZE)
                    cont_cache.Remove(cont_cache.Keys.GetEnumerator().Current);

                cont_cache.Add(cache_key, c);
            }

            return c;
        }

        public HistData RequestHistoricalData(string symbol, string exchange, SecurityType type, string currency, DateTime end_date, bool cache_ok)        
        {
            // make sure we are connected
            if (!Connect)
            {
                Connect = true;
                if (!Connect) return null;
            }

            // check validity of end-date
            if (end_date < DateTime.Now.AddMonths(-6)) end_date = DateTime.Now.AddMonths(-6);

            // check cache
            string cache_key = symbol + "," + exchange + "," + type.ToString() + "," + currency;
            if (hist_cache.ContainsKey(cache_key) && cache_ok)
            {
                HistData h_cache = hist_cache[cache_key];
                if (h_cache.history.Count > 0 &&
                    h_cache.history[h_cache.history.Count-1].Date <= end_date &&
                    h_cache.timestamp.Date == DateTime.Now.Date) return h_cache;
            }

            // get id from id queue
            if (id_queue.Count == 0) return null;
            int id = id_queue.Dequeue();

            // get tick object
            if (!hist_list.ContainsKey(id)) hist_list[id] = new HistData(id);
            HistData h = hist_list[id];
            h.timestamp = DateTime.Now;

            // request underlying contract
            Contract contract = new Contract(symbol, exchange, type, currency);
            tws.RequestHistoricalData(id, contract, end_date, new TimeSpan(1,0,0,0), BarSize.OneDay, HistoricalDataType.Midpoint, 1);

            // wait for result
            h.ready_event.WaitOne(new TimeSpan(0, 0, tws_quote_timeout*2), false);

            // remove tick from list
            hist_list.Remove(id);

            // return id to id queue
            id_queue.Enqueue(id);

            // keep in cache
            if (hist_cache.ContainsKey(cache_key)) hist_cache[cache_key] = h;
            else
            {
                if (hist_cache.Count >= MAX_CACHE_SIZE)
                    hist_cache.Remove(hist_cache.Keys.GetEnumerator().Current);

                hist_cache.Add(cache_key, h);
            }

            return h;
        }

        void tws_NextValidId(object sender, NextValidIdEventArgs e)
        {
        }

        void tws_ConnectionClosed(object sender, ConnectionClosedEventArgs e)
        {
            tws_connection = false;
        }

        void tws_Error(object sender, Krs.Ats.IBNet.ErrorEventArgs e)
        {
            if (tick_list.ContainsKey(e.TickerId))
            {
                TickData t = tick_list[e.TickerId];
                t.error_code = e.ErrorCode;
                t.ready_event.Set();
            }
            else if (cont_list.ContainsKey(e.TickerId))
            {
                ContData c = cont_list[e.TickerId];
                c.error_code = e.ErrorCode;
                c.ready_event.Set();
            }
            else if (e.ErrorCode == ErrorMessage.NotConnected)
            {
                Connect = false;
            }
            else if (e.ErrorMsg.Contains("max rate"))
            {
                Thread.Sleep(1000 + tws_backoff_time);
            }
            else if (e.ErrorMsg.StartsWith("Max number of tickers has been reached"))
            {
                if (!tws_err_max_tickers_reached)
                {
                    MessageBox.Show("Max number of tickers has been reached.\nPlease reduce number of parallel tickers in plug-in configuration, or number of active tickers in TWS.    ", "Interactive Brokers Plug-In - TWS Error Message", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    tws_err_max_tickers_reached = true;
                }
            }
            else if (e.ErrorMsg.StartsWith("Connectivity between IB and TWS has been lost"))
            {
                MessageBox.Show("Connectivity between IB and TWS has been lost.    ", "Interactive Brokers Plug-In - TWS Error Message", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Connect = false;
            }
            else if (e.ErrorMsg.StartsWith("Market data farm connection is OK") ||
                     e.ErrorMsg.StartsWith("Can't find EId with tickerId") ||
                     e.ErrorMsg.StartsWith("No historical data query found for ticker id") ||
                     e.ErrorMsg.StartsWith("AlreadyConnected"))
            {
                // ignore
            }
#if DEBUG
            else MessageBox.Show(e.ErrorMsg, "TWS Client Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
#endif
        }

        void tws_TickPrice(object sender, TickPriceEventArgs e)
        {
            // get tick object
            if (!tick_list.ContainsKey(e.TickerId)) return;
            TickData t = tick_list[e.TickerId];

            switch (e.TickType)
            {
                case TickType.BidPrice:   // bid
                    t.price.bid = (double)e.Price;
                    break;
                case TickType.AskPrice:   // ask
                    t.price.ask = (double)e.Price;
                    break;
                case TickType.LastPrice:  // last
                    t.price.last = (double)e.Price;
                    break;
                case TickType.HighPrice:  // high
                    t.price.high = (double)e.Price;
                    break;
                case TickType.LowPrice:   // low
                    t.price.low = (double)e.Price;
                    break;
                case TickType.ClosePrice: // close
                    t.price.close = (double)e.Price;
                    break;
                case TickType.OpenPrice:  // open
                    t.price.open = (double)e.Price;
                    break;
                default:
                    break;
            }

            // remove from pending requests
            t.pending_requests.Remove(e.TickType);
            if (t.pending_requests.Count == 0) t.ready_event.Set();
        }

        void tws_TickSize(object sender, TickSizeEventArgs e)
        {
            // get tick object
            if (!tick_list.ContainsKey(e.TickerId)) return;
            TickData t = tick_list[e.TickerId];

            switch (e.TickType)
            {
                case TickType.BidSize:   // bid
                    t.size.bid = e.Size;
                    break;
                case TickType.AskSize:   // ask
                    t.size.ask = e.Size;
                    break;
                case TickType.LastSize:  // last
                    t.size.last = e.Size;
                    break;
                case TickType.Volume:    // volume
                    t.size.volume = e.Size;
                    break;
                case TickType.OpenInterest:
                    t.size.open_int = e.Size;
                    break;
                default:
                    break;
            }

            // remove from pending requests
            t.pending_requests.Remove(e.TickType);
            if (t.pending_requests.Count == 0) t.ready_event.Set();
        }

        void tws_TickGeneric(object sender, TickGenericEventArgs e)
        {
            // get tick object
            if (!tick_list.ContainsKey(e.TickerId)) return;
            TickData t = tick_list[e.TickerId];

            switch (e.TickType)
            {
                case TickType.OpenInterest:
                    t.size.open_int = (int)e.Value;
                    break;
                default:
                    break;
            }

            // remove from pending requests
            t.pending_requests.Remove(e.TickType);
            if (t.pending_requests.Count == 0) t.ready_event.Set();
        }

        void tws_TickString(object sender, TickStringEventArgs e)
        {
            // get tick object
            if (!tick_list.ContainsKey(e.TickerId)) return;
            TickData t = tick_list[e.TickerId];

            switch (e.TickType)
            {
                case TickType.LastTimestamp:
                    t.timestamp = new DateTime(1970, 1, 1).AddSeconds(double.Parse(e.Value));
                    break;
                default:
                    break;
            }

            // remove from pending requests
            t.pending_requests.Remove(e.TickType);
            if (t.pending_requests.Count == 0) t.ready_event.Set();
        }

        void tws_TickEfp(object sender, TickEfpEventArgs e)
        {
            // get tick object
            if (!tick_list.ContainsKey(e.TickerId)) return;
            TickData t = tick_list[e.TickerId];
        }

        void tws_TickOptionComputation(object sender, TickOptionComputationEventArgs e)
        {
            // get tick object
            if (!tick_list.ContainsKey(e.TickerId)) return;
            TickData t = tick_list[e.TickerId];

            switch (e.TickType)
            {
                default:
                    break;
            }

            // remove from pending requests
            t.pending_requests.Remove(e.TickType);
            if (t.pending_requests.Count == 0) t.ready_event.Set();
        }

        void tws_ContractDetailsEnd(object sender, ContractDetailsEndEventArgs e)
        {
            // get contact object
            if (!cont_list.ContainsKey(e.RequestId)) return;
            ContData c = cont_list[e.RequestId];

            // release waiting thread
            c.ready_event.Set();
        }

        void tws_ContractDetails(object sender, ContractDetailsEventArgs e)
        {
            // get contact object
            if (!cont_list.ContainsKey(e.RequestId)) return;
            ContData c = cont_list[e.RequestId];

            c.contracts.Add(e);
        }

        void tws_HistoricalData(object sender, HistoricalDataEventArgs e)
        {
            // get history object
            if (!hist_list.ContainsKey(e.RequestId)) return;
            HistData h = hist_list[e.RequestId];

            h.history.Add(e);
        }
    }
}
