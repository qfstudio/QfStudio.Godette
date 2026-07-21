using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using Godot;

namespace QfStudio.Godette.IntegrationTests.ViewModels.Interaction;

public record FileDialogConfig(
    FileDialog.FileModeEnum FileMode = FileDialog.FileModeEnum.OpenFile,
    string? Title = null,
    string[]? Filters = null);

public partial class InteractionTestViewModel : ViewModelBase
{
    public Interaction<string, bool> ConfirmDelete { get; } = new();
    public Interaction<string, Unit> ShowAlert { get; } = new();
    public Interaction<FileDialogConfig, string?> OpenFile { get; } = new();

    public ReactiveCommand<Unit, Unit> DeleteCommand { get; }
    public ReactiveCommand<Unit, Unit> AlertCommand { get; }
    public ReactiveCommand<Unit, Unit> OpenFileCommand { get; }

    [Reactive]
    public partial string ResultText { get; set; } = "The result of interactions will be shown here.";

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

        OpenFileCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var config = new FileDialogConfig(
                FileMode: FileDialog.FileModeEnum.OpenFile,
                Title: "Select a file",
                Filters: new[] { "*.txt;Text files", "*.cs;C# files" });
            var filePath = await OpenFile.Handle(config);
            ResultText = filePath ?? "No file selected";
        });
    }
}
