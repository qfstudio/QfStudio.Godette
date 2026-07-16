using System.Windows.Input;
using ReactiveUI;

namespace QfStudio.Godette.ReactiveUI;

public class GodotCommandBinder : ICreatesCommandBinding
{
    public int GetAffinityForObject<T>(bool hasEventTarget)
    {
        throw new NotImplementedException();
    }

    public IDisposable? BindCommandToObject<T>(ICommand? command, T? target, IObservable<object?> commandParameter) where T : class
    {
        throw new NotImplementedException();
    }

    public IDisposable? BindCommandToObject<T, TEventArgs>(ICommand? command, T? target, IObservable<object?> commandParameter,
        string eventName) where T : class
    {
        throw new NotImplementedException();
    }

    public IDisposable? BindCommandToObject<T, TEventArgs>(ICommand? command, T? target, IObservable<object?> commandParameter,
        Action<EventHandler<TEventArgs>> addHandler, Action<EventHandler<TEventArgs>> removeHandler) where T : class where TEventArgs : EventArgs
    {
        throw new NotImplementedException();
    }
}
