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
}
