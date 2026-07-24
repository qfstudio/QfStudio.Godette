using System.Reactive;

namespace QfStudio.Godette.Tests.Operators;

public class EveryUpdateTests
{
    [Fact]
    public void EmitsUnitEveryFrame()
    {
        var scheduler = new GodotFrameScheduler();
        var results = new List<Unit>();

        Observable.EveryUpdate(scheduler)
            .Subscribe(results.Add);

        scheduler.AdvanceFrame(5);
        Assert.Equal(5, results.Count);
        Assert.All(results, v => Assert.Equal(Unit.Default, v));
    }

    [Fact]
    public void Unsubscribe_StopsEmission()
    {
        var scheduler = new GodotFrameScheduler();
        var count = 0;

        var subscription = Observable.EveryUpdate(scheduler)
            .Subscribe(_ => count++);

        scheduler.AdvanceFrame(3);
        Assert.Equal(3, count);

        subscription.Dispose();

        scheduler.AdvanceFrame(3);
        Assert.Equal(3, count);
    }

    [Fact]
    public void MultipleSubscriptions_Independent()
    {
        var scheduler = new GodotFrameScheduler();
        var count1 = 0;
        var count2 = 0;

        var sub1 = Observable.EveryUpdate(scheduler).Subscribe(_ => count1++);
        var sub2 = Observable.EveryUpdate(scheduler).Subscribe(_ => count2++);

        scheduler.AdvanceFrame(3);
        Assert.Equal(3, count1);
        Assert.Equal(3, count2);

        sub1.Dispose();
        scheduler.AdvanceFrame(2);
        Assert.Equal(3, count1); // Stopped
        Assert.Equal(5, count2); // Still going
    }

    [Fact]
    public void ObserverOnNextThrows_SubscriptionTerminated()
    {
        var scheduler = new GodotFrameScheduler();
        var count = 0;

        Observable.EveryUpdate(scheduler)
            .Subscribe(_ =>
            {
                count++;
                throw new InvalidOperationException("test");
            });

        // OnNext throws → exception propagates; item deactivated internally
        var ex = Record.Exception(() => scheduler.AdvanceFrame(1));
        Assert.NotNull(ex);
        Assert.Equal(1, count);

        // After the exception, the item is deactivated — no more OnNext calls
        scheduler.AdvanceFrame(5);
        Assert.Equal(1, count);
    }

    [Fact]
    public void DisposeFromWithinOnNext_StopsNextFrame()
    {
        var scheduler = new GodotFrameScheduler();
        var count = 0;
        IDisposable? subscription = null;

        subscription = Observable.EveryUpdate(scheduler)
            .Subscribe(_ =>
            {
                count++;
                subscription?.Dispose();
            });

        scheduler.AdvanceFrame(1);
        Assert.Equal(1, count);

        // Dispose marks item as disposed; next MoveNext returns false
        scheduler.AdvanceFrame(3);
        Assert.Equal(1, count);
    }
}
