using ReactiveUI;

namespace QfStudio.Godette.IntegrationTests.ViewModels.Routing;

public class ShellViewModel : ReactiveObject, IScreen
{
    public ShellViewModel()
    {
        HomeViewModel = new RoutableViewModelA(this);
        Router.NavigationStack.Add(HomeViewModel);
    }

    public RoutingState Router { get; } = new();

    public RoutableViewModelA HomeViewModel { get; }
}
