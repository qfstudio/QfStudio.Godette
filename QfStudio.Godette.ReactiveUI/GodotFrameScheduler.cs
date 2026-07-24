using System.Collections.Concurrent;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;

namespace QfStudio.Godette.ReactiveUI;

public interface IFrameWorkItem : IDisposable
{
    /// <summary>Advances the work item by one frame.</summary>
    /// <returns><c>false</c> to deactivate and dispose this item.</returns>
    /// <remarks>Should not throw; any exception deactivates this item and skips the remaining items in the current frame.</remarks>
    public bool MoveNext(double delta);
}

public class GodotFrameScheduler : IScheduler
{
    private readonly List<IFrameWorkItem> _activeWorkItems = [];
    private readonly ConcurrentQueue<IFrameWorkItem> _pendingItemsToActivate = [];
    private readonly Stack<int> _pendingItemIndicesToDeactivate = [];
    private double _now;

    public ulong FrameCount { get; private set; }

    public DateTimeOffset Now => new DateTimeOffset().AddSeconds(_now);

    public void NotifyProcess(double delta)
    {
        _now += delta;
        FrameCount++;

        ProcessWorkItems(delta);
    }

    private void ProcessWorkItems(double delta)
    {
        ActivatePendingItems();

        try
        {
            ProcessActiveItems(delta);
        }
        finally
        {
            RemoveNonActiveItems();
        }
    }

    private void ActivatePendingItems()
    {
        while (_pendingItemsToActivate.TryDequeue(out var item))
        {
            _activeWorkItems.Add(item);
        }
    }

    private void ProcessActiveItems(double delta)
    {
        for (var i = 0; i < _activeWorkItems.Count; i++)
        {
            var isActive = false;

            try {
                isActive = _activeWorkItems[i].MoveNext(delta); // should not throw
            } finally {
                if (!isActive)
                {
                    _pendingItemIndicesToDeactivate.Push(i);
                }
            }
        }
    }

    private void RemoveNonActiveItems()
    {
        while (_pendingItemIndicesToDeactivate.TryPop(out var idx))
        {
            var tail = _activeWorkItems.Count - 1;
            try
            {
                _activeWorkItems[idx].Dispose(); // should not throw
            }
            finally
            {
                _activeWorkItems[idx] = _activeWorkItems[tail];
                _activeWorkItems.RemoveAt(tail);
            }
        }
    }

    /// <remarks>Thread-safe</remarks>
    public IDisposable Schedule(IFrameWorkItem item)
    {
        var disposable = Disposable.Create(item.Dispose);
        _pendingItemsToActivate.Enqueue(item);
        return disposable;
    }

    /// <inheritdoc/>
    /// <remarks>Thread-safe</remarks>
    public IDisposable Schedule<TState>(TState state, Func<IScheduler, TState, IDisposable> action)
    {
        var item = new OneShotWorkItem<TState>(this, state, action);
        _pendingItemsToActivate.Enqueue(item);
        return Disposable.Create(item.Dispose);
    }

    /// <inheritdoc/>
    /// <remarks>Thread-safe</remarks>
    public IDisposable Schedule<TState>(TState state, TimeSpan dueTime, Func<IScheduler, TState, IDisposable> action)
    {
        if (dueTime <= TimeSpan.Zero)
            return Schedule(state, action);

        var item = new DelayedWorkItem<TState>(this, state, action, dueTime.TotalSeconds);
        _pendingItemsToActivate.Enqueue(item);
        return Disposable.Create(item.Dispose);
    }

    /// <inheritdoc/>
    /// <remarks>Thread-safe</remarks>
    public IDisposable Schedule<TState>(TState state, DateTimeOffset dueTime, Func<IScheduler, TState, IDisposable> action)
    {
        return Schedule(state, dueTime - Now, action);
    }
}

public abstract class FrameWorkItemBase : IFrameWorkItem
{
    private bool _isDisposed;

    public bool MoveNext(double delta)
    {
        if (_isDisposed)
            return false;

        return MoveNextCore(delta);
    }
    
    /// <summary>Frame-advancement logic; should not throw. See <see cref="IFrameWorkItem.MoveNext"/></summary>
    protected abstract bool MoveNextCore(double delta);

    protected abstract void Dispose(bool disposing);

    public void Dispose()
    {
        if (Interlocked.CompareExchange(ref _isDisposed, true, false))
            return;

        Dispose(true);
        GC.SuppressFinalize(this);
    }
}

internal sealed class OneShotWorkItem<TState>(IScheduler scheduler, TState state, Func<IScheduler, TState, IDisposable> action) : FrameWorkItemBase
{
    private IDisposable? _disposable;

    protected override bool MoveNextCore(double delta)
    {
        _disposable = action(scheduler, state);
        return false;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            _disposable?.Dispose();
    }
}

internal sealed class DelayedWorkItem<TState>(GodotFrameScheduler scheduler, TState state, Func<IScheduler, TState, IDisposable> action, double delay)
    : FrameWorkItemBase
{
    private IDisposable? _disposable;
    private double _elapsed;

    protected override bool MoveNextCore(double delta)
    {
        _elapsed += delta;
        if (_elapsed < delay)
            return true;

        _disposable = action(scheduler, state);
        return false;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            _disposable?.Dispose();
    }
}
