using System.Reactive.Disposables;

namespace QfStudio.Godette.ReactiveUI.Operators;

internal sealed class ReturnFrame<T>(T value, int dueFrameCount, GodotFrameScheduler scheduler) : IObservable<T>
{
    public IDisposable Subscribe(IObserver<T> observer)
    {
        if (dueFrameCount <= 0)
        {
            observer.OnNext(value);
            observer.OnCompleted();
            return Disposable.Empty;
        }

        var sink = new Sink(observer, value, dueFrameCount);
        scheduler.Schedule(sink);
        return sink;
    }

    private sealed class Sink(IObserver<T> observer, T value, int dueFrameCount) : FrameSink<T>(observer)
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
