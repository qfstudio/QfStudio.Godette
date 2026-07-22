using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using Godot;
using QfStudio.Godette.ReactiveUI;
using ReactiveUI;
using Splat;

namespace QfStudio.Godette.IntegrationTests.Views.Routing;

public class RoutedViewController
{
    private readonly RoutingState _router;
    private readonly IViewLocator _viewLocator;

    private bool _isConnected;
    private Node? _currentView;
    private Node? _connectedNode;

    public RoutedViewController(RoutingState router, IViewLocator? viewLocator = null)
    {
        _router = router;
        _viewLocator ??= viewLocator ?? Locator.Current.GetService<GodotViewLocator>() ?? throw new InvalidOperationException($"Couldn't find {nameof(GodotViewLocator)}.");
    }

    public IDisposable Connect(Node node)
    {
        if (_isConnected)
            throw new InvalidOperationException("Can't connect twice.");

        _connectedNode = node;

        var disposable = new CompositeDisposable();

        _router.CurrentViewModel
            .ObserveOn(RxSchedulers.MainThreadScheduler)
            .Subscribe(ResolveAndSwapChild)
            .DisposeWith(disposable);

        Disposable.Create(() =>
        {
            _isConnected = false;
        }).DisposeWith(disposable);

        return disposable;
    }

    private void ResolveAndSwapChild(IRoutableViewModel? viewModel)
    {
        if (viewModel is null)
            return;

        var view = _viewLocator.ResolveView(viewModel)! ?? throw new InvalidOperationException($"Couldn't find view for '{viewModel.GetType().FullName}'.");
        view.ViewModel = viewModel;

        if (_currentView != null)
        {
            _connectedNode?.RemoveChild(_currentView);
            _currentView.QueueFree();
        }

        _currentView = (Node)view;
        _connectedNode?.AddChild(_currentView);
    }
}
