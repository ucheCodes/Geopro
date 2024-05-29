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
}
interface IFunctions
{
    bool SaveProject(Project project);
    Tuple<double,double> ComputeOverburdenPressure(double bulkDensityInKNperM3, double depth) ;
}