namespace QfStudio.Godette.Tests.Operators;

public class DelayFrameTests
{
    [Fact]
    public void DelaysValueByFrameCount()
    {
        var scheduler = new GodotFrameScheduler();
        var subject = new Subject<int>();
        var results = new List<int>();

        subject.DelayFrame(2, scheduler)
            .Subscribe(results.Add);

        subject.OnNext(1);

        scheduler.AdvanceFrame(1);
        Assert.Empty(results);

        scheduler.AdvanceFrame(1);
        Assert.Single(results);
        Assert.Equal(1, results[0]);
    }

    [Fact]
    public void MultipleValues_DelayedIndependently()
    {
        var scheduler = new GodotFrameScheduler();
        var subject = new Subject<int>();
        var results = new List<int>();

        subject.DelayFrame(2, scheduler)
            .Subscribe(results.Add);

        subject.OnNext(1);
        scheduler.AdvanceFrame(1); // Frame 1: nothing yet

        subject.OnNext(2);
        scheduler.AdvanceFrame(1); // Frame 2: value 1 ready

        Assert.Single(results);
        Assert.Equal(1, results[0]);

        scheduler.AdvanceFrame(1); // Frame 3: value 2 ready
        Assert.Equal(2, results.Count);
        Assert.Equal(2, results[1]);
    }

    [Fact]
    public void MultipleValuesSameFrame_SameTargetFrame()
    {
        var scheduler = new GodotFrameScheduler();
        var subject = new Subject<int>();
        var results = new List<int>();

        subject.DelayFrame(2, scheduler)
            .Subscribe(results.Add);

        subject.OnNext(1);
        subject.OnNext(2);

        scheduler.AdvanceFrame(2);
        Assert.Equal(2, results.Count);
        Assert.Equal(1, results[0]);
        Assert.Equal(2, results[1]);
    }

    [Fact]
    public void UpstreamCompletes_CompletesAfterPendingFlushed()
    {
        var scheduler = new GodotFrameScheduler();
        var subject = new Subject<int>();
        var results = new List<int>();
        var completed = false;

        subject.DelayFrame(2, scheduler)
            .Subscribe(
                v => results.Add(v),
                () => completed = true);

        subject.OnNext(1);
        subject.OnCompleted();

        // Not complete yet — pending value still in queue
        scheduler.AdvanceFrame(1);
        Assert.False(completed);

        // Value flushed at frame 2, then complete
        scheduler.AdvanceFrame(1);
        Assert.Single(results);
        Assert.Equal(1, results[0]);
        Assert.True(completed);
    }

    [Fact]
    public void UpstreamError_ForwardedImmediately()
    {
        var scheduler = new GodotFrameScheduler();
        var subject = new Subject<int>();
        var error = (Exception?)null;
        var completed = false;

        subject.DelayFrame(2, scheduler)
            .Subscribe(
                _ => { },
                ex => error = ex,
                () => completed = true);

        subject.OnNext(1);
        subject.OnError(new InvalidOperationException("test"));

        scheduler.AdvanceFrame(1);
        Assert.NotNull(error);
        Assert.IsType<InvalidOperationException>(error);
        Assert.Equal("test", error.Message);
        Assert.False(completed);
    }

    [Fact]
    public void Unsubscribe_NoMoreEmission()
    {
        var scheduler = new GodotFrameScheduler();
        var subject = new Subject<int>();
        var results = new List<int>();

        var subscription = subject.DelayFrame(2, scheduler)
            .Subscribe(results.Add);

        subject.OnNext(1);
        subscription.Dispose();

        scheduler.AdvanceFrame(3);
        Assert.Empty(results);
    }

    [Fact]
    public void FrameCount_0_EmptyUpstream_CompletesImmediately()
    {
        var scheduler = new GodotFrameScheduler();
        var subject = new Subject<int>();
        var completed = false;

        subject.DelayFrame(0, scheduler)
            .Subscribe(
                _ => { },
                () => completed = true);

        subject.OnCompleted();

        // delay=0: _pending is empty, IsUpstreamTerminated → Complete on first frame
        scheduler.AdvanceFrame(1);
        Assert.True(completed);
    }

    [Fact]
    public void ValueReceivedThenUpstreamCompletes_DelayedThenCompletes()
    {
        var scheduler = new GodotFrameScheduler();
        var subject = new Subject<int>();
        var results = new List<int>();
        var completed = false;

        subject.DelayFrame(3, scheduler)
            .Subscribe(
                v => results.Add(v),
                () => completed = true);

        subject.OnNext(42);
        subject.OnCompleted();

        // Frames 1-2: value pending
        scheduler.AdvanceFrame(2);
        Assert.Empty(results);
        Assert.False(completed);

        // Frame 3: value flushed + complete
        scheduler.AdvanceFrame(1);
        Assert.Single(results);
        Assert.Equal(42, results[0]);
        Assert.True(completed);
    }

    [Fact]
    public void ObserverOnNextThrows_SubscriptionTerminated()
    {
        var scheduler = new GodotFrameScheduler();
        var subject = new Subject<int>();
        var completed = false;

        subject.DelayFrame(1, scheduler)
            .Subscribe(
                _ => throw new InvalidOperationException("test"),
                _ => { },
                () => completed = true);

        subject.OnNext(1);

        // OnNext throws -> exception propagates; item deactivated internally
        var ex = Record.Exception(() => scheduler.AdvanceFrame(1));
        Assert.NotNull(ex);

        // OnNext threw, item deactivated, OnCompleted should NOT fire
        scheduler.AdvanceFrame(3);
        Assert.False(completed);
    }
}
