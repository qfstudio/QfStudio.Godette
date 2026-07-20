using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using Godot;
using QfStudio.Godette.IntegrationTests.ViewModels.Activation;
using QfStudio.Godette.ReactiveUI;
using ReactiveUI;

namespace QfStudio.Godette.IntegrationTests.Views.Activation;

[SceneTree(root: "_root")]
[GodotViewFor<ActivationTestViewModel>]
public partial class ActivatableScene : Control
{
    public ActivatableScene()
    {
        this.WhenActivated(d =>
        {
            GD.Print("[View] Activated");
            Disposable.Create(() => GD.Print("[View] Deactivated")).DisposeWith(d);

            this.OneWayBind(ViewModel, vm => vm.ActivationState, v => v.StateLabel.Text)
                .DisposeWith(d);

            this.WhenAnyValue(x => x.ViewModel!.ActivateCount, x => x.ViewModel!.DeactivateCount)
                .Select(t => $"Activate: {t.Item1}, Deactivate: {t.Item2}")
                .ObserveOn(RxSchedulers.MainThreadScheduler)
                .Subscribe(text =>
                {
                    CountLabel.Text = text;
                })
                .DisposeWith(d);
        });
    }

    public ActivatableHostScene? Host { get; set; }

    public override void _Ready()
    {
        ViewModel = new ActivationTestViewModel();

        FreeButton.Pressed += () =>
        {
            Host!.RemoveInnerScene(this);
        };
        ReAttachButton.Pressed += () =>
        {
            Host!.DetachAndThenAttachSceneIn3Seconds(this);
        };
    }
}
