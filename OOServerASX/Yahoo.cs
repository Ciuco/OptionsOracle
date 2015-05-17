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
using System.Text.RegularExpressions;
using OOServerLib.Web;
using OOServerLib.Interface;
using OOServerLib.Global;
using OOServerLib.Config;

namespace OOServerASX
{
    public class Yahoo
    {
        // consts
        private const int MAX_CACHE_SIZE = 32;

        // configuration
        private string yahoo_exchange_suffix = "AX";
        public string YahooExchangeSuffix { set { yahoo_exchange_suffix = value; } }

        // web capture client
        private WebCapture cap = null;

        // parse html page
        private XmlParser prs = new XmlParser();

        // cache dictionary
        private Dictionary<string, HistData> hist_cache = new Dictionary<string, HistData>();

        // culture
        private CultureInfo ci = new CultureInfo("en-US", false);

        public class HistData
        {
            public ArrayList history = new ArrayList();
            public DateTime timestamp;
        }

        public Yahoo(WebCapture cap)
        {
            this.cap = cap;
        }

        public string CorrectSymbol(string ticker)
        {
            ticker = ticker.ToUpper().Trim();

            switch (ticker)
            {
                case "^XJO":
                    return "^AXJO";
                default:
                    break;
            }

            return ticker;
        }

        // get stock latest options chain
        public ArrayList GetOptionsChain(string ticker)
        {
            // correct ticker symbol
            ticker = CorrectSymbol(ticker);             
            bool is_index = ticker.StartsWith("^");

            // create url ticker
            string yahoo_ticker;
            if (is_index || yahoo_exchange_suffix == "") yahoo_ticker = ticker;
            else yahoo_ticker = ticker + "." + yahoo_exchange_suffix;

            string main_url = @"http://finance.yahoo.com/q/op?s=" + yahoo_ticker;

            XmlDocument main_xml = DownloadXmlPartialWebPage(main_url, "<body", "</body>", 1, 1);
            if (main_xml == null) return null;

            XmlNode nd, table_nd = prs.GetXmlNodeByPath(main_xml.FirstChild, @"body\div\div(3)\br\table");
            if (table_nd == null) return null;

            List<string> url_list = new List<string>();
            url_list.Add(main_url);

            for (int i = 1; ; i++)
            {
                nd = prs.GetXmlNodeByPath(table_nd, @"tr(2)\td\a(" + i.ToString() + ")");
                if (nd == null) break;

                if (nd.Attributes.Count == 1 && nd.Attributes[0].Name == "href")
                    url_list.Add(@"http://finance.yahoo.com" + System.Web.HttpUtility.HtmlDecode(nd.Attributes[0].Value));
            }

            // create options array list
            ArrayList options_list = new ArrayList();
            options_list.Clear();
            options_list.Capacity = 1024;

            try
            {
                foreach (string url in url_list)
                {
                    XmlDocument xml;

                    if (url == main_url) xml = main_xml;
                    else xml = DownloadXmlPartialWebPage(url, "<body", "</body>", 1, 1);

                    List<XmlNode> table_nd_list = new List<XmlNode>();

                    for (int i = 1; ; i++)
                    {
                        table_nd = prs.FindXmlNodeByName(xml.FirstChild, "td", "class=yfnc_tablehead1", i);
                        if (table_nd == null) break;

                        if (table_nd.ParentNode == null || table_nd.ParentNode.ParentNode == null) continue;
                        else table_nd = table_nd.ParentNode.ParentNode;

                        if (!table_nd_list.Contains(table_nd))
                            table_nd_list.Add(table_nd);
                    }

                    for (int i = 0; i < table_nd_list.Count; i++)
                    {
                        table_nd = table_nd_list[i];

                        for (int j = 2; ; j++)
                        {
                            try
                            {
                                XmlNode row_nd = prs.GetXmlNodeByPath(table_nd, @"tr(" + j.ToString() + ")");
                                if (row_nd == null) break;

                                // get option symbol
                                nd = prs.GetXmlNodeByPath(row_nd, @"td(2)");
                                if (nd == null || string.IsNullOrEmpty(nd.InnerText)) break;

                                string symbol = "." + nd.InnerText.ToUpper().Trim();

                                Option option = new Option();

                                option.stock = ticker;
                                option.stocks_per_contract = 100;

                                // get option type from symbol
                                if (symbol[9] == 'C') option.type = "Call";
                                else if (symbol[9] == 'P') option.type = "Put";
                                else continue;

                                // get option expiration from symbol
                                option.expiration = new DateTime(int.Parse(symbol.Substring(3, 2)) + 2000, int.Parse(symbol.Substring(5, 2)), int.Parse(symbol.Substring(7, 2))).Date;

                                // add option symbol prefix
                                option.symbol = "." + symbol;

                                // get option strike
                                nd = prs.GetXmlNodeByPath(row_nd, @"td(1)");
                                if (nd == null || string.IsNullOrEmpty(nd.InnerText)) break;
                                if (!double.TryParse(nd.InnerText, NumberStyles.Number, ci, out option.strike))
                                    continue;

                                // get last price
                                nd = prs.GetXmlNodeByPath(row_nd, @"td(3)");
                                if (nd == null || !double.TryParse(nd.InnerText, NumberStyles.Number, ci, out option.price.last))
                                    option.price.last = double.NaN;

                                // get change price
                                nd = prs.GetXmlNodeByPath(row_nd, @"td(4)");
                                if (nd == null || !double.TryParse(nd.InnerText, NumberStyles.Number, ci, out option.price.change))
                                    option.price.change = double.NaN;

                                if (option.price.change != 0)
                                {
                                    nd = prs.GetXmlNodeByPath(row_nd, @"td(4)\span\img");
                                    if (nd == null) option.price.change = 0;

                                    foreach (XmlAttribute attr in nd.Attributes)
                                        if (attr.Name == "alt" && attr.Value == "Down") option.price.change *= -1;
                                }

                                // get bid price
                                nd = prs.GetXmlNodeByPath(row_nd, @"td(5)");
                                if (nd == null || !double.TryParse(nd.InnerText, NumberStyles.Number, ci, out option.price.bid))
                                    option.price.bid = double.NaN;

                                // get ask price
                                nd = prs.GetXmlNodeByPath(row_nd, @"td(6)");
                                if (nd == null || !double.TryParse(nd.InnerText, NumberStyles.Number, ci, out option.price.ask))
                                    option.price.ask = double.NaN;

                                // get volume
                                nd = prs.GetXmlNodeByPath(row_nd, @"td(7)");
                                if (nd == null || !double.TryParse(nd.InnerText, NumberStyles.Number, ci, out option.volume.total))
                                    option.volume.total = 0;

                                // get open-int
                                nd = prs.GetXmlNodeByPath(row_nd, @"td(8)");
                                if (nd == null || !int.TryParse(nd.InnerText, NumberStyles.Number, ci, out option.open_int))
                                    option.open_int = 0;

                                option.update_timestamp = DateTime.Now;

                                options_list.Add(option);
                            }
                            catch { }
                        }
                    }
                }
            }
            catch { }

            return options_list;
        }

        // get stock latest quote
        public Quote GetQuote(string ticker)
        {
            // correct ticker symbol
            ticker = CorrectSymbol(ticker);
            bool is_index = ticker.StartsWith("^");

            // create url ticker
            string yahoo_ticker;
            if (is_index || yahoo_exchange_suffix == "") yahoo_ticker = ticker;
            else yahoo_ticker = ticker + "." + yahoo_exchange_suffix;

            string[] split;
            string url = @"http://finance.yahoo.com/q?s=" + yahoo_ticker;

            // create xml parser
            int div_idx;
            XmlNode table, nd;

            XmlDocument xml = DownloadXmlPartialWebPage(url, "<body", "</body>", 1, 1);
            if (xml == null) xml = DownloadXmlPartialWebPage(url, "<body", "</body>", 1, 1);
            if (xml == null) return null;

            // create qutote
            Quote quote = new Quote();

            // get ticker
            quote.stock = ticker;

            // get name
            nd = null;
            for (div_idx = 1; nd == null && div_idx < 4; div_idx++)
            {
                nd = prs.GetXmlNodeByPath(xml.FirstChild, @"body\div\div(2)\div(" + div_idx.ToString() + @")\h1");
            }
            if (nd == null) return null;

            split = nd.InnerText.Replace("&amp;", "&").Split(new char[] { '(' });
            quote.name = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(split[0].Trim().ToLower());
            if (quote.name == "") quote.name = quote.stock;

            // get first data table    
            table = prs.GetXmlNodeByPath(xml.FirstChild, @"body\div\div(2)\div(" + (div_idx + 1).ToString() + @")\div(2)\div\div(3)");
            if (table == null || prs.GetXmlNodeByPath(table, @"table(1)\tr(1)") == null) table = prs.GetXmlNodeByPath(xml.FirstChild, @"body\div\div(2)\div(" + (div_idx + 1).ToString() + @")\div(2)\div\div(2)");
            if (table == null) return null;

            string colm = "td(1)";
            if (prs.GetXmlNodeByPath(table, @"table(1)\tr(1)\th(1)") == null) colm = "td(2)";

            // get last price            
            nd = prs.GetXmlNodeByPath(table, @"table(1)\tr(1)\" + colm);
            if (nd == null || !double.TryParse(nd.InnerText, NumberStyles.Number, ci, out quote.price.last)) return null;

            // get date and time
#if (false)
            nd = prs.GetXmlNodeByPath(table, @"table(1)\tr(2)\" + colm);
            if (nd == null) return null;
            split = nd.InnerText.Split(new char[] { ' ', '-' });
            string dat = DateTime.Now.ToString("yyyy-MM-dd"); ;
            string tim = Quote.DAY_END;
            try
            {
                if (split[0].Contains("AM") || split[1].Contains("PM"))
                    tim = DateTime.Parse(split[0], ci).TimeOfDay.ToString();
            }
            catch { }
            quote.update_timestamp = DateTime.Parse(tim + " " + dat, ci);
#else
            quote.update_timestamp = DateTime.Now;
#endif

            if (ticker.Contains("^"))
            {
                // get open price            
                nd = prs.GetXmlNodeByPath(table, @"table(2)\tr(2)\" + colm);
                if (nd == null || !double.TryParse(nd.InnerText, NumberStyles.Number, ci, out quote.price.open)) return null;

                // get price change
                quote.price.change = Math.Round(quote.price.last - quote.price.open, 4);
            }
            else
            {
                // get open price            
                nd = prs.GetXmlNodeByPath(table, @"table(1)\tr(4)\" + colm);
                if (nd == null || !double.TryParse(nd.InnerText, NumberStyles.Number, ci, out quote.price.open))
                {
                    nd = prs.GetXmlNodeByPath(table, @"table(1)\tr(5)\" + colm);
                    if (nd == null || !double.TryParse(nd.InnerText, NumberStyles.Number, ci, out quote.price.open)) return null;
                }

                // get price change
                quote.price.change = Math.Round(quote.price.last - quote.price.open, 4);
                // get bid price
                nd = prs.GetXmlNodeByPath(table, @"table(1)\tr(6)\" + colm);
                if (nd == null) return null;
                split = nd.InnerText.Split(new char[] { ' ', 'x' });
                quote.price.bid = double.NaN;
                double.TryParse(split[0], NumberStyles.Number, ci, out quote.price.bid);

                // get ask price
                nd = prs.GetXmlNodeByPath(table, @"table(1)\tr(7)\" + colm);
                if (nd == null) return null;
                split = nd.InnerText.Split(new char[] { ' ', 'x' });
                quote.price.ask = double.NaN;
                double.TryParse(split[0], NumberStyles.Number, ci, out quote.price.ask);

                // get high-low prices
                nd = prs.GetXmlNodeByPath(table, @"table(2)\tr(1)\" + colm);
                if (nd == null) return null;
                split = nd.InnerText.Split(new char[] { ' ', '-' });
                quote.price.low = double.NaN;
                quote.price.high = double.NaN;
                if (split.Length >= 4)
                {
                    double.TryParse(split[0], NumberStyles.Number, ci, out quote.price.low);
                    double.TryParse(split[3], NumberStyles.Number, ci, out quote.price.high);
                }

                // get date and time
                nd = prs.GetXmlNodeByPath(table, @"table(2)\tr(3)\" + colm);
                if (nd == null || !double.TryParse(nd.InnerText, NumberStyles.Number, ci, out quote.volume.total)) return null;
            }

            return quote;
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

            double p_factor = 1.0;

            ArrayList list = new ArrayList();

            string em = (end.Month - 1).ToString();
            string ed = (end.Day).ToString();
            string ey = (end.Year).ToString();
            string sm = (start.Month - 1).ToString();
            string sd = (start.Day).ToString();
            string sy = (start.Year).ToString();

            // correct ticker symbol
            ticker = CorrectSymbol(ticker);
            bool is_index = ticker.StartsWith("^");

            // create url ticker
            string yahoo_ticker;
            if (is_index || yahoo_exchange_suffix == "") yahoo_ticker = ticker;
            else yahoo_ticker = ticker + "." + yahoo_exchange_suffix;

            string page = cap.DownloadHtmlWebPage(@"http://ichart.yahoo.com/table.csv?s=" + yahoo_ticker + @"&amp;d=" + em + @"&amp;e=" + ed + @"&amp;f=" + ey + @"&amp;g=d&amp;a=" + sm + @"&amp;b=" + sd + @"&amp;c=" + sy + @"&amp;ignore=.csv");

            string[] split1 = page.Split(new char[] { '\r', '\n' });

            for (int i = 1; i < split1.Length; i++)
            {
                History history = new History();
                history.stock = ticker;

                try
                {
                    string[] split2 = split1[i].Split(new char[] { ',' });
                    if (split2.Length < 6) continue;

                    history.date = DateTime.Parse(split2[0], ci);
                    history.price.open = double.Parse(split2[1], ci) * p_factor;
                    history.price.high = double.Parse(split2[2], ci) * p_factor;
                    history.price.low = double.Parse(split2[3], ci) * p_factor;
                    history.price.close = double.Parse(split2[4], ci) * p_factor;
                    history.price.close_adj = double.Parse(split2[6], ci) * p_factor;
                    history.volume.total = double.Parse(split2[5], ci);

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

        public ArrayList GetStocksWithEarningDate(DateTime date)
        {
            string lookup_url = @"http://biz.yahoo.com/research/earncal/" + date.Year.ToString() + date.Month.ToString("d2") + date.Day.ToString("d2") + @".html";

            XmlDocument xml = cap.DownloadXmlWebPage(lookup_url);
            if (xml == null) return null;

            ArrayList symbol_list = new ArrayList();
            symbol_list.Capacity = 256;

            for (int i = 0; i < symbol_list.Capacity; i++)
            {
                string entry = "";

                XmlNode nd, root_node = prs.GetXmlNodeByPath(xml.FirstChild, @"body\link\p\p\p\table\tr\td\table\tr(" + (i + 3).ToString() + @")");
                if (root_node == null) break;

                try
                {
                    // stock name
                    nd = prs.GetXmlNodeByPath(root_node, @"td(1)");
                    if (nd == null) break;
                    entry = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(System.Web.HttpUtility.HtmlDecode(nd.InnerText).Trim().ToLower()).Replace('(', '[').Replace(')', ']');

                    // stock ticker
                    nd = prs.GetXmlNodeByPath(root_node, @"td(2)\a");
                    if (nd == null) break;
                    if (nd.InnerText.Contains(".")) continue;
                    entry += " (" + nd.InnerText + ")";

                    // add name + ticker entry
                    symbol_list.Add(entry);
                }
                catch { }
            }

            symbol_list.TrimToSize();
            return symbol_list;
        }

        // get stock name lookup results
        public ArrayList GetStockSymbolLookup(string name)
        {
            string lookup_url = @"http://finance.yahoo.com/lookup?s=" + name + @"&t=S&m=ALL";

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
                if (entry.Contains("." + yahoo_exchange_suffix)) symbol_list.Add(entry.Replace("." + yahoo_exchange_suffix, ""));
            }

            symbol_list.TrimToSize();
            return symbol_list;
        }

        private XmlDocument DownloadXmlPartialWebPage(string url, string sta_str, string end_str, int sta_count, int end_count)
        {
            try
            {
                string page = Regex.Replace(cap.DownloadHtmlWebPage(url), "<script.*?</script>", "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
                return cap.ConvertHtmlToXml(@"<html>" + cap.GetPartialWebPage(page, sta_str, end_str, sta_count, end_count) + @"</html>");
            }
            catch { return null; }
        }
    }
}
