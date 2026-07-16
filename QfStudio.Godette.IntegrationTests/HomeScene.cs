using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using Godot;
using QfStudio.Godette.ReactiveUI;
using ReactiveUI;
using System;
using System.Reactive.Linq;
using ReactiveUI.SourceGenerators;

namespace QfStudio.Godette.IntegrationTests;

public partial class HomeViewModel : ReactiveObject
{
    [Reactive]
    public partial string Name { get; set; } = string.Empty;

    [Reactive]
    public partial double Score { get; set; }
}

public partial class HomeScene : ReactiveControl<HomeViewModel>
{
    private Label? _label;
    private HSlider? _slider;

    public HomeScene()
    {
        this.ViewModel = null;

        this.WhenActivated(d =>
        {
            GD.Print("[Activation] HomeScene activated");
            GD.Print($"ViewModel is null: {ViewModel is null}");

            Disposable.Create(() =>
            {
                GD.Print("[Deactivation] HomeScene deactivated");
            }).DisposeWith(d);

            this.WhenAnyValue(x => x.ViewModel!.Name)
                .Subscribe(x =>
                {
                    GD.Print(x);
                })
                .DisposeWith(d);

            Observable.Timer(TimeSpan.FromSeconds(3))
                .Subscribe(_ =>
                {
                    GD.Print("hook!");
                    this.ViewModel = new HomeViewModel()
                    {
                        Name = "ta ta ka!"
                    };
                })
                .DisposeWith(d);
        });
    }

    public override void _Ready()
    {
    }
}
