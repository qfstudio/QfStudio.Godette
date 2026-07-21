using System.Reactive;
using System.Reactive.Disposables.Fluent;
using System.Threading.Tasks;
using Godot;
using QfStudio.Godette.IntegrationTests.ViewModels.Interaction;
using QfStudio.Godette.ReactiveUI;
using ReactiveUI;

namespace QfStudio.Godette.IntegrationTests.Views.Interaction;

[SceneTree(root: "_root")]
[GodotViewFor<InteractionTestViewModel>]
public partial class InteractionTestScene : Control
{
    public InteractionTestScene()
    {
        this.WhenActivated(d =>
        {
            this.BindInteraction(ViewModel, vm => vm.ConfirmDelete, CreateConfirmationHandler(ConfirmDialog))
                .DisposeWith(d);

            this.BindInteraction(ViewModel, vm => vm.ShowAlert, CreateAlertHandler(AlertDialog))
                .DisposeWith(d);

            this.BindCommand(ViewModel, vm => vm.DeleteCommand, v => v.DeleteButton)
                .DisposeWith(d);

            this.BindCommand(ViewModel, vm => vm.AlertCommand, v => v.AlertButton)
                .DisposeWith(d);

            this.BindCommand(ViewModel, vm => vm.OpenFileCommand, v => v.OpenFileButton)
                .DisposeWith(d);

            this.BindInteraction(ViewModel, vm => vm.OpenFile, CreateFilePickerHandler(FileDialog))
                .DisposeWith(d);

            this.OneWayBind(ViewModel, vm => vm.ResultText, v => v.ResultLabel.Text)
                .DisposeWith(d);
        });
    }

    public override void _Ready()
    {
        ViewModel = new InteractionTestViewModel();

        BackButton.Pressed += () => GetTree().ChangeSceneToFile(HomeScene.TscnFilePath);
    }

    private static Func<IInteractionContext<string, bool>, Task> CreateConfirmationHandler(ConfirmationDialog dialog)
    {
        return async context =>
        {
            dialog.DialogText = context.Input;

            var tcs = new TaskCompletionSource<bool>();
            dialog.Confirmed += OnConfirmed;
            dialog.Canceled += OnCanceled;

            dialog.PopupCentered();

            try
            {
                context.SetOutput(await tcs.Task);
            }
            finally
            {
                dialog.Confirmed -= OnConfirmed;
                dialog.Canceled -= OnCanceled;
            }

            return;

            void OnConfirmed() => tcs.TrySetResult(true);
            void OnCanceled() => tcs.TrySetResult(false);
        };
    }

    private static Func<IInteractionContext<string, Unit>, Task> CreateAlertHandler(AcceptDialog dialog)
    {
        return async context =>
        {
            dialog.DialogText = context.Input;

            var tcs = new TaskCompletionSource<bool>();
            dialog.Confirmed += OnConfirmed;
            dialog.Canceled += OnCanceled;

            dialog.PopupCentered();

            try
            {
                await tcs.Task;
            }
            finally
            {
                dialog.Confirmed -= OnConfirmed;
                dialog.Canceled -= OnCanceled;
            }

            context.SetOutput(Unit.Default);
            return;

            void OnConfirmed() => tcs.TrySetResult(true);
            void OnCanceled() => tcs.TrySetResult(false);
        };
    }

    private static Func<IInteractionContext<FileDialogConfig, string?>, Task> CreateFilePickerHandler(FileDialog dialog)
    {
        return async context =>
        {
            var config = context.Input;
            dialog.FileMode = config.FileMode;

            if (config.Title is not null)
            {
                dialog.Title = config.Title;
            }

            if (config.Filters is not null)
            {
                dialog.Filters = config.Filters;
            }

            var tcs = new TaskCompletionSource<string?>();
            dialog.FileSelected += OnFileSelected;
            dialog.Canceled += OnCanceled;

            dialog.PopupCentered();

            try
            {
                context.SetOutput(await tcs.Task);
            }
            finally
            {
                dialog.FileSelected -= OnFileSelected;
                dialog.Canceled -= OnCanceled;
            }

            return;

            void OnFileSelected(string path) => tcs.TrySetResult(path);
            void OnCanceled() => tcs.TrySetResult(null);
        };
    }
}
