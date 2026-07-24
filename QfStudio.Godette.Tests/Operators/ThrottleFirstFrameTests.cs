namespace QfStudio.Godette.Tests.Operators;

public class ThrottleFirstFrameTests
{
    [Fact]
    public void EmitsFirstValueInWindow()
    {
        var scheduler = new GodotFrameScheduler();
        var subject = new Subject<int>();
        var results = new List<int>();

        subject.ThrottleFirstFrame(3, scheduler)
            .Subscribe(results.Add);

        subject.OnNext(1);
        scheduler.AdvanceFrame(3); // Window expires

        Assert.Single(results);
        Assert.Equal(1, results[0]);
    }

    [Fact]
    public void IgnoresSubsequentValuesInWindow()
    {
        var scheduler = new GodotFrameScheduler();
        var subject = new Subject<int>();
        var results = new List<int>();

        subject.ThrottleFirstFrame(3, scheduler)
            .Subscribe(results.Add);

        subject.OnNext(1);
        subject.OnNext(2); // Ignored — window occupied
        subject.OnNext(3); // Ignored

        scheduler.AdvanceFrame(3);
        Assert.Single(results);
        Assert.Equal(1, results[0]);
    }

    [Fact]
    public void MultipleValuesSameFrame_OnlyFirstCaptured()
    {
        var scheduler = new GodotFrameScheduler();
        var subject = new Subject<int>();
        var results = new List<int>();

        subject.ThrottleFirstFrame(2, scheduler)
            .Subscribe(results.Add);

        subject.OnNext(1);
        subject.OnNext(2); // Same frame, ignored

        scheduler.AdvanceFrame(2);
        Assert.Single(results);
        Assert.Equal(1, results[0]);
    }

    [Fact]
    public void NewWindowEmitsNextValue()
    {
        var scheduler = new GodotFrameScheduler();
        var subject = new Subject<int>();
        var results = new List<int>();

        subject.ThrottleFirstFrame(2, scheduler)
            .Subscribe(results.Add);

        subject.OnNext(1);
        scheduler.AdvanceFrame(2); // Window 1 expires, emits 1
        Assert.Single(results);

        subject.OnNext(2); // New window
        scheduler.AdvanceFrame(2); // Window 2 expires, emits 2
        Assert.Equal(2, results.Count);
        Assert.Equal(2, results[1]);
    }

    [Fact]
    public void NoValueInWindow_NoEmission()
    {
        var scheduler = new GodotFrameScheduler();
        var subject = new Subject<int>();
        var results = new List<int>();

        subject.ThrottleFirstFrame(3, scheduler)
            .Subscribe(results.Add);

        // No value sent
        scheduler.AdvanceFrame(3);
        Assert.Empty(results);
    }

    [Fact]
    public void UpstreamError_ForwardedImmediately()
    {
        var scheduler = new GodotFrameScheduler();
        var subject = new Subject<int>();
        Exception? error = null;

        subject.ThrottleFirstFrame(3, scheduler)
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

        subject.ThrottleFirstFrame(2, scheduler)
            .Subscribe(
                v => results.Add(v),
                () => completed = true);

        subject.OnNext(1);

        // Window expires
        scheduler.AdvanceFrame(2);
        Assert.Single(results);
        Assert.Equal(1, results[0]);

        subject.OnCompleted();
        scheduler.AdvanceFrame(1);
        Assert.True(completed);
    }

    [Fact]
    public void UpstreamCompletes_WindowNotExpired_ValueLost()
    {
        var scheduler = new GodotFrameScheduler();
        var subject = new Subject<int>();
        var results = new List<int>();
        var completed = false;

        subject.ThrottleFirstFrame(3, scheduler)
            .Subscribe(
                v => results.Add(v),
                () => completed = true);

        subject.OnNext(1);
        scheduler.AdvanceFrame(1); // 1 frame into window

        subject.OnCompleted();

        // Window not expired: value is lost, complete immediately
        scheduler.AdvanceFrame(1);
        Assert.Empty(results);
        Assert.True(completed);
    }

    [Fact]
    public void Unsubscribe_StopsEmission()
    {
        var scheduler = new GodotFrameScheduler();
        var subject = new Subject<int>();
        var results = new List<int>();

        var subscription = subject.ThrottleFirstFrame(2, scheduler)
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

        subject.ThrottleFirstFrame(0, scheduler)
            .Subscribe(results.Add);

        subject.OnNext(42);

        // frameCount=0: ++_elapsed >= 0 is always true → emits on first frame
        scheduler.AdvanceFrame(1);
        Assert.Single(results);
        Assert.Equal(42, results[0]);
    }
}
