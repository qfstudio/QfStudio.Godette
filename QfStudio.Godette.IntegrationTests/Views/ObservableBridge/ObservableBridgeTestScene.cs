using System.Reactive.Disposables.Fluent;
using Godot;
using QfStudio.Godette.IntegrationTests.ViewModels.ObservableBridge;
using QfStudio.Godette.ReactiveUI;
using ReactiveUI;

namespace QfStudio.Godette.IntegrationTests.Views.ObservableBridge;

[SceneTree(root: "_root")]
[GodotViewFor<ObservableBridgeTestViewModel>]
public partial class ObservableBridgeTestScene : Control
{
    public ObservableBridgeTestScene()
    {
        this.WhenActivated(d =>
        {
            ToggleButton.ObserveToggled()
                .Subscribe(toggled =>
                {
                    ViewModel!.ToggledValue = toggled;
                    GD.Print($"[ObservableBridge] Toggled = {toggled}");
                })
                .DisposeWith(d);

            this.OneWayBind(ViewModel, vm => vm.ToggledValue, v => v.ToggledLabel.Text,
                    v => $"Toggled: {v}")
                .DisposeWith(d);

            ValueSlider.ObserveValueChanged()
                .Subscribe(value =>
                {
                    ViewModel!.SliderValue = value;
                    GD.Print($"[ObservableBridge] SliderValue = {value}");
                })
                .DisposeWith(d);

            this.OneWayBind(ViewModel, vm => vm.SliderValue, v => v.SliderLabel.Text,
                    v => $"Slider: {v:F1}")
                .DisposeWith(d);

            GetTree().ObserveProcessFrame()
                .Subscribe(_ =>
                {
                    ViewModel!.FrameCount++;
                })
                .DisposeWith(d);

            this.OneWayBind(ViewModel, vm => vm.FrameCount, v => v.FrameLabel.Text,
                    v => $"Frames: {v}")
                .DisposeWith(d);

            GetTree().ObservePhysicsFrame()
                .Subscribe(_ =>
                {
                    ViewModel!.PhysicsFrameCount++;
                })
                .DisposeWith(d);

            this.OneWayBind(ViewModel, vm => vm.PhysicsFrameCount, v => v.PhysicsFrameLabel.Text,
                    v => $"Physics Frames: {v}")
                .DisposeWith(d);

            PressButton.ObservePressed()
                .Subscribe(_ =>
                {
                    ViewModel!.PressedCount++;
                    GD.Print($"[ObservableBridge] Pressed = {ViewModel!.PressedCount}");
                })
                .DisposeWith(d);

            this.OneWayBind(ViewModel, vm => vm.PressedCount, v => v.PressedLabel.Text,
                    v => $"Pressed: {v}")
                .DisposeWith(d);

            LineEdit.ObserveTextChanged()
                .Subscribe(text =>
                {
                    ViewModel!.LineEditText = text;
                    GD.Print($"[ObservableBridge] LineEditText = {text}");
                })
                .DisposeWith(d);

            this.OneWayBind(ViewModel, vm => vm.LineEditText, v => v.LineEditLabel.Text,
                    v => $"Line Text: {v}")
                .DisposeWith(d);

            LineEdit.ObserveTextSubmitted()
                .Subscribe(text =>
                {
                    ViewModel!.LineEditSubmitted = text;
                    GD.Print($"[ObservableBridge] LineEditSubmitted = {text}");
                })
                .DisposeWith(d);

            this.OneWayBind(ViewModel, vm => vm.LineEditSubmitted, v => v.LineEditSubmittedLabel.Text,
                    v => $"Line Submitted: {v}")
                .DisposeWith(d);

            TextEdit.ObserveTextChanged()
                .Subscribe(_ =>
                {
                    ViewModel!.TextEditChangeCount++;
                    GD.Print($"[ObservableBridge] TextEditChangeCount = {ViewModel!.TextEditChangeCount}");
                })
                .DisposeWith(d);

            this.OneWayBind(ViewModel, vm => vm.TextEditChangeCount, v => v.TextEditLabel.Text,
                    v => $"TextEdit Changes: {v}")
                .DisposeWith(d);

            ItemList.ObserveItemSelected()
                .Subscribe(index =>
                {
                    ViewModel!.ItemListSelectedIndex = index;
                    GD.Print($"[ObservableBridge] ItemListSelected = {index}");
                })
                .DisposeWith(d);

            this.OneWayBind(ViewModel, vm => vm.ItemListSelectedIndex, v => v.ItemListLabel.Text,
                    v => $"ItemList Selected: {v}")
                .DisposeWith(d);

            OptionButton.ObserveItemSelected()
                .Subscribe(index =>
                {
                    ViewModel!.OptionSelectedIndex = index;
                    GD.Print($"[ObservableBridge] OptionSelected = {index}");
                })
                .DisposeWith(d);

            this.OneWayBind(ViewModel, vm => vm.OptionSelectedIndex, v => v.OptionLabel.Text,
                    v => $"Option Selected: {v}")
                .DisposeWith(d);

            TabBar.ObserveTabChanged()
                .Subscribe(tab =>
                {
                    ViewModel!.TabBarTabIndex = tab;
                    GD.Print($"[ObservableBridge] TabBarTab = {tab}");
                })
                .DisposeWith(d);

            this.OneWayBind(ViewModel, vm => vm.TabBarTabIndex, v => v.TabBarLabel.Text,
                    v => $"TabBar Tab: {v}")
                .DisposeWith(d);

            TestTabContainer.ObserveTabChanged()
                .Subscribe(tab =>
                {
                    ViewModel!.TabContainerTabIndex = tab;
                    GD.Print($"[ObservableBridge] TabContainerTab = {tab}");
                })
                .DisposeWith(d);

            this.OneWayBind(ViewModel, vm => vm.TabContainerTabIndex, v => v.TabContainerLabel.Text,
                    v => $"TabContainer Tab: {v}")
                .DisposeWith(d);

            ColorPicker.ObserveColorChanged()
                .Subscribe(color =>
                {
                    ViewModel!.ColorPickerColor = color;
                    GD.Print($"[ObservableBridge] ColorPicker = {color}");
                })
                .DisposeWith(d);

            this.OneWayBind(ViewModel, vm => vm.ColorPickerColor, v => v.ColorLabel.Text,
                    v => $"ColorPicker: ({v.R:F2}, {v.G:F2}, {v.B:F2}, {v.A:F2})")
                .DisposeWith(d);

            ColorPickerButton.ObserveColorChanged()
                .Subscribe(color =>
                {
                    ViewModel!.ColorPickerButtonColor = color;
                    GD.Print($"[ObservableBridge] ColorPickerButton = {color}");
                })
                .DisposeWith(d);

            this.OneWayBind(ViewModel, vm => vm.ColorPickerButtonColor, v => v.ColorPickerButtonLabel.Text,
                    v => $"ColorPickerButton: ({v.R:F2}, {v.G:F2}, {v.B:F2}, {v.A:F2})")
                .DisposeWith(d);

            Tree.ObserveItemSelected()
                .Subscribe(_ =>
                {
                    ViewModel!.TreeItemSelected = true;
                    GD.Print($"[ObservableBridge] TreeItemSelected = true");
                })
                .DisposeWith(d);

            this.OneWayBind(ViewModel, vm => vm.TreeItemSelected, v => v.TreeLabel.Text,
                    v => $"Tree Selected: {v}")
                .DisposeWith(d);

            PopupMenu.ObserveIdPressed()
                .Subscribe(id =>
                {
                    ViewModel!.PopupSelectedId = id;
                    GD.Print($"[ObservableBridge] PopupSelectedId = {id}");
                })
                .DisposeWith(d);

            this.OneWayBind(ViewModel, vm => vm.PopupSelectedId, v => v.PopupSelectedLabel.Text,
                    v => $"Popup Selected Id: {v}")
                .DisposeWith(d);

            FileDialog.ObserveFileSelected()
                .Subscribe(path =>
                {
                    ViewModel!.FileSelectedPath = path;
                    GD.Print($"[ObservableBridge] FileSelected = {path}");
                })
                .DisposeWith(d);

            this.OneWayBind(ViewModel, vm => vm.FileSelectedPath, v => v.FileSelectedLabel.Text,
                    v => $"File Selected: {v}")
                .DisposeWith(d);
        });
    }

    public override void _Ready()
    {
        ViewModel = new ObservableBridgeTestViewModel();

        BackButton.Pressed += () => GetTree().ChangeSceneToFile(HomeScene.TscnFilePath);

        ItemList.AddItem("Item 0");
        ItemList.AddItem("Item 1");
        ItemList.AddItem("Item 2");

        OptionButton.AddItem("Option 0");
        OptionButton.AddItem("Option 1");
        OptionButton.AddItem("Option 2");

        TabBar.AddTab("Tab A");
        TabBar.AddTab("Tab B");

        var treeItem = Tree.CreateItem();
        treeItem.SetText(0, "Tree Item 0");
        var treeItem2 = Tree.CreateItem();
        treeItem2.SetText(0, "Tree Item 1");

        PopupMenu.AddItem("Popup 0", 0);
        PopupMenu.AddItem("Popup 1", 1);
        PopupMenu.AddItem("Popup 2", 2);

        PopupMenuButton.Pressed += () => PopupMenu.PopupCentered();
        FileDialogButton.Pressed += () => FileDialog.PopupCentered();
    }
}
