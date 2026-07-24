using System.Reactive;

namespace QfStudio.Godette.ReactiveUI.Operators;

internal sealed class AfterFrame(uint dueFrameCount, GodotFrameScheduler scheduler) : IObservable<Unit>
{
    public IDisposable Subscribe(IObserver<Unit> observer)
    {
        var frameCount = dueFrameCount == 0 ? 1u : dueFrameCount;
        var sink = new Sink(observer, frameCount);
        scheduler.Schedule(sink);
        return sink;
    }

    private sealed class Sink(IObserver<Unit> observer, uint dueFrameCount) : FrameSink<Unit>(observer)
    {
        private int _currentFrame;

        protected override bool MoveNextCore(double delta)
        {
            if (++_currentFrame >= dueFrameCount)
            {
                EmitNext(Unit.Default);
                Complete();
                return false;
            }
            return true;
        }
    }
}
