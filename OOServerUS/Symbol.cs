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

namespace OOServerUS
{
    public class Symbol
    {
        private static string IndexSymbol = " BXO RUH DJX DXL OEX OEX XEO XEO VIX BVZ SPX XSP BSZ SPL SML VXN NDX MNX MML RUI RVX RUT RMN NFT RUH OEX OEX XEO XEO SPX XSP BSZ SPL SML VIX BVZ DJX DTX DUX VXN NDX MNX MML RUI RVX RUT RMN EXQ GOX INX OIX TXX MVR MVB MGO MSV NFT CJN CTH CYX NFT ";
        private static string FundSymbol = " AINV AGQ AUS BGU BGZ BWX DAX DBA DBB DBC DBE DBO DBP DBS DBV DDM DGL DGT DIA DIG DND DOG DUG DVY DXD EDC EDZ EEB EEM EEV EFA EFU EPI EPP ERY ERX EUO EWA EWC EWD EWG EWH EWJ EWL EWM EWP EWT EWW EWY EWZ EZA EZU FAS FAZ FEZ FPX FPX FRC FXB FXC FXE FXP FXY GDX GEX GLD GLL GSG GWX GXC HKG HYG IAI IAT IAU IBB IDU IEF IEZ IGE IGM IGN IGV IGW IIF IJR ILF ITB IVV IYY IWB IWC IWD IWF IWM IWN IWO IWP IWR IWS IWV IWW IWZ IYE IYF IYH IYM IYR IYY IYZ KBE KCE KIE KOL KRE KWT LDN LQD MDY MOO MWJ MWN MZZ NLR OEF ONEQ PBP PBW PDP PEJ PFF PGF PGX PHO PIN PIO PRF PSQ PST PXJ QDA QDF QID QLD QQQQ RSP RSU RTH RWM SCO SDS SH SHY SIJ SKF SLV SLX SMN SPY SRS SSO TAN TBT TFI TIP TLT TNA TWM TYH TYP TZA UCO UDN UGL UNG URE USL USO UUP UWM UXY UYG UYM VDE VFH VGK VGT VHT VIS VNQ VPL VTI VTV VUG VV VWO VXF XBI XES XHB XLB XLE XLF XLI XLK XLP XLU XLV XLY XME XOP XRT XSD YCS ZSL OIH ";

        public enum Type 
        {
            Unknown,
            Stock, 
            Fund, 
            Index, 
            Future 
        };

        public static string CorrectSymbol(string ticker, out Type type)
        {
            ticker = ticker.ToUpper().Trim();

            if (ticker.StartsWith("^") || IndexSymbol.Contains(" " + ticker + " "))            
                type = Type.Index;
            else if (FundSymbol.Contains(" " + ticker + " "))
                type = Type.Fund;
            else if (ticker.StartsWith("~"))
                type = Type.Future;
            else
                type = Type.Stock;

            switch (ticker)
            {
                default:
                    if (type == Type.Index && ticker[0] != '^') ticker = "^" + ticker;
                    else if (type == Type.Future && ticker[0] != '~') ticker = "~" + ticker;
                    return ticker;
            }
        }
    }
}
