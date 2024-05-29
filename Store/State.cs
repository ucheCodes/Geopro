using PenPro.Data;

namespace PenPro.Store;

#region Application's State
public class State 
{
  public StateParameters<string> Data2Show{ get;set;} = new StateParameters<string>("");
  public StateParameters<double> WaterColumnDepth{ get;set;} = new StateParameters<double>(0);
  public StateParameters<double> ConeAreaRatio{ get;set;} = new StateParameters<double>(0);
  public StateParameters<List<double>> StrokeInfo{ get;set;} = new StateParameters<List<double>>(new());
  public StateParameters<List<string[]>> DataArray{ get;set;} = new StateParameters<List<string[]>>(new());
  public StateParameters<bool> IsNewDataClicked{ get; set;} = new StateParameters<bool>(false);
  public StateParameters<double> WaterNature{ get; set;} = new StateParameters<double>(0);
  public StateParameters<bool> IsDataLoaded{ get; set;} = new StateParameters<bool>(false);
  public StateParameters<List<string[]>> HeaderArray{ get; set;} = new StateParameters<List<string[]>>(new());
  public StateParameters<Dictionary<int, string>> HeaderIndexDict{ get; set;} = new StateParameters<Dictionary<int, string>>(new());
  public StateParameters<Dictionary<int, string>> HeaderUnitDict{ get; set;} = new StateParameters<Dictionary<int, string>>(new());
  public StateParameters<Dictionary<int, List<double>>> FilteredAndComputedCPTDataInColumnsDict { get;set;} = new (new());
  public StateParameters<Project> Project{ get;set;} = new StateParameters<Project>(new());
  public StateParameters<Project> SaveProjectParameter{ get;set;} = new StateParameters<Project>(new());
  public StateParameters<Tuple<string,string>> ChartImages{ get;set;} = new StateParameters<Tuple<string,string>>(new("",""));
}
#endregion

#region State Parameters
public class StateParameters<T>
{
  public T Value { get;}
  public StateParameters(T value)
  {
    Value = value;
  }
}

#endregion