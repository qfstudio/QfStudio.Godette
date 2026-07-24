using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Godot;
using QfStudio.Godette.ReactiveUI;
using ReactiveUI;

namespace QfStudio.Godette.IntegrationTests.Views.FrameOperators;

[SceneTree(root: "_root")]
public partial class FrameOperatorsTestScene : Control, IActivatableView
{
    private long _frameCount;
    private long _physicsFrameCount;

    public FrameOperatorsTestScene()
    {
        this.WhenActivated(d =>
        {
            EveryUpdateTest().DisposeWith(d);
            TimerFrameTest().DisposeWith(d);
            DelayFrameTest().DisposeWith(d);
            DebounceFrameTest().DisposeWith(d);
            ThrottleFirstFrameTest().DisposeWith(d);
            ChunkFrameTest().DisposeWith(d);
            ReturnFrameTest().DisposeWith(d);
            IntervalFrameTest().DisposeWith(d);
        });
    }

    public override void _Ready()
    {
        BackButton.Pressed += () => GetTree().ChangeSceneToFile(HomeScene.TscnFilePath);
    }

    private IDisposable EveryUpdateTest()
    {
        var count = 0;
        var countPhysics = 0;

        var disposable = new CompositeDisposable();
        Observable.EveryUpdate()
            .Subscribe(_ =>
            {
                count++;
                EveryUpdateLabel.Text = $"EveryUpdate: {count} frames";
            })
            .DisposeWith(disposable);

        Observable.EveryUpdate(RxSchedulers.PhysicsFrameScheduler)
            .Subscribe(_ =>
            {
                countPhysics++;
                EveryPhysicsUpdateLabel.Text = $"EveryUpdate (physics): {countPhysics} frames";
            })
            .DisposeWith(disposable);

        return disposable;
    }

    private IDisposable TimerFrameTest()
    {
        return Observable.AfterFrame(60)
            .Subscribe(_ =>
            {
                TimerFrameLabel.Text = "TimerFrame: Fired after 60 frames!";
                GD.Print("TimerFrame: Fired after 60 frames");
            });
    }

    private IDisposable DelayFrameTest()
    {
        return Observable.AfterFrame(0)
            .DelayFrame(30)
            .Subscribe(_ =>
            {
                DelayFrameLabel.Text = "DelayFrame: Fired after 30 frame delay!";
                GD.Print("DelayFrame: Fired after 30 frame delay");
            });
    }

    private IDisposable DebounceFrameTest()
    {
        var input = new Subject<string>();
        var disposable = new CompositeDisposable();

        input
            .DebounceFrame(30)
            .Subscribe(value =>
            {
                DebounceFrameLabel.Text = $"DebounceFrame: {value}";
                GD.Print($"DebounceFrame: {value}");
            })
            .DisposeWith(disposable);

        Observable.AfterFrame(10)
            .Subscribe(_ => input.OnNext("First"))
            .DisposeWith(disposable);
        Observable.AfterFrame(15)
            .Subscribe(_ => input.OnNext("Second"))
            .DisposeWith(disposable);
        Observable.AfterFrame(20)
            .Subscribe(_ => input.OnNext("Third"))
            .DisposeWith(disposable);

        Observable.AfterFrame(120)
            .Subscribe(_ => input.OnNext("Fifth"))
            .DisposeWith(disposable);
        Observable.AfterFrame(125)
            .Subscribe(_ => input.OnNext("Sixth"))
            .DisposeWith(disposable);

        return disposable;
    }

    private IDisposable ThrottleFirstFrameTest()
    {
        var input = new Subject<int>();
        var disposable = new CompositeDisposable();

        input
            .ThrottleFirstFrame(60)
            .Subscribe(value =>
            {
                ThrottleFirstFrameLabel.Text = $"ThrottleFirstFrame: {value}";
                GD.Print($"ThrottleFirstFrame: {value}");
            })
            .DisposeWith(disposable);

        for (int i = 0; i < 10; i++)
        {
            var value = i;
            Observable.AfterFrame((uint)(i * 5))
                .Subscribe(_ => input.OnNext(value))
                .DisposeWith(disposable);
        }

        return disposable;
    }

    private IDisposable ChunkFrameTest()
    {
        var input = new Subject<int>();
        var disposable = new CompositeDisposable();

        input
            .ChunkFrame(30)
            .Subscribe(values =>
            {
                var text = $"ChunkFrame: [{string.Join(", ", values)}]";
                ChunkFrameLabel.Text = text;
                GD.Print(text);
            })
            .DisposeWith(disposable);

        for (int i = 0; i < 5; i++)
        {
            var value = i;
            Observable.AfterFrame((uint)(i * 10))
                .Subscribe(_ => input.OnNext(value))
                .DisposeWith(disposable);
        }

        return disposable;
    }

    private IDisposable ReturnFrameTest()
    {
        return Observable.ReturnFrame("Hello from ReturnFrame!", 60)
            .Subscribe(value =>
            {
                ReturnFrameLabel.Text = $"ReturnFrame: {value}";
                GD.Print($"ReturnFrame: {value}");
            });
    }

    private IDisposable IntervalFrameTest()
    {
        var count = 0;

        return Observable.IntervalFrame(30)
            .Subscribe(_ =>
            {
                count++;
                IntervalFrameLabel.Text = $"IntervalFrame: {count} times";
            });
    }

    public override void _Process(double delta)
    {
        FrameCountLabel.Text = (++_frameCount).ToString();
    }

    public override void _PhysicsProcess(double delta)
    {
        PhycicsFrameCountLabel.Text = (++_physicsFrameCount).ToString();
    }
}
