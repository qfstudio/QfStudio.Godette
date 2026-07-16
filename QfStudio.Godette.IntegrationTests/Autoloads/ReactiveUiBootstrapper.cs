using System.Threading;
using QfStudio.Godette.ReactiveUI;
using ReactiveUI;
using Splat;

namespace QfStudio.Godette.IntegrationTests.Autoloads;

public partial class ReactiveUiBootstrapper : Godot.Node
{
    public ReactiveUiBootstrapper()
    {
        var locator = Locator.CurrentMutable;
        RxSchedulers.MainThreadScheduler = GodotMainThreadScheduler.Create(SynchronizationContext.Current!);
        locator.RegisterConstant(new GodotActivationFetcher(), typeof(IActivationForViewFetcher));
        locator.RegisterConstant(new GodotPropertyBindingHook(), typeof(ICreatesObservableForProperty));
        locator.RegisterConstant(new GodotCommandBinder(), typeof(ICreatesCommandBinding));
    }
}
