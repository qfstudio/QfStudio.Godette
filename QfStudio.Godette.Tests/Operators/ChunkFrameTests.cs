namespace QfStudio.Godette.Tests.Operators;

public class ChunkFrameTests
{
    [Fact]
    public void CollectsAndEmitsBatch()
    {
        var scheduler = new GodotFrameScheduler();
        var subject = new Subject<int>();
        var results = new List<IList<int>>();

        subject.ChunkFrame(3, scheduler)
            .Subscribe(results.Add);

        subject.OnNext(1);
        subject.OnNext(2);
        subject.OnNext(3);

        // Window boundary at frame 3 (frameCount=2, ++_currentFrame >= 2 on 3rd frame)
        scheduler.AdvanceFrame(3);
        Assert.Single(results);
        Assert.Equal([1, 2, 3], results[0]);
    }

    [Fact]
    public void EmptyWindow_NoEmission()
    {
        var scheduler = new GodotFrameScheduler();
        var subject = new Subject<int>();
        var results = new List<IList<int>>();

        subject.ChunkFrame(2, scheduler)
            .Subscribe(results.Add);

        // No values sent
        scheduler.AdvanceFrame(4);
        Assert.Empty(results);
    }

    [Fact]
    public void MultipleValuesInSameBatch_EmittedTogether()
    {
        var scheduler = new GodotFrameScheduler();
        var subject = new Subject<int>();
        var results = new List<IList<int>>();

        subject.ChunkFrame(2, scheduler)
            .Subscribe(results.Add);

        subject.OnNext(10);
        subject.OnNext(20);
        subject.OnNext(30);

        // frameCount=2: first window boundary at frame 3
        scheduler.AdvanceFrame(3);
        Assert.Single(results);
        Assert.Equal([10, 20, 30], results[0]);
    }

    [Fact]
    public void UpstreamCompletes_FlushesRemainingBatch_AtWindowBoundary()
    {
        var scheduler = new GodotFrameScheduler();
        var subject = new Subject<int>();
        var results = new List<IList<int>>();
        var completed = false;

        subject.ChunkFrame(3, scheduler)
            .Subscribe(
                v => results.Add(v),
                () => completed = true);

        subject.OnNext(1);
        subject.OnCompleted();

        // frameCount=3: window boundary at frame 4
        // Batch not empty, so at window boundary: flush + complete
        scheduler.AdvanceFrame(4);
        Assert.Single(results);
        Assert.Equal([1], results[0]);
        Assert.True(completed);
    }

    [Fact]
    public void UpstreamError_DiscardsBufferedData()
    {
        var scheduler = new GodotFrameScheduler();
        var subject = new Subject<int>();
        var results = new List<IList<int>>();
        Exception? error = null;

        subject.ChunkFrame(3, scheduler)
            .Subscribe(
                v => results.Add(v),
                ex => error = ex);

        subject.OnNext(1);
        subject.OnNext(2);
        subject.OnError(new InvalidOperationException("test"));

        scheduler.AdvanceFrame(1);
        Assert.NotNull(error);
        Assert.IsType<InvalidOperationException>(error);
        Assert.Empty(results); // Buffer discarded
    }

    [Fact]
    public void Unsubscribe_StopsEmission()
    {
        var scheduler = new GodotFrameScheduler();
        var subject = new Subject<int>();
        var results = new List<IList<int>>();

        var subscription = subject.ChunkFrame(2, scheduler)
            .Subscribe(results.Add);

        subject.OnNext(1);
        subscription.Dispose();

        scheduler.AdvanceFrame(5);
        Assert.Empty(results);
    }

    [Fact]
    public void FrameCount_0_EmitsEveryFrameWithItems()
    {
        var scheduler = new GodotFrameScheduler();
        var subject = new Subject<int>();
        var results = new List<IList<int>>();

        subject.ChunkFrame(0, scheduler)
            .Subscribe(results.Add);

        subject.OnNext(1);

        // frameCount=0: ++_currentFrame >= 0 is always true → emits on first frame
        scheduler.AdvanceFrame(1);
        Assert.Single(results);
        Assert.Equal([1], results[0]);
    }
}
