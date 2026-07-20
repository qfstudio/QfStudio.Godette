using Godot;

namespace QfStudio.Godette.IntegrationTests.Views;

[SceneTree(root: "_root")]
public partial class HomeScene : Control
{
    public override void _Ready()
    {
        ActivationButton.Pressed += () => GetTree().ChangeSceneToFile(Activation.ActivatableHostScene.TscnFilePath);
        DataBindingButton.Pressed += () => GetTree().ChangeSceneToFile(DataBinding.DataBindingTestScene.TscnFilePath);
        CommandButton.Pressed += () => GetTree().ChangeSceneToFile("res://Views/Command/CommandTestScene.tscn");
        CollectionButton.Pressed += () => GetTree().ChangeSceneToFile("res://Views/Collection/ItemsTestScene.tscn");
        PollingBindingButton.Pressed += () => GetTree().ChangeSceneToFile("res://Views/PollingBinding/PollingBindingTestScene.tscn");
        ItemListButton.Pressed += () => GetTree().ChangeSceneToFile("res://Views/Collection/ItemListTestScene.tscn");
        ItemBinderButton.Pressed += () => GetTree().ChangeSceneToFile("res://Views/Collection/ItemBinderTestScene.tscn");
        RoutingButton.Pressed += () => GetTree().ChangeSceneToFile("res://Views/Routing/RoutingDemoScene.tscn");
        InteractionButton.Pressed += () => GetTree().ChangeSceneToFile("res://Views/Interaction/InteractionTestScene.tscn");
        ObservableBridgeButton.Pressed += () => GetTree().ChangeSceneToFile("res://Views/ObservableBridge/ObservableBridgeTestScene.tscn");
        ValidationButton.Pressed += () => GetTree().ChangeSceneToFile("res://Views/Validation/ValidationTestScene.tscn");
        ExitButton.Pressed += Exit;
    }

    private void Exit()
    {
        GC.Collect();
        GetTree().Quit();
    }
}
