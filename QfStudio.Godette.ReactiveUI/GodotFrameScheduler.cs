using System.Collections.Concurrent;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;

namespace QfStudio.Godette.ReactiveUI;

public interface IFrameSchedulerWorkItem : IDisposable
{
    public bool MoveNext(double delta);
}

public class GodotFrameScheduler : IScheduler
{
    private readonly List<IFrameSchedulerWorkItem> _activeWorkItems = [];
    private readonly ConcurrentQueue<IFrameSchedulerWorkItem> _pendingItemsToActivate = [];
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
        while (_pendingItemsToActivate.TryDequeue(out var item))
        {
            _activeWorkItems.Add(item);
        }

        for (var i = 0; i < _activeWorkItems.Count; i++)
        {
            if (!_activeWorkItems[i].MoveNext(delta))
            {
                _pendingItemIndicesToDeactivate.Push(i);
            }
        }

        while (_pendingItemIndicesToDeactivate.TryPop(out var idx))
        {
            var tail = _activeWorkItems.Count - 1;
            try
            {
                _activeWorkItems[idx].Dispose();
            }
            finally
            {
                _activeWorkItems[idx] = _activeWorkItems[tail];
                _activeWorkItems.RemoveAt(tail);
            }
        }
    }

    /// <remarks>Thread-safe</remarks>
    public IDisposable Schedule(IFrameSchedulerWorkItem item)
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

public abstract class WorkItemBase : IFrameSchedulerWorkItem
{
    private bool _isDisposed;

    public bool MoveNext(double delta)
    {
        if (_isDisposed)
            return false;

        return MoveNextCore(delta);
    }

    protected abstract bool MoveNextCore(double delta);
    protected abstract void Dispose(bool disposing);

    public void Dispose()
    {
        if (_isDisposed)
            return;

        Dispose(true);
        _isDisposed = true;
        GC.SuppressFinalize(this);
    }
}

internal sealed class OneShotWorkItem<TState>(IScheduler scheduler, TState state, Func<IScheduler, TState, IDisposable> action) : WorkItemBase
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
    : WorkItemBase
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
