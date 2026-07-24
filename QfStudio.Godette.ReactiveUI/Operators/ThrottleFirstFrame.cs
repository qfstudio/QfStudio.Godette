namespace QfStudio.Godette.ReactiveUI.Operators;

internal sealed class ThrottleFirstFrame<T>(IObservable<T> source, uint frameCount, GodotFrameScheduler scheduler) : IObservable<T>
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

        private T? _value;
        private bool _hasValue;
        private int _elapsed;

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
                if (!_hasValue)
                {
                    _value = value;
                    _hasValue = true;
                }
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

                if (++_elapsed >= _frameCount)
                {
                    _elapsed = 0;
                    if (_hasValue)
                    {
                        EmitNext(_value!);
                        _hasValue = false;
                    }
                }

                if (IsUpstreamTerminated)
                {
                    Complete();
                    return false;
                }

                return true;
            }
        }
    }
}
