namespace QfStudio.Godette.ReactiveUI.Operators;

/// <summary>
/// 帧首节流：每 N 帧窗口内只取第一个值，后续值被丢弃。
/// 适用于限制高频操作（路径重算、特效触发等）。
/// </summary>
internal sealed class ThrottleFirstFrame<T>(IObservable<T> source, int frameCount, GodotFrameScheduler scheduler) : IObservable<T>
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
        private readonly int _frameCount;
        private T? _value;
        private bool _hasValue;
        private int _elapsed;

        public Sink(IObserver<T> observer, IObservable<T> source, int frameCount)
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
                    _elapsed = 0;
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
                if (_hasValue)
                {
                    EmitNext(_value!);
                    _hasValue = false;
                }
                if (IsUpstreamTerminated)
                {
                    Complete();
                    return false;
                }
                if (++_elapsed >= _frameCount)
                    _elapsed = 0;
                return true;
            }
        }
    }
}
