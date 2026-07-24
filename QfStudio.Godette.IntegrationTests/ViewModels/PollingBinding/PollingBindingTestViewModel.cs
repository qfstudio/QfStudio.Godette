using Godot;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace QfStudio.Godette.IntegrationTests.ViewModels.PollingBinding;

public partial class PollingBindingTestViewModel : ReactiveObject
{
    [Reactive]
    public partial string DisplayName { get; set; } = "Hello";

    [Reactive]
    public partial Godot.Vector2 IconPosition { get; set; } = new();
}
