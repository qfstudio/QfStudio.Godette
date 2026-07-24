using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using Godot;
using QfStudio.Godette.IntegrationTests.ViewModels.PollingBinding;
using QfStudio.Godette.ReactiveUI;
using ReactiveUI;

namespace QfStudio.Godette.IntegrationTests.Views.PollingBinding;

[SceneTree(root: "_root")]
[GodotViewFor<PollingBindingTestViewModel>]
public partial class PollingBindingTestScene : Control
{
    private readonly RandomNumberGenerator _rng = new();

    public PollingBindingTestScene()
    {
        this.WhenActivated(d =>
        {
            this.OneWayBind(ViewModel, vm => vm.DisplayName, v => v.NameLabel.Text)
                .DisposeWith(d);

            this.Bind(ViewModel, vm => vm.DisplayName, v => v.NameLabelPolling.Text)
                .DisposeWith(d);

            GetTree().CreateTimer(5.0).Timeout += () =>
            {
                NameLabelPolling.Text = "Externally Changed";
                GD.Print("[PollingBinding] External modification applied");
            };

            this.WhenAnyValue(v => v.NameLabelPolling.Text)
                .Subscribe(text =>
                {
                    GD.Print($"[PollingBinding] label text value: {text}");
                })
                .DisposeWith(d);

            this.WhenAnyValue(v => v.FreeIcon.Position)
                .BindTo(ViewModel, vm => vm.IconPosition)
                .DisposeWith(d);

            this.OneWayBind(ViewModel, vm => vm.IconPosition, v => v.IconPositionLabel.Text, pos => $"icon position: {pos}")
                .DisposeWith(d);

            Observable.PollEveryUpdate(this, v => v.FreeIcon.Position)
                .ThrottleFirstFrame(60)
                .Subscribe(pos =>
                {
                    GD.Print($"[PollEveryUpdate] {pos}");
                })
                .DisposeWith(d);
        });
    }

    public override void _Ready()
    {
        ViewModel = new PollingBindingTestViewModel();

        BackButton.Pressed += () => GetTree().ChangeSceneToFile("res://Views/HomeScene.tscn");
    }

    public override void _Process(double delta)
    {
        var pos = FreeIcon.Position;
        pos.X += _rng.RandfRange(-1.0f, 1.0f);
        pos.Y += _rng.RandfRange(-1.0f, 1.0f);
        pos.X = Mathf.Clamp(pos.X, 100f, 1500f);
        pos.Y = Mathf.Clamp(pos.Y, 100f, 800f);
        FreeIcon.Position = pos;
    }
}
