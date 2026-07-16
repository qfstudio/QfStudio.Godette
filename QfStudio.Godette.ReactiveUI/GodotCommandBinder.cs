using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using System.Windows.Input;
using Godot;
using ReactiveUI;

namespace QfStudio.Godette.ReactiveUI;

public class GodotCommandBinder : ICreatesCommandBinding
{
    int ICreatesCommandBinding.GetAffinityForObject<T>(bool hasEventTarget)
    {
        return typeof(BaseButton).IsAssignableFrom(typeof(T)) ? 10 : 0;
    }

    public IDisposable? BindCommandToObject<T>(ICommand? command, T? target, IObservable<object?> commandParameter) where T : class
    {
        if (command is null || target is not BaseButton button)
            return null;

        var disposable = new CompositeDisposable();
        var latestParam = commandParameter.StartWith((object?)null);

        Observable.FromEvent(
                h => button.Pressed += h,
                h => button.Pressed -= h)
            .WithLatestFrom(latestParam, (_, param) => param)
            .Subscribe(param =>
            {
                if (command.CanExecute(param))
                    command.Execute(param);
            })
            .DisposeWith(disposable);

        Observable.FromEvent<EventHandler, EventArgs>(
                h => (_, e) => h(e),
                h => command.CanExecuteChanged += h,
                h => command.CanExecuteChanged -= h)
            .WithLatestFrom(latestParam, (_, param) => param)
            .Subscribe(param => button.Disabled = !command.CanExecute(param))
            .DisposeWith(disposable);

        return disposable;
    }

    public IDisposable? BindCommandToObject<T, TEventArgs>(ICommand? command, T? target, IObservable<object?> commandParameter,
        string eventName) where T : class
    {
        throw new NotSupportedException($"GodotCommandBinder does not support binding by event name. Use the basic overload for BaseButton.Pressed.");
    }

    public IDisposable? BindCommandToObject<T, TEventArgs>(ICommand? command, T? target, IObservable<object?> commandParameter,
        Action<EventHandler<TEventArgs>> addHandler, Action<EventHandler<TEventArgs>> removeHandler) where T : class where TEventArgs : EventArgs
    {
        throw new NotSupportedException("GodotCommandBinder does not support custom event handlers. Use the basic overload for BaseButton.Pressed.");
    }
}
