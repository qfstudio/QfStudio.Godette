namespace QfStudio.Godette.ReactiveUI.Operators;

internal abstract class FrameSink<T>(IObserver<T> observer) : FrameWorkItemBase
{
    protected void EmitNext(T value) => observer.OnNext(value);

    protected void Complete()
    {
        try
        {
            observer.OnCompleted();
        }
        finally
        {
            Dispose();
        }
    }

    protected void Error(Exception error)
    {
        try
        {
            observer.OnError(error);
        }
        finally
        {
            Dispose();
        }
    }

    protected override void Dispose(bool disposing) { }
}

internal abstract class FrameSink<TSource, TResult>(IObserver<TResult> observer)
    : FrameSink<TResult>(observer), IObserver<TSource>
{
    private abstract class UpstreamResult
    {
        public static readonly UpstreamResult None = new NoneType();
        public static readonly UpstreamResult Completed = new CompletedType();

        public sealed class Errored(Exception error) : UpstreamResult
        {
            public Exception Error { get; } = error;
        }
        public sealed class NoneType : UpstreamResult;
        public sealed class CompletedType : UpstreamResult;
    }

    private IDisposable? _subscription;
    private volatile UpstreamResult _upstreamResult = UpstreamResult.None;

    protected bool IsUpstreamTerminated => _upstreamResult is not UpstreamResult.NoneType;
    protected Exception? TerminalError => _upstreamResult is UpstreamResult.Errored e ? e.Error : null;

    protected void ConnectSource(IObservable<TSource> source)
    {
        _subscription = source.Subscribe(this);
    }

    public virtual void OnNext(TSource value) =>
        throw new NotSupportedException();

    public virtual void OnError(Exception error) => _upstreamResult = new UpstreamResult.Errored(error);

    public virtual void OnCompleted() => _upstreamResult = UpstreamResult.Completed;

    protected override void Dispose(bool disposing)
    {
        if (!disposing)
            return;

        _subscription?.Dispose();
    }
}
