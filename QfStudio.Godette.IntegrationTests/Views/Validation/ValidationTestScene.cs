using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using Godot;
using QfStudio.Godette.IntegrationTests.ViewModels.Validation;
using QfStudio.Godette.ReactiveUI;
using ReactiveUI;
using ReactiveUI.Validation.Extensions;

namespace QfStudio.Godette.IntegrationTests.Views.Validation;

[SceneTree(root: "_root")]
[GodotViewFor<ValidationTestViewModel>]
public partial class ValidationTestScene : Control
{
    public ValidationTestScene()
    {
        this.WhenActivated(d =>
        {
            this.Bind(ViewModel, vm => vm.Name, v => v.NameEdit.Text)
                .DisposeWith(d);

            this.Bind(ViewModel, vm => vm.Email, v => v.EmailEdit.Text)
                .DisposeWith(d);

            this.BindValidation(ViewModel, vm => vm.Name, v => v.NameErrorLabel.Text)
                .DisposeWith(d);

            this.BindValidation(ViewModel, vm => vm.Email, v => v.EmailErrorLabel.Text)
                .DisposeWith(d);

            this.WhenAnyValue(x => x.ViewModel!.ValidationContext.Valid)
                .Switch()
                .Select(x => !x)
                .BindTo(this, x => x.SubmitButton.Disabled)
                .DisposeWith(d);
        });
    }

    public override void _Ready()
    {
        ViewModel = new ValidationTestViewModel();

        BackButton.Pressed += () => GetTree().ChangeSceneToFile(HomeScene.TscnFilePath);
        SubmitButton.Pressed += OnSubmit;
    }

    private void OnSubmit()
    {
        var statusText = $"Submitted: email={ViewModel!.Email} name={ViewModel.Name}";
        StatusLabel.Text = statusText;
    }
}
