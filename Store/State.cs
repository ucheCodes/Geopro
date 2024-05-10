namespace PenPro.Store;

#region Application's State
public class State 
{
  public StateParameters<string> Data2Show{ get;set;} = new StateParameters<string>("");
  public StateParameters<List<double>> StrokeInfo{ get;set;} = new StateParameters<List<double>>(new());
  public StateParameters<List<string[]>> DataArray{ get;set;} = new StateParameters<List<string[]>>(new());
  public StateParameters<bool> IsNewDataClicked{ get; set;} = new StateParameters<bool>(false);
  public StateParameters<bool> IsDataLoaded{ get; set;} = new StateParameters<bool>(false);
  public StateParameters<List<string[]>> HeaderArray{ get; set;} = new StateParameters<List<string[]>>(new());
  public StateParameters<Dictionary<int, string>> HeaderIndexDict{ get; set;} = new StateParameters<Dictionary<int, string>>(new());
  public StateParameters<Dictionary<int, string>> HeaderUnitDict{ get; set;} = new StateParameters<Dictionary<int, string>>(new());
  //List<string[]>
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