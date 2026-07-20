using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using Godot;
using ReactiveUI;

namespace QfStudio.Godette.IntegrationTests.Views.Activation;

[SceneTree(root: "_root")]
public partial class ActivatableHostScene : Control, IActivatableView
{
    private readonly List<ActivatableScene> _scenes = [];
    private readonly CompositeDisposable _timerDisposables = new();

    public ActivatableHostScene()
    {
        this.WhenActivated(d =>
        {
            GD.Print("[Host] Activated");

            Disposable.Create(() =>
            {
                GD.Print("[Host] Deactivated");

                foreach (var scene in _scenes)
                {
                    scene.QueueFree();
                }
                _scenes.Clear();
            }).DisposeWith(d);

            _timerDisposables.Clear();
            _timerDisposables.DisposeWith(d);
        });
    }

    public override void _Ready()
    {
        BackButton.Pressed += () => GetTree().ChangeSceneToFile(HomeScene.TscnFilePath);
        AddChildButton.Pressed += AddInnerScene;
    }

    private void AddInnerScene()
    {
        var packedScene = GD.Load<PackedScene>(ActivatableScene.TscnFilePath);
        var scene = packedScene.Instantiate<ActivatableScene>();
        scene.Host = this;

        _scenes.Add(scene);
        Container.AddChild(scene);

        GD.Print("[Host] Adding ActivatableScene to Container: ", scene);
    }

    public void RemoveInnerScene(ActivatableScene scene)
    {
        GD.Print("[Host] Removing ActivatableScene from Container: ", scene);

        _scenes.Remove(scene);
        scene.QueueFree();
    }

    public void DetachAndThenAttachSceneIn3Seconds(ActivatableScene scene)
    {
        Container.RemoveChild(scene);

        Observable.Timer(TimeSpan.FromSeconds(3))
            .ObserveOn(RxSchedulers.MainThreadScheduler)
            .Subscribe(_ =>
            {
                Container.AddChild(scene);
            })
            .DisposeWith(_timerDisposables);
    }
}
