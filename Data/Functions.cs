using PenPro.Database;
using Newtonsoft.Json;
using PenPro.Store;

namespace PenPro.Data;
class Functions : IFunctions
{
    public  DatabaseService database { get; set; }
    public State state{ get; set; }
    public Functions(DatabaseService db, State _state)
    {
        database = db;
        state = _state;
    }
    public Dictionary<string,string> GetSamplingTool()
    {
        Dictionary<string,string> samplingTool = new Dictionary<string,string>()
        {
            {"Wison Push Sampler", "WIP"},
            {"Push Sampler", "PS"},
            {"Piston Sampler", "P"},
            {"Hammer Sampler", "SH"},
            {"Split Spoon", "SS"},
           // {"No Recovery", "N/R"}
        };
        return samplingTool;
    }
    public bool SaveProject(Project project)
    {
        if (project != null && project.Id != "")
        {
            string key = JsonConvert.SerializeObject(project.Id);
            string value = JsonConvert.SerializeObject(project);
            bool isCreated = database.Create("Projects",key,value); 
            return isCreated;
        }
        return false;
    }
    public Tuple<double,double> ComputeOverburdenPressure(double bulkDensityInKNperM3, double depth)
    {
        double yw = state.WaterNature.Value;//10.05;//KN/m3
        double waterColumnDepth = state.WaterColumnDepth.Value;
        double overburdenOfWaterColumn = yw  * waterColumnDepth;
        double overburdenOfSoilLayers = bulkDensityInKNperM3  * depth;
        double totalOverburden = (overburdenOfWaterColumn + overburdenOfSoilLayers) / 1000;//convert result
        double effectiveOverburden = overburdenOfSoilLayers / 1000;//from KN/m2 to MPa   
        return new (effectiveOverburden, totalOverburden);
    }
    public double RoundNumber(double value)
    {
        if (value > 10.0)
        {
            return Math.Ceiling(value);
        }
        else if (value < -10.0)
        {
            return Math.Floor(value);
        }
        else if (value > 0 && value < 0.1)
        {
            return 0.1;
        }
        else if (value < 0 && value > -0.1)
        {
            return -0.1;
        }
        else
        {
            if (value > 0)
            {
                return Math.Ceiling(value * 10) / 10.0;
            }
            else
            {
                return Math.Floor(value * 10) / 10.0;
            }
        }
    }
}
interface IFunctions
{
    bool SaveProject(Project project);
    Tuple<double,double> ComputeOverburdenPressure(double bulkDensityInKNperM3, double depth);
    Dictionary<string,string> GetSamplingTool();
    double RoundNumber(double value);
}