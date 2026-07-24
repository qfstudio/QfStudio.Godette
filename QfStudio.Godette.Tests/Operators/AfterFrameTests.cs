using System.Reactive;

namespace QfStudio.Godette.Tests.Operators;

public class AfterFrameTests
{
    [Fact]
    public void EmitsAfterDueFrames()
    {
        var scheduler = new GodotFrameScheduler();
        var results = new List<Unit>();

        Observable.AfterFrame(3, scheduler)
            .Subscribe(results.Add);

        scheduler.AdvanceFrame(2);
        Assert.Empty(results);

        scheduler.AdvanceFrame(1);
        Assert.Single(results);
    }

    [Fact]
    public void DueFrameCount_0_TreatedAs1()
    {
        var scheduler = new GodotFrameScheduler();
        var results = new List<Unit>();

        Observable.AfterFrame(0, scheduler)
            .Subscribe(results.Add);

        // dueFrameCount=0 -> normalized to 1, emits after 1 frame
        scheduler.AdvanceFrame(1);
        Assert.Single(results);
    }

    [Fact]
    public void Unsubscribe_BeforeDueFrame_NoEmission()
    {
        var scheduler = new GodotFrameScheduler();
        var results = new List<Unit>();

        var subscription = Observable.AfterFrame(3, scheduler)
            .Subscribe(results.Add);

        scheduler.AdvanceFrame(2);
        subscription.Dispose();

        scheduler.AdvanceFrame(3);
        Assert.Empty(results);
    }

    [Fact]
    public void CompletesAfterEmission()
    {
        var scheduler = new GodotFrameScheduler();
        var completed = false;

        Observable.AfterFrame(2, scheduler)
            .Subscribe(_ => { }, () => completed = true);

        Assert.False(completed);
        scheduler.AdvanceFrame(2);
        Assert.True(completed);
    }

    [Fact]
    public void ObserverOnCompletedThrows_ItemStillDisposed()
    {
        var scheduler = new GodotFrameScheduler();

        var subscription = Observable.AfterFrame(1, scheduler)
            .Subscribe(
                _ => { },
                (_) => throw new InvalidOperationException("test"),
                () => { });

        // Trigger completion — observer.OnCompleted throws, but Dispose is still called
        scheduler.AdvanceFrame(1);

        // Verify item is deactivated by checking no further emissions
        // (a second AfterFrame subscription on the same scheduler should still work)
        var secondCompleted = false;
        Observable.AfterFrame(1, scheduler)
            .Subscribe(_ => { }, () => secondCompleted = true);

        scheduler.AdvanceFrame(1);
        Assert.True(secondCompleted);
    }
}
