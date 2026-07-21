using System.Reactive.Disposables.Fluent;
using Godot;
using QfStudio.Godette.IntegrationTests.ViewModels.ObservableBridge;
using QfStudio.Godette.ReactiveUI;
using ReactiveUI;

namespace QfStudio.Godette.IntegrationTests.Views.ObservableBridge;

[SceneTree(root: "_root")]
[GodotViewFor<ObservableBridgeTestViewModel>]
public partial class ObservableBridgeTestScene : Control
{
    public ObservableBridgeTestScene()
    {
        this.WhenActivated(d =>
        {
            ToggleButton.ObserveToggled()
                .Subscribe(toggled =>
                {
                    ViewModel!.ToggledValue = toggled;
                    GD.Print($"[ObservableBridge] Toggled = {toggled}");
                })
                .DisposeWith(d);

            this.OneWayBind(ViewModel, vm => vm.ToggledValue, v => v.ToggledLabel.Text,
                    v => $"Toggled: {v}")
                .DisposeWith(d);

            ValueSlider.ObserveValueChanged()
                .Subscribe(value =>
                {
                    ViewModel!.SliderValue = value;
                    GD.Print($"[ObservableBridge] SliderValue = {value}");
                })
                .DisposeWith(d);

            this.OneWayBind(ViewModel, vm => vm.SliderValue, v => v.SliderLabel.Text,
                    v => $"Slider: {v:F1}")
                .DisposeWith(d);

            GetTree().ObserveProcessFrame()
                .Subscribe(_ =>
                {
                    ViewModel!.FrameCount++;
                })
                .DisposeWith(d);

            this.OneWayBind(ViewModel, vm => vm.FrameCount, v => v.FrameLabel.Text,
                    v => $"Frames: {v}")
                .DisposeWith(d);
        });
    }

    public override void _Ready()
    {
        ViewModel = new ObservableBridgeTestViewModel();

        BackButton.Pressed += () => GetTree().ChangeSceneToFile(HomeScene.TscnFilePath);
    }
}
