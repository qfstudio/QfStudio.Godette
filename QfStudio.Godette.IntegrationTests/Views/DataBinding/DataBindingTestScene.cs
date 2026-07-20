using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using Godot;
using QfStudio.Godette.IntegrationTests.ViewModels.DataBinding;
using QfStudio.Godette.ReactiveUI;
using ReactiveUI;

namespace QfStudio.Godette.IntegrationTests.Views.DataBinding;

[SceneTree(root: "_root")]
[GodotViewFor<DataBindingTestViewModel>]
public partial class DataBindingTestScene : Control
{
    public DataBindingTestScene()
    {
        this.WhenActivated(d =>
        {
            GD.Print("[Activation] activated");

            Disposable.Create(() => GD.Print("[Deactivation] deactivated"))
                .DisposeWith(d);

            WireTextTests(d);
            WireNumericTests(d);
            WireBooleanTests(d);
            WireSelectionTests(d);
            WireColorTests(d);
            WireConverterTests(d);
        });
    }

    private void WireTextTests(CompositeDisposable d)
    {
        this.WhenAnyValue(x => x.ViewModel!.Name)
            .Subscribe(name => GD.Print($"[WhenAnyValue] Name = {name}"))
            .DisposeWith(d);

        this.WhenAnyValue(x => x.ViewModel!.Name, x => x.ViewModel!.Notes)
            .ObserveOn(RxSchedulers.MainThreadScheduler)
            .Subscribe(tuple =>
            {
                var (name, note) = tuple;
                DerivedLabel.Text = string.IsNullOrEmpty(note) ? $"{name} has no note." : $"{name} has some notes.";
            })
            .DisposeWith(d);

        this.OneWayBind(ViewModel, vm => vm.Greeting, v => v.GreetingLabel.Text)
            .DisposeWith(d);

        this.OneWayBind(ViewModel, vm => vm.Notes, v => v.NotesLabel.Text,
                s => $"Notes: {(string.IsNullOrEmpty(s) ? "(empty)" : s)}")
            .DisposeWith(d);

        this.OneWayBind(ViewModel, vm => vm.Name, v => v.NameLabel.Text)
            .DisposeWith(d);

        this.Bind(ViewModel, vm => vm.Name, v => v.NameEdit.Text)
            .DisposeWith(d);

        this.Bind(ViewModel, vm => vm.Notes, v => v.NotesEdit.Text)
            .DisposeWith(d);
    }

    private void WireNumericTests(CompositeDisposable d)
    {
        this.Bind(ViewModel, vm => vm.Score, v => v.ScoreSlider.Value)
            .DisposeWith(d);

        this.OneWayBind(ViewModel, vm => vm.Score, v => v.ScoreLabel.Text,
                score => $"{score:F1}")
            .DisposeWith(d);
    }

    private void WireBooleanTests(CompositeDisposable d)
    {
        this.Bind(ViewModel, vm => vm.IsEnabled, v => v.EnableCheck.ButtonPressed)
            .DisposeWith(d);

        this.OneWayBind(ViewModel, vm => vm.IsEnabled, v => v.IsEnabledLabel.Text,
                enabled => enabled ? "true" : "false")
            .DisposeWith(d);
    }

    private void WireSelectionTests(CompositeDisposable d)
    {
        this.Bind(ViewModel, vm => vm.SelectedOption, v => v.OptionSelect.Selected)
            .DisposeWith(d);

        this.OneWayBind(ViewModel, vm => vm.SelectedOption, v => v.SelectedOptionLabel.Text,
                option => $"Option {(char)('A' + option)}")
            .DisposeWith(d);

        this.Bind(ViewModel, vm => vm.CurrentTab, v => v.TabBarSelect.CurrentTab)
            .DisposeWith(d);

        this.Bind(ViewModel, vm => vm.CurrentTab, v => v.MainTabs.CurrentTab)
            .DisposeWith(d);

        this.OneWayBind(ViewModel, vm => vm.CurrentTab, v => v.CurrentTabLabel.Text,
                tab => $"Tab {tab}")
            .DisposeWith(d);
    }

    private void WireColorTests(CompositeDisposable d)
    {
        this.Bind(ViewModel, vm => vm.SelectedColor, v => v.ColorSelect.Color)
            .DisposeWith(d);

        this.Bind(ViewModel, vm => vm.SelectedColor, v => v.ColorPicker.Color)
            .DisposeWith(d);
    }

    private void WireConverterTests(CompositeDisposable d)
    {
        this.Bind(ViewModel, vm => vm.Temperature, v => v.TempSlider.Value)
            .DisposeWith(d);

        this.OneWayBind(ViewModel, vm => vm.Temperature, v => v.TempLabel.Text,
                temp => $"Temp: {temp:F1}")
            .DisposeWith(d);
    }

    public override void _Ready()
    {
        ViewModel = new DataBindingTestViewModel();

        BackButton.Pressed += () => GetTree().ChangeSceneToFile(HomeScene.TscnFilePath);
    }
}
