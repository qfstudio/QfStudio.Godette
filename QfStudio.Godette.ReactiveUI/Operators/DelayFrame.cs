using System.Collections.Concurrent;

namespace QfStudio.Godette.ReactiveUI.Operators;

internal sealed class DelayFrame<T>(IObservable<T> source, int frameCount, GodotFrameScheduler scheduler) : IObservable<T>
{
    public IDisposable Subscribe(IObserver<T> observer)
    {
        var sink = new Sink(observer, source, frameCount, scheduler);
        scheduler.Schedule(sink);
        return sink;
    }

    private sealed class Sink : FrameSink<T, T>
    {
        private readonly ConcurrentQueue<(ulong TargetFrame, T Value)> _inbox = new();
        private readonly List<(ulong TargetFrame, T Value)> _pending = [];
        private readonly int _frameCount;
        private readonly GodotFrameScheduler _scheduler;

        public Sink(IObserver<T> observer, IObservable<T> source, int frameCount, GodotFrameScheduler scheduler)
            : base(observer)
        {
            _frameCount = frameCount;
            _scheduler = scheduler;
            ConnectSource(source);
        }

        public override void OnNext(T value)
        {
            var targetFrame = _scheduler.FrameCount + (ulong)_frameCount;
            _inbox.Enqueue((targetFrame, value));
        }

        protected override bool MoveNextCore(double delta)
        {
            if (TerminalError is not null)
            {
                Error(TerminalError);
                return false;
            }

            while (_inbox.TryDequeue(out var item))
                _pending.Add(item);

            var currentFrame = _scheduler.FrameCount;
            _pending.RemoveAll(item =>
            {
                if (item.TargetFrame <= currentFrame)
                {
                    EmitNext(item.Value);
                    return true;
                }
                return false;
            });

            if (IsUpstreamTerminated && _pending.Count == 0)
            {
                Complete();
                return false;
            }
            return true;
        }
    }
}
