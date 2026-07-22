using System.Reactive.Disposables;
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
            Disposable.Create(() => GD.Print("[Deactivation] deactivated"))
                .DisposeWith(d);

            WireButtonTests(d);
            WireTextTests(d);
            WireNumericTests(d);
            WireFrameTests(d);
            WireSelectionTests(d);
            WireColorTests(d);
            WireTreePopupFileTests(d);
            WireCustomSignalTests(d);
        });
    }

    private void WireButtonTests(CompositeDisposable d)
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
    }

    private void WireTextTests(CompositeDisposable d)
    {
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
    }

    private void WireNumericTests(CompositeDisposable d)
    {
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
    }

    private void WireFrameTests(CompositeDisposable d)
    {
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
    }

    private void WireSelectionTests(CompositeDisposable d)
    {
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
    }

    private void WireColorTests(CompositeDisposable d)
    {
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
    }

    private void WireTreePopupFileTests(CompositeDisposable d)
    {
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
    }

    private void WireCustomSignalTests(CompositeDisposable d)
    {
        // 0 args
        CustomSignalOwner.ObserveSignal("my_signal")
            .Subscribe(_ =>
            {
                ViewModel!.CustomSignal0Count++;
                GD.Print($"[ObservableBridge] my_signal count = {ViewModel!.CustomSignal0Count}");
            })
            .DisposeWith(d);

        this.OneWayBind(ViewModel, vm => vm.CustomSignal0Count, v => v.CustomSignal0Label.Text,
                v => $"Count: {v}")
            .DisposeWith(d);

        EmitSignalButton.Pressed += () => CustomSignalOwner.Call("emit_my_signal");

        // 1 arg
        CustomSignalOwner.ObserveSignal<string>("my_signal_1arg")
            .Subscribe(args =>
            {
                ViewModel!.CustomSignal1Text = $"arg1={args.Item1}";
                GD.Print($"[ObservableBridge] my_signal_1arg: {args}");
            })
            .DisposeWith(d);

        this.OneWayBind(ViewModel, vm => vm.CustomSignal1Text, v => v.CustomSignal1Label.Text)
            .DisposeWith(d);

        EmitSignal1Button.Pressed += () => CustomSignalOwner.Call("emit_my_signal_1arg");

        // 2 args
        CustomSignalOwner.ObserveSignal<string, long>("my_signal_2args")
            .Subscribe(args =>
            {
                ViewModel!.CustomSignal2Text = $"arg1={args.Item1}, arg2={args.Item2}";
                GD.Print($"[ObservableBridge] my_signal_2args: {args}");
            })
            .DisposeWith(d);

        this.OneWayBind(ViewModel, vm => vm.CustomSignal2Text, v => v.CustomSignal2Label.Text)
            .DisposeWith(d);

        EmitSignal2Button.Pressed += () => CustomSignalOwner.Call("emit_my_signal_2args");

        // 3 args
        CustomSignalOwner.ObserveSignal<string, long, double>("my_signal_3args")
            .Subscribe(args =>
            {
                ViewModel!.CustomSignal3Text = $"arg1={args.Item1}, arg2={args.Item2}, arg3={args.Item3:F2}";
                GD.Print($"[ObservableBridge] my_signal_3args: {args}");
            })
            .DisposeWith(d);

        this.OneWayBind(ViewModel, vm => vm.CustomSignal3Text, v => v.CustomSignal3Label.Text)
            .DisposeWith(d);

        EmitSignal3Button.Pressed += () => CustomSignalOwner.Call("emit_my_signal_3args");

        // 4 args
        CustomSignalOwner.ObserveSignal<string, long, double, bool>("my_signal_4args")
            .Subscribe(args =>
            {
                ViewModel!.CustomSignal4Text = $"arg1={args.Item1}, arg2={args.Item2}, arg3={args.Item3:F2}, arg4={args.Item4}";
                GD.Print($"[ObservableBridge] my_signal_4args: {args}");
            })
            .DisposeWith(d);

        this.OneWayBind(ViewModel, vm => vm.CustomSignal4Text, v => v.CustomSignal4Label.Text)
            .DisposeWith(d);

        EmitSignal4Button.Pressed += () => CustomSignalOwner.Call("emit_my_signal_4args");

        // 5 args
        CustomSignalOwner.ObserveSignal<string, long, double, bool, Vector2>("my_signal_5args")
            .Subscribe(args =>
            {
                ViewModel!.CustomSignal5Text = $"arg1={args.Item1}, arg2={args.Item2}, arg3={args.Item3:F2}, arg4={args.Item4}, arg5={args.Item5}";
                GD.Print($"[ObservableBridge] my_signal_5args: {args}");
            })
            .DisposeWith(d);

        this.OneWayBind(ViewModel, vm => vm.CustomSignal5Text, v => v.CustomSignal5Label.Text)
            .DisposeWith(d);

        EmitSignal5Button.Pressed += () => CustomSignalOwner.Call("emit_my_signal_5args");

        // 6 args
        CustomSignalOwner.ObserveSignal<string, long, double, bool, Vector2, Color>("my_signal_6args")
            .Subscribe(args =>
            {
                ViewModel!.CustomSignal6Text = $"arg1={args.Item1}, arg2={args.Item2}, arg3={args.Item3:F2}, arg4={args.Item4}, arg5={args.Item5}, arg6={args.Item6}";
                GD.Print($"[ObservableBridge] my_signal_6args: {args}");
            })
            .DisposeWith(d);

        this.OneWayBind(ViewModel, vm => vm.CustomSignal6Text, v => v.CustomSignal6Label.Text)
            .DisposeWith(d);

        EmitSignal6Button.Pressed += () => CustomSignalOwner.Call("emit_my_signal_6args");

        // 7 args
        CustomSignalOwner.ObserveSignal<string, long, double, bool, Vector2, Color, NodePath>("my_signal_7args")
            .Subscribe(args =>
            {
                ViewModel!.CustomSignal7Text = $"arg1={args.Item1}, arg2={args.Item2}, arg3={args.Item3:F2}, arg4={args.Item4}, arg5={args.Item5}, arg6={args.Item6}, arg7={args.Item7}";
                GD.Print($"[ObservableBridge] my_signal_7args: {args}");
            })
            .DisposeWith(d);

        this.OneWayBind(ViewModel, vm => vm.CustomSignal7Text, v => v.CustomSignal7Label.Text)
            .DisposeWith(d);

        EmitSignal7Button.Pressed += () => CustomSignalOwner.Call("emit_my_signal_7args");
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
