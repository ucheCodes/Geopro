namespace PenPro.Data;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel; // For .xlsx
using NPOI.HSSF.UserModel; // For .xls
using System;
using System.IO;

interface INPOIExcelReader
{
    List<string[]> ReadExcelData(Stream fileStream, string fileName);
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
}