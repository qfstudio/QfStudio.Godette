using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using System.Text;
using Godot;
using QfStudio.Godette.IntegrationTests.ViewModels.Misc;
using QfStudio.Godette.ReactiveUI;
using ReactiveUI;

namespace QfStudio.Godette.IntegrationTests.Views.Misc;

internal enum BindingMethods
{
    Bind,
    BindToTargetAtViewMoel,
    BindToTargetAtThis,
    ManualPropertyAccess,
    ManualDisposable,
}

[SceneTree(root: "_root")]
[GodotViewFor<MiscTestViewModel>]
public partial class MiscTestScene : Control
{
    private readonly List<MiscTestViewModel?> _vms = [];
    private readonly BindingMethods _wayToBind = BindingMethods.ManualDisposable;

    public MiscTestScene()
    {
        this.WhenActivated(d =>
        {
            // Change view model every 3 seconds. Changing a view model won't trigger view's reactivation.
            Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(3))
                .ObserveOn(RxSchedulers.MainThreadScheduler)
                .Subscribe(i =>
                {
                    var vm = i % 2 == 0 ? new MiscTestViewModel() : null;
                    _vms.Add(vm);
                    ViewModel = vm;
                })
                .DisposeWith(d);

            switch (_wayToBind)
            {
                case BindingMethods.Bind:
                    // `this.Bind` [OK]
                    // This always binds to the lastest view model and cancels previous subscription
                    this.Bind(ViewModel, vm => vm.Text, v => v.TestLineEdit.Text)
                        .DisposeWith(d);
                    break;

                case BindingMethods.BindToTargetAtViewMoel:
                    // `IObservable.BindTo` @ ViewModel [FAIL]
                    // It throws System.ArgumentNullException: Value cannot be null. (Parameter 'target')
                    // `BindTo` is just a shorthand for `value => target.Property = value`
                    this.WhenAnyValue(v => v.TestLineEdit.Text)
                        .BindTo(ViewModel, vm => vm.Text)
                        .DisposeWith(d);
                    break;

                case BindingMethods.BindToTargetAtThis:
                    // `IObservable.BindTo` @ this [OK]
                    // `BindTo` handles null in property chain like `this.Bind`
                    this.WhenAnyValue(v => v.TestLineEdit.Text)
                        .BindTo(this, v => v.ViewModel!.Text)
                        .DisposeWith(d);
                    break;

                case BindingMethods.ManualPropertyAccess:
                    // This is also OK, because `?.` is just a property access.
                    this.WhenAnyValue(x => x.TestLineEdit.Text)
                        .Subscribe(text =>
                        {
                            ViewModel?.Text = text;
                        })
                        .DisposeWith(d);
                    break;

                case BindingMethods.ManualDisposable:
                    // It works, but a little overcomplicated.
                    // Remember to cancel previous subscription.
                    var bindingsDisposable = new CompositeDisposable().DisposeWith(d);
                    this.WhenAnyValue(v => v.ViewModel)
                        .Subscribe(vm =>
                        {
                            bindingsDisposable.Clear();

                            if (vm is null)
                            {
                                return;
                            }

                            TestLineEdit.ObserveTextChanged()
                                .Subscribe(text =>
                                {
                                    vm.Text = text;
                                })
                                .DisposeWith(bindingsDisposable);
                        })
                        .DisposeWith(d);
                    break;
            }
        });
    }

    public override void _Ready()
    {
        DumpButton.Pressed += DumpViewModels;
    }

    private void DumpViewModels()
    {
        var sb = new StringBuilder();
        for (var i = 0; i < _vms.Count; i++)
        {
            sb.Append($"#{i} {_vms[i]?.Text ?? "(null)"}\n");
        }

        var result = sb.ToString();
        ResultLabel.Text = result;
        GD.Print(result);
    }
}
