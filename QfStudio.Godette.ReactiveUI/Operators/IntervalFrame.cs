using System.Reactive;

namespace QfStudio.Godette.ReactiveUI.Operators;

internal sealed class IntervalFrame(int dueFrameCount, int periodFrameCount, GodotFrameScheduler scheduler) : IObservable<Unit>
{
    public IDisposable Subscribe(IObserver<Unit> observer)
    {
        var sink = new Sink(observer, dueFrameCount, periodFrameCount);
        scheduler.Schedule(sink);
        return sink;
    }

    private sealed class Sink(IObserver<Unit> observer, int dueFrameCount, int periodFrameCount) : FrameSink<Unit>(observer)
    {
        private int _currentFrame;
        private bool _isInPeriod;

        protected override bool MoveNextCore(double delta)
        {
            if (_isInPeriod)
            {
                if (++_currentFrame >= periodFrameCount)
                {
                    EmitNext(Unit.Default);
                    _currentFrame = 0;
                }
                return true;
            }

            if (++_currentFrame >= dueFrameCount)
            {
                EmitNext(Unit.Default);
                _currentFrame = 0;
                _isInPeriod = true;
            }
            return true;
        }
    }
}