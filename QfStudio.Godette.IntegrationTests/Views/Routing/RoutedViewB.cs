using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using Godot;
using QfStudio.Godette.IntegrationTests.ViewModels.Routing;
using QfStudio.Godette.ReactiveUI;
using ReactiveUI;

namespace QfStudio.Godette.IntegrationTests.Views.Routing;

[SceneTree(root: "_root")]
[GodotViewFor<RoutableViewModelB>]
public partial class RoutedViewB : Control
{
    public RoutedViewB()
    {
        this.WhenActivated(d =>
        {
            this.OneWayBind(ViewModel, vm => vm.ClickCount, v => v.CountLabel.Text,
                    count => $"Click count: {count}")
                .DisposeWith(d);

            CountButton.Pressed += OnCountButtonPressed;
            Disposable.Create(() => CountButton.Pressed -= OnCountButtonPressed)
                .DisposeWith(d);
        });
    }

    private void OnCountButtonPressed()
    {
        if (ViewModel is not null)
            ViewModel.ClickCount++;
    }
}
