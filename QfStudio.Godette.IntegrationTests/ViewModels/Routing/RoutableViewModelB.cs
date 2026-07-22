using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace QfStudio.Godette.IntegrationTests.ViewModels.Routing;

public partial class RoutableViewModelB : ViewModelBase, IRoutableViewModel
{
    public RoutableViewModelB(IScreen hostScreen)
    {
        HostScreen = hostScreen;
    }

    public string? UrlPathSegment => "Page B";

    public IScreen HostScreen { get; }

    [Reactive]
    public partial int ClickCount { get; set; }
}
