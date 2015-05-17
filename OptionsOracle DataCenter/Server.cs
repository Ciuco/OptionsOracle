/*
 * OptionsOracle DataCenter
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
using System.Xml;
using System.IO;
using System.Data;
using System.Net.Sockets;
using System.Threading;
using OptionsOracle.DataCenter.Data;

namespace OptionsOracle.DataCenter
{
    public class Server
    {
        private const int WEB_SERVER_PORT = 31013;

        private TcpListener server = null; 

        public Server()      
        {
        }

        public void Start(MarketSet marketSet)
        {                       
            try
            {
                // create server end-point
                server = new TcpListener(IPAddress.Loopback, WEB_SERVER_PORT);
                server.Start();

                while (true)
                {
                    // wait for requests
                    TcpClient client = server.AcceptTcpClient();
                    new Thread(new Worker(client, marketSet).ProcessRequest).Start();
                }
            }
            catch
            {
                server.Stop();
                server = null;
            }
        }

        class Worker
        {
            const string HTTP_OK = "HTTP/1.1 200 OK";
            const string HTTP_BAD_REQUEST = "HTTP/1.1 400 Bad Request Message";

            private TcpClient client = null;
            private MarketSet marketSet = null;

            public Worker(TcpClient client, MarketSet marketSet)
            {
                this.client = client;
                this.marketSet = marketSet;
            }

            public void ProcessRequest()
            {

                NetworkStream stream = client.GetStream();
                StreamReader  reader = new StreamReader(stream);
                StreamWriter  writer = new StreamWriter(stream);

                try
                {
                    while (true)
                    {
                        string req = reader.ReadLine();
                        if (req == "") break;

                        string[] split = req.Split(' ');
                        if (split[0].ToUpper() == "GET")
                        {
                            string cmd = split[1].TrimStart(new char[] { '/' });

                            string rep = ProcessCommand(cmd);

                            if (rep != null)
                            {
                                writer.WriteLine(HTTP_OK);
                                writer.WriteLine("Content-Type: text/html; charset=ascii");
                                writer.WriteLine("Server: OptionsOracle DataServer " + Config.Local.CurrentVersion);
                                writer.WriteLine("");
                                writer.WriteLine(rep);
                            }
                            else
                            {
                                writer.Write(HTTP_BAD_REQUEST);
                                writer.WriteLine("");
                            }
                        }
                    }

                }
                catch { }
               
                writer.Close();
                reader.Close();
                stream.Close();
                             
                client.Close();
                client = null;
            }

            private Dictionary<string, string> ProcessArgs(string arg_line)
            {
                Dictionary<string, string> dict = new Dictionary<string, string>();

                foreach (string arg in arg_line.Split('&'))
                {
                    string[] split = arg.Split('=');
                    if (split.Length == 2 && !dict.ContainsKey(split[0])) dict[split[0]] = split[1];
                }

                return dict;
            }

            private string ProcessCommand(string cmd_line)
            {
                string[] split = cmd_line.ToLower().Split('?');
                if (split.Length < 1 || split.Length > 2) return null;

                string cmd = split[0];
                Dictionary<string, string> args = (split.Length == 1) ? null : ProcessArgs(split[1]);

                switch (cmd)
                {
                    case "getquote":
                        return GetQuote(args);
                    case "getoptionchain":
                        return GetOptionChain(args);
                    case "gethistoricalvolatility":
                        return GetHistoricalVolatility(args);
                    case "getinterestrate":
                        return GetAnnualInterestRate(args);
                    case "gethistoricaldata":
                        return GetHistoricalData(args);
                    case "getsymbollookup":
                        return GetStockSymbolLookup(args);
                    default:
                        return null;
                }
            }

            private string GetQuote(Dictionary<string, string> args)
            {
                if (!args.ContainsKey("symbol")) return null;

                MarketSet.QuoteTableRow row = marketSet.QuoteTable.FindBySymbol(args["symbol"].ToUpper());
                if (row == null) return "FAILED\r\n";

                string rep = "";
                foreach (object obj in row.ItemArray)
                {
                    if (rep == "") rep = obj.ToString();
                    else rep += "\t" + obj.ToString();
                }
                rep += "\r\n";

                return rep;
            }

            private string GetOptionChain(Dictionary<string, string> args)
            {
                if (!args.ContainsKey("symbol")) return null;

                DataRow[] rows = marketSet.OptionTable.Select("Underlying = '" + args["symbol"].ToUpper() + "'");
                if (rows == null) return "FAILED\r\n";
                
                string rep_list = "";
                
                foreach (DataRow row in rows)
                {
                    string rep = "";
                    foreach (object obj in row.ItemArray)
                    {
                        if (rep == "") rep = obj.ToString();
                        else rep += "\t" + obj.ToString();
                    }
                    rep_list += rep + "\r\n";
                }

                return rep_list;
            }

            // get historical volatility
            private string GetHistoricalVolatility(Dictionary<string, string> args)
            {
                if (!args.ContainsKey("symbol")) return null;

                MarketSet.QuoteTableRow row = marketSet.QuoteTable.FindBySymbol(args["symbol"].ToUpper());
                if (row == null) return "FAILED\r\n";

                string rep = row.HistoricalVolatility.ToString() + "\r\n";

                return rep;
            }

            // get annual interest rate
            private string GetAnnualInterestRate(Dictionary<string, string> args)
            {
                return "NaN\r\n";
            }

            // get stock/option historical prices 
            private string GetHistoricalData(Dictionary<string, string> args)
            {
                if (!args.ContainsKey("symbol") || 
                    !args.ContainsKey("start")  || 
                    !args.ContainsKey("end")) return null;

                DataRow[] rows = marketSet.HistoryTable.Select("(Symbol = '" + args["symbol"].ToUpper() + "') AND (Date >= '" + args["start"] + "') AND (Date <= '" + args["end"] + "')");
                if (rows == null) return "\r\n";

                string rep_list = "";

                foreach (DataRow row in rows)
                {
                    string rep = "";
                    foreach (object obj in row.ItemArray)
                    {
                        if (rep == "") rep = obj.ToString();
                        else rep += "\t" + obj.ToString();
                    }
                    rep_list += rep + "\r\n";
                }

                return rep_list;
            }

            // get stock name lookup results
            private string GetStockSymbolLookup(Dictionary<string, string> args)
            {
                if (!args.ContainsKey("lookup")) return null;

                DataRow[] rows = marketSet.QuoteTable.Select("(Symbol LIKE '*" + args["lookup"].ToUpper() + "*') OR (Name LIKE '*" + args["lookup"].ToUpper() + "*')");
                if (rows == null) return "\r\n";

                string rep_list = "";

                foreach (MarketSet.QuoteTableRow row in rows)
                {
                    rep_list += row.Name + "\t" + row.Symbol + "\r\n";
                }

                return rep_list;
            }
        }
    }
}
