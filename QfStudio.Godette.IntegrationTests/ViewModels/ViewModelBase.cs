using ReactiveUI;

namespace QfStudio.Godette.IntegrationTests.ViewModels;

public class ViewModelBase : ReactiveObject, IActivatableViewModel
{
    public ViewModelActivator Activator { get; } = new();
}
