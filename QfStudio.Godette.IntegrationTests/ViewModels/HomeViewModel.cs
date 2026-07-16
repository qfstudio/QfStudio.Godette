using System.Reactive;
using System.Reactive.Linq;
using Godot;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace QfStudio.Godette.IntegrationTests.ViewModels;

public partial class HomeViewModel : ReactiveObject
{
    [Reactive]
    public partial string Name { get; set; } = "World";

    [Reactive]
    public partial double Score { get; set; }

    public ReactiveCommand<Unit, Unit> SubmitCommand { get; }
    public ReactiveCommand<Unit, Unit> FetchCommand { get; }

    public HomeViewModel()
    {
        var canSubmit = this.WhenAnyValue(x => x.Name)
            .Select(name => !string.IsNullOrEmpty(name));

        SubmitCommand = ReactiveCommand.Create(() =>
        {
            GD.Print($"[Command] Submit: Name={Name}, Score={Score}");
        }, canSubmit);

        FetchCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            GD.Print("[Command] Fetch: started");
            await System.Threading.Tasks.Task.Delay(2000);
            GD.Print("[Command] Fetch: completed");
        });
    }
}
