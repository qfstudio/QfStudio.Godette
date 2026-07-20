using System.Reactive.Linq;
using Godot;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace QfStudio.Godette.IntegrationTests.ViewModels.DataBinding;

public partial class DataBindingTestViewModel : ReactiveObject
{
    private readonly ObservableAsPropertyHelper<string> _greetingHelper;

    [Reactive]
    public partial string Name { get; set; } = "World";

    [Reactive]
    public partial string Notes { get; set; } = "";

    [Reactive]
    public partial double Score { get; set; }

    [Reactive]
    public partial float Temperature { get; set; } = 36.5f;

    [Reactive]
    public partial bool IsEnabled { get; set; } = true;

    [Reactive]
    public partial int SelectedOption { get; set; }

    [Reactive]
    public partial Color SelectedColor { get; set; } = Godot.Colors.White;

    [Reactive]
    public partial int CurrentTab { get; set; }

    public string Greeting => _greetingHelper.Value;

    public DataBindingTestViewModel()
    {
        _greetingHelper = this.WhenAnyValue(x => x.Name)
            .Select(n => $"Hello, {n}!")
            .ToProperty(this, x => x.Greeting);
    }
}
