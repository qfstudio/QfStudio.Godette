using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using System.Windows.Input;
using Godot;
using ReactiveUI;

namespace QfStudio.Godette.ReactiveUI;

// TODO how to support command binding for PopupMenu?
// PopupMenu uses id to distinguish different menus when it is pressed, however that id should not be the parameter of a ViewModel command because it is a ui stuff.
// so a native PopupMenu is not suitable target to bind commands.
// Similar situations: OptionButton, TabBar, ItemList, Tree, FileDialog
public class GodotCommandBinder : ICreatesCommandBinding
{
    int ICreatesCommandBinding.GetAffinityForObject<T>(bool hasEventTarget)
    {
        var type = typeof(T);

        return type switch
        {
            not null when typeof(BaseButton).IsAssignableFrom(type) => 10,
            not null when typeof(LineEdit).IsAssignableFrom(type) => 10,
            _ => 0
        };
    }

    public IDisposable? BindCommandToObject<T>(ICommand? command, T? target, IObservable<object?> commandParameter) where T : class
    {
        if (command is null)
            return null;

        return target switch
        {
            BaseButton button => GodotCommandBinderImpl.BindButton(command, button, commandParameter),
            LineEdit lineEdit => GodotCommandBinderImpl.BindLineEdit(command, lineEdit, commandParameter),
            _ => null
        };
    }

    public IDisposable? BindCommandToObject<T, TEventArgs>(ICommand? command, T? target, IObservable<object?> commandParameter,
        string eventName) where T : class
    {
        throw new NotSupportedException($"GodotCommandBinder does not support binding by event name.");
    }

    public IDisposable? BindCommandToObject<T, TEventArgs>(ICommand? command, T? target, IObservable<object?> commandParameter,
        Action<EventHandler<TEventArgs>> addHandler, Action<EventHandler<TEventArgs>> removeHandler) where T : class where TEventArgs : EventArgs
    {
        throw new NotSupportedException("GodotCommandBinder does not support custom event handlers, as most Godot controls do not follow the standard .NET event pattern.");
    }
}

internal static class GodotCommandBinderImpl
{
    private static IDisposable BindViewCore(ICommand command, Action<Action> addHandler, Action<Action> removeHandler, IObservable<object?> commandParameter, Action<bool>? setViewEnabled)
    {
        return BindViewCore(command, Observable.FromEvent(addHandler, removeHandler), commandParameter, setViewEnabled);
    }

    private static IDisposable BindViewCore<TDelegate, TEventArgs>(ICommand command, Action<TDelegate> addHandler, Action<TDelegate> removeHandler, IObservable<object?> commandParameter, Action<bool>? setViewEnabled)
    {
        return BindViewCore(command,
            Observable.FromEvent<TDelegate, TEventArgs>(addHandler, removeHandler).Select(_ => Unit.Default),
            commandParameter,
            setViewEnabled);
    }

    private static IDisposable BindViewCore(ICommand command, IObservable<Unit> commandTrigger, IObservable<object?> commandParameter, Action<bool>? setViewEnabled)
    {
        var disposable = new CompositeDisposable();

        commandTrigger
            .WithLatestFrom(commandParameter.StartWith((object?)null), (_, param) => param)
            .Subscribe(param =>
            {
                if (command.CanExecute(param))
                    command.Execute(param);
            })
            .DisposeWith(disposable);

        if (setViewEnabled != null)
        {
            setViewEnabled(command.CanExecute(null));

            Observable.FromEventPattern(
                    h => command.CanExecuteChanged += h,
                    h => command.CanExecuteChanged -= h)
                .WithLatestFrom(commandParameter.StartWith((object?)null), (_, param) => param)
                .Select(command.CanExecute)
                .DistinctUntilChanged()
                .Subscribe(setViewEnabled)
                .DisposeWith(disposable);
        }

        return disposable;
    }

    public static IDisposable BindButton(ICommand command, BaseButton button, IObservable<object?> param)
    {
        return BindViewCore(command,
            h => button.Pressed += h,
            h => button.Pressed -= h,
            param,
            enabled => button.Disabled = !enabled);
    }

    public static IDisposable BindLineEdit(ICommand command, LineEdit lineEdit, IObservable<object?> param)
    {
        return BindViewCore<LineEdit.TextSubmittedEventHandler, string>(command,
            h => lineEdit.TextSubmitted += h,
            h => lineEdit.TextSubmitted -= h,
            param,
            enabled => lineEdit.Editable = enabled);
    }
}
