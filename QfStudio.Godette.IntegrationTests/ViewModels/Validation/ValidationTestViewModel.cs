using ReactiveUI.SourceGenerators;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Contexts;
using ReactiveUI.Validation.Extensions;

namespace QfStudio.Godette.IntegrationTests.ViewModels.Validation;

public partial class ValidationTestViewModel : ViewModelBase, IValidatableViewModel
{
    [Reactive]
    public partial string Name { get; set; } = "";

    [Reactive]
    public partial string Email { get; set; } = "";

    public IValidationContext ValidationContext { get; } = new ValidationContext();

    public ValidationTestViewModel()
    {
        this.ValidationRule(
            vm => vm.Name,
            name => !string.IsNullOrWhiteSpace(name) && name.Length >= 2,
            "Name must be at least 2 characters.");

        this.ValidationRule(
            vm => vm.Email,
            email => !string.IsNullOrWhiteSpace(email) && email.Contains('@'),
            "Email must contain '@'.");

        this.ValidationRule(
            vm => vm.Email,
            email => email?.EndsWith(".net") ?? false,
            "Email must end with '.net'.");
    }
}
