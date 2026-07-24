using System.Reactive;
using System.Reactive.Disposables;
using QfStudio.Godette.Tests.Operators;

namespace QfStudio.Godette.Tests;

public class GodotFrameSchedulerTests
{
    [Fact]
    public void NotifyProcess_IncrementsFrameCount()
    {
        var scheduler = new GodotFrameScheduler();
        Assert.Equal(0ul, scheduler.FrameCount);

        scheduler.AdvanceFrame(3);
        Assert.Equal(3ul, scheduler.FrameCount);
    }

    [Fact]
    public void NotifyProcess_AccumulatesDelta()
    {
        var scheduler = new GodotFrameScheduler();
        Assert.Equal(DateTimeOffset.MinValue, scheduler.Now);

        scheduler.AdvanceFrame(3, 0.5);
        Assert.Equal(new DateTimeOffset().AddSeconds(1.5), scheduler.Now);
    }

    [Fact]
    public void Schedule_WorkItemActivatesOnNextFrame()
    {
        var scheduler = new GodotFrameScheduler();
        var executed = false;

        scheduler.Schedule(Unit.Default, (s, st) =>
        {
            executed = true;
            return Disposable.Empty;
        });

        // Item is pending; current frame does not execute it
        Assert.False(executed);

        scheduler.AdvanceFrame();
        Assert.True(executed);
    }

    [Fact]
    public void WorkItem_ReturnsFalse_IsDeactivated()
    {
        var scheduler = new GodotFrameScheduler();
        var callCount = 0;

        scheduler.Schedule(Unit.Default, (s, st) =>
        {
            callCount++;
            return Disposable.Empty; // OneShotWorkItem returns false after first call
        });

        scheduler.AdvanceFrame();
        Assert.Equal(1, callCount);

        // Second frame: item already deactivated
        scheduler.AdvanceFrame();
        Assert.Equal(1, callCount);
    }

    [Fact]
    public void Dispose_BeforeFirstFrame_NoMoveNextCore()
    {
        var scheduler = new GodotFrameScheduler();
        var executed = false;

        var item = scheduler.Schedule(Unit.Default, (s, st) =>
        {
            executed = true;
            return Disposable.Empty;
        });

        // Dispose before any frame
        item.Dispose();

        scheduler.AdvanceFrame();
        Assert.False(executed);
    }

    [Fact]
    public void Dispose_IsIdempotent()
    {
        var scheduler = new GodotFrameScheduler();
        var disposeCount = 0;

        var item = scheduler.Schedule(Unit.Default, (s, st) =>
        {
            return Disposable.Create(() => disposeCount++);
        });

        scheduler.AdvanceFrame(); // Executes and OneShotWorkItem returns false

        // Item is already deactivated; double-dispose should not throw
        var ex = Record.Exception(() =>
        {
            item.Dispose();
            item.Dispose();
        });
        Assert.Null(ex);
    }

    [Fact]
    public void Schedule_DuringProcessing_ActivatesNextFrame()
    {
        var scheduler = new GodotFrameScheduler();
        var secondExecuted = false;

        scheduler.Schedule(Unit.Default, (s, st) =>
        {
            // Schedule another item during processing
            s.Schedule(Unit.Default, (_, _) =>
            {
                secondExecuted = true;
                return Disposable.Empty;
            });
            return Disposable.Empty;
        });

        scheduler.AdvanceFrame();
        Assert.False(secondExecuted); // New item is pending

        scheduler.AdvanceFrame();
        Assert.True(secondExecuted);
    }

    [Fact]
    public void Schedule_IScheduler_SchedulesOnNextFrame()
    {
        var scheduler = new GodotFrameScheduler();
        var executed = false;

        // Use the generic IScheduler.Schedule<TState> overload
        scheduler.Schedule("test", (s, state) =>
        {
            executed = true;
            return Disposable.Empty;
        });

        Assert.False(executed);
        scheduler.AdvanceFrame();
        Assert.True(executed);
    }

    [Fact]
    public void WorkItem_DisposedAfterDeactivation()
    {
        var scheduler = new GodotFrameScheduler();
        var disposed = false;

        scheduler.Schedule(Unit.Default, (s, st) =>
        {
            return Disposable.Create(() => disposed = true);
        });

        Assert.False(disposed);
        scheduler.AdvanceFrame();
        Assert.True(disposed);
    }
}
