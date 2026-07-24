namespace QfStudio.Godette.Tests.Operators;

public class EveryValueChangedTests
{
    private class MockSource
    {
        public int Value { get; set; }
    }

    [Fact]
    public void EmitsWhenValueChanges()
    {
        var scheduler = new GodotFrameScheduler();
        var source = new MockSource { Value = 1 };
        var results = new List<int>();

        Observable.PollEveryUpdate(source, s => s.Value, scheduler: scheduler)
            .Subscribe(results.Add);

        scheduler.AdvanceFrame(1);
        Assert.Empty(results); // Initial value not emitted

        source.Value = 42;
        scheduler.AdvanceFrame(1);
        Assert.Single(results);
        Assert.Equal(42, results[0]);
    }

    [Fact]
    public void DoesNotEmitWhenUnchanged()
    {
        var scheduler = new GodotFrameScheduler();
        var source = new MockSource { Value = 1 };
        var results = new List<int>();

        Observable.PollEveryUpdate(source, s => s.Value, scheduler: scheduler)
            .Subscribe(results.Add);

        scheduler.AdvanceFrame(5);
        Assert.Empty(results);
    }

    [Fact]
    public void InitialValue_NotEmitted()
    {
        var scheduler = new GodotFrameScheduler();
        var source = new MockSource { Value = 99 };
        var results = new List<int>();

        Observable.PollEveryUpdate(source, s => s.Value, scheduler: scheduler)
            .Subscribe(results.Add);

        // First frame: current value == initial value → no emission
        scheduler.AdvanceFrame(1);
        Assert.Empty(results);
    }

    [Fact]
    public void CustomEqualityComparer_Used()
    {
        var scheduler = new GodotFrameScheduler();
        var source = new MockSource { Value = 1 };
        var results = new List<int>();

        // Always considers values as different — every poll emits
        Observable.PollEveryUpdate(source, s => s.Value,
            equalityComparer: EqualityComparer<int>.Create((a, b) => false, _ => 0),
            scheduler: scheduler)
            .Subscribe(results.Add);

        scheduler.AdvanceFrame(1);
        Assert.Single(results); // Comparer says different → emit

        source.Value = 1; // Same value, but comparer says different
        scheduler.AdvanceFrame(1);
        Assert.Equal(2, results.Count); // Emit again
    }

    [Fact]
    public void MultipleChanges_EachEmitted()
    {
        var scheduler = new GodotFrameScheduler();
        var source = new MockSource { Value = 0 };
        var results = new List<int>();

        Observable.PollEveryUpdate(source, s => s.Value, scheduler: scheduler)
            .Subscribe(results.Add);

        scheduler.AdvanceFrame(1); // Initial, not emitted

        source.Value = 1;
        scheduler.AdvanceFrame(1);
        Assert.Single(results);

        source.Value = 2;
        scheduler.AdvanceFrame(1);
        Assert.Equal(2, results.Count);

        source.Value = 3;
        scheduler.AdvanceFrame(1);
        Assert.Equal(3, results.Count);
    }

    [Fact]
    public void Unsubscribe_StopsPolling()
    {
        var scheduler = new GodotFrameScheduler();
        var source = new MockSource { Value = 0 };
        var results = new List<int>();

        var subscription = Observable.PollEveryUpdate(source, s => s.Value, scheduler: scheduler)
            .Subscribe(results.Add);

        scheduler.AdvanceFrame(1);

        source.Value = 1;
        subscription.Dispose();

        source.Value = 2;
        scheduler.AdvanceFrame(3);
        Assert.Empty(results);
    }

    [Fact]
    public void PropertySelectorThrowsAtSubscribe_PropagatesSynchronously()
    {
        var scheduler = new GodotFrameScheduler();
        var ex = Record.Exception(() =>
        {
            Observable.PollEveryUpdate<string, string>(
                "hello",
                _ => throw new InvalidOperationException("init-fail"),
                scheduler: scheduler)
                .Subscribe();
        });

        Assert.NotNull(ex);
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal("init-fail", ex.Message);
    }

    [Fact]
    public void PropertySelectorThrowsDuringPolling_ErrorForwarded()
    {
        var scheduler = new GodotFrameScheduler();
        var source = new MockSource { Value = 1 };
        var error = (Exception?)null;
        var completed = false;

        var callCount = 0;
        Observable.PollEveryUpdate(source, s =>
        {
            callCount++;
            if (callCount > 1) // First call is constructor (initial read), second is first frame
                throw new InvalidOperationException("poll-fail");
            return s.Value;
        }, scheduler: scheduler)
            .Subscribe(
                _ => { },
                ex => error = ex,
                () => completed = true);

        scheduler.AdvanceFrame(1); // Initial read succeeds, first poll succeeds

        source.Value = 2;
        scheduler.AdvanceFrame(1); // Second poll throws

        Assert.NotNull(error);
        Assert.IsType<InvalidOperationException>(error);
        Assert.Equal("poll-fail", error.Message);
        Assert.False(completed);
    }
}
