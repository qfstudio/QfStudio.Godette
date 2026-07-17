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
        if (typeof(BaseButton).IsAssignableFrom(typeof(T))) return 10;
        if (typeof(LineEdit).IsAssignableFrom(typeof(T))) return 10;
        if (typeof(PopupMenu).IsAssignableFrom(typeof(T))) return 10;
        return 0;
    }

    public IDisposable? BindCommandToObject<T>(ICommand? command, T? target, IObservable<object?> commandParameter) where T : class
    {
        if (command is null)
            return null;

        var disposable = new CompositeDisposable();
        var latestParam = commandParameter.StartWith((object?)null);

        switch (target)
        {
            case BaseButton button:
                BindButton(command, button, latestParam, disposable);
                break;
            case LineEdit lineEdit:
                BindLineEdit(command, lineEdit, latestParam, disposable);
                break;
            case PopupMenu popupMenu:
                BindPopupMenu(command, popupMenu, latestParam, disposable);
                break;
            default:
                return null;
        }

        return disposable;
    }

    private void BindButton(ICommand command, BaseButton button, IObservable<object?> latestParam, CompositeDisposable disposable)
    {
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
    }

    private void BindLineEdit(ICommand command, LineEdit lineEdit, IObservable<object?> latestParam, CompositeDisposable disposable)
    {
        Observable.FromEvent<Godot.LineEdit.TextSubmittedEventHandler, string>(
                h => lineEdit.TextSubmitted += h,
                h => lineEdit.TextSubmitted -= h)
            .WithLatestFrom(latestParam, (_, param) => param)
            .Subscribe(param =>
            {
                if (command.CanExecute(param))
                    command.Execute(param);
            })
            .DisposeWith(disposable);
    }

    private void BindPopupMenu(ICommand command, PopupMenu popupMenu, IObservable<object?> latestParam, CompositeDisposable disposable)
    {
        Observable.FromEvent<Godot.PopupMenu.IdPressedEventHandler, long>(
                h => popupMenu.IdPressed += h,
                h => popupMenu.IdPressed -= h)
            .WithLatestFrom(latestParam, (id, param) => param ?? id)  // 默认使用 id，但允许用户覆盖
            .Subscribe(param =>
            {
                if (command.CanExecute(param))
                    command.Execute(param);
            })
            .DisposeWith(disposable);

        // TODO: CanExecute -> Disabled
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
