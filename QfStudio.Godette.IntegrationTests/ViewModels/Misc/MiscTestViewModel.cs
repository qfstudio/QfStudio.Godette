using ReactiveUI.SourceGenerators;

namespace QfStudio.Godette.IntegrationTests.ViewModels.Misc;

public partial class MiscTestViewModel : ViewModelBase
{
    [Reactive]
    public partial string Text { get; set; } = string.Empty;
}
