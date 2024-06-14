namespace PenPro.Data;
using Microsoft.JSInterop;
using System;
using System.IO;
using System.Text;

class WriteToFile
{
    public  IJSRuntime jSRuntime { get; set; }
    public WriteToFile(IJSRuntime jsr)
    {
        jSRuntime = jsr;
    }
    public string BuildColumnsToCSVString<Tkey,Tval>(Dictionary<Tkey, List<Tval>> columns)
        where Tkey : notnull
        where Tval : notnull
    {
        StringBuilder dataBuilder = new StringBuilder();
        int maxColumnCount = columns.Values.Max(list => list.Count);
        for(int i = 0; i < maxColumnCount; i++)
        {
            string line = "";
            int j = 0;
            foreach(var column in columns)
            {
                if(i < column.Value.Count && j < columns.Count - 1)
                {
                    line += $"{column.Value[i]},";
                }
                else if(i < column.Value.Count && j <= columns.Count - 1)
                {
                    line += $"{column.Value[i]}";
                }
                j++;
            }
            line += "\n";
            dataBuilder.Append(line);
        }
        string data = dataBuilder.ToString();
        return data;
    }
    public string BuildRowsToCSVString(IEnumerable<Array> rows)
    {
        StringBuilder dataBuilder = new StringBuilder();
        foreach(var row in rows)
        {
            string line = "";
            int j = 0;
            foreach(var data in row)
            {
                if(j < row.Length - 1)
                {
                    line += $"{data},";
                }
                else{
                    line += $"{data}";
                }
                j++;
            }
            line += "\n";
            dataBuilder.Append(line);
        }
        string _data = dataBuilder.ToString();
        return _data;
    }
    public async void WriteToCSVFile(string path,string data)
    {
        //string path = $"{Guid.NewGuid().ToString()}.csv";
        await jSRuntime.InvokeVoidAsync("saveAsFile", path, data);
    }
    public static void WriteDataToFile(string filepath, string data)
    {
        try
        {
            string directoryPath = Path.GetDirectoryName(filepath) ?? "hello";
            if(directoryPath != null)
            {
                Directory.CreateDirectory(directoryPath);
                using (StreamWriter writer = new StreamWriter(filepath, append:true))
                {
                    writer.WriteLine(data);
                }
            }
        }
        catch(Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
}