namespace QfStudio.Godette.ReactiveUI.Operators;

/// <remarks>
/// Equivalent to <c>Throttle</c> in Rx.NET or <c>debounce</c> in RxJS.
/// See <see cref="FrameObservableExtensions.DebounceFrame"/>.
/// </remarks>
internal sealed class DebounceFrame<T>(IObservable<T> source, uint frameCount, GodotFrameScheduler scheduler) : IObservable<T>
{
    public IDisposable Subscribe(IObserver<T> observer)
    {
        var sink = new Sink(observer, source, frameCount);
        scheduler.Schedule(sink);
        return sink;
    }

    private sealed class Sink : FrameSink<T, T>
    {
        private readonly Lock _gate = new();
        private readonly uint _frameCount;

        private T? _latestValue;
        private bool _hasValue;
        private int _currentFrame;

        public Sink(IObserver<T> observer, IObservable<T> source, uint frameCount)
            : base(observer)
        {
            _frameCount = frameCount;
            ConnectSource(source);
        }

        public override void OnNext(T value)
        {
            lock (_gate)
            {
                _latestValue = value;
                _hasValue = true;
                _currentFrame = 0;
            }
        }

        protected override bool MoveNextCore(double delta)
        {
            lock (_gate)
            {
                if (TerminalError is not null)
                {
                    Error(TerminalError);
                    return false;
                }

                if (IsUpstreamTerminated)
                {
                    // On upstream completion, flush the latest pending value before completing
                    // (matches Rx.NET Throttle and RxJS debounceTime semantics).
                    if (_hasValue)
                        EmitNext(_latestValue!);

                    Complete();
                    return false;
                }

                if (_hasValue && ++_currentFrame >= _frameCount)
                {
                    EmitNext(_latestValue!);
                    _hasValue = false;
                }

                return true;
            }
        }
    }
}
