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
    public class Yahoo
    {
        // consts
        private const int MAX_CACHE_SIZE = 32;

        // configuration
        private string yahoo_exchange_suffix = "";
        public string YahooExchangeSuffix { set { yahoo_exchange_suffix = value; } }

        // web capture client
        private WebCapture cap = null;

        // cache dictionary
        private Dictionary<string, HistData> hist_cache = new Dictionary<string, HistData>();

        public class HistData
        {
            public ArrayList history = new ArrayList();
            public DateTime timestamp;
        }

        public Yahoo(WebCapture cap)
        {
            this.cap = cap;
        }

        // get stock/option historical prices 
        public ArrayList GetHistoricalData(string ticker, DateTime start, DateTime end)
        {
            // check cache
            string cache_key = ticker + "," + yahoo_exchange_suffix;
            if (hist_cache.ContainsKey(cache_key))
            {
                HistData h_cache = hist_cache[cache_key];
                if (h_cache.history.Count > 0 &&
                    ((History)h_cache.history[h_cache.history.Count - 1]).date <= start &&
                    h_cache.timestamp.Date == DateTime.Now.Date) return h_cache.history;
            }

            // force en-US culture
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US", false);

            double p_factor = 1.0;

            ArrayList list = new ArrayList();

            string em = (end.Month - 1).ToString();
            string ed = (end.Day).ToString();
            string ey = (end.Year).ToString();
            string sm = (start.Month - 1).ToString();
            string sd = (start.Day).ToString();
            string sy = (start.Year).ToString();

            // check for index
            bool is_index = ticker.StartsWith("^");

            // create url ticker
            string url_ticker;
            if (is_index || yahoo_exchange_suffix == "") url_ticker = ticker;
            else url_ticker = ticker + "." + yahoo_exchange_suffix;

            string page = cap.DownloadHtmlWebPage(@"http://ichart.yahoo.com/table.csv?s=" + url_ticker + @"&amp;d=" + em + @"&amp;e=" + ed + @"&amp;f=" + ey + @"&amp;g=d&amp;a=" + sm + @"&amp;b=" + sd + @"&amp;c=" + sy + @"&amp;ignore=.csv");

            string[] split1 = page.Split(new char[] { '\r', '\n' });

            for (int i = 1; i < split1.Length; i++)
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

            HistData h = new HistData();
            h.history = list;
            h.timestamp = DateTime.Now;

            // keep in cache
            if (hist_cache.ContainsKey(cache_key)) hist_cache[cache_key] = h;
            else
            {
                if (hist_cache.Count >= MAX_CACHE_SIZE)
                    hist_cache.Remove(hist_cache.Keys.GetEnumerator().Current);

                hist_cache.Add(cache_key, h);
            }

            return list;
        }
    }
}
