using System.Threading;
using Godot;
using QfStudio.Godette.ReactiveUI;
using ReactiveUI;
using ReactiveUI.Builder;
using Splat;

namespace QfStudio.Godette.IntegrationTests.Autoloads;

public partial class RxAppBootstrapper : Godot.Node
{
    /// <remarks>
    /// <seealso href="https://www.reactiveui.net/documentation/upgrading/rxappbuilder-migration/" />
    /// </remarks>
    public RxAppBootstrapper()
    {
        RxAppBuilder.CreateReactiveUIBuilder()
            .WithMainThreadScheduler(GodotMainThreadScheduler.Create(SynchronizationContext.Current!))
            .WithConverter(new FloatToDoubleConverter())
            .WithConverter(new DoubleToFloatConverter())
            .WithConverter(new VariantToIntConverter())
            .WithConverter(new VariantToFloatConverter())
            .WithConverter(new VariantToDoubleConverter())
            .WithConverter(new VariantToStringConverter())
            .WithConverter(new VariantToBoolConverter())
            .WithConverter(new IntToVariantConverter())
            .WithConverter(new FloatToVariantConverter())
            .WithConverter(new DoubleToVariantConverter())
            .WithConverter(new StringToVariantConverter())
            .WithConverter(new BoolToVariantConverter())
            .WithRegistration(locator =>
            {
                locator.RegisterConstant(new GodotActivationFetcher(), typeof(IActivationForViewFetcher));
                locator.RegisterConstant(new GodotPropertyBinder(), typeof(ICreatesObservableForProperty));
                locator.RegisterConstant(new GodotCommandBinder(), typeof(ICreatesCommandBinding));
            })
            .WithCoreServices()
            .BuildApp();

        Callable.From(QueueFree).CallDeferred();
    }
}
