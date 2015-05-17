using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Resources;
using System.Data;
using System.Collections;
using System.Reflection;

namespace OOServerInteractiveBrokers 
{
    partial class MappingSet
    {
        private const string CONFIG_FILE_MAPPING_TABLE = "plugin_interactivebrokers_mapping.xml";

        public void Load()
        {
            try
            {
                Stream stream;
                Assembly assembly = Assembly.GetExecutingAssembly();

                stream = assembly.GetManifestResourceStream("OOServerInteractiveBrokers.Resources." + CONFIG_FILE_MAPPING_TABLE);

                if (stream != null)
                {
                    ReadXml(stream);
                    stream.Close();
                }
            }
            catch { }
        }
    }
}
