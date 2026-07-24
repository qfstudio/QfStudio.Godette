namespace QfStudio.Godette.Tests.Operators;

public class ReturnFrameTests
{
    [Fact]
    public void EmitsValueAfterDueFrames()
    {
        var scheduler = new GodotFrameScheduler();
        var results = new List<string?>();

        Observable.ReturnFrame("hello", 3, scheduler)
            .Subscribe(results.Add);

        scheduler.AdvanceFrame(2);
        Assert.Empty(results);

        scheduler.AdvanceFrame(1);
        Assert.Single(results);
        Assert.Equal("hello", results[0]);
    }

    [Fact]
    public void DueFrameCount_0_TreatedAs1()
    {
        var scheduler = new GodotFrameScheduler();
        var results = new List<int>();

        Observable.ReturnFrame(42, 0, scheduler)
            .Subscribe(results.Add);

        scheduler.AdvanceFrame(1);
        Assert.Single(results);
        Assert.Equal(42, results[0]);
    }

    [Fact]
    public void Unsubscribe_BeforeDueFrame_NoEmission()
    {
        var scheduler = new GodotFrameScheduler();
        var results = new List<int>();

        var subscription = Observable.ReturnFrame(42, 3, scheduler)
            .Subscribe(results.Add);

        scheduler.AdvanceFrame(2);
        subscription.Dispose();

        scheduler.AdvanceFrame(3);
        Assert.Empty(results);
    }

    [Fact]
    public void ReferenceType_ValueEmittedCorrectly()
    {
        var scheduler = new GodotFrameScheduler();
        var value = new object();
        object? captured = null;

        Observable.ReturnFrame(value, 1, scheduler)
            .Subscribe(v => captured = v);

        scheduler.AdvanceFrame(1);
        Assert.Same(value, captured);
    }

    [Fact]
    public void NullValue_EmittedCorrectly()
    {
        var scheduler = new GodotFrameScheduler();
        string? captured = "not-null";

        Observable.ReturnFrame<string?>(null, 1, scheduler)
            .Subscribe(v => captured = v);

        scheduler.AdvanceFrame(1);
        Assert.Null(captured);
    }

    [Fact]
    public void ObserverOnNextThrows_SubscriptionTerminated()
    {
        var scheduler = new GodotFrameScheduler();
        var completed = false;

        Observable.ReturnFrame(42, 1, scheduler)
            .Subscribe(
                _ => throw new InvalidOperationException("test"),
                _ => { },
                () => completed = true);

        // OnNext throws → exception propagates; item deactivated internally
        var ex = Record.Exception(() => scheduler.AdvanceFrame(1));
        Assert.NotNull(ex);

        // OnNext threw, so the item is deactivated; OnCompleted should NOT fire
        scheduler.AdvanceFrame(3);
        Assert.False(completed);
    }
}
