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
using System.Text;
using System.Collections;
using System.Threading;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Drawing;
using System.Xml;
using System.Windows;
using Microsoft.Win32;
using System.Windows.Forms;

namespace OptionsOracle.DataCenter
{
    public class Global
    {
        // open html link on external browser
        static public void OpenExternalBrowser(string url)
        {
            try
            {
                // get default browser
                RegistryKey registryKey = Registry.ClassesRoot.OpenSubKey(@"htmlfile\shell\open\command", false);
                string browser = ((string)registryKey.GetValue(null, null)).Split('"')[1];

                // open page on default browser
                Process ieProc = Process.Start(browser, url);
            }
            catch { }
        }

        static public void LoadXmlDataset(DataSet ds, string data)
        {
            StringReader stream = null;
            XmlTextReader reader = null;

            try
            {
                ds.Clear();
                stream = new StringReader(data.Replace("\\\"", "\""));
                reader = new XmlTextReader(stream);
                ds.ReadXml(reader);
            }
            catch
            {
            }
            finally
            {
                if (reader != null) reader.Close();
            }
        }

        public static void SaveAsExcel(DataSet source, string fileName, string filter)
        {
            System.IO.StreamWriter excelDoc;

            excelDoc = new System.IO.StreamWriter(fileName);

            const string startExcelXML = "<xml version>\r\n<Workbook " +
                  "xmlns=\"urn:schemas-microsoft-com:office:spreadsheet\"\r\n" +
                  " xmlns:o=\"urn:schemas-microsoft-com:office:office\"\r\n " +
                  "xmlns:x=\"urn:schemas-    microsoft-com:office:" +
                  "excel\"\r\n xmlns:ss=\"urn:schemas-microsoft-com:" +
                  "office:spreadsheet\">\r\n <Styles>\r\n " +
                  "<Style ss:ID=\"Default\" ss:Name=\"Normal\">\r\n " +
                  "<Alignment ss:Vertical=\"Bottom\"/>\r\n <Borders/>" +
                  "\r\n <Font/>\r\n <Interior/>\r\n <NumberFormat/>" +
                  "\r\n <Protection/>\r\n </Style>\r\n " +
                  "<Style ss:ID=\"BoldColumn\">\r\n <Font " +
                  "x:Family=\"Swiss\" ss:Bold=\"1\"/>\r\n </Style>\r\n " +
                  "<Style     ss:ID=\"StringToRight\">\r\n <Alignment" +
                  " ss:Horizontal=\"Right\"/> </Style>\r\n" +
                  "<Style     ss:ID=\"StringDefault\">\r\n <NumberFormat" +
                  " ss:Format=\"@\"/>\r\n </Style>\r\n <Style " +
                  "ss:ID=\"Decimal\">\r\n <NumberFormat " +
                  "ss:Format=\"0.0000\"/>\r\n </Style>\r\n " +
                  "<Style ss:ID=\"Percent\">\r\n <NumberFormat " +
                  "ss:Format=\"Percent\"/>\r\n </Style>\r\n " +
                  "<Style ss:ID=\"Integer\">\r\n <NumberFormat " +
                  "ss:Format=\"0\"/>\r\n </Style>\r\n <Style " +
                  "ss:ID=\"DateLiteral\">\r\n <NumberFormat " +
                  "ss:Format=\"mm/dd/yyyy;@\"/>\r\n </Style>\r\n " +
                  "</Styles>\r\n ";

            const string endExcelXML = "</Workbook>";

            // <xml version>
            // <Workbook xmlns="urn:schemas-microsoft-com:office:spreadsheet"
            // xmlns:o="urn:schemas-microsoft-com:office:office"
            // xmlns:x="urn:schemas-microsoft-com:office:excel"
            // xmlns:ss="urn:schemas-microsoft-com:office:spreadsheet">
            // <Styles>
            // <Style ss:ID="Default" ss:Name="Normal">
            //   <Alignment ss:Vertical="Bottom"/>
            //   <Borders/>
            //   <Font/>
            //   <Interior/>
            //   <NumberFormat/>
            //   <Protection/>
            // </Style>
            // <Style ss:ID="BoldColumn">
            //   <Font x:Family="Swiss" ss:Bold="1"/>
            // </Style>
            // <Style ss:ID="StringDefault">
            //   <NumberFormat ss:Format="@"/>
            // </Style>
            // <Style ss:ID="Decimal">
            //   <NumberFormat ss:Format="0.0000"/>
            // </Style>
            // <Style ss:ID="Integer">
            //   <NumberFormat ss:Format="0"/>
            // </Style>
            // <Style ss:ID="DateLiteral">
            //   <NumberFormat ss:Format="mm/dd/yyyy;@"/>
            // </Style>
            // </Styles>
            // <Worksheet ss:Name="Sheet1">
            // </Worksheet>
            // </Workbook>

            excelDoc.Write(startExcelXML);

            ArrayList tables_list = new ArrayList();
            ArrayList filter_list = new ArrayList();

            string[] list1 = filter.Split(new char[] { ';' });
            foreach (string item in list1)
            {
                string[] list2 = item.Split(new char[] { '|' });
                if (list2.Length == 2)
                {
                    tables_list.Add(list2[0]);
                    filter_list.Add(list2[1]);
                }
            }

            for (int i = 0; i < filter_list.Count; i++)
            {
                DataTable table = source.Tables[(string)(tables_list[i])];

                ArrayList columns_list = new ArrayList(); // column name
                ArrayList formats_list = new ArrayList(); // format name
                ArrayList titles_list  = new ArrayList(); // column title name
                ArrayList nullstr_list = new ArrayList(); // column null cell replacement

                string[] list3 = ((string)(filter_list[i])).Split(new char[] { ',' });
                foreach (string item in list3)
                {
                    string[] list4 = item.Split(new char[] { ':' });
                    if (list4.Length > 0)
                    {
                        columns_list.Add(list4[0]);
                        if (list4.Length > 1) formats_list.Add(list4[1]);
                        else formats_list.Add("");
                        if (list4.Length > 2) titles_list.Add(list4[2]);
                        else titles_list.Add("");
                        if (list4.Length > 3) nullstr_list.Add(list4[3]);
                        else nullstr_list.Add("");
                    }
                }

                excelDoc.Write("<Worksheet ss:Name=\"" + table.TableName.Replace("Table", "") + "\">");
                excelDoc.Write("<Table>");
                excelDoc.Write("<Row>");

                for (int y = 0; y < columns_list.Count; y++)
                {
                    DataColumn col = table.Columns[(string)(columns_list[y])];

                    excelDoc.Write("<Cell ss:StyleID=\"BoldColumn\"><Data ss:Type=\"String\">");
                    if ((string)titles_list[y] == "") excelDoc.Write(col.ColumnName);
                    else excelDoc.Write((string)titles_list[y]);
                    excelDoc.Write("</Data></Cell>");
                }
                excelDoc.Write("</Row>");

                foreach (DataRow x in table.Rows)
                {
                    excelDoc.Write("<Row>");

                    for (int z = 0; z < columns_list.Count; z++)
                    {
                        int y = table.Columns.IndexOf(table.Columns[(string)(columns_list[z])]);
                        string format = (string)(formats_list[z]);

                        System.Type rowType;
                        rowType = x[y].GetType();

                        if (x[y] == DBNull.Value && (string)nullstr_list[z] != "")
                        {
                            excelDoc.Write("<Cell ss:StyleID=\"StringDefault\">" + "<Data ss:Type=\"String\">");
                            excelDoc.Write((string)nullstr_list[z]);
                            excelDoc.Write("</Data></Cell>");
                        }
                        else
                        {
                            switch (rowType.ToString())
                            {
                                case "System.String":
                                    string XMLstring = x[y].ToString();
                                    XMLstring = XMLstring.Trim();
                                    XMLstring = XMLstring.Replace("&", "&");
                                    XMLstring = XMLstring.Replace(">", ">");
                                    XMLstring = XMLstring.Replace("<", "<");
                                    excelDoc.Write("<Cell ss:StyleID=\"StringDefault\">" + "<Data ss:Type=\"String\">");
                                    excelDoc.Write(XMLstring);
                                    excelDoc.Write("</Data></Cell>");
                                    break;
                                case "System.DateTime":
                                    //Excel has a specific Date Format of YYYY-MM-DD followed by  
                                    //the letter 'T' then hh:mm:sss.lll Example 2005-01-31T24:01:21.000
                                    //The Following Code puts the date stored in XMLDate 
                                    //to the format above
                                    DateTime XMLDate = (DateTime)x[y];
                                    string XMLDatetoString = ""; //Excel Converted Date
                                    XMLDatetoString = XMLDate.Year.ToString() +
                                         "-" +
                                         (XMLDate.Month < 10 ? "0" +
                                         XMLDate.Month.ToString() : XMLDate.Month.ToString()) +
                                         "-" +
                                         (XMLDate.Day < 10 ? "0" +
                                         XMLDate.Day.ToString() : XMLDate.Day.ToString()) +
                                         "T" +
                                         (XMLDate.Hour < 10 ? "0" +
                                         XMLDate.Hour.ToString() : XMLDate.Hour.ToString()) +
                                         ":" +
                                         (XMLDate.Minute < 10 ? "0" +
                                         XMLDate.Minute.ToString() : XMLDate.Minute.ToString()) +
                                         ":" +
                                         (XMLDate.Second < 10 ? "0" +
                                         XMLDate.Second.ToString() : XMLDate.Second.ToString()) +
                                         ".000";
                                    excelDoc.Write("<Cell ss:StyleID=\"DateLiteral\">" + "<Data ss:Type=\"DateTime\">");
                                    excelDoc.Write(XMLDatetoString);
                                    excelDoc.Write("</Data></Cell>");
                                    break;
                                case "System.Boolean":
                                    excelDoc.Write("<Cell ss:StyleID=\"StringDefault\">" + "<Data ss:Type=\"String\">");
                                    excelDoc.Write(x[y].ToString());
                                    excelDoc.Write("</Data></Cell>");
                                    break;
                                case "System.Int16":
                                case "System.Int32":
                                case "System.Int64":
                                case "System.Byte":
                                    excelDoc.Write("<Cell ss:StyleID=\"Integer\">" + "<Data ss:Type=\"Number\">");
                                    excelDoc.Write(x[y].ToString());
                                    excelDoc.Write("</Data></Cell>");
                                    break;
                                case "System.Decimal":
                                case "System.Double":
                                    if (double.IsNaN((double)x[y]) || double.IsInfinity((double)x[y]))
                                    {
                                        excelDoc.Write("<Cell ss:StyleID=\"StringToRight\">" + "<Data ss:Type=\"String\">");
                                        excelDoc.Write(x[y].ToString());
                                        excelDoc.Write("</Data></Cell>");
                                    }
                                    else if (format != "")
                                    {
                                        excelDoc.Write("<Cell ss:StyleID=\"" + format + "\">" + "<Data ss:Type=\"Number\">");
                                        excelDoc.Write(x[y].ToString());
                                        excelDoc.Write("</Data></Cell>");
                                    }
                                    else
                                    {
                                        excelDoc.Write("<Cell ss:StyleID=\"Decimal\">" + "<Data ss:Type=\"Number\">");
                                        excelDoc.Write(x[y].ToString());
                                        excelDoc.Write("</Data></Cell>");
                                    }
                                    break;
                                case "System.DBNull":
                                    excelDoc.Write("<Cell ss:StyleID=\"StringDefault\">" + "<Data ss:Type=\"String\">");
                                    excelDoc.Write("");
                                    excelDoc.Write("</Data></Cell>");
                                    break;
                                default:
                                    throw (new Exception(rowType.ToString() + " not handled."));
                            }
                        }
                    }
                    excelDoc.Write("</Row>");
                }

                excelDoc.Write("</Table>");
                excelDoc.Write(" </Worksheet>");
            }

            excelDoc.Write(endExcelXML);
            excelDoc.Close();
        }
    }
}
