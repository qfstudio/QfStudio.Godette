using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using Godot;
using QfStudio.Godette.IntegrationTests.ViewModels.Routing;
using QfStudio.Godette.ReactiveUI;
using ReactiveUI;

namespace QfStudio.Godette.IntegrationTests.Views.Routing;

[SceneTree(root: "_root")]
[GodotViewFor<RoutableViewModelA>]
public partial class RoutedViewA : Control
{
    public RoutedViewA()
    {
        this.WhenActivated(d =>
        {
            this.WhenAnyValue(x => x.ViewModel!.Message, x => x.ViewModel!.Id)
                .Select(((string message, int id) tuple) => $"{tuple.message} (Page id: #{tuple.id})")
                .BindTo(this, x => x.MessageLabel.Text)
                .DisposeWith(d);
        });
    }
}
