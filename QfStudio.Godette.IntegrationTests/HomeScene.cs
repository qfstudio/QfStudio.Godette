using System;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using Godot;
using QfStudio.Godette.ReactiveUI;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace QfStudio.Godette.IntegrationTests;

public partial class HomeViewModel : ReactiveObject
{
    [Reactive]
    public partial string Name { get; set; } = "World";

    [Reactive]
    public partial double Score { get; set; }
}

[SceneTree(root: "_root")]
public partial class HomeScene : ReactiveControl<HomeViewModel>
{
    public HomeScene()
    {
        this.WhenActivated(d =>
        {
            GD.Print("[Activation] activated");

            Disposable.Create(() => GD.Print("[Deactivation] deactivated"))
                .DisposeWith(d);

            // Test 1: WhenAnyValue — single property
            this.WhenAnyValue(x => x.ViewModel!.Name)
                .Subscribe(name => GD.Print($"[WhenAnyValue] Name = {name}"))
                .DisposeWith(d);

            // Test 2: this.OneWayBind — ViewModel.Name -> Label.Text (ReactiveUI standard)
            this.OneWayBind(ViewModel, vm => vm.Name, v => v.NameLabel.Text)
                .DisposeWith(d);

            // Test 3: this.Bind — ViewModel.Score <-> HSlider.Value (ReactiveUI standard, two-way)
            this.Bind(ViewModel, vm => vm.Score, v => v.ScoreSlider.Value)
                .DisposeWith(d);

            // Test 4: WhenAnyValue — derived from multiple properties
            this.WhenAnyValue(x => x.ViewModel!.Name, x => x.ViewModel!.Score)
                .Select(t => $"{t.Item1}: {t.Item2}")
                .ObserveOn(RxSchedulers.MainThreadScheduler)
                .Subscribe(text =>
                {
                    DerivedLabel.Text = text;
                    GD.Print($"[Derived] {text}");
                })
                .DisposeWith(d);

            // Test 5: ViewModel property change after 1s
            Observable.Timer(TimeSpan.FromSeconds(1))
                .ObserveOn(RxSchedulers.MainThreadScheduler)
                .Subscribe(_ =>
                {
                    GD.Print("[Test] Set Name = Alice");
                    ViewModel!.Name = "Alice";
                })
                .DisposeWith(d);

            // Test 6: ViewModel property change after 2s
            Observable.Timer(TimeSpan.FromSeconds(2))
                .ObserveOn(RxSchedulers.MainThreadScheduler)
                .Subscribe(_ =>
                {
                    GD.Print("[Test] Set Score = 42");
                    ViewModel!.Score = 42;
                })
                .DisposeWith(d);

            // Test 7: ViewModel replacement after 3s
            Observable.Timer(TimeSpan.FromSeconds(3))
                .ObserveOn(RxSchedulers.MainThreadScheduler)
                .Subscribe(_ =>
                {
                    GD.Print("[Test] Replace ViewModel");
                    ViewModel = new HomeViewModel { Name = "Replaced", Score = 99 };
                })
                .DisposeWith(d);
        });
    }

    public override void _Ready()
    {
        ViewModel = new HomeViewModel();
    }
}
