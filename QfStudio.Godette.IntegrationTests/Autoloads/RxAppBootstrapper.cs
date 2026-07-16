using System.Threading;
using Godot;
using QfStudio.Godette.ReactiveUI;
using ReactiveUI;
using ReactiveUI.Builder;
using Splat;

namespace QfStudio.Godette.IntegrationTests.Autoloads;

public partial class RxAppBootstrapper : Godot.Node
{
    // TODO https://www.reactiveui.net/documentation/upgrading/rxappbuilder-migration/
    public RxAppBootstrapper()
    {
        RxAppBuilder.CreateReactiveUIBuilder()
            .WithMainThreadScheduler(GodotMainThreadScheduler.Create(SynchronizationContext.Current!))
            .WithRegistration(locator =>
            {
                locator.RegisterConstant(new GodotActivationFetcher(), typeof(IActivationForViewFetcher));
                locator.RegisterConstant(new GodotPropertyBindingHook(), typeof(ICreatesObservableForProperty));
                locator.RegisterConstant(new GodotCommandBinder(), typeof(ICreatesCommandBinding));
            })
            .WithCoreServices()
            .BuildApp();

        Callable.From(QueueFree).CallDeferred();
    }
}
