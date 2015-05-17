
using System;
using System.Collections;
using System.Collections.Generic;

namespace OptionsOracle.DataCenter.Data 
{
    partial class MarketSet
    {
        partial class QuoteTableDataTable
        {
            public string GetSymbolList()
            {
                string list = "";
                
                foreach (MarketSet.QuoteTableRow row in Rows)
                    if (list == "") list = row.Symbol;
                    else list += "," + row.Symbol;

                return list;
            }
        }
    }
}
