namespace QfStudio.Godette.ReactiveUI.Operators;

internal sealed class ReturnFrame<T>(T value, uint dueFrameCount, GodotFrameScheduler scheduler) : IObservable<T>
{
    public IDisposable Subscribe(IObserver<T> observer)
    {
        var frameCount = dueFrameCount == 0 ? 1u : dueFrameCount;
        var sink = new Sink(observer, value, frameCount);
        scheduler.Schedule(sink);
        return sink;
    }

    private sealed class Sink(IObserver<T> observer, T value, uint dueFrameCount) : FrameSink<T>(observer)
    {
        private int _currentFrame;

        protected override bool MoveNextCore(double delta)
        {
            if (++_currentFrame >= dueFrameCount)
            {
                EmitNext(value);
                Complete();
                return false;
            }

            return true;
        }
    }
}
