namespace QfStudio.Godette.ReactiveUI.Operators;

/// <summary>
/// 帧防抖：上游静默 N 帧后才推送最新值。
/// 适用于搜索框输入防抖、按钮连点防护等场景。
/// </summary>
internal sealed class DebounceFrame<T>(IObservable<T> source, int frameCount, GodotFrameScheduler scheduler) : IObservable<T>
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
        private T? _latestValue;
        private bool _hasValue;
        private int _currentFrame;

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
                if (_hasValue && ++_currentFrame >= _frameCount)
                {
                    EmitNext(_latestValue!);
                    _hasValue = false;
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
