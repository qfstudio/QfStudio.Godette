using System.Collections.Concurrent;

namespace QfStudio.Godette.ReactiveUI.Operators;

internal sealed class DelayFrame<T>(IObservable<T> source, uint frameCount, GodotFrameScheduler scheduler) : IObservable<T>
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
        private readonly Queue<(ulong TargetFrame, T Value)> _pending = new();
        private readonly uint _frameCount;
        private readonly GodotFrameScheduler _scheduler;

        public Sink(IObserver<T> observer, IObservable<T> source, uint frameCount, GodotFrameScheduler scheduler)
            : base(observer)
        {
            _frameCount = frameCount;
            _scheduler = scheduler;
            ConnectSource(source);
        }

        public override void OnNext(T value)
        {
            var targetFrame = _scheduler.FrameCount + _frameCount;
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
                _pending.Enqueue(item);

            var currentFrame = _scheduler.FrameCount;
            while (_pending.Count > 0 && _pending.Peek().TargetFrame <= currentFrame)
                EmitNext(_pending.Dequeue().Value);

            if (IsUpstreamTerminated && _pending.Count == 0)
            {
                Complete();
                return false;
            }

            return true;
        }
    }
}
