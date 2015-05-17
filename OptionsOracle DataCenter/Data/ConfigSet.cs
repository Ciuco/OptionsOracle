using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Resources;
using System.Data;
using System.Collections;
using System.Reflection;

namespace OptionsOracle.DataCenter.Data
{
    partial class ConfigSet
    {
        private const string CONFIG_FILE = @"config.xml";
        private const string REMOTE_CONFIG = @"http://www.samoasky.com/datacenter_config.xml";

        public const string PRM_LAST_SELECTED_RULE = "Last Selected Rule";
        public const string PRM_CREATED_VERSION = "Created with Version";
        public const string PRM_REMOTE_CONFIG = "Remote Configuration";
        
        public const string PRM_USE_PROXY = "Use Proxy";
        public const string PRM_PROXY_ADDRESS = "Proxy Address";

        public const string PRM_DEFAULT_IMPORT_FILTER_INDEX = "Default Import Filter Index";
        public const string PRM_DEFAULT_IMPORT_FILENAME = "Default Import FileName";

        private string[] FieldList = new string[] 
        {
            // underlying
            "Underlying.Symbol",
            "Underlying.Name",
            "Underlying.Last",
            "Underlying.Change",
            "Underlying.Open",
            "Underlying.Low",
            "Underlying.High",
            "Underlying.Bid",
            "Underlying.Ask",
            "Underlying.Volume",
            "Underlying.DivRate",
            "Underlying.HisVolatility",
            "Underlying.TimeStamp",

            // option
            "Option.Symbol",
            "Option.Symbol.Base",
            "Option.Symbol.Suffix",
            "Option.Underlying",
            "Option.Type",
            "Option.Strike",
            "Option.Expiration",
            "Option.Last",
            "Option.Change",
            "Option.Bid",
            "Option.Ask",
            "Option.Volume",
            "Option.OpenInt",
            "Option.ContractSize",
            "Option.TimeStamp"
        };        

        public string CurrentVersion
        {
            get { return Assembly.GetExecutingAssembly().GetName().Version.ToString(); }
        }

        public bool FirstRun = false;

        public List<string> DelimitingRuleList
        {
            get
            {
                List<string> list = new List<string>();
                foreach (ParsingTableRow row in ParsingTable.Rows) list.Add(row.Rule);
                return list;
            }
        }

        public bool UseProxy
        {
            get { return bool.Parse(GetParameter(PRM_USE_PROXY)); }
            set { SetParameter(PRM_USE_PROXY, value.ToString()); }
        }

        public string ProxyAddress
        {
            get { return GetParameter(PRM_PROXY_ADDRESS); }
            set { SetParameter(PRM_PROXY_ADDRESS, value);}
        }

        public string GetRemoteConfigurationUrl
        {
            get { return GetParameter(PRM_REMOTE_CONFIG); }
        }

        public void CreateDelimitingRule(string rule, string delimiters)
        {
            // add rule to parsing table
            ParsingTable.AddParsingTableRow(rule, true, true, delimiters);

            // add rule to mapping table
            foreach (string field in FieldList) MappingTable.AddMappingTableRow(rule, field, -1, null, "en-US");
        }

        public void DeleteDelimitingRule(string rule)
        {
            DataRow row1 = ParsingTable.FindByRule(rule);
            if (row1 != null) row1.Delete();
            ParsingTable.AcceptChanges();

            DataRow[] rows = MappingTable.Select("Rule = '" + rule + "'");
            foreach (DataRow row2 in rows) row2.Delete();
            MappingTable.AcceptChanges();
        }

        public void Load()
        {
            // get config directory path
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\OptionsOracle DataCenter\";
            string conf = path + CONFIG_FILE;

            // check if config directory exist, if not create it
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            Clear(); // clear data-set

            // load / create-new configuration
            try
            {
                // load configuration file (if existed)
                if (File.Exists(conf)) ReadXml(conf);
                else FirstRun = true;
            }
            catch { }

            try
            {
                // create missing entries in configuration file, and save it if changed.
                if (FirstRun && New()) WriteXml(conf);
            }
            catch { }
        }

        public void Save()
        {
            // get config directory path
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\OptionsOracle DataCenter\";
            string conf = path + CONFIG_FILE;

            // check if config directory exist, if not create it
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            try
            {
                this.WriteXml(conf); // save data-set to xml
            }
            catch { }
        }

        public static void Delete()
        {
            // get config directory path
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\OptionsOracle DataCenter\";
            string conf = path + CONFIG_FILE;

            try
            {
                File.Delete(conf);
            }
            catch { }
        }

        private bool New()
        {
            Clear();

            ParametersTable.AddParametersTableRow(PRM_CREATED_VERSION, CurrentVersion);
            ParametersTable.AddParametersTableRow(PRM_LAST_SELECTED_RULE, "");
            ParametersTable.AddParametersTableRow(PRM_REMOTE_CONFIG, REMOTE_CONFIG);

            ParametersTable.AddParametersTableRow(PRM_USE_PROXY, bool.FalseString);
            ParametersTable.AddParametersTableRow(PRM_PROXY_ADDRESS, "");
            
            ParametersTable.AddParametersTableRow(PRM_DEFAULT_IMPORT_FILTER_INDEX, "1");
            ParametersTable.AddParametersTableRow(PRM_DEFAULT_IMPORT_FILENAME, "");

            return true;
        }

        public bool Update()
        {
            // get parser xml
            XmlDocument xml = Config.Parser.Xml;

            try
            {
                using (StringReader reader = new StringReader(xml.OuterXml))
                {
                    ConfigSet tempSet = new ConfigSet();
                    tempSet.ReadXml(reader);

                    // remove duplicate entries from current config
                    foreach (ParsingTableRow row in tempSet.ParsingTable.Rows)
                        DeleteDelimitingRule(row.Rule);
                }

                using (StringReader reader = new StringReader(xml.OuterXml))
                {
                    ReadXml(reader);
                }
            }
            catch { }

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

        public bool RenameParameter(string org_parameter, string new_parameter)
        {
            DataRow row = ParametersTable.FindByParameter(org_parameter);
            if (row != null)
            {
                try
                {
                    row["Parameter"] = new_parameter;
                    row.AcceptChanges();
                    return true;
                }
                catch { }
            }

            return false;
        }

        public bool DeleteParameter(string parameter)
        {
            DataRow row = ParametersTable.FindByParameter(parameter);
            if (row != null)
            {
                try
                {
                    row.Delete();
                    row.AcceptChanges();
                    return true;
                }
                catch { }
            }

            return false;
        }
    }
}
