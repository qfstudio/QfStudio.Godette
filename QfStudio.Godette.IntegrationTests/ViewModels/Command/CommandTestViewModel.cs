using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Godot;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace QfStudio.Godette.IntegrationTests.ViewModels.Command;

public partial class CommandTestViewModel : ReactiveObject
{
    [Reactive]
    public partial string QueryString { get; set; } = "World";

    [Reactive]
    public partial bool ConditionalCommandEnabled { get; set; } = false;

    public ReactiveCommand<Unit, Unit> SimpleCommand { get; }
    public ReactiveCommand<Unit, Unit> AsyncCommand { get; }
    public ReactiveCommand<string, Unit> SearchCommand { get; }

    public ReactiveCommand<Unit, Unit> ConditionalCommand { get; }

    public CommandTestViewModel()
    {
        SimpleCommand = ReactiveCommand.Create(() =>
        {
            GD.Print($"[Command] Simple command");
        });

        AsyncCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            GD.Print("[Command] Async command: started");
            await Task.Delay(2000);
            GD.Print("[Command] Async command: completed");
        });

        SearchCommand = ReactiveCommand.CreateFromTask<string>(async query =>
        {
            GD.Print($"[Command] Search: query='{query}'");
            await Task.Delay(300);
        });

        var commandEnabled = this.WhenAnyValue(x => x.ConditionalCommandEnabled);
        ConditionalCommand = ReactiveCommand.Create(() =>
        {
            GD.Print($"[Command] Conditional command");
        }, commandEnabled);
    }
}
