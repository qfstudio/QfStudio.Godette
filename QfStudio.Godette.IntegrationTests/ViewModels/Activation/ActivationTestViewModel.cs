using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using Godot;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace QfStudio.Godette.IntegrationTests.ViewModels.Activation;

public partial class ActivationTestViewModel : ViewModelBase
{
    [Reactive]
    public partial string ActivationState { get; set; } = "Not Activated";

    [Reactive]
    public partial int ActivateCount { get; set; }

    [Reactive]
    public partial int DeactivateCount { get; set; }

    public ActivationTestViewModel()
    {
        this.WhenActivated(d =>
        {
            GD.Print("[VM Activation] ViewModel WhenActivated fired");
            ActivateCount++;
            ActivationState = $"Activated ({ActivateCount})";

            Disposable.Create(() =>
            {
                GD.Print("[VM Deactivation] ViewModel WhenActivated disposed");
                DeactivateCount++;
                ActivationState = $"Deactivated ({DeactivateCount})";
            }).DisposeWith(d);
        });
    }
}
