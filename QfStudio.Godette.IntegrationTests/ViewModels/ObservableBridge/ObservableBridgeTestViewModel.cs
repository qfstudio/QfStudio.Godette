using Godot;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace QfStudio.Godette.IntegrationTests.ViewModels.ObservableBridge;

public partial class ObservableBridgeTestViewModel : ViewModelBase
{
    [Reactive]
    public partial bool ToggledValue { get; set; }

    [Reactive]
    public partial double SliderValue { get; set; }

    [Reactive]
    public partial long FrameCount { get; set; }

    [Reactive]
    public partial long PhysicsFrameCount { get; set; }

    [Reactive]
    public partial long PressedCount { get; set; }

    [Reactive]
    public partial string LineEditText { get; set; } = string.Empty;

    [Reactive]
    public partial string LineEditSubmitted { get; set; } = string.Empty;

    [Reactive]
    public partial long TextEditChangeCount { get; set; }

    [Reactive]
    public partial long ItemListSelectedIndex { get; set; }

    [Reactive]
    public partial long OptionSelectedIndex { get; set; }

    [Reactive]
    public partial long TabBarTabIndex { get; set; }

    [Reactive]
    public partial long TabContainerTabIndex { get; set; }

    [Reactive]
    public partial Color ColorPickerColor { get; set; }

    [Reactive]
    public partial Color ColorPickerButtonColor { get; set; }

    [Reactive]
    public partial bool TreeItemSelected { get; set; }

    [Reactive]
    public partial long PopupSelectedId { get; set; }

    [Reactive]
    public partial string FileSelectedPath { get; set; } = string.Empty;
}
