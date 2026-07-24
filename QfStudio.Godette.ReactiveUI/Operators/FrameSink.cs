namespace QfStudio.Godette.ReactiveUI.Operators;

internal abstract class FrameSink<T>(IObserver<T> observer) : FrameWorkItemBase
{
    protected void EmitNext(T value) => observer.OnNext(value);

    protected void Complete()
    {
        observer.OnCompleted();
        Dispose();
    }

    protected void Error(Exception error)
    {
        observer.OnError(error);
        Dispose();
    }

    protected override void Dispose(bool disposing) { }
}

internal abstract class FrameSink<TSource, TResult>(IObserver<TResult> observer)
    : FrameSink<TResult>(observer), IObserver<TSource>
{
    private IDisposable? _subscription;
    private volatile bool _isUpstreamTerminated;
    private volatile Exception? _terminalError;

    protected bool IsUpstreamTerminated => _isUpstreamTerminated;
    protected Exception? TerminalError => _terminalError;

    /// <summary>
    /// Subscribe to the source observable. Must be called at the end of the derived constructor.
    /// </summary>
    protected void ConnectSource(IObservable<TSource> source)
    {
        _subscription = source.Subscribe(this);
    }

    public virtual void OnNext(TSource value) =>
        throw new NotSupportedException();
    public virtual void OnError(Exception error)
    {
        _isUpstreamTerminated = true;
        _terminalError = error;
    }
    public virtual void OnCompleted() => _isUpstreamTerminated = true;

    protected override void Dispose(bool disposing)
    {
        if (!disposing)
            return;

        _subscription?.Dispose();
    }
}
