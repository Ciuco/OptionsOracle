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
using System.Reflection;
using OOServerLib.Web;
using OOServerLib.Interface;
using OOServerLib.Global;
using OOServerLib.Config;

namespace OOServerUS
{
    public class MarketWatch
    {
        // web capture client
        private WebCapture cap = null;

        // parse html page
        private XmlParser prs = new XmlParser();

        // culture info
        private CultureInfo ci = new CultureInfo("en-US", false);

        private const string IndexSymbol = " BXO RUH DJX DXL OEX OEX XEO XEO VIX BVZ SPX XSP BSZ SPL SML VXN NDX MNX MML RUI RVX RUT RMN NFT RUH OEX OEX XEO XEO SPX XSP BSZ SPL SML VIX BVZ DJX DTX DUX VXN NDX MNX MML RUI RVX RUT RMN EXQ GOX INX OIX TXX MVR MVB MGO MSV NFT CJN CTH CYX NFT ";
        private const string FundSymbol = " AINV AGQ AUS BGU BGZ BWX DAX DBA DBB DBC DBE DBO DBP DBS DBV DDM DGL DGT DIA DIG DND DOG DUG DVY DXD EDC EDZ EEB EEM EEV EFA EFU EPI EPP ERY ERX EUO EWA EWC EWD EWG EWH EWJ EWL EWM EWP EWT EWW EWY EWZ EZA EZU FAS FAZ FEZ FPX FPX FRC FXB FXC FXE FXP FXY GDX GEX GLD GLL GSG GWX GXC HKG HYG IAI IAT IAU IBB IDU IEF IEZ IGE IGM IGN IGV IGW IIF IJR ILF ITB IVV IYY IWB IWC IWD IWF IWM IWN IWO IWP IWR IWS IWV IWW IWZ IYE IYF IYH IYM IYR IYY IYZ KBE KCE KIE KOL KRE KWT LDN LQD MDY MOO MWJ MWN MZZ NLR OEF ONEQ PBP PBW PDP PEJ PFF PGF PGX PHO PIN PIO PRF PSQ PST PXJ QDA QDF QID QLD QQQQ RSP RSU RTH RWM SCO SDS SH SHY SIJ SKF SLV SLX SMN SPY SRS SSO TAN TBT TFI TIP TLT TNA TWM TYH TYP TZA UCO UDN UGL UNG URE USL USO UUP UWM UXY UYG UYM VDE VFH VGK VGT VHT VIS VNQ VPL VTI VTV VUG VV VWO VXF XBI XES XHB XLB XLE XLF XLI XLK XLP XLU XLV XLY XME XOP XRT XSD YCS ZSL OIH ";

        public MarketWatch(WebCapture cap)
        {
            this.cap = cap;
        }

        // get stock latest options chain
        public ArrayList GetOptionsChain(string ticker, Symbol.Type type)
        {
            string symbol_type;

            switch (type)
            {
                case Symbol.Type.Stock:
                    symbol_type = "stock";
                    break;
                case Symbol.Type.Index:
                    symbol_type = "index";
                    break;
                case Symbol.Type.Fund:
                    symbol_type = "fund";
                    break;                
                case Symbol.Type.Future:
                case Symbol.Type.Unknown:
                default:
                    return null;
            }            

            // create options array list
            ArrayList options_list = new ArrayList();
            options_list.Clear();
            options_list.Capacity = 1024;

            string symbol = ticker.TrimStart(new char[] { '^', '~' }).ToLower();
            string url = @"http://www.marketwatch.com/investing/" + symbol_type + "/" + symbol + "/options?countrycode=US&showAll=True";

            string page = cap.DownloadHtmlWebPage(url);
            if (page == null) return null;

            int xb = page.IndexOf("<table class=\"optiontable ");
            int xe = page.LastIndexOf("</table>");
            if (xb == -1 || xe < xb) return null;

            string partial_page = page.Substring(xb, xe - xb) + "</table>";

            XmlDocument xml = cap.ConvertHtmlToXml(partial_page);
            if (xml == null) return null;

            XmlNode table_nd = xml.FirstChild;
            if (table_nd == null) return null;

            // expiration date & strike
            DateTime exp_date = DateTime.MinValue;
            double strike = double.NaN;

            for (int r = 1; ; r++)
            {
                XmlNode row_nd = prs.GetXmlNodeByPath(table_nd, @"tr(" + r + ")");
                if (row_nd == null) break;

                try
                {
                    XmlNode tmp_nd = prs.GetXmlNodeByPath(row_nd, @"form\td\h4\a");

                    if (tmp_nd != null)
                    {
                        string row_text = tmp_nd.InnerText.Trim();

                        if (row_text.EndsWith("Options"))
                        {
                            string[] split = row_text.Replace(",", "").Split(' ');
#if (true)
                            if (DateTime.TryParse("1 " + split[0] + " " + split[1], ci, DateTimeStyles.None, out exp_date))
                            {
                                // get the day after the 3rd firday
                                int days_to_1st_friday = (int)DayOfWeek.Friday - (int)exp_date.DayOfWeek;
                                if (days_to_1st_friday < 0) days_to_1st_friday += 7;
                                exp_date = exp_date.AddDays(15 + days_to_1st_friday);
                            }
                            else exp_date = DateTime.MinValue;
#else
                            DateTime.TryParse(split[2] + "-" + split[1] + "-" + split[3], ci, DateTimeStyles.None, out exp_date);
#endif
                            continue;
                        }
                    }

                    if (exp_date == DateTime.MinValue) continue;

                    // strike price
                    XmlNode cell_nd = prs.GetXmlNodeByPath(row_nd, @"td(8)");
                    if (cell_nd == null || string.IsNullOrEmpty(cell_nd.InnerText)) continue;

                    string cell_text = cell_nd.InnerText.Trim();
                    if (!double.TryParse(cell_text, NumberStyles.Currency, ci, out strike)) continue;

                    for (int j = 0; j < 2; j++)
                    {
                        Option option = new Option();

                        option.stock = ticker;
                        option.expiration = exp_date;
                        option.strike = strike;
                        option.stocks_per_contract = 100;
                        option.type = (j == 0) ? "Call" : "Put";

                        // symbol
                        cell_nd = prs.GetXmlNodeByPath(row_nd, @"td(" + (1 + j * 8) + ")");
                        if (cell_nd == null || cell_nd.InnerText == null || cell_nd.FirstChild == null) continue;

                        option.symbol = "." + cell_nd.FirstChild.Attributes["title"].Value.Trim().ToUpper();
                        if (option.symbol == ".") continue;

                        // last-price
                        cell_nd = prs.GetXmlNodeByPath(row_nd, @"td(" + (2 + j * 8) + ")");
                        if (cell_nd == null || cell_nd.InnerText == null) continue;
 
                        if (!double.TryParse(cell_nd.InnerText.Trim(), NumberStyles.Number, ci, out option.price.last)) 
                            option.price.last = double.NaN;

                        // price-change                        
                        cell_nd = prs.GetXmlNodeByPath(row_nd, @"td(" + (3 + j * 8) + ")");
                        if (cell_nd == null || cell_nd.InnerText == null) continue;

                        if (!double.TryParse(cell_nd.InnerText.Trim(), NumberStyles.Number, ci, out option.price.change))
                            option.price.change = 0;

                        // volume
                        cell_nd = prs.GetXmlNodeByPath(row_nd, @"td(" + (4 + j * 8) + ")");
                        if (cell_nd == null || cell_nd.InnerText == null) continue;

                        if (!double.TryParse(cell_nd.InnerText.Trim(), NumberStyles.Number, ci, out option.volume.total))
                            option.volume.total = 0;

                        // bid-price
                        cell_nd = prs.GetXmlNodeByPath(row_nd, @"td(" + (5 + j * 8) + ")");
                        if (cell_nd == null || cell_nd.InnerText == null) continue;

                        if (!double.TryParse(cell_nd.InnerText.Trim(), NumberStyles.Number, ci, out option.price.bid))
                            option.price.bid = double.NaN;

                        // ask-price
                        cell_nd = prs.GetXmlNodeByPath(row_nd, @"td(" + (6 + j * 8) + ")");
                        if (cell_nd == null || cell_nd.InnerText == null) continue;

                        if (!double.TryParse(cell_nd.InnerText.Trim(), NumberStyles.Number, ci, out option.price.ask))
                            option.price.ask = double.NaN;

                        // open-int
                        cell_nd = prs.GetXmlNodeByPath(row_nd, @"td(" + (7 + j * 8) + ")");
                        if (cell_nd == null || cell_nd.InnerText == null) continue;

                        if (!int.TryParse(cell_nd.InnerText.Trim(), NumberStyles.Number, ci, out option.open_int))
                            option.open_int = 0;

                        options_list.Add(option);
                    }
                }
                catch { }
            }

            return options_list;
        }
    }
}
