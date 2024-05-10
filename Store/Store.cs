namespace PenPro.Store;
using System.Reflection;

#region IStore
public interface IStore
{
  void Mutate<T>(string parameterName, T value);
}
#endregion

#region Store
public class Store : IStore
{
  private readonly object _lock = new object();
  private readonly State _state = new State();
  public Store(State state)
  {
    lock (_lock)
    {
          _state = state;
    }
  }
  private void UpdateState(Action<State> action)
  {
    lock (_lock) {action(_state);}
  }
  #endregion

#region Mutations

  public void Mutate<T>(string parameterName, T value)
  {
    try
    {
      PropertyInfo? property = typeof(State).GetProperty(parameterName);
      if (property != null)
      {
          Action<State> update = s => property.SetValue(s, new StateParameters<T>(value));
          UpdateState(update);
      }
      else
      {
        Console.WriteLine($"{parameterName} not found or defined");
      }
    }
    catch (Exception)
    {
      //Console.WriteLine(e.Message);
    }
  }
#endregion Mutations
}