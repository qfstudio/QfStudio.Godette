using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace QfStudio.Godette.IntegrationTests.ViewModels.Routing;

public partial class RoutableViewModelA : ViewModelBase, IRoutableViewModel
{
    private static int _counter;

    public RoutableViewModelA(IScreen hostScreen)
    {
        Id = ++_counter;
        HostScreen = hostScreen;
    }

    public string? UrlPathSegment => "Page A";

    public IScreen HostScreen { get; }

    [Reactive]
    public partial int Id { get; set; }

    [Reactive]
    public partial string Message { get; set; } = $"Hello from Page A";
}
