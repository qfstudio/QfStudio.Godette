using System.Threading;
using Godot;
using QfStudio.Godette.ReactiveUI;
using ReactiveUI;
using ReactiveUI.Builder;
using Splat;

namespace QfStudio.Godette.IntegrationTests.Autoloads;

public partial class RxAppBootstrapper : Godot.Node
{
    public RxAppBootstrapper()
    {
        var scheduler = GodotMainThreadScheduler.Create(SynchronizationContext.Current!);

        RxAppBuilder.CreateReactiveUIBuilder()
            .WithMainThreadScheduler(scheduler)
            .WithConverters()
            .WithRegistration(locator =>
            {
                locator.RegisterConstant(new GodotActivationFetcher(), typeof(IActivationForViewFetcher));
                locator.RegisterConstant(new GodotPropertyBinder(), typeof(ICreatesObservableForProperty));
                locator.RegisterConstant(new GodotCommandBinder(), typeof(ICreatesCommandBinding));
            })
            .WithCoreServices()
            .BuildApp();
    }
}

internal static class RxAppBuilderExtensions
{
    extension(IReactiveUIBuilder builder)
    {
        public IReactiveUIBuilder WithConverters()
        {
            return builder
                .WithConverter(new FloatToDoubleConverter())
                .WithConverter(new DoubleToFloatConverter());
        }
    }
}
