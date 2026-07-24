namespace QfStudio.Godette.Tests.Operators;

public class DebounceFrameTests
{
    [Fact]
    public void EmitsAfterQuietPeriod()
    {
        var scheduler = new GodotFrameScheduler();
        var subject = new Subject<int>();
        var results = new List<int>();

        subject.DebounceFrame(3, scheduler)
            .Subscribe(results.Add);

        subject.OnNext(1);

        // 2 frames of quiet: not enough
        scheduler.AdvanceFrame(2);
        Assert.Empty(results);

        // 3rd frame: quiet period expired
        scheduler.AdvanceFrame(1);
        Assert.Single(results);
        Assert.Equal(1, results[0]);
    }

    [Fact]
    public void NewValueResetsCountdown()
    {
        var scheduler = new GodotFrameScheduler();
        var subject = new Subject<int>();
        var results = new List<int>();

        subject.DebounceFrame(3, scheduler)
            .Subscribe(results.Add);

        subject.OnNext(1);
        scheduler.AdvanceFrame(2); // 2 frames quiet

        subject.OnNext(2); // Reset counter
        scheduler.AdvanceFrame(2); // 2 frames quiet since reset
        Assert.Empty(results);

        scheduler.AdvanceFrame(1); // 3rd frame since reset
        Assert.Single(results);
        Assert.Equal(2, results[0]);
    }

    [Fact]
    public void MultipleValuesInBurst_OnlyLastEmitted()
    {
        var scheduler = new GodotFrameScheduler();
        var subject = new Subject<int>();
        var results = new List<int>();

        subject.DebounceFrame(3, scheduler)
            .Subscribe(results.Add);

        subject.OnNext(1);
        subject.OnNext(2);
        subject.OnNext(3);

        scheduler.AdvanceFrame(3);
        Assert.Single(results);
        Assert.Equal(3, results[0]);
    }

    [Fact]
    public void UpstreamError_ForwardedImmediately()
    {
        var scheduler = new GodotFrameScheduler();
        var subject = new Subject<int>();
        Exception? error = null;

        subject.DebounceFrame(3, scheduler)
            .Subscribe(
                _ => { },
                ex => error = ex);

        subject.OnNext(1);
        subject.OnError(new InvalidOperationException("test"));

        scheduler.AdvanceFrame(1);
        Assert.NotNull(error);
        Assert.IsType<InvalidOperationException>(error);
    }

    [Fact]
    public void UpstreamCompletes_WindowExpired_EmitsThenCompletes()
    {
        var scheduler = new GodotFrameScheduler();
        var subject = new Subject<int>();
        var results = new List<int>();
        var completed = false;

        subject.DebounceFrame(2, scheduler)
            .Subscribe(
                v => results.Add(v),
                () => completed = true);

        subject.OnNext(1);

        // Wait for window to expire
        scheduler.AdvanceFrame(2);
        Assert.Single(results);
        Assert.Equal(1, results[0]);

        subject.OnCompleted();
        scheduler.AdvanceFrame(1);
        Assert.True(completed);
    }

    [Fact]
    public void UpstreamCompletes_WindowNotExpired_FlushesThenCompletes()
    {
        var scheduler = new GodotFrameScheduler();
        var subject = new Subject<int>();
        var results = new List<int>();
        var completed = false;

        subject.DebounceFrame(3, scheduler)
            .Subscribe(
                v => results.Add(v),
                () => completed = true);

        subject.OnNext(1);

        // Window not yet expired (only 1 frame quiet)
        scheduler.AdvanceFrame(1);
        Assert.Empty(results);

        subject.OnCompleted();

        // Upstream completed: flush pending value + complete
        scheduler.AdvanceFrame(1);
        Assert.Single(results);
        Assert.Equal(1, results[0]);
        Assert.True(completed);
    }

    [Fact]
    public void UpstreamCompletes_NoValue_JustCompletes()
    {
        var scheduler = new GodotFrameScheduler();
        var subject = new Subject<int>();
        var completed = false;

        subject.DebounceFrame(3, scheduler)
            .Subscribe(
                _ => { },
                () => completed = true);

        subject.OnCompleted();

        scheduler.AdvanceFrame(1);
        Assert.True(completed);
    }

    [Fact]
    public void Unsubscribe_StopsEmission()
    {
        var scheduler = new GodotFrameScheduler();
        var subject = new Subject<int>();
        var results = new List<int>();

        var subscription = subject.DebounceFrame(2, scheduler)
            .Subscribe(results.Add);

        subject.OnNext(1);
        subscription.Dispose();

        scheduler.AdvanceFrame(5);
        Assert.Empty(results);
    }

    [Fact]
    public void FrameCount_0_EmitsOnNextFrame()
    {
        var scheduler = new GodotFrameScheduler();
        var subject = new Subject<int>();
        var results = new List<int>();

        subject.DebounceFrame(0, scheduler)
            .Subscribe(results.Add);

        subject.OnNext(42);

        // frameCount=0: ++_currentFrame >= 0 is always true → emits on next frame
        scheduler.AdvanceFrame(1);
        Assert.Single(results);
        Assert.Equal(42, results[0]);
    }

    [Fact]
    public void ObserverOnNextThrows_SubscriptionTerminated()
    {
        var scheduler = new GodotFrameScheduler();
        var subject = new Subject<int>();
        var completed = false;

        subject.DebounceFrame(2, scheduler)
            .Subscribe(
                _ => throw new InvalidOperationException("test"),
                _ => { },
                () => completed = true);

        subject.OnNext(1);
        // Triggers EmitNext which throws; item deactivated internally
        var ex = Record.Exception(() => scheduler.AdvanceFrame(2));
        Assert.NotNull(ex);

        // Item deactivated, no OnCompleted
        scheduler.AdvanceFrame(3);
        Assert.False(completed);
    }
}
