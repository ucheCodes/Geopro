namespace PenPro.Data;

public class Project
{
    public List<string[]> DataArray { get; set; } = new List<string[]>();
    public string Id { get; set; } = "";
    public List<double[]> FilteredRawDataInRowsArray { get; set; } = new List<double[]>();
    public string TableTitle { get; set; } = string.Empty;
    public string[]? LastDataArray { get; set; } 
    public List<string[]> HeaderArray { get; set; } = new();
    public List<double> SubmergedDensityInCol { get; set; } = new();
    public List<string> SoilNatureInCols { get; set; } = new();
    public List<double> StrokeInfo { get; set; } = new();
    public Dictionary<int, List<double>> FilteredRawDataInColumnsDict { get; set; } = new();
    public Dictionary<int, string> HeaderIndexDict { get;set;} = new();
    public Dictionary<int, string> HeaderUnitDict {get;set;} = new();
    public bool IsFiltered {get;set;} = false;
    public ProjectInfo Info { get; set; } = new();
}