using System.Collections.Concurrent;

namespace QfStudio.Godette.ReactiveUI.Operators;

internal sealed class ChunkFrame<T>(IObservable<T> source, uint frameCount, GodotFrameScheduler scheduler) : IObservable<IList<T>>
{
    public IDisposable Subscribe(IObserver<IList<T>> observer)
    {
        var sink = new Sink(observer, source, frameCount);
        scheduler.Schedule(sink);
        return sink;
    }

    private sealed class Sink : FrameSink<T, IList<T>>
    {
        private readonly ConcurrentQueue<T> _inbox = new();
        private readonly List<T> _batch = [];
        private readonly uint _frameCount;
        private int _currentFrame;

        public Sink(IObserver<IList<T>> observer, IObservable<T> source, uint frameCount)
            : base(observer)
        {
            _frameCount = frameCount;
            ConnectSource(source);
        }

        public override void OnNext(T value) => _inbox.Enqueue(value);

        /// <remarks>
        /// Align with Rx .NET Buffer behavior:
        /// When an error occurs in the upstream, discard all buffered data and forward the error to downstream.
        /// </remarks>
        protected override bool MoveNextCore(double delta)
        {
            if (TerminalError is not null)
            {
                Error(TerminalError);
                return false;
            }

            while (_inbox.TryDequeue(out var item))
                _batch.Add(item);

            if (++_currentFrame >= _frameCount)
            {
                if (_batch.Count > 0)
                {
                    EmitNext(_batch.ToArray());
                    _batch.Clear();
                }
                _currentFrame = 0;
            }

            if (IsUpstreamTerminated && _batch.Count == 0)
            {
                Complete();
                return false;
            }
            return true;
        }
    }
}
