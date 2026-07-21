using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace QfStudio.Godette.IntegrationTests.ViewModels.Interaction;

public partial class InteractionTestViewModel : ViewModelBase
{
    public Interaction<string, bool> ConfirmDelete { get; } = new();
    public Interaction<string, Unit> ShowAlert { get; } = new();

    public ReactiveCommand<Unit, Unit> DeleteCommand { get; }
    public ReactiveCommand<Unit, Unit> AlertCommand { get; }

    [Reactive]
    public partial string ResultText { get; set; } = string.Empty;

    public InteractionTestViewModel()
    {
        DeleteCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var confirmed = await ConfirmDelete.Handle("Confirm to delete?");
            ResultText = confirmed ? "Confirmed" : "Canceled";
        });

        AlertCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            await ShowAlert.Handle("This is just a tip.");
            ResultText = "The tip message has been closed.";
        });
    }
}
