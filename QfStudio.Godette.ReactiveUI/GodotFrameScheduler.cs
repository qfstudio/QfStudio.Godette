using System.Collections.Concurrent;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;

namespace QfStudio.Godette.ReactiveUI;

public interface IFrameSchedulerWorkItem : IDisposable
{
    public bool MoveNext(double delta);
}

public abstract class WorkItemBase : IFrameSchedulerWorkItem
{
    private bool _isDisposed;

    public bool MoveNext(double delta)
    {
        if (_isDisposed)
            return false;
        return OnMoveNext(delta);
    }

    protected abstract bool OnMoveNext(double delta);

    protected virtual void Dispose(bool disposing)
    {
        if (_isDisposed)
            return;

        _isDisposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}

public class GodotFrameScheduler : IScheduler
{
    private readonly List<IFrameSchedulerWorkItem> _items = [];
    private readonly ConcurrentQueue<IFrameSchedulerWorkItem> _pendingAdds = [];
    private readonly Stack<int> _pendingRemoves = [];

    private double _now;

    public ulong FrameCount { get; private set; }

    public DateTimeOffset Now => new DateTimeOffset().AddSeconds(_now);

    public void NotifyProcess(double delta)
    {
        _now += delta;
        FrameCount++;

        while (_pendingAdds.TryDequeue(out var item))
        {
            _items.Add(item);
        }

        for (var i = 0; i < _items.Count; i++)
        {
            if (!_items[i].MoveNext(delta))
            {
                _pendingRemoves.Push(i);
            }
        }

        while (_pendingRemoves.TryPop(out var idx))
        {
            var tail = _items.Count - 1;
            _items[idx].Dispose();
            _items[idx] = _items[tail];
            _items.RemoveAt(tail);
        }
    }

    public IDisposable Schedule(IFrameSchedulerWorkItem item)
    {
        var disposable = Disposable.Create(item.Dispose);
        _pendingAdds.Enqueue(item);
        return disposable;
    }

    public IDisposable Schedule<TState>(TState state, Func<IScheduler, TState, IDisposable> action)
    {
        throw new NotImplementedException();
    }

    public IDisposable Schedule<TState>(TState state, TimeSpan dueTime, Func<IScheduler, TState, IDisposable> action)
    {
        throw new NotImplementedException();
    }

    public IDisposable Schedule<TState>(TState state, DateTimeOffset dueTime, Func<IScheduler, TState, IDisposable> action)
    {
        throw new NotImplementedException();
    }
}
