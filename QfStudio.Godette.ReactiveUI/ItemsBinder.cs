using System.Collections.Specialized;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using ReactiveUI;

namespace QfStudio.Godette.ReactiveUI;

/// <summary>
/// Binds an <see cref="ObservableCollection{TViewModel}"/> to a Godot container by creating and
/// managing child nodes via <see cref="ItemBuilder"/>.
///
/// Usage: call <see cref="Connect"/> in <c>WhenActivated</c> and dispose the result with the activation composite.
/// Disposing removes all nodes created by this binder. Other nodes already in the container are not affected.
/// </summary>
public class ItemsBinder<TContainerNode, TNode, TViewModel>
    where TContainerNode : Godot.Node
    where TNode : Godot.Node
    where TViewModel : class
{
    private CompositeDisposable? _disposable;
    private readonly Dictionary<TNode, TViewModel> _nodeToViewModel = new();
    private readonly Dictionary<TViewModel, TNode> _viewModelToNode = new();

    public ItemsBinder(Func<TNode> itemBuilder) { ItemBuilder = itemBuilder; }

    public bool IsConnected { get; protected set; }
    public Func<TNode> ItemBuilder { get; protected set; }

    public IDisposable Connect(TContainerNode container, IEnumerable<TViewModel> collection)
    {
        if (IsConnected) throw new InvalidOperationException("Already connected");

        _disposable = new CompositeDisposable();
        IsConnected = true;
        Disposable.Create(() => IsConnected = false).DisposeWith(_disposable);

        AddInitialItems(container, collection);

        if (collection is INotifyCollectionChanged notifyCollection)
        {
            NotifyCollectionChangedEventHandler handler = (s, e) => OnCollectionChanged(container, e);
            notifyCollection.CollectionChanged += handler;
            Disposable.Create(() => notifyCollection.CollectionChanged -= handler)
                .DisposeWith(_disposable);
        }

        // Remove all managed nodes on dispose.
        Disposable.Create(() => HandleReset(container)).DisposeWith(_disposable);

        return _disposable;
    }

    protected virtual void AddInitialItems(TContainerNode container, IEnumerable<TViewModel> items)
    {
        foreach (var item in items)
            AddNode(container, item);
    }

    protected virtual void OnCollectionChanged(TContainerNode container, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                HandleAdd(container, e);
                break;
            case NotifyCollectionChangedAction.Remove:
                HandleRemove(container, e);
                break;
            case NotifyCollectionChangedAction.Replace:
                HandleReplace(container, e);
                break;
            case NotifyCollectionChangedAction.Move:
                HandleMove(container, e);
                break;
            case NotifyCollectionChangedAction.Reset:
                HandleReset(container);
                break;
        }
    }

    protected virtual void HandleAdd(TContainerNode container, NotifyCollectionChangedEventArgs e)
    {
        var idx = e.NewStartingIndex;
        foreach (var item in e.NewItems!)
            AddNode(container, (TViewModel)item, idx++);
    }

    protected virtual void HandleRemove(TContainerNode container, NotifyCollectionChangedEventArgs e)
    {
        foreach (var item in e.OldItems!)
        {
            if (_viewModelToNode.TryGetValue((TViewModel)item, out var node))
                RemoveNode(container, node);
        }
    }

    protected virtual void HandleReplace(TContainerNode container, NotifyCollectionChangedEventArgs e)
    {
        var node = (TNode)container.GetChildren()[e.OldStartingIndex];
        var newViewModel = (TViewModel)e.NewItems![0]!;
        var oldViewModel = _nodeToViewModel[node];
        _nodeToViewModel[node] = newViewModel;
        _viewModelToNode.Remove(oldViewModel);
        _viewModelToNode[newViewModel] = node;
        ApplyViewModel(node, newViewModel);
    }

    protected virtual void HandleMove(TContainerNode container, NotifyCollectionChangedEventArgs e)
    {
        var node = (TNode)container.GetChildren()[e.OldStartingIndex];
        container.MoveChild(node, e.NewStartingIndex);
    }

    protected virtual void HandleReset(TContainerNode container)
    {
        foreach (var node in _nodeToViewModel.Keys.ToList())
        {
            container.RemoveChild(node);
            node.QueueFree();
        }
        _nodeToViewModel.Clear();
        _viewModelToNode.Clear();
    }

    protected virtual void AddNode(TContainerNode container, TViewModel viewModel, int index = -1)
    {
        var node = ItemBuilder();
        ApplyViewModel(node, viewModel);
        _nodeToViewModel[node] = viewModel;
        _viewModelToNode[viewModel] = node;
        container.AddChild(node);
        if (index >= 0) container.MoveChild(node, index);
    }

    protected virtual void RemoveNode(TContainerNode container, TNode node)
    {
        if (_nodeToViewModel.Remove(node, out var viewModel))
        {
            _viewModelToNode.Remove(viewModel);
        }
        container.RemoveChild(node);
        node.QueueFree();
    }

    protected virtual TNode? GetNode(TViewModel viewModel)
    {
        return _viewModelToNode.GetValueOrDefault(viewModel);
    }

    protected virtual TViewModel? GetViewModel(TNode node)
    {
        return _nodeToViewModel.GetValueOrDefault(node);
    }

    protected virtual void ApplyViewModel(TNode node, TViewModel viewModel)
    {
        if (node is IViewFor<TViewModel> viewFor)
        {
            viewFor.ViewModel = viewModel;
        }
        else
        {
            throw new InvalidOperationException(
                $"Node of type '{node.GetType().Name}' does not implement IViewFor<{typeof(TViewModel).Name}>. " +
                $"Override ApplyViewModel to provide custom logic.");
        }
    }
}
