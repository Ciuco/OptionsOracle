using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Resources;
using System.Data;
using System.Collections;
using System.Reflection;

namespace OOServerNordicEx 
{
    partial class SymbolSet
    {
        private const string CONFIG_FILE_SYMBOL_TABLE = "plugin_nordicex_symbol.xml";

        partial class SymbolTableDataTable
        {
            public SymbolTableRow FindByTicker(string ticker)
            {
                ticker = ticker.Trim();
                string ticker_uc = ticker.ToUpper();

                foreach (SymbolTableRow row in Rows)
                {
                    if (row.Id == ticker_uc || row.Isin == ticker_uc || "#" + row.Isin == ticker_uc ||
                        row.Symbol == ticker_uc || row.Name == ticker) return row;
                }

                return null;
            }
        }
    
        public void Load()
        {
            try
            {
                Stream stream;
                Assembly assembly = Assembly.GetExecutingAssembly();

                stream = assembly.GetManifestResourceStream("OOServerNordicEx.Resources." + CONFIG_FILE_SYMBOL_TABLE);

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
