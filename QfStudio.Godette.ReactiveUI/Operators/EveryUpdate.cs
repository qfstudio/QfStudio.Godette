using System.Reactive;

namespace QfStudio.Godette.ReactiveUI.Operators;

internal sealed class EveryUpdate(GodotFrameScheduler scheduler) : IObservable<Unit>
{
    public IDisposable Subscribe(IObserver<Unit> observer)
    {
        var sink = new Sink(observer);
        scheduler.Schedule(sink);
        return sink;
    }

    private sealed class Sink(IObserver<Unit> observer) : FrameSink<Unit>(observer)
    {
        protected override bool MoveNextCore(double delta)
        {
            EmitNext(Unit.Default);
            return true;
        }
    }
}
