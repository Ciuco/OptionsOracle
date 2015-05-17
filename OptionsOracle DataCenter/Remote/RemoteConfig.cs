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
using System.Xml;
using System.Collections;

namespace OptionsOracle.DataCenter.Remote
{
    public class RemoteConfig : WebSite
    {
        private const string DEFAULT_BLANK_PAGE = "about:blank";

        private XmlDocument xml;

        public class Command
        {
            public int id = 0;
            public string message = null;
            public string condition = null;
            public string command = null;
        }

        public RemoteConfig()
        {
        }

        public void Load()
        {
            // update proxy settings
            cap.ProxyAddress = Config.Local.ProxyAddress;
            cap.UseProxy = Config.Local.UseProxy;

            // load remote configuration.
            xml = cap.DownloadXmlWebFile(Config.Local.GetRemoteConfigurationUrl);
        }

        private string GetAttrValueByName(XmlNode node, string name)
        {
            if (node == null || node.Attributes == null) return null;

            foreach (XmlAttribute attr in node.Attributes)
            {
                if (attr == null) continue;
                if (attr.Name == name) return attr.Value;
            }

            return null;
        }

        private XmlNode GetNodeByName(XmlNode parent, string name)
        {
            if (parent == null) return null;
            
            foreach(XmlNode node in parent.ChildNodes)
            {
                if (node == null || node.Attributes == null) continue;

                foreach (XmlAttribute attr in node.Attributes) 
                {
                    if (attr == null) continue;
                    if (GetAttrValueByName(node, "name") == name) return node;
                }
            }

            return null;
        }

        private XmlNode GetConfigurationNodeByName(string name)
        {
            if (xml == null || xml.FirstChild == null || xml.FirstChild.NextSibling == null) return null;
            XmlNode body = xml.FirstChild.NextSibling;
            
            XmlNode conf = GetNodeByName(body, "configuration");
            if (conf == null) return null;

            return GetNodeByName(conf, name);
        }

        private string GetParameterFromVersionSection(string name)
        {
            XmlNode vern = GetConfigurationNodeByName(Config.Local.CurrentVersion);
            if (vern == null) return null;

            XmlNode node = GetNodeByName(vern, name);
            if (node == null) return null;

            string value = GetAttrValueByName(node, "value");
            if (value != null) return value;

            return null;
        }

        private string GetParameterFromNamedSection(string section, string name)
        {
            XmlNode vern = GetConfigurationNodeByName(section);
            if (vern == null) return null;

            XmlNode node = GetNodeByName(vern, name);
            if (node == null) return null;

            string value = GetAttrValueByName(node, "value");
            if (value != null) return value;

            return null;
        }

        public string GetRemoteGlobalVariable(string variable_name)
        {
            if (xml == null) return null;

            string value = GetParameterFromNamedSection("global", variable_name);
            if (value != null && value != "") return value;

            return null;
        }

        public string GetRemoteGlobalVersion(string variable_name)
        {
            string value = GetRemoteGlobalVariable(variable_name + "-version");
            return (value != null && value != "") ? value : "0.0.0.0";
        }

        public string GetRemoteGlobalUrl(string variable_name)
        {
            string value = GetRemoteGlobalVariable(variable_name + "-page");
            return (value != null && value != "") ? value : DEFAULT_BLANK_PAGE;
        }

        public string GetLatestRemoteModuleVersion(string remote_module)
        {
            if (xml == null) return null;

            string value = GetParameterFromVersionSection(remote_module + "-version");
            if (value != null && value != "") return value;

            return null;
        }

        public string GetRemoteModuleUrl(string remote_module)
        {
            if (xml == null) return DEFAULT_BLANK_PAGE;

            string value = GetParameterFromVersionSection(remote_module + "-page");
            if (value != null && value != "") return value;

            return DEFAULT_BLANK_PAGE;
        }

        public static int CompareVersions(string ver1, string ver2)
        {
            if (ver1 == ver2 || ver1 == null) return 0;
            if (ver2 == null) return 1;

            string[] ver1_split = ver1.Trim().Split(new char[] { '.' });
            string[] ver2_split = ver2.Trim().Split(new char[] { '.' });

            for (int i = 0; i < 4; i++)
            {
                int x1 = int.Parse(ver1_split[i]);
                int x2 = int.Parse(ver2_split[i]);
                if (x1 > x2) return 1;
                else if (x1 < x2) return -1;
            }

            return 0;
        }

        public bool IsObsolete()
        {
            string value = GetParameterFromVersionSection("obsolete");
            return (value != null && value == "yes");
        }

        public int GetLastCommand()
        {
            if (xml == null) return 0;

            string value = GetParameterFromVersionSection("commands");
            if (value != null && value != "") return int.Parse(value);

            return 0;
        }

        public Command GetCommand(int id)
        {
            XmlNode vern = GetConfigurationNodeByName(Config.Local.CurrentVersion);
            if (vern == null) return null;

            XmlNode node = GetNodeByName(vern, "commands");
            if (node == null) return null;

            XmlNode cmdn = GetNodeByName(node, id.ToString());
            if (cmdn == null) return null;

            Command cmd = new Command();

            cmd.id = id;
            cmd.message = GetAttrValueByName(cmdn, "message");
            cmd.condition = GetAttrValueByName(cmdn, "condition");
            cmd.command = GetAttrValueByName(cmdn, "command");

            return cmd;
        }

        public bool ExecuteCommand(Command cmd)
        {
            string[] split0, split1;

            // check conditions
            if (cmd.condition != null && cmd.condition != "")
            {
                split0 = cmd.condition.Split(new char[] { '!' });
                foreach (string cnd in split0)
                {
                    split1 = cnd.Split(new char[] { ':', '[', ']' });
                    try
                    {
                        if (split1[0] == "eql" && Config.Local.GetParameter(split1[1]) != split1[2]) return false;
                        else if (split1[0] == "neq" && Config.Local.GetParameter(split1[1]) == split1[2]) return false;
                    }
                    catch { return false; }
                }
            }

            // execute command
            if (cmd.command != null && cmd.command != "")
            {
                split0 = cmd.command.Split(new char[] { '!' });
                foreach (string cnd in split0)
                {
                    split1 = cnd.Split(new char[] { ':', '[', ']' });
                    try
                    {
                        if (split1[0] == "set") Config.Local.SetParameter(split1[1], split1[2]);
                        else if (split1[0] == "del") Config.Local.DeleteParameter(split1[1]);
                        else if (split1[0] == "url") Global.OpenExternalBrowser(@"http://" + split1[1]);
                    }
                    catch { return false; }
                }
            }

            return true;
        }
    }
}
