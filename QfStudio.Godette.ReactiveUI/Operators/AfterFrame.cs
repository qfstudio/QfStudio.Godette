using System.Reactive;
using System.Reactive.Disposables;

namespace QfStudio.Godette.ReactiveUI.Operators;

internal sealed class AfterFrame(int dueFrameCount, GodotFrameScheduler scheduler) : IObservable<Unit>
{
    public IDisposable Subscribe(IObserver<Unit> observer)
    {
        if (dueFrameCount <= 0)
        {
            observer.OnNext(Unit.Default);
            observer.OnCompleted();
            return Disposable.Empty;
        }

        var sink = new Sink(observer, dueFrameCount);
        scheduler.Schedule(sink);
        return sink;
    }

    private sealed class Sink(IObserver<Unit> observer, int dueFrameCount) : FrameSink<Unit>(observer)
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
