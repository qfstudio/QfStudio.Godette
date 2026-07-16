using System.Reactive.Disposables;
using Godot;

namespace QfStudio.Godotte.ReactiveUI;

public interface IActivatable
{
    public GodotSynchronizationContext UiContext { get; }

    public bool IsActivated { get; }

    public void Activate();

    public void Deactivate();

    public void WhenActivated(Action<CompositeDisposable> block);
}
