namespace QfStudio.Godette.Tests.Operators;

public class FrameSinkBoundaryTests
{
    [Fact]
    public void ObserverOnNextThrows_ItemDeactivated_NoOnErrorNoOnCompleted()
    {
        var scheduler = new GodotFrameScheduler();
        var completed = false;
        var errorReceived = false;

        Observable.EveryUpdate(scheduler)
            .Subscribe(
                _ => throw new InvalidOperationException("test"),
                _ => errorReceived = true,
                () => completed = true);

        // OnNext throws → exception propagates; item is deactivated internally
        var ex = Record.Exception(() => scheduler.AdvanceFrame(1));
        Assert.NotNull(ex);
        scheduler.AdvanceFrame(5);

        Assert.False(completed);
        Assert.False(errorReceived);
    }

    [Fact]
    public void ObserverOnCompletedThrows_ItemStillDisposed()
    {
        var scheduler = new GodotFrameScheduler();
        var secondCompleted = false;

        var subscription = Observable.AfterFrame(1, scheduler)
            .Subscribe(
                _ => { },
                _ => throw new InvalidOperationException("test"),
                () => { });

        scheduler.AdvanceFrame(1); // OnCompleted throws, but Dispose is still called

        // Second subscription should still work — proves item was disposed
        Observable.AfterFrame(1, scheduler)
            .Subscribe(_ => { }, () => secondCompleted = true);

        scheduler.AdvanceFrame(1);
        Assert.True(secondCompleted);
    }

    [Fact]
    public void ObserverOnErrorThrows_ItemStillDisposed()
    {
        var scheduler = new GodotFrameScheduler();
        var subject = new Subject<int>();
        var secondError = false;

        var subscription = subject.DelayFrame(1, scheduler)
            .Subscribe(
                _ => { },
                ex => throw new InvalidOperationException("test"),
                () => { });

        subject.OnError(new InvalidOperationException("first"));
        // OnError throws, but Dispose is called in finally
        var ex = Record.Exception(() => scheduler.AdvanceFrame(1));
        Assert.NotNull(ex);

        // Second subscription on same scheduler proves first item was disposed
        var subject2 = new Subject<int>();
        subject2.DelayFrame(1, scheduler)
            .Subscribe(
                _ => { },
                ex => secondError = true);

        subject2.OnError(new InvalidOperationException("second"));
        scheduler.AdvanceFrame(1);
        Assert.True(secondError);
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

        // Item is now disposed; next MoveNext returns false
        scheduler.AdvanceFrame(3);
        Assert.Equal(1, count);
    }

    [Fact]
    public void DisposeFromWithinOnNext_DisposesUpstream()
    {
        var scheduler = new GodotFrameScheduler();
        var subject = new Subject<int>();
        var results = new List<int>();
        IDisposable? subscription = null;

        subscription = subject.DelayFrame(1, scheduler)
            .Subscribe(_ =>
            {
                // First value passes through; dispose cancels upstream
                throw new InvalidOperationException("stop");
            });

        subject.OnNext(1);

        // DelayFrame sink processes OnNext in the ConcurrentQueue, then MoveNextCore emits
        // OnNext callback throws → item deactivated, upstream disposed
        var ex = Record.Exception(() => scheduler.AdvanceFrame(1));
        Assert.NotNull(ex);

        // After disposal, sending more values to subject should not reach the observer
        subject.OnNext(2);
        scheduler.AdvanceFrame(3);
        Assert.Empty(results); // OnNext callback threw, so results stays empty
    }

    [Fact]
    public void UpstreamNextAndCompleted_SameSynchronousBurst_DelayedByOneFrame()
    {
        var scheduler = new GodotFrameScheduler();
        var subject = new Subject<int>();
        var results = new List<int>();
        var completed = false;

        subject.DelayFrame(2, scheduler)
            .Subscribe(
                v => results.Add(v),
                () => completed = true);

        // Synchronous burst: OnNext + OnCompleted in same call stack
        subject.OnNext(42);
        subject.OnCompleted();

        // Frame 1: sink activated, processes pending queue, value has targetFrame=2+0=2 (wrong?)
        // Actually: at Subscribe time, FrameCount=0. OnNext stores targetFrame=0+2=2.
        // Frame 1: MoveNextCore runs, FrameCount=1, _inbox→_pending, TargetFrame=2 > 1 → no emit.
        // But IsUpstreamTerminated=true and _pending.Count=1 → does NOT complete (pending not empty).
        scheduler.AdvanceFrame(1);
        Assert.False(completed);
        Assert.Empty(results);

        // Frame 2: FrameCount=2, TargetFrame=2 <= 2 → emit, then IsUpstreamTerminated && pending empty → complete
        scheduler.AdvanceFrame(1);
        Assert.Single(results);
        Assert.Equal(42, results[0]);
        Assert.True(completed);
    }

    [Fact]
    public void NextAndCompletedSameBurst_DebounceFrame_FlushesThenCompletes()
    {
        var scheduler = new GodotFrameScheduler();
        var subject = new Subject<int>();
        var results = new List<int>();
        var completed = false;

        subject.DebounceFrame(3, scheduler)
            .Subscribe(
                v => results.Add(v),
                () => completed = true);

        // Synchronous burst
        subject.OnNext(42);
        subject.OnCompleted();

        // Frame 1: MoveNextCore runs. TerminalError is null. IsUpstreamTerminated=true.
        // _hasValue=true → EmitNext(42) + Complete.
        scheduler.AdvanceFrame(1);
        Assert.Single(results);
        Assert.Equal(42, results[0]);
        Assert.True(completed);
    }

    [Fact]
    public void NextAndCompletedSameBurst_ChunkFrame_FlushesAtWindowThenCompletes()
    {
        var scheduler = new GodotFrameScheduler();
        var subject = new Subject<int>();
        var results = new List<IList<int>>();
        var completed = false;

        subject.ChunkFrame(2, scheduler)
            .Subscribe(
                v => results.Add(v),
                () => completed = true);

        subject.OnNext(1);
        subject.OnCompleted();

        // Frame 1: value moves to batch, not yet at window boundary (frameCount=2)
        scheduler.AdvanceFrame(1);
        Assert.Empty(results);
        Assert.False(completed);

        // Frame 2: window boundary (frameCount=2, ++_currentFrame >= 2 → true)
        // Batch has items → flush. Then IsUpstreamTerminated && batch empty → complete.
        scheduler.AdvanceFrame(1);
        Assert.Single(results);
        Assert.Equal([1], results[0]);
        Assert.True(completed);
    }

    [Fact]
    public void NullValue_Propagated_ReferenceTypeStream()
    {
        var scheduler = new GodotFrameScheduler();
        var subject = new Subject<string?>();
        var results = new List<string?>();

        subject.DebounceFrame(2, scheduler)
            .Subscribe(results.Add);

        subject.OnNext(null);

        scheduler.AdvanceFrame(2);
        Assert.Single(results);
        Assert.Null(results[0]);
    }
}
