namespace PenPro.Data;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel; // For .xlsx
using NPOI.HSSF.UserModel; // For .xls
using System;
using System.IO;

interface INPOIExcelReader
{
    List<string[]> ReadExcelData(Stream fileStream, string fileName);
    Dictionary<string, List<double>> ReadAndParseExcelData(Stream fileStream, string fileName, int sheetIndex);
}
public class NPOIExcelReader : INPOIExcelReader
{
    public List<string[]> ReadExcelData(Stream fileStream, string fileName)
    { 
        List<string[]> ListOfRows = new List<string[]>();
        IWorkbook? workbook = null;
        if (fileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
        {
            workbook = new XSSFWorkbook(fileStream);
        }
        else if (fileName.EndsWith(".xls", StringComparison.OrdinalIgnoreCase))
        {
            workbook = new HSSFWorkbook(fileStream);
        }
        else
        {
            Console.WriteLine("The file is neither an .xlsx nor .xls file.");
        }

        if(workbook != null)
        {
            ISheet sheet = workbook.GetSheetAt(1);
            for (int row = 0; row <= sheet.LastRowNum; row++)
            {
                IRow currentRow = sheet.GetRow(row);
                if (currentRow != null)
                {
                    string[]  rowsData = new string[currentRow.LastCellNum];
                    for (int col = 0; col < currentRow.LastCellNum; col++)
                    {
                        ICell cell = currentRow.GetCell(col);
                        if (cell != null)
                        {
                            rowsData[col] = cell.ToString() ?? "";
                            //Console.Write(cell.ToString() + "\t");
                        }
                    }
                    ListOfRows.Add(rowsData);
                    //Console.WriteLine($"List of rows {ListOfRows.Count} rows: {rowsData.Length}");
                }
                else{Console.WriteLine($"Current row at {row} is null");}
            }
        }
        return ListOfRows;
    }
public Dictionary<string, List<double>> ReadAndParseExcelData(Stream fileStream, string fileName, int sheetIndex)
{
    Dictionary<string, List<double>> columnsData = new Dictionary<string, List<double>>();
    IWorkbook? workbook = null;

    if (fileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
    {
        workbook = new XSSFWorkbook(fileStream);
    }
    else if (fileName.EndsWith(".xls", StringComparison.OrdinalIgnoreCase))
    {
        workbook = new HSSFWorkbook(fileStream);
    }
    else
    {
        Console.WriteLine("The file is neither an .xlsx nor .xls file.");
        return columnsData;
    }

    if (workbook != null)
    {
        ISheet sheet = workbook.GetSheetAt(sheetIndex);
        if (sheet.PhysicalNumberOfRows > 0)
        {
            // Read headers
            //string h = "header = ";
            IRow headerRow = sheet.GetRow(0);
            if (headerRow != null)
            {
                int numberOfColumns = headerRow.LastCellNum;
                for (int col = 0; col < numberOfColumns; col++)
                {
                    ICell headerCell = headerRow.GetCell(col);
                    if (headerCell != null)
                    {
                        string? header = headerCell?.ToString() ?? string.Empty;
                        if (!columnsData.ContainsKey(header))
                        {
                            columnsData[header] = new List<double>();
                            //h += $"-{header}-";//remove
                        }
                    }
                }
                //Console.WriteLine(h);//remove
                // Read data rows
                //string d = "data = ";//remove
                for (int row = 1; row <= sheet.LastRowNum; row++)
                {
                    IRow currentRow = sheet.GetRow(row);
                    if (currentRow != null)
                    {
                        for (int col = 0; col < numberOfColumns; col++)
                        {
                            ICell cell = currentRow.GetCell(col);
                            if (cell != null)
                            {
                                string? header = headerRow?.GetCell(col).ToString() ?? "";
                                if (double.TryParse(cell.ToString(), out double cellValue))
                                {
                                    columnsData[header].Add(cellValue);
                                    //d += $"-{cellValue}-";
                                }
                            }
                        }
                       // Console.WriteLine(d);
                    }
                }
            }
        }
    }

    return columnsData;
}

/*
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;

public Dictionary<string, List<double>> ReadExcelDataAsColumns(Stream fileStream, string fileName)
{
    Dictionary<string, List<double>> columnsData = new Dictionary<string, List<double>>();
    IWorkbook? workbook = null;

    if (fileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
    {
        workbook = new XSSFWorkbook(fileStream);
    }
    else if (fileName.EndsWith(".xls", StringComparison.OrdinalIgnoreCase))
    {
        workbook = new HSSFWorkbook(fileStream);
    }
    else
    {
        Console.WriteLine("The file is neither an .xlsx nor .xls file.");
        return columnsData;
    }

    if (workbook != null)
    {
        ISheet sheet = workbook.GetSheetAt(0); // Assuming we are reading the first sheet
        if (sheet.PhysicalNumberOfRows > 0)
        {
            // Read headers and initialize dictionary
            IRow headerRow = sheet.GetRow(0);
            int numberOfColumns = headerRow.LastCellNum;

            for (int col = 0; col < numberOfColumns; col++)
            {
                ICell headerCell = headerRow.GetCell(col);
                if (headerCell != null)
                {
                    string header = headerCell.ToString();
                    if (!columnsData.ContainsKey(header))
                    {
                        columnsData[header] = new List<double>();
                    }
                }
            }

            // Read data column by column
            for (int col = 0; col < numberOfColumns; col++)
            {
                ICell headerCell = headerRow.GetCell(col);
                if (headerCell != null)
                {
                    string header = headerCell.ToString();

                    // Get column data
                    List<double> columnData = GetColumnData(sheet, col);

                    columnsData[header].AddRange(columnData);
                }
            }
        }
    }

    return columnsData;
}

private List<double> GetColumnData(ISheet sheet, int colIndex)
{
    List<double> columnData = new List<double>();

    for (int row = 1; row <= sheet.LastRowNum; row++) // Start from row 1 to skip the header
    {
        IRow currentRow = sheet.GetRow(row);
        if (currentRow != null)
        {
            ICell cell = currentRow.GetCell(colIndex);
            if (cell != null && double.TryParse(cell.ToString(), out double cellValue))
            {
                columnData.Add(cellValue);
            }
        }
    }

    return columnData;
}


*/
}