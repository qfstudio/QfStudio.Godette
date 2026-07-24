using System.Reactive;

namespace QfStudio.Godette.Tests.Operators;

public class IntervalFrameTests
{
    [Fact]
    public void EmitsFirstAfterDueFrames()
    {
        var scheduler = new GodotFrameScheduler();
        var results = new List<Unit>();

        // Extension method passes dueFrameCount=0 internally
        Observable.IntervalFrame(3, scheduler)
            .Subscribe(results.Add);

        // dueFrameCount=0 is passed to IntervalFrame constructor (not normalized like AfterFrame)
        // IntervalFrame: ++_currentFrame >= 0 is true on first frame
        scheduler.AdvanceFrame(1);
        Assert.Single(results);

        // Next period: 3 frames
        scheduler.AdvanceFrame(2);
        Assert.Single(results);

        scheduler.AdvanceFrame(1);
        Assert.Equal(2, results.Count);
    }

    [Fact]
    public void EmitsPeriodicallyAfterFirst()
    {
        var scheduler = new GodotFrameScheduler();
        var count = 0;

        Observable.IntervalFrame(2, scheduler)
            .Subscribe(_ => count++);

        // dueFrameCount=0: first emit on frame 1
        scheduler.AdvanceFrame(1);
        Assert.Equal(1, count);

        // period=2: next emit after 2 more frames
        scheduler.AdvanceFrame(1);
        Assert.Equal(1, count);

        scheduler.AdvanceFrame(1);
        Assert.Equal(2, count);

        scheduler.AdvanceFrame(2);
        Assert.Equal(3, count);
    }

    [Fact]
    public void Unsubscribe_StopsEmission()
    {
        var scheduler = new GodotFrameScheduler();
        var count = 0;

        var subscription = Observable.IntervalFrame(2, scheduler)
            .Subscribe(_ => count++);

        scheduler.AdvanceFrame(5);
        var countAtUnsub = count;
        Assert.True(countAtUnsub > 0);

        subscription.Dispose();

        scheduler.AdvanceFrame(5);
        Assert.Equal(countAtUnsub, count);
    }

    [Fact]
    public void PeriodFrameCount_1_EmitsEveryFrame()
    {
        var scheduler = new GodotFrameScheduler();
        var count = 0;

        // period=1: emits every frame after the first
        Observable.IntervalFrame(1, scheduler)
            .Subscribe(_ => count++);

        scheduler.AdvanceFrame(5);
        Assert.Equal(5, count);
    }

    [Fact]
    public void PeriodFrameCount_0_EmitsEveryFrame()
    {
        var scheduler = new GodotFrameScheduler();
        var count = 0;

        // period=0: ++_currentFrame >= 0 is always true → emits every frame
        Observable.IntervalFrame(0, scheduler)
            .Subscribe(_ => count++);

        scheduler.AdvanceFrame(5);
        Assert.Equal(5, count);
    }
}
