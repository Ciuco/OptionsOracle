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
using System.Windows.Forms;

using OOServerLib.Web;
using OOServerLib.Interface;
using OOServerLib.Global;
using OOServerLib.Config;
using OOServerLib.Forms;

using MSXML2;

namespace OOServerTDAmeritrade
{
    public class Tda
    {
        // protocol consts
        private const string _result_tag = "RESULT";
        private const string _sessionid_tag = "SESSION-ID";
        private const string _userid_tag = "USER-ID";
        private const string _cdi_tag = "CDI";
        private const string _timeout_tag = "TIMEOUT";
        private const string _associated_acct_tag = "ASSOCIATED-ACCOUNT-ID";
        private const string _accountid_tag = "ACCOUNT-ID";
        private const string _description_tag = "DESCRIPTION";
        private const string _company_tag = "COMPANY";
        private const string _segment_tag = "SEGMENT";
        private const string _margintrading_tag = "MARGIN-TRADING";

        // global variables
        private string SourceId = "OO";
        private string Version = "1";
        public  string Username = string.Empty;
        private string Password = string.Empty;

        private bool LoginStatus = false;

        // loging variables
        private string _result = string.Empty;
        private string _sessionid = string.Empty;
        private string _userid = string.Empty;
        private string _cdi = string.Empty;
        private string _timeout = string.Empty;
        private string _associated_acct = string.Empty;
        private string _accountid = string.Empty;
        private string _description = string.Empty;
        private string _company = string.Empty;
        private string _segment = string.Empty;
        private string _margintrading = string.Empty;

        // global structures
        public class TDQuote
        {
            public string symbol;
            public string description;
            public string bid;
            public string ask;
            public string bid_ask_size;
            public string last;
            public string last_trade_size;
            public string last_trade_date;
            public string open;
            public string high;
            public string low;
            public string close;
            public string volume;
            public string year_high;
            public string year_low;
            public string real_time;
            public string exchange;
            public string asset_type;
            public string change;
            public string change_percent;
        }

        public class TDOptionQuote
        {
            public string date;
            public string expiration_type;
            public string days_to_expiration;
            public string strike_price;
            public string standard_option;
            public string type;
            public string option_symbol;
            public string bid;
            public string ask;
            public string bid_ask_size;
            public string last;
            public string last_trade_date;
            public string volume;
            public string open_intereset;
            public string real_time;
            public string underlying_symbol;
            public string delta;
            public string gamma;
            public string theta;
            public string vega;
            public string rho;
            public string implied_volatility;
            public string time_value_index;
            public string multiplier;
            public string change;
            public string change_percent;
            public string in_the_money;
            public string near_the_money;
            public string theoretical_value;
            public string cash_in_lieu_dollar_amount;
            public string cash_dollar_amount;
            public string index_option;
            public string symbol;
            public string shares;
        }

        public class TDPriceHistory
        {
            public double close;
            public double high;
            public double low;
            public double open;
            public double volume;
            public DateTime timestamp;
        }

        public enum SecurityType
        {
            Undefined,
            Stock,
            Index,
            Future,
            Option,
            FutureOption
        };

        public bool Connect
        {
            get
            {
                return LoginStatus;
            }

            set
            {
                if (value)
                {
                    LoginForm loginForm = new LoginForm();

                    loginForm.Text = "TD Ameritrade Login";
                    loginForm.Username = Username;
                    loginForm.Password = Password;
                    loginForm.StartPosition = FormStartPosition.CenterParent;

                    if (loginForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        Username = loginForm.Username;
                        Password = loginForm.Password;

                        try
                        {
                            Login();
                        }
                        catch { LoginStatus = false; }

                        if (!LoginStatus)
                        {
                            MessageBox.Show("Failed to Connect TD-Ameritrade, Please Check Username and Password.    ", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                            Password = string.Empty;
                        }
                    }
                }
                else
                {
                    try
                    {
                        Logout();
                    }
                    catch { }
                }
            }
        }

        // misc functions
        private string Encode_URL(string cUrlString)
        {
            if (cUrlString == null) return "";

            StringBuilder encodedString = new StringBuilder();
            char[] encBytes = cUrlString.ToCharArray();

            foreach (char cb in encBytes)
            {
                switch ((byte)cb)
                {
                    case 58: encodedString.Append("%3A"); break;
                    case 32: encodedString.Append("%20"); break;
                    case 40: encodedString.Append("%28"); break;
                    case 41: encodedString.Append("%29"); break;
                    case 43: encodedString.Append("%2B"); break;
                    case 45: encodedString.Append("%2D"); break;
                    case 61: encodedString.Append("%3D"); break;
                    case 124: encodedString.Append("%7C"); break;
                    case 38: encodedString.Append("%26"); break;
                    case 44: encodedString.Append("%2C"); break;
                    case 126: encodedString.Append("%7E"); break;
                    default: encodedString.Append(cb); break;
                }
            }

            return encodedString.ToString();
        }


        private bool Login()
        {
            bool retValue = false;
            XMLHTTP xmlHttp_ = new XMLHTTP();
            StringBuilder cpostdata = new StringBuilder();
            string lcPostUrl = string.Empty;

            cpostdata.Append("userid=" + Encode_URL(Username));
            cpostdata.Append("&password=" + Encode_URL(Password));
            cpostdata.Append("&source=" + Encode_URL(SourceId));
            cpostdata.Append("&version=" + Encode_URL(Version));

            lcPostUrl = "https://apis.tdameritrade.com/apps/100/LogIn?source=" + Encode_URL(SourceId) + "&version=" + Encode_URL(Version);

            xmlHttp_.open("POST", lcPostUrl, false, Username, Password);
            xmlHttp_.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
            xmlHttp_.setRequestHeader("Accept", "Accept	image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/vnd.ms-powerpoint, application/vnd.ms-excel, application/msword, application/x-shockwave-flash, */*");

            xmlHttp_.send(cpostdata.ToString());


            string xmlData = xmlHttp_.responseText.ToString();
            string cResponseHeaders = xmlHttp_.getAllResponseHeaders();

            /*/
             * Assign Login values from the response string
             * 
            /*/

            StringReader reader = new StringReader(xmlData);
            XmlTextReader xml = new XmlTextReader(reader);

            // Check for errors.
            if (null == xmlData || "" == xmlData)
            {
                // Throw an exception.
                throw new Exception("Failed to connect, check username and password?");
            }

            // Read the Xml.
            while (xml.Read())
            {
                // Got an element?
                if (xml.NodeType == XmlNodeType.Element)
                {
                    // Get this node.
                    string name = xml.Name;

                    // Get Result/Status
                    if (name.ToLower().ToString().CompareTo(_result_tag.ToLower()) == 0)
                    {
                        xml.Read();
                        this._result = xml.Value;
                    }

                    // Get Session ID
                    if (name.ToLower().ToString().CompareTo(_sessionid_tag.ToLower()) == 0)
                    {
                        xml.Read();
                        this._sessionid = xml.Value;
                    }

                    // Get USER ID
                    if (name.ToLower().ToString().CompareTo(_userid_tag.ToLower()) == 0)
                    {
                        xml.Read();
                        this._userid = xml.Value;
                    }

                    // Get CDI
                    if (name.ToLower().ToString().CompareTo(_cdi_tag.ToLower()) == 0)
                    {
                        xml.Read();
                        this._cdi = xml.Value;
                    }

                    // Get TimeOut
                    if (name.ToLower().ToString().CompareTo(_timeout_tag.ToLower()) == 0)
                    {
                        xml.Read();
                        this._timeout = xml.Value;
                    }

                    // Get Associated User Account
                    if (name.ToLower().ToString().CompareTo(_associated_acct_tag.ToLower()) == 0)
                    {
                        xml.Read();
                        this._associated_acct = xml.Value;
                    }

                    // Get Account ID
                    if (name.ToLower().ToString().CompareTo(_accountid_tag.ToLower()) == 0)
                    {
                        xml.Read();
                        this._accountid = xml.Value;
                    }

                    // Get Description
                    if (name.ToLower().ToString().CompareTo(_description_tag.ToLower()) == 0)
                    {
                        xml.Read();
                        this._description = xml.Value;
                    }

                    // Get Company
                    if (name.ToLower().ToString().CompareTo(_company_tag.ToLower()) == 0)
                    {
                        xml.Read();
                        this._company = xml.Value;
                    }

                    // Get Segment
                    if (name.ToLower().ToString().CompareTo(_segment_tag.ToLower()) == 0)
                    {
                        xml.Read();
                        this._segment = xml.Value;
                    }

                    // Get Margined Account
                    if (name.ToLower().ToString().CompareTo(_margintrading_tag.ToLower()) == 0)
                    {
                        xml.Read();
                        this._margintrading = xml.Value;
                    }
                }

            }

            retValue = (_result.ToUpper().CompareTo("OK") >= 0 ? true : false);

            LoginStatus = retValue;
            return retValue;
        }

        private bool Logout()
        {
            bool retValue = false;

            if (LoginStatus)
            {

                XMLHTTP xmlHttp_ = new XMLHTTP();
                StringBuilder cpostdata = new StringBuilder();
                string lcPostUrl = string.Empty;

                cpostdata.Append("source=" + Encode_URL(SourceId));


                lcPostUrl = "https://apis.tdameritrade.com/apps/100/LogOut?" + cpostdata.ToString();

                xmlHttp_.open("POST", lcPostUrl, false, Username, Password);
                xmlHttp_.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
                xmlHttp_.setRequestHeader("Accept", "Accept	image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/vnd.ms-powerpoint, application/vnd.ms-excel, application/msword, application/x-shockwave-flash, */*");
                xmlHttp_.send(null);

                retValue = true;
            }
            else
                retValue = false;

            // reset status
            this._result = string.Empty;
            this._sessionid = string.Empty;
            this._userid = string.Empty;
            this._cdi = string.Empty;
            this._timeout = string.Empty;
            this._associated_acct = string.Empty;
            this._accountid = string.Empty;
            this._description = string.Empty;
            this._company = string.Empty;
            this._segment = string.Empty;
            this._margintrading = string.Empty;

            LoginStatus = false;
            return retValue;
        }

        private bool KeepAlive()
        {
            bool retValue = false;

            if (LoginStatus)
            {

                XMLHTTP xmlHttp_ = new XMLHTTP();
                StringBuilder cpostdata = new StringBuilder();
                string lcPostUrl = string.Empty;

                cpostdata.Append("source=" + Encode_URL(SourceId));

                lcPostUrl = "https://apis.tdameritrade.com/apps/KeepAlive?" + cpostdata.ToString();

                xmlHttp_.open("POST", lcPostUrl, false, Username, Password);
                xmlHttp_.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
                xmlHttp_.setRequestHeader("Accept", "Accept	image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/vnd.ms-powerpoint, application/vnd.ms-excel, application/msword, application/x-shockwave-flash, */*");
                xmlHttp_.send(null);

                retValue = true;

            }
            else
                retValue = false;

            return retValue;
        }

        // get xml node text
        private string GetXmlNodeText(XmlNode node, string SelectString)
        {
            XmlNode Found_node = node.SelectSingleNode(SelectString);
            if (Found_node != null) return Found_node.InnerText;
            else return "";
        }

        // send an Ameritrde generic request 
        private XmlDocument SendAmeritradeRequest(string Url)
        {
            XmlDocument XmlDoc = null;
            string lcPostUrl = string.Empty;

            if (LoginStatus)
            {

                XMLHTTP xmlHttp_ = new XMLHTTP();
                StringBuilder cpostdata = new StringBuilder();

                lcPostUrl = Url;

                xmlHttp_.open("POST", lcPostUrl, false, Username, Password);
                xmlHttp_.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
                xmlHttp_.setRequestHeader("Accept", "Accept	image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/vnd.ms-powerpoint, application/vnd.ms-excel, application/msword, application/x-shockwave-flash, */*");
                xmlHttp_.send(null);

                string xmlData = xmlHttp_.responseText.ToString();

                // Check for errors.
                if (null == xmlData || "" == xmlData)
                {
                    // Throw an exception.
                    throw new Exception("Failed to connect, check username and password?");
                }

                string cResponseHeaders = xmlHttp_.getAllResponseHeaders();

                StringReader reader = new StringReader(xmlData);
                XmlTextReader xml = new XmlTextReader(reader);

                XmlDoc = new XmlDocument();
                XmlDoc.LoadXml(xmlData);
            }
            return XmlDoc;
        }

        // send an Ameritrde binary request 
        private byte[] SendAmeritradeBinaryRequest(string Url)
        {
            byte[] xmlData = null;
            string lcPostUrl = string.Empty;

            if (LoginStatus)
            {

                XMLHTTP xmlHttp_ = new XMLHTTP();
                StringBuilder cpostdata = new StringBuilder();

                lcPostUrl = Url;

                xmlHttp_.open("POST", lcPostUrl, false, Username, Password);
                xmlHttp_.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
                xmlHttp_.setRequestHeader("Accept", "Accept	image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/vnd.ms-powerpoint, application/vnd.ms-excel, application/msword, application/x-shockwave-flash, */*");
                xmlHttp_.send(null);

                xmlData = (byte[])xmlHttp_.responseBody;

                // Check for errors.
                if (null == xmlData || xmlData.Length == 0)
                {
                    // Throw an exception.
                    throw new Exception("Failed to connect, check username and password?");
                }

                string cResponseHeaders = xmlHttp_.getAllResponseHeaders();
            }

            return xmlData;
        }

        public TDQuote GetQuote(string symbol)
        {
            if (!Connect)
            {
                Connect = true;
                if (!Connect) return null;
            }

            try
            {
                StringBuilder cpostdata = new StringBuilder();
                cpostdata.Append("source=" + Encode_URL(SourceId));
                cpostdata.Append("&symbol=" + Encode_URL(symbol));

                string lcPostUrl = "https://apis.tdameritrade.com/apps/100/Quote?" + cpostdata.ToString();
                
                XmlDocument doc = SendAmeritradeRequest(lcPostUrl);
                if (doc == null) return null;

                XmlNodeList option_date_list = doc.SelectNodes(@"//quote");
                if (option_date_list == null || option_date_list.Count == 0) return null;

                XmlNode quote_nd = option_date_list[0];

                TDQuote quote = new TDQuote();

                quote.symbol = GetXmlNodeText(quote_nd, "symbol");
                quote.description = GetXmlNodeText(quote_nd, "description");
                quote.bid = GetXmlNodeText(quote_nd, "bid");
                quote.ask = GetXmlNodeText(quote_nd, "ask");
                quote.bid_ask_size = GetXmlNodeText(quote_nd, "bid-ask-size");
                quote.last = GetXmlNodeText(quote_nd, "last");
                quote.last_trade_size = GetXmlNodeText(quote_nd, "last-trade-size");
                quote.last_trade_date = GetXmlNodeText(quote_nd, "last-trade-date");
                quote.open = GetXmlNodeText(quote_nd, "open");
                quote.high = GetXmlNodeText(quote_nd, "high");
                quote.low = GetXmlNodeText(quote_nd, "low");
                quote.close = GetXmlNodeText(quote_nd, "close");
                quote.volume = GetXmlNodeText(quote_nd, "volume");
                quote.year_high = GetXmlNodeText(quote_nd, "year-high");
                quote.year_low = GetXmlNodeText(quote_nd, "year-low");
                quote.real_time = GetXmlNodeText(quote_nd, "real-time");
                quote.exchange = GetXmlNodeText(quote_nd, "exchange");
                quote.asset_type = GetXmlNodeText(quote_nd, "asset-type");
                quote.change = GetXmlNodeText(quote_nd, "change");
                quote.change_percent = GetXmlNodeText(quote_nd, "change-percent");

                return quote;
            }
            catch { }

            return null;
        }

        // returns All Ameritrade option information
        public List<TDOptionQuote> GetOptionChain(string symbol, string type)
        {
            if (!Connect)
            {
                Connect = true;
                if (!Connect) return null;
            }

            try
            {
                StringBuilder cpostdata = new StringBuilder();
                cpostdata.Append("source=" + Encode_URL(SourceId));
                cpostdata.Append("&symbol=" + Encode_URL(symbol));
                cpostdata.Append("&type=" + Encode_URL(type));
                cpostdata.Append("&quotes=" + Encode_URL("true"));

                string lcPostUrl = "https://apis.tdameritrade.com/apps/100/OptionChain?" + cpostdata.ToString();

                XmlDocument doc = SendAmeritradeRequest(lcPostUrl);
                if (doc == null) return null;

                List<TDOptionQuote> option_list = new List<TDOptionQuote>();

                XmlNodeList option_date_list = doc.SelectNodes(@"//option-date");
                foreach (XmlNode date_nd in option_date_list)
                {
                    string _date = GetXmlNodeText(date_nd, "date");
                    string _expiration_type = GetXmlNodeText(date_nd, "expiration-type");
                    string _days_to_expiration = GetXmlNodeText(date_nd, "days-to-expiration");

                    XmlNodeList option_strike_list = date_nd.SelectNodes(@"option-strike");
                    foreach (XmlNode strike_nd in option_strike_list)
                    {
                        string _strike_price = GetXmlNodeText(strike_nd, "strike-price");
                        string _standard_option = GetXmlNodeText(strike_nd, "standard-option");

                        XmlNode call_nd = strike_nd.SelectSingleNode("call");
                        XmlNode put_nd = strike_nd.SelectSingleNode("put");

                        List<XmlNode> option_node_list = new List<XmlNode>();
                        if (call_nd != null) option_node_list.Add(call_nd);
                        if (put_nd != null) option_node_list.Add(put_nd);

                        foreach (XmlNode option_nd in option_node_list)
                        {
                            TDOptionQuote option = new TDOptionQuote();

                            if (option_nd.Name == "call") option.type = "Call";
                            else if (option_nd.Name == "put") option.type = "Put";
                            else continue;

                            option.date = _date;
                            option.expiration_type = _expiration_type;
                            option.days_to_expiration = _days_to_expiration;

                            option.strike_price = _strike_price;
                            option.standard_option = _standard_option;
                            option.option_symbol = GetXmlNodeText(option_nd, "option-symbol");
                            option.bid = GetXmlNodeText(option_nd, "bid");
                            option.ask = GetXmlNodeText(option_nd, "ask");
                            option.bid_ask_size = GetXmlNodeText(option_nd, "bid-ask-size");
                            option.last = GetXmlNodeText(option_nd, "last");
                            option.last_trade_date = GetXmlNodeText(option_nd, "last-trade-date");
                            option.volume = GetXmlNodeText(option_nd, "volume");
                            option.open_intereset = GetXmlNodeText(option_nd, "open-interest");
                            option.real_time = GetXmlNodeText(option_nd, "real-time");
                            option.underlying_symbol = GetXmlNodeText(option_nd, "underlying-symbol");
                            option.delta = GetXmlNodeText(option_nd, "delta");
                            option.gamma = GetXmlNodeText(option_nd, "gamma");
                            option.theta = GetXmlNodeText(option_nd, "theta");
                            option.vega = GetXmlNodeText(option_nd, "vega");
                            option.rho = GetXmlNodeText(option_nd, "rho");
                            option.implied_volatility = GetXmlNodeText(option_nd, "implied-volatility");
                            option.time_value_index = GetXmlNodeText(option_nd, "time-value-index");
                            option.multiplier = GetXmlNodeText(option_nd, "multiplier");
                            option.change = GetXmlNodeText(option_nd, "change");
                            option.change_percent = GetXmlNodeText(option_nd, "change-percent");
                            option.in_the_money = GetXmlNodeText(option_nd, "in-the-money");
                            option.near_the_money = GetXmlNodeText(option_nd, "near-the-money");
                            option.theoretical_value = GetXmlNodeText(option_nd, "theoretical-value");

                            XmlNode deliv_nd = option_nd.SelectSingleNode("deliverable-list");
                            if (deliv_nd != null)
                            {
                                option.cash_in_lieu_dollar_amount = GetXmlNodeText(deliv_nd, "cash-in-lieu-dollar-amount");
                                option.cash_dollar_amount = GetXmlNodeText(deliv_nd, "cash-dollar-amount");
                                option.index_option = GetXmlNodeText(deliv_nd, "index-option");

                                XmlNode row_nd = deliv_nd.SelectSingleNode("row");
                                if (row_nd != null)
                                {
                                    option.symbol = GetXmlNodeText(row_nd, "symbol");
                                    option.shares = GetXmlNodeText(row_nd, "shares");
                                }
                            }

                            option_list.Add(option);
                        }
                    }
                }

                return option_list;
            }
            catch { }

            return null;
        }

        private object DecodeBinaryField(byte[] buf, Type type, ref int n)
        {
            int i;

            if (type == typeof(float))
            {
                try
                {
                    byte[] temp = new byte[4];
                    for (i = 0; i < 4; i++) temp[3 - i] = buf[n++];
                    return BitConverter.ToSingle(temp, 0);
                }
                catch { return float.NaN; }
            }
            if (type == typeof(byte))
            {
                try
                {
                    return (byte)buf[n++];
                }
                catch { return (char)255; }
            }
            if (type == typeof(int))
            {
                try
                {
                    byte[] temp = new byte[4];
                    for (i = 0; i < 4; i++) temp[3 - i] = buf[n++];
                    return BitConverter.ToInt32(temp, 0);
                }
                catch { return (int)-1; }
            }
            if (type == typeof(long))
            {
                try
                {
                    byte[] temp = new byte[8];
                    for (i = 0; i < 8; i++) temp[7 - i] = buf[n++];
                    return BitConverter.ToInt64(temp, 0);
                }
                catch { return (long)-1; }
            }
            else if (type == typeof(string))
            {
                try
                {
                    byte[] temp = new byte[2];
                    for (i = 0; i < 2; i++) temp[1 - i] = buf[n++];
                    int length = BitConverter.ToInt16(temp, 0);

                    string text = Encoding.ASCII.GetString(buf, n, length);
                    n += length;

                    return text;
                    
                }
                catch { return null; }
            }

            return null;
        }

        // returns price history information
        public List<TDPriceHistory> GetHistoricalData(string symbol, DateTime start, DateTime end)
        {
            if (!Connect)
            {
                Connect = true;
                if (!Connect) return null;
            }

            try
            {
                // check start/end dates
                if (end >= DateTime.Now.Date) end = DateTime.Now.Date.AddDays(-1);
                if (start >= end) start = end.AddDays(-1);

                StringBuilder cpostdata = new StringBuilder();
                cpostdata.Append("source=" + Encode_URL(SourceId));
                cpostdata.Append("&requestidentifiertype=" + Encode_URL("SYMBOL"));
                cpostdata.Append("&requestvalue=" + Encode_URL(symbol));
                cpostdata.Append("&intervaltype=" + Encode_URL("DAILY"));
                cpostdata.Append("&intervalduration=" + Encode_URL("1"));
                cpostdata.Append("&startdate=" + Encode_URL(start.ToString("yyyyMMdd")));
                cpostdata.Append("&enddate=" + Encode_URL(end.ToString("yyyyMMdd")));

                string lcPostUrl = "https://apis.tdameritrade.com/apps/100/PriceHistory?" + cpostdata.ToString();

                byte[] data = SendAmeritradeBinaryRequest(lcPostUrl);
                if (data == null) return null;

                List<TDPriceHistory> history_list = new List<TDPriceHistory>();

                try
                {
                    int n = 0;

                    // Symbol Count        Integer     4           Number of symbols for which data is being returned.
                    //                                             The subsequent sections are repeated this many times

                    int tda_symbol_count = (int)DecodeBinaryField(data, typeof(int), ref n);

                    // Symbol Length       Short       2           Length of the Symbol field
                    // Symbol              String      Variable    The symbol for which the historical data is returned 
                    // Error Code          Byte        1           0=OK, 1=ERROR
                    // Error Length        Short       2           Only returned if Error Code=1. Length of the Error string
                    // Error Text          String      Variable    Only returned if Error Code=1. The string describing the error
                    // Bar Count           Integer     4           # of chart bars; only if error code=0

                    string tda_symbol = (string)DecodeBinaryField(data, typeof(string), ref n);
                    byte tda_error_code = (byte)DecodeBinaryField(data, typeof(byte), ref n);

                    // we got an error, abort.
                    if (tda_error_code != 0) return null;

                    int tda_bar_count = (int)DecodeBinaryField(data, typeof(int), ref n);

                    // close               Float       4
                    // high                Float       4
                    // low                 Float       4
                    // open                Float       4
                    // volume              Float       4           in 100's 
                    // timestamp           Long        8           time in milliseconds from 00:00:00 UTC on January 1, 1970

                    while (data.Length - n >= 28)
                    {
                        TDPriceHistory tda_history = new TDPriceHistory();

                        tda_history.close = (float)DecodeBinaryField(data, typeof(float), ref n);
                        tda_history.high = (float)DecodeBinaryField(data, typeof(float), ref n);
                        tda_history.low = (float)DecodeBinaryField(data, typeof(float), ref n);
                        tda_history.open = (float)DecodeBinaryField(data, typeof(float), ref n);
                        tda_history.volume = (float)DecodeBinaryField(data, typeof(float), ref n) * 100;
                        tda_history.timestamp = new DateTime(1970, 1, 1).AddMilliseconds((long)DecodeBinaryField(data, typeof(long), ref n));

                        history_list.Add(tda_history);
                    }

                }
                catch { }

                return history_list;
            }
            catch { }

            return null;
        }
    }
}
