using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using Godot;
using QfStudio.Godette.IntegrationTests.ViewModels.Routing;
using QfStudio.Godette.ReactiveUI;
using ReactiveUI;

namespace QfStudio.Godette.IntegrationTests.Views.Routing;

[SceneTree(root: "_root")]
[GodotViewFor<ShellViewModel>]
public partial class RoutingDemoScene : Control
{
    public override void _Ready()
    {
        BackButton.Pressed += () => GetTree().ChangeSceneToFile(HomeScene.TscnFilePath);
    }

    public RoutingDemoScene()
    {
        var locator = new GodotViewLocator();
        locator.RegisterView<RoutedViewA, RoutableViewModelA>(RoutedViewA.TscnFilePath);
        locator.RegisterView<RoutableViewModelB>(RoutedViewB.TscnFilePath);

        ViewModel = new ShellViewModel();
        var routedViewController = new RoutedViewController(ViewModel.Router, locator);

        this.WhenActivated(d =>
        {
            ViewModel.Router.CurrentViewModel
                .ObserveOn(RxSchedulers.MainThreadScheduler)
                .Subscribe(vm => CurrentPageLabel.Text = vm is not null
                    ? $"Current: {vm.UrlPathSegment} (stack depth: {ViewModel.Router.NavigationStack.Count})"
                    : "Current: (none)")
                .DisposeWith(d);

            routedViewController.Connect(ContentContainer).DisposeWith(d);

            NavigateToAButton.ObservePressed()
                .Select(IRoutableViewModel (_) => new RoutableViewModelA(ViewModel))
                .InvokeCommand(ViewModel, vm => vm.Router.Navigate)
                .DisposeWith(d);

            NavigateToBButton.ObservePressed()
                .Select(IRoutableViewModel (_) => new RoutableViewModelB(ViewModel))
                .InvokeCommand(ViewModel, vm => vm.Router.Navigate)
                .DisposeWith(d);

            this.BindCommand(ViewModel, vm => vm.Router.NavigateBack, v => v.NavigateBackButton)
                .DisposeWith(d);

            this.BindCommand(ViewModel, vm => vm.Router.NavigateAndReset, v => v.NavigateAndResetButton,
                    Observable.Return(ViewModel.HomeViewModel))
                .DisposeWith(d);
        });
    }
}
