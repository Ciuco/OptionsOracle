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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Globalization;
using OptionsOracle.DataCenter.Data;

namespace OptionsOracle.DataCenter
{
    public class Core
    {
        public enum FileFormat 
        { 
            OpoFile, 
            OpdFile, 
            CsvFile 
        };

        public class Opd
        {
            public static bool Load(string filename, string rule, MarketSet data)
            {
                try
                {
                    data.Clear();
                    data.ReadXml(filename);
                }
                catch { return false; }

                return true;
            }
        }

        public class Opo
        {
            public static bool Load(string filename, string rule, MarketSet data)
            {
                OptionsSet opod = new OptionsSet();
                
                try
                {
                    opod.ReadXml(filename);
                }
                catch { return false; }

                // disable data notifications
                data.QuoteTable.BeginLoadData();
                data.OptionTable.BeginLoadData();

                foreach (OptionsSet.QuotesTableRow row in opod.QuotesTable.Rows)
                {
                    DateTime timestamp;
                    string symbol, name, currency;
                    double last, change, open, low, high, bid, ask, volume, dvrate, hisvol;

                    try { symbol = row.Stock; }
                    catch { continue; }

                    try { name = row.Name; }
                    catch { name = row.Stock; }

                    try { currency = ""; }
                    catch { currency = ""; }

                    try { last = row.Last; }
                    catch { continue; }

                    try { change = row.Change; }
                    catch { change = double.NaN; }

                    try { open = row.Open; }
                    catch { open = double.NaN; }

                    try { low = row.Low; }
                    catch { low = double.NaN; }

                    try { high = row.High; }
                    catch { high = double.NaN; }

                    try { bid = row.Bid; }
                    catch { bid = double.NaN; }

                    try { ask = row.Ask; }
                    catch { ask = double.NaN; }

                    try { volume = row.Volume; }
                    catch { volume = double.NaN; }

                    try { dvrate = row.DividendRate; }
                    catch { dvrate = double.NaN; }

                    try { hisvol = row.HistoricalVolatility; }
                    catch { hisvol = double.NaN; }

                    try { timestamp = row.UpdateTimeStamp; }
                    catch { timestamp = DateTime.Now; }

                    // delete old entry in option table
                    DataRow del = data.QuoteTable.FindBySymbol(symbol);
                    if (del != null) del.Delete();

                    // add new entry to quote table
                    data.QuoteTable.AddQuoteTableRow(
                        symbol, name, currency, last, change, open, low, high,
                        bid, ask, volume, dvrate, timestamp, hisvol);
                }

                foreach (OptionsSet.OptionsTableRow row in opod.OptionsTable.Rows)
                {
                    int openint;
                    DateTime expiration, timestamp;
                    string symbol, underlying, type;
                    double strike, last, change, bid, ask, volume, contract_size;

                    try { symbol = row.Symbol.TrimStart(new char[] { '.' }); }
                    catch { continue; }

                    try { underlying = row.Stock; }
                    catch { continue; }

                    try { type = row.Type; }
                    catch { continue; }

                    try { strike = row.Strike; }
                    catch { continue; }

                    try { expiration = row.Expiration; }
                    catch { continue; }

                    try { last = row.Last; }
                    catch { last = double.NaN; }

                    try { change = row.Change; }
                    catch { change = double.NaN; }

                    try { bid = row.Bid; }
                    catch { bid = double.NaN; }

                    try { ask = row.Ask; }
                    catch { ask = double.NaN; }

                    try { volume = row.Volume; }
                    catch { volume = double.NaN; }

                    try { openint = row.OpenInt; }
                    catch { openint = 0; }

                    try { contract_size = row.StocksPerContract; }
                    catch { contract_size = 1; }

                    try { timestamp = row.UpdateTimeStamp; }
                    catch { timestamp = DateTime.Now; }

                    // delete old entry in option table
                    DataRow del = data.OptionTable.FindBySymbol(symbol);
                    if (del != null) del.Delete();

                    // add new entry to option table
                    data.OptionTable.AddOptionTableRow(
                        symbol, underlying, type, strike, expiration,
                        last, change, bid, ask, volume, openint, contract_size, timestamp);
                }

                // enable data notifications
                data.QuoteTable.EndLoadData();
                data.OptionTable.EndLoadData();
                data.AcceptChanges();

                return true;
            }
        }

        public class Csv
        {
            public static bool Load(string filename, string rule, MarketSet data)
            {
                ConfigSet.ParsingTableRow rul = Config.Local.ParsingTable.FindByRule(rule);
                if (rul == null) return false;

                // get delimiters
                char[] delimiters = rul.Delimiters.ToCharArray();

                // rows dictionary
                Dictionary<string, ConfigSet.MappingTableRow> dict = new Dictionary<string, ConfigSet.MappingTableRow>();
                ConfigSet.MappingTableRow[] rows = (ConfigSet.MappingTableRow[])Config.Local.MappingTable.Select("Rule = '" + rule + "'");
                foreach (ConfigSet.MappingTableRow row in rows) if (!dict.ContainsKey(row.Field)) dict.Add(row.Field, row);

                bool ret = true;
                
                try
                {
                    FileStream stream = new FileStream(filename, FileMode.Open);
                    StreamReader reader = new StreamReader(stream);

                    // disable data notifications
                    data.QuoteTable.BeginLoadData();
                    data.OptionTable.BeginLoadData();

                    try
                    {
                        for (int i = 0; !reader.EndOfStream && ret; i++)
                        {
                            string s = reader.ReadLine();
                            if (s == null) break;

                            string[] split;
                            if (delimiters == null || delimiters.Length == 0)
                                split = new string[] { s };
                            else
                                split = s.Split(delimiters);

                            try
                            {
                                CultureInfo ci = new CultureInfo("en-US", false);

                                if (rul.IncludesUnderlying)
                                {
                                    ConfigSet.MappingTableRow row;

                                    DateTime timestamp;
                                    string symbol, name, currency;
                                    double last, change, open, low, high, bid, ask, volume, dvrate, hisvol;

                                    try
                                    {
                                        // symbol
                                        row = dict["Underlying.Symbol"];
                                        if (row == null || row.Column == -1) continue;
                                        else if (row.Column == 0) symbol = row.Value;
                                        else symbol = split[row.Column-1];
                                        symbol = symbol.Trim().ToUpper();
                                    }
                                    catch { continue; }

                                    try
                                    {
                                        // name
                                        row = dict["Underlying.Name"];
                                        if (row == null || row.Column == -1) name = symbol;
                                        else if (row.Column == 0) name = row.Value;
                                        else name = split[row.Column-1];
                                        name = name.Trim().ToUpper();
                                    }
                                    catch { name = symbol; }

                                    try
                                    {
                                        // currency
                                        row = dict["Underlying.Currency"];
                                        if (row == null || row.Column == -1) currency = "";
                                        else if (row.Column == 0) currency = row.Value;
                                        else currency = split[row.Column-1];
                                        currency = currency.Trim().ToUpper();
                                    }
                                    catch { currency = ""; }

                                    try
                                    {
                                        // last
                                        row = dict["Underlying.Last"];
                                        if (row == null || row.Column == -1) last = double.NaN;
                                        else
                                        {
                                            if (ci.Name != row.Culture) ci = new CultureInfo(row.Culture, false);
                                            if (row.Column == 0) last = (double)Convert.ToDecimal(row.Value, ci);
                                            else last = (double)Convert.ToDecimal(split[row.Column-1], ci);
                                        }
                                    }
                                    catch { continue; }

                                    try
                                    {
                                        // change
                                        row = dict["Underlying.Change"];
                                        if (row == null || row.Column == -1) change = double.NaN;
                                        else
                                        {
                                            if (ci.Name != row.Culture) ci = new CultureInfo(row.Culture, false);
                                            if (row.Column == 0) change = (double)Convert.ToDecimal(row.Value, ci);
                                            else change = (double)Convert.ToDecimal(split[row.Column-1], ci);
                                        }
                                    }
                                    catch { change = double.NaN; }

                                    try
                                    {
                                        // open
                                        row = dict["Underlying.Open"];
                                        if (row == null || row.Column == -1) open = double.NaN;
                                        else
                                        {
                                            if (ci.Name != row.Culture) ci = new CultureInfo(row.Culture, false);
                                            if (row.Column == 0) open = (double)Convert.ToDecimal(row.Value, ci);
                                            else open = (double)Convert.ToDecimal(split[row.Column-1], ci);
                                        }
                                    }
                                    catch { open = double.NaN; }

                                    try
                                    {
                                        // low
                                        row = dict["Underlying.Low"];
                                        if (row == null || row.Column == -1) low = double.NaN;
                                        else
                                        {
                                            if (ci.Name != row.Culture) ci = new CultureInfo(row.Culture, false);
                                            if (row.Column == 0) low = (double)Convert.ToDecimal(row.Value, ci);
                                            else low = (double)Convert.ToDecimal(split[row.Column-1], ci);
                                        }
                                    }
                                    catch { low = double.NaN; }

                                    try
                                    {
                                        // high
                                        row = dict["Underlying.High"];
                                        if (row == null || row.Column == -1) high = double.NaN;
                                        else
                                        {
                                            if (ci.Name != row.Culture) ci = new CultureInfo(row.Culture, false);
                                            if (row.Column == 0) high = (double)Convert.ToDecimal(row.Value, ci);
                                            else high = (double)Convert.ToDecimal(split[row.Column-1], ci);
                                        }
                                    }
                                    catch { high = double.NaN; }

                                    try
                                    {
                                        // ask
                                        row = dict["Underlying.Ask"];
                                        if (row == null || row.Column == -1) ask = double.NaN;
                                        else
                                        {
                                            if (ci.Name != row.Culture) ci = new CultureInfo(row.Culture, false);
                                            if (row.Column == 0) ask = (double)Convert.ToDecimal(row.Value, ci);
                                            else ask = (double)Convert.ToDecimal(split[row.Column-1], ci);
                                        }
                                    }
                                    catch { ask = double.NaN; }

                                    try
                                    {
                                        // bid
                                        row = dict["Underlying.Bid"];
                                        if (row == null || row.Column == -1) bid = double.NaN;
                                        else
                                        {
                                            if (ci.Name != row.Culture) ci = new CultureInfo(row.Culture, false);
                                            if (row.Column == 0) bid = (double)Convert.ToDecimal(row.Value, ci);
                                            else bid = (double)Convert.ToDecimal(split[row.Column-1], ci);
                                        }
                                    }
                                    catch { bid = double.NaN; }

                                    try
                                    {
                                        // volume
                                        row = dict["Underlying.Volume"];
                                        if (row == null || row.Column == -1) volume = double.NaN;
                                        else
                                        {
                                            if (ci.Name != row.Culture) ci = new CultureInfo(row.Culture, false);
                                            if (row.Column == 0) volume = (double)Convert.ToDecimal(row.Value, ci);
                                            else volume = (double)Convert.ToDecimal(split[row.Column-1], ci);
                                        }
                                    }
                                    catch { volume = double.NaN; }

                                    try
                                    {
                                        // dvrate
                                        row = dict["Underlying.DivRate"];
                                        if (row == null || row.Column == -1) dvrate = double.NaN;
                                        else
                                        {
                                            if (ci.Name != row.Culture) ci = new CultureInfo(row.Culture, false);
                                            if (row.Column == 0) dvrate = (double)Convert.ToDecimal(row.Value, ci);
                                            else dvrate = (double)Convert.ToDecimal(split[row.Column-1], ci);
                                        }
                                    }
                                    catch { dvrate = double.NaN; }

                                    try
                                    {
                                        // hisvol
                                        row = dict["Underlying.HisVolatility"];
                                        if (row == null || row.Column == -1) hisvol = double.NaN;
                                        else
                                        {
                                            if (ci.Name != row.Culture) ci = new CultureInfo(row.Culture, false);
                                            if (row.Column == 0) hisvol = (double)Convert.ToDecimal(row.Value, ci);
                                            else hisvol = (double)Convert.ToDecimal(split[row.Column-1], ci);
                                        }
                                    }
                                    catch { hisvol = double.NaN; }

                                    try
                                    {
                                        // timestamp
                                        row = dict["Underlying.TimeStamp"];
                                        if (row == null || row.Column == -1) timestamp = DateTime.Now;
                                        else
                                        {
                                            if (ci.Name != row.Culture) ci = new CultureInfo(row.Culture, false);
                                            if (row.Column == 0) timestamp = DateTime.Parse(row.Value, ci);
                                            else timestamp = DateTime.Parse(split[row.Column-1], ci);
                                        }
                                    }
                                    catch { timestamp = DateTime.Now; }

                                    // delete old entry in quote table
                                    DataRow del = data.QuoteTable.FindBySymbol(symbol);
                                    if (del != null) del.Delete();

                                    // add new entry to quote table
                                    data.QuoteTable.AddQuoteTableRow(
                                        symbol, name, currency, last, change, open, low, high,
                                        bid, ask, volume, dvrate, timestamp, hisvol);
                                }

                                if (rul.IncludesOptionsChain)
                                {
                                    ConfigSet.MappingTableRow row;

                                    int openint;
                                    DateTime expiration, timestamp;
                                    string symbol, underlying, type;
                                    double strike, last, change, bid, ask, volume, contract_size;

                                    try
                                    {
                                        // underlying
                                        row = dict["Option.Underlying"];
                                        if (row == null || row.Column == -1) continue;
                                        else if (row.Column == 0) underlying = row.Value;
                                        else underlying = split[row.Column - 1];
                                        underlying = underlying.Trim().ToUpper();
                                    }
                                    catch { continue; }

                                    try
                                    {
                                        // strike
                                        row = dict["Option.Strike"];
                                        if (row == null || row.Column == -1) continue;
                                        else
                                        {
                                            if (ci.Name != row.Culture) ci = new CultureInfo(row.Culture, false);
                                            if (row.Column == 0) strike = (double)Convert.ToDecimal(row.Value, ci);
                                            else strike = (double)Convert.ToDecimal(split[row.Column - 1], ci);
                                        }
                                    }
                                    catch { continue; }

                                    try
                                    {
                                        // expiration
                                        row = dict["Option.Expiration"];
                                        if (row == null || row.Column == -1) continue;
                                        else
                                        {
                                            if (ci.Name != row.Culture) ci = new CultureInfo(row.Culture, false);
                                            if (row.Column == 0) expiration = DateTime.Parse(row.Value, ci);
                                            else expiration = DateTime.Parse(split[row.Column - 1], ci);
                                        }
                                    }
                                    catch { continue; }

                                    try
                                    {
                                        // type
                                        row = dict["Option.Type"];
                                        if (row == null || row.Column == -1) continue;
                                        else if (row.Column == 0) type = row.Value;
                                        else type = split[row.Column-1];
                                        type = type.Trim().ToUpper();
                                        if (type[0] == 'C') type = "Call";
                                        else if (type[0] == 'P') type = "Put";
                                        else continue;
                                    }
                                    catch { continue; }

                                    try
                                    {
                                        // symbol
                                        row = dict["Option.Symbol"];
                                        if (row == null || row.Column == -1)
                                        {
                                            row = dict["Option.Symbol.Base"];
                                            if (row == null || row.Column == -1)
                                            {
                                                symbol = ".";
                                            }
                                            else if (row.Column == 0) symbol = row.Value;
                                            else symbol = split[row.Column - 1];

                                            row = dict["Option.Symbol.Suffix"];
                                            if (row == null || row.Column == -1)
                                            {
                                                symbol += underlying + expiration.ToString("ddMMMyyyy") + type[0] + strike.ToString("f2");
                                                symbol = symbol.ToUpper();
                                            }
                                            else if (row.Column == 0) symbol += row.Value;
                                            else symbol += split[row.Column - 1];
                                        }
                                        else if (row.Column == 0) symbol = row.Value;
                                        else symbol = split[row.Column-1];
                                        symbol = symbol.Trim().ToUpper();
                                    }
                                    catch { continue; }

                                    try
                                    {
                                        // last
                                        row = dict["Option.Last"];
                                        if (row == null || row.Column == -1) continue;
                                        else
                                        {
                                            if (ci.Name != row.Culture) ci = new CultureInfo(row.Culture, false);
                                            if (row.Column == 0) last = (double)Convert.ToDecimal(row.Value, ci);
                                            else last = (double)Convert.ToDecimal(split[row.Column-1], ci);
                                        }
                                    }
                                    catch { continue; }

                                    try
                                    {
                                        // change
                                        row = dict["Option.Change"];
                                        if (row == null || row.Column == -1) change = double.NaN;
                                        else
                                        {
                                            if (ci.Name != row.Culture) ci = new CultureInfo(row.Culture, false);
                                            if (row.Column == 0) change = (double)Convert.ToDecimal(row.Value, ci);
                                            else change = (double)Convert.ToDecimal(split[row.Column-1], ci);
                                        }
                                    }
                                    catch { change = double.NaN; }

                                    try
                                    {
                                        // ask
                                        row = dict["Option.Ask"];
                                        if (row == null || row.Column == -1) ask = double.NaN;
                                        else
                                        {
                                            if (ci.Name != row.Culture) ci = new CultureInfo(row.Culture, false);
                                            if (row.Column == 0) ask = (double)Convert.ToDecimal(row.Value, ci);
                                            else ask = (double)Convert.ToDecimal(split[row.Column-1], ci);
                                        }
                                    }
                                    catch { ask = double.NaN; }

                                    try
                                    {
                                        // bid
                                        row = dict["Option.Bid"];
                                        if (row == null || row.Column == -1) bid = double.NaN;
                                        else
                                        {
                                            if (ci.Name != row.Culture) ci = new CultureInfo(row.Culture, false);
                                            if (row.Column == 0) bid = (double)Convert.ToDecimal(row.Value, ci);
                                            else bid = (double)Convert.ToDecimal(split[row.Column-1], ci);
                                        }
                                    }
                                    catch { bid = double.NaN; }

                                    try
                                    {
                                        // volume
                                        row = dict["Option.Volume"];
                                        if (row == null || row.Column == -1) volume = double.NaN;
                                        else
                                        {
                                            if (ci.Name != row.Culture) ci = new CultureInfo(row.Culture, false);
                                            if (row.Column == 0) volume = (double)Convert.ToDecimal(row.Value, ci);
                                            else volume = (double)Convert.ToDecimal(split[row.Column-1], ci);
                                        }
                                    }
                                    catch { volume = double.NaN; }

                                    try
                                    {
                                        // openint
                                        row = dict["Option.OpenInt"];
                                        if (row == null || row.Column == -1) openint = 0;
                                        else
                                        {
                                            if (ci.Name != row.Culture) ci = new CultureInfo(row.Culture, false);
                                            if (row.Column == 0) openint = (int)Convert.ToDecimal(row.Value, ci);
                                            else openint = (int)Convert.ToDecimal(split[row.Column-1], ci);
                                        }
                                    }
                                    catch { openint = 0; }

                                    try
                                    {
                                        // contract_size
                                        row = dict["Option.ContractSize"];
                                        if (row == null || row.Column == -1) contract_size = 1;
                                        else
                                        {
                                            if (ci.Name != row.Culture) ci = new CultureInfo(row.Culture, false);
                                            if (row.Column == 0) contract_size = (double)Convert.ToDecimal(row.Value, ci);
                                            else contract_size = (double)Convert.ToDecimal(split[row.Column - 1], ci);
                                        }
                                    }
                                    catch { contract_size = 1; }

                                    try
                                    {
                                        // timestamp
                                        row = dict["Option.TimeStamp"];
                                        if (row == null || row.Column == -1) timestamp = DateTime.Now;
                                        else
                                        {
                                            if (ci.Name != row.Culture) ci = new CultureInfo(row.Culture, false);
                                            if (row.Column == 0) timestamp = DateTime.Parse(row.Value, ci);
                                            else timestamp = DateTime.Parse(split[row.Column-1], ci);
                                        }
                                    }
                                    catch { timestamp = DateTime.Now; }

                                    // delete old entry in option table
                                    DataRow del = data.OptionTable.FindBySymbol(symbol);
                                    if (del != null) del.Delete();

                                    // add new entry to option table
                                    data.OptionTable.AddOptionTableRow(
                                        symbol, underlying, type, strike, expiration,
                                        last, change, bid, ask, volume, openint, contract_size, timestamp);
                                }
                            }
                            catch { ret = false; }
                        }
                    }
                    catch { ret = false; }

                    // enable data notifications
                    data.QuoteTable.EndLoadData();
                    data.OptionTable.EndLoadData();
                    data.AcceptChanges();

                    reader.Close();
                    stream.Close();
                }
                catch { ret = false; }

                return ret;
            }
        }
    }
}
