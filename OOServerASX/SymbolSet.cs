using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Resources;
using System.Data;
using System.Collections;
using System.Reflection;

using OOServerLib.Web;

namespace OOServerASX
{
    partial class SymbolSet
    {
        private const string CONFIG_FILE_SYMBOL_TABLE = "plugin_asx_symbol.xml";

        partial class SymbolTableDataTable
        {
            public SymbolTableRow FindByTicker(string ticker)
            {
                ticker = ticker.Trim();
                string ticker_uc = ticker.ToUpper();

                foreach (SymbolTableRow row in Rows)
                {
                    if (row.Symbol == ticker_uc || row.Name == ticker) return row;
                }

                return null;
            }
        }

        private int connection_reties = 2;
        private bool use_proxy = false;
        private string proxy_address = null;

        public int ConnectionsRetries
        {
            get { return connection_reties; }
            set { connection_reties = value; }
        }

        public bool UseProxy
        {
            get { return use_proxy; }
            set { use_proxy = value; }
        }

        public string ProxyAddress
        {
            get { return proxy_address; }
            set { proxy_address = value; }
        }

        public string CurrentVersion
        {
            get { return Assembly.GetExecutingAssembly().GetName().Version.ToString(); }
        }

        public void Load()
        {
            // get config directory path
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\OptionsOracle\";
            string conf = path + CONFIG_FILE_SYMBOL_TABLE;

            // check if config directory exist, if not create it
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            Clear();

            // load symbol list
            try
            {
                if (File.Exists(conf)) ReadXml(conf);
            }
            catch { }

            // if version is old, clear database, and recreate it
            if (GetParameter("Created with Version") != CurrentVersion) Clear();

            if (SymbolTable.Rows.Count == 0)
            {
                try
                {
                    if (New()) WriteXml(conf);
                }
                catch { }
            }
        }

        public void Save()
        {
            // get config directory path
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\OptionsOracle\";
            string conf = path + CONFIG_FILE_SYMBOL_TABLE;

            // check if config directory exist, if not create it
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            try
            {
                this.WriteXml(conf); // save data-set to xml
            }
            catch { }
        }

        private bool New()
        {
            WebCapture cap = new WebCapture();            

            // setup web-capture configuration
            cap.UseProxy = UseProxy;
            cap.ProxyAddress = ProxyAddress;
            cap.ConnectionsRetries = ConnectionsRetries;

            string url = @"http://www.asx.com.au/asx/research/ASXListedCompanies.csv";
            string page = cap.DownloadHtmlWebPage(url);

            foreach (string line in page.Split('\n'))
            {
                string[] items = line.Split(',');
                if (items.Length == 3 && !items[0].Contains("Company"))
                {
                    string name = items[0].Trim().TrimEnd(new char[] { '\"' }).TrimStart(new char[] { '\"' });
                    string symbol = items[1].Trim().TrimEnd(new char[] { '\"' }).TrimStart(new char[] { '\"' });

                    // add row
                    SymbolTable.AddSymbolTableRow(name, symbol);
                }
            }

            // save creation table
            SetParameter("Created with Version", CurrentVersion);

            return true;
        }

        public bool SetParameter(string parameter, string value)
        {
            bool created = false;

            DataRow row = ParametersTable.FindByParameter(parameter);
            if (row == null)
            {
                row = ParametersTable.NewRow();
                row["Parameter"] = parameter;
                ParametersTable.Rows.Add(row);
                created = true;
            }
            row["Value"] = value;
            ParametersTable.AcceptChanges();

            return created;
        }

        public string GetParameter(string parameter)
        {
            DataRow row = ParametersTable.FindByParameter(parameter);
            if (row != null)
            {
                try
                {
                    string value = (string)row["Value"];
                    if (value != null) return value;
                }
                catch { }
            }

            return "";
        }
    }
}
