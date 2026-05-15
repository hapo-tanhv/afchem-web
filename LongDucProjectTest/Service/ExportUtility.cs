using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using CsvHelper;
using OfficeOpenXml;

namespace LongDucProjectTest.Service
{
    public static class ExportUtility
    {
        static ExportUtility()
        {
            // Required for EPPlus 5+
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        /// <summary>
        /// Exports a generic list of objects to an Excel byte array using EPPlus.
        /// </summary>
        public static byte[] ExportToExcel<T>(List<T> data, string worksheetName = "Data")
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add(worksheetName);
                
                PropertyInfo[] properties = typeof(T).GetProperties();
                
                // Write headers
                for (int i = 0; i < properties.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = properties[i].Name;
                    worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                }
                
                // Write data
                for (int row = 0; row < data.Count; row++)
                {
                    for (int col = 0; col < properties.Length; col++)
                    {
                        var val = properties[col].GetValue(data[row]);
                        if (val is DateTime dt)
                        {
                            worksheet.Cells[row + 2, col + 1].Value = dt.ToString("yyyy-MM-dd HH:mm:ss");
                        }
                        else
                        {
                            worksheet.Cells[row + 2, col + 1].Value = val;
                        }
                    }
                }
                
                if (worksheet.Dimension != null)
                {
                    worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                }
                return package.GetAsByteArray();
            }
        }

        /// <summary>
        /// Exports a generic list of objects to a CSV byte array using CsvHelper.
        /// </summary>
        public static byte[] ExportToCsv<T>(List<T> data)
        {
            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8))
            using (var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture))
            {
                csvWriter.WriteRecords(data);
                streamWriter.Flush();
                return memoryStream.ToArray();
            }
        }
    }
}
