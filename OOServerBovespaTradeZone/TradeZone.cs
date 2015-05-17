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
using System.Net;
using System.Globalization;
using System.IO;
using System.Threading;
using Microsoft.Win32;

namespace OOServerBovespaTradeZone
{
    public class TradeZone
    {
        private const int MaxSymbolsAtRegCommand = 32;

        // url templates
        private static string Server = "http://localhost:31008";
        private static string Login_Template = Server + @"/login?product={0}&version={1}&description={2}";
        private static string Logout_Template = Server + @"/logout?id={0}";
        private static string Symbols_Template = Server + @"/getsymbols?id={0}&pattern={1}";
        private static string History_Template = Server + @"/gethistory?id={0}&symbol={1}&from={2:yyyy-MM-dd HH:mm:ss}&to={3:yyyy-MM-dd HH:mm:ss}&size={4}&maxbars={5}";
        private static string Add_Order_Template = Server + @"/rt_order_symb?id={0}&symbol={1}";
        private static string Remove_Order_Template = Server + @"/rt_order_symb_remove?id={0}&symbol={1}";
        private static string Add_Mini_Template = Server + @"/rt_mini_symb?id={0}&symbol={1}";
        private static string Remove_Mini_Template = Server + @"/rt_mini_symb_remove?id={0}&symbol={1}";
        private static string Realtime_Template = Server + @"/getrealtime?id={0}";

        // login-id
        public string LoginId = null;

        // trade-zone data-center login parameters
        public string ProductId   = @"b2f415d2-4039-ae45-5372-100000009100";
        public string Description = @"OptionsOracle";
        public string Version     = @"2.0.0.0";

        // web-client
        public WebClient WebClient = new WebClient();

        // registed symbols lists
        private List<string>[] reg_list = new List<string>[2] { new List<string>(), new List<string>() };

        // cache dictionary
        private Dictionary<string, Tick> tick_list = new Dictionary<string, Tick>();
        private Dictionary<string, Minibook> minibook_list = new Dictionary<string, Minibook>();

        // listener thread
        Thread listen_thread = null;
        public ManualResetEvent listen_event = new ManualResetEvent(false);

        // debugging
        public bool   LogEnable = false;
        public string DebugLog = "";

        public TradeZone()
        {
            // increase connection limit to 64
            System.Net.ServicePointManager.DefaultConnectionLimit = 64;

            // setup encoding
            WebClient.Encoding = System.Text.Encoding.ASCII;
        }

        public enum TickType
        {
            FullTick,
            HistoryTick,
            TimeStamp
        }

        public class Tick
        {
            public string   Stock;
            public DateTime Timestamp;
            public uint     PreviousOrderNumber;
            public uint     OrderNumber;
            public uint     TradeVolume;
            public UInt64   TotalVolume;
            public float    TradeFinancialVolume;
            public float    TotalFinancialVolume;
            public float    Open;
            public float    High;
            public float    Low;
            public float    PreviousClose;
            public float    Last;
            public string   Status = " ";
            public TickType Type = TickType.TimeStamp;
        }

        public enum MinibookType
        {
            FullMinibook,
            HistoryMinibook,
            CompactMinibook
        }

        public class Minibook
        {
            public string Stock;
            public List<MinibookItem> Items;
            public string Status;
            public MinibookType Type = MinibookType.FullMinibook;
        }

        public class MinibookItem
        {
            public byte  order;
            public ulong TotalQuantityBids;
            public uint  NumberOfBids;
            public float BestBidPrice;
            public float BestAskPrice;
            public uint  NumberOfAsks;
            public ulong TotalQuantityAsks;
        }

        public static Minibook ParseMinibook(string minibook)
        {
            CultureInfo ci = new CultureInfo("pt-BR", false);
            MinibookType mt = MinibookType.HistoryMinibook;

            if (minibook.StartsWith("HM")) mt = MinibookType.HistoryMinibook;
            else if (minibook.StartsWith("FM")) mt = MinibookType.FullMinibook;
            else if (minibook.StartsWith("CM")) mt = MinibookType.CompactMinibook;
            else return null;

            minibook = minibook.Remove(0, 3);
            string[] cols = minibook.Split(';');
            Minibook m = new Minibook();
            m.Items = new List<MinibookItem>();
            m.Type = mt;
            m.Stock = cols[0].Trim();
            m.Status = cols[1].Trim();

            for (int i = 2; i < cols.Length; i++)
            {
                string[] i1 = cols[i].Split('=');
                if (i1.Length != 2) continue;

                string order = i1[0];
                string[] fields = i1[1].Substring(1, i1[1].Length - 2).Split(':');

                MinibookItem mi = new MinibookItem();
                mi.order = Convert.ToByte(order, ci);

                try
                {
                    mi.BestAskPrice = Convert.ToSingle(fields[0].Trim(), ci);
                    mi.NumberOfAsks = Convert.ToUInt32(fields[1].Trim(), ci);
                    mi.TotalQuantityAsks = Convert.ToUInt64(fields[2].Trim(), ci);
                    mi.BestBidPrice = Convert.ToSingle(fields[3].Trim(), ci);
                    mi.NumberOfBids = Convert.ToUInt32(fields[4].Trim(), ci);
                    mi.TotalQuantityBids = Convert.ToUInt64(fields[5].Trim(), ci);
                    m.Items.Add(mi);
                }
                catch { };
            }

            m.Items.Sort(delegate(MinibookItem m1, MinibookItem m2) { return m1.order.CompareTo(m2.order); });
            return m;
        }

        public static Tick ParseTick(string tick)
        {
            CultureInfo ci = new CultureInfo("pt-BR", false);            

            Tick t = new Tick();
            if (string.IsNullOrEmpty(tick)) return null;

            string type = tick.Substring(0, 2);
            tick = tick.Remove(0, 3);
            string[] cols = tick.Split(';');

            if (type.Equals("FT") || type.Equals("HT"))
            {
                if (type.Equals("FT")) t.Type = TickType.FullTick;
                else t.Type = TickType.HistoryTick;

                t.Stock = cols[0].Trim();
                DateTime Data = DateTime.Parse(cols[1].Trim() + " " + cols[2].Trim(), ci);

                t.Timestamp = Data;
                t.PreviousClose = Convert.ToSingle(cols[3].Trim(), ci);
                t.Open = Convert.ToSingle(cols[4].Trim(), ci);
                t.High = Convert.ToSingle(cols[5].Trim(), ci);
                t.Low = Convert.ToSingle(cols[6].Trim(), ci);
                t.Last = Convert.ToSingle(cols[7].Trim(), ci);
                t.TradeVolume = Convert.ToUInt32(cols[8].Trim(), ci);
                t.TotalVolume = Convert.ToUInt64(cols[9].Trim(), ci);
                t.TradeFinancialVolume = Convert.ToSingle(cols[10].Trim(), ci);
                t.TotalFinancialVolume = Convert.ToSingle(cols[11].Trim(), ci);
                t.PreviousOrderNumber = Convert.ToUInt32(cols[12].Trim(), ci);
                t.OrderNumber = Convert.ToUInt32(cols[13].Trim(), ci);
                if (cols.Length == 15) t.Status = cols[14].Trim();

                return t;
            }
            else if (type.Equals("TS"))
            {
                t.Type = TickType.TimeStamp;
                t.Stock = ".";
                DateTime Data = DateTime.Parse(cols[0].Trim() + " " + cols[1].Trim(), ci);
                t.Timestamp = Data;
                
                return t;
            }
            else return null;
        }

        public string Login()
        {
            try
            {
                // login
                string req = string.Format(Login_Template, new object[] { ProductId, Version, Description });
                string rep = WebClient.DownloadString(req);

                // debug
                if (LogEnable) DebugLog += req + "\r\n" + LoginId + "\r\n";

                // save login-id
                LoginId = rep;

                // clear lists
                reg_list[0].Clear();
                reg_list[1].Clear();

                // create listen thread
                listen_thread = new Thread(new ThreadStart(ListenerThread));
                listen_thread.Start();

                return LoginId;
            }
            catch (Exception e) { if (LogEnable) DebugLog += "(Login)" + e.Message + "\r\n"; return null; }
        }

        public bool Logout()
        {
            try
            {
                // logout
                string req = string.Format(Logout_Template, new object[] { LoginId });
                string rep = WebClient.DownloadString(req);

                // debug
                if (LogEnable) DebugLog += req + "\r\n" + rep + "\r\n";

                // clear list
                reg_list[0].Clear();
                reg_list[1].Clear();
                LoginId = null;

                // kill listen thread
                listen_thread.Abort();
                listen_thread.Join();

                return (rep != null && rep == "OK");
            }
            catch (Exception e) { if (LogEnable) DebugLog += "(Logout)" + e.Message + "\r\n"; return false; }
        }

        public bool ResetLogin()
        {
            Logout();
            return (Login() != null);
        }

        public void ListenerThread()
        {
            WebClient client = new WebClient();
            client.Encoding = System.Text.Encoding.ASCII;

            // open stream and reader
            string req = string.Format(Realtime_Template, new object[] { LoginId });
            Stream stream = client.OpenRead(req);
            StreamReader reader = new StreamReader(stream);

            // debug
            if (LogEnable) DebugLog += req + "\r\n";

            try
            {
                while (true)
                {
                    string rep = reader.ReadLine();
                    bool   parsed = false;

                    // debug
                    if (LogEnable) DebugLog += rep + "\r\n";

                    Tick t = ParseTick(rep);
                    if (t != null && t.Type != TickType.TimeStamp)
                    {
                        tick_list[t.Stock] = t;
                        parsed = true;
                    }

                    Minibook m = ParseMinibook(rep);
                    if (m != null && m.Items.Count > 0)
                    {
                        minibook_list[m.Stock] = m;
                        parsed = true;
                    }

                    // send signal if parsed
                    if (parsed) listen_event.Set();
                }
            }
            catch (Exception e) { if (LogEnable) DebugLog += "(ListenerThread)" + e.Message + "\r\n"; reader.Close(); stream.Close(); }
        }

        private List<string> RegisterSymbol(List<string> sym_list, int mode)
        {
            List<string> list = new List<string>();

            try
            {               
                for (int i = 0; i < sym_list.Count; )
                {
                    string sym_string = "";

                    for (int j = 0; j < MaxSymbolsAtRegCommand && i < sym_list.Count; )
                    {
                        if (!reg_list[mode].Contains(sym_list[i]))
                        {
                            if (j == 0) sym_string = sym_list[i];
                            else sym_string += "," + sym_list[i];
                            j++;
                        }
                        i++;
                    }
                    if (sym_string == "") continue;

                    string req = string.Format(mode == 0 ? Add_Order_Template : Add_Mini_Template, new object[] { LoginId, sym_string });
                    string rep = WebClient.DownloadString(req);

                    // debug
                    if (LogEnable) DebugLog += req + "\r\n" + rep + "\r\n";

                    foreach (string s in rep.Split(new char[] { ',' }))
                    {
                        string[] x = s.Split(new char[] { '=', ' ' });
                        if (x.Length >= 3 && x[x.Length - 1] == "OK")
                        {
                            list.Add(x[0]);
                            reg_list[mode].Add(x[0]);
                        }
                    }                    
                }
            }
            catch (Exception e) { if (LogEnable) DebugLog += "(RegisterSymbol)" + e.Message + "\r\n"; return list; }

            return list;            
        }

        private bool UnregisterSymbol(List<string> sym_list, int mode)
        {
            try
            {
                for (int i = 0; i < sym_list.Count; )
                {
                    string sym_string = "";

                    for (int j = 0; j < MaxSymbolsAtRegCommand && i < sym_list.Count; )
                    {
                        if (reg_list[mode].Contains(sym_list[i]))
                        {
                            if (j == 0) sym_string = sym_list[i];
                            else sym_string += "," + sym_list[i];
                            j++;

                            reg_list[mode].Remove(sym_list[i]);
                        }
                        i++;
                    }
                    if (sym_string == "") continue;

                    string req = string.Format(mode == 0 ? Remove_Order_Template : Remove_Mini_Template, new object[] { LoginId, sym_string });
                    string rep = WebClient.DownloadString(req);

                    // debug
                    if (LogEnable) DebugLog += req + "\r\n" + rep + "\r\n";
                }
            }
            catch (Exception e) { if (LogEnable) DebugLog += "(UnregisterSymbol)" + e.Message + "\r\n"; ResetLogin(); }

            return true;
        }

        public Dictionary<string, Minibook> GetQuoteMiniBook(List<string> sym_list)
        {
            Dictionary<string, Minibook> list = new Dictionary<string, Minibook>();
            List<string> fnd_list = new List<string>();

            // register symbols
            List<string> reg_list = RegisterSymbol(sym_list, 1);            

            while (sym_list.Count > fnd_list.Count)
            {
                int changed = 0;

                // wait for thread to provide additional quotes
                listen_event.WaitOne(new TimeSpan(0, 0, 10), false);

                foreach (string sym in sym_list)
                {
                    if (!list.ContainsKey(sym) && minibook_list.ContainsKey(sym))
                    {
                        list[sym] = minibook_list[sym];
                        fnd_list.Add(sym);
                        changed++;
                    }
                }
                if (changed == 0 || reg_list.Count == 0) break;
            }

            return list;
        }

        public Dictionary<string, Tick> GetQuoteTick(List<string> sym_list)
        {
            Dictionary<string, Tick> list = new Dictionary<string, Tick>();
            List<string> fnd_list = new List<string>();

            // register symbols
            List<string> reg_list = RegisterSymbol(sym_list, 0);
            
            while (sym_list.Count > fnd_list.Count)
            {
                int changed = 0;

                // wait for thread to provide additional quotes
                listen_event.WaitOne(new TimeSpan(0, 0, 10), false);   

                foreach (string sym in sym_list)
                {
                    if (!list.ContainsKey(sym) && tick_list.ContainsKey(sym))
                    {
                        list[sym] = tick_list[sym];
                        fnd_list.Add(sym);
                    }
                }
                if (changed == 0 || reg_list.Count == 0) break;
            }

            return list;
        }

        public string GetHistoryStream(string symbol, DateTime start_date, DateTime end_date)
        {
            try
            {
                string req = string.Format(History_Template, new object[] { LoginId, symbol, start_date, end_date, 1440, 0 });
                string rep = WebClient.DownloadString(req);

                // debug
                if (LogEnable) DebugLog += req + "\r\n" + rep + "\r\n";

                return rep;
            }
            catch (Exception e) { if (LogEnable) DebugLog += "(GetHistoryStream)" + e.Message + "\r\n"; return null; }
        }

        public string GetSymbolsStream(string patern)
        {
            try
            {
                string req = string.Format(Symbols_Template, new object[] { LoginId, patern });
                string rep = WebClient.DownloadString(req);

                // debug
                if (LogEnable) DebugLog += req + "\r\n" + rep + "\r\n";

                return rep;
            }
            catch (Exception e) { if (LogEnable) DebugLog += "(GetSymbolsStream)" + e.Message + "\r\n"; return null; }
        }

        //private static void Start()
        //{
        //    RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\Tradezone\\DC");
        //    string path = "";
            
        //    if (key != null)
        //    {
        //        path = key.GetValue("path").ToString();
        //        key.Close();
        //    }
        //    System.Win3
        //    Process[] ps = Process.GetProcessesByName("TZDC");
        //    if (ps.Length != 0) return;

        //    ProcessStartInfo si = new ProcessStartInfo(path);
        //    si.WorkingDirectory = System.IO.Path.GetDirectoryName(path);
        //    Process.Start(si);

        //    System.Threading.Thread.Sleep(2000);

        //    ps = Process.GetProcessesByName("TZDC");
        //    if (ps.Length != 0) ps[0].WaitForInputIdle();
        //}
    }
}
