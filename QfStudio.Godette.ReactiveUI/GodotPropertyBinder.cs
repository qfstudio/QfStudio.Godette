using System.Linq.Expressions;
using System.Reactive.Linq;
using ReactiveUI;

namespace QfStudio.Godette.ReactiveUI;

public class GodotPropertyBinder : ICreatesObservableForProperty
{
    public int GetAffinityForObject(Type type, string propertyName, bool beforeChanged = false)
    {
        if (beforeChanged) return 0;

        return propertyName switch
        {
            nameof(Godot.Range.Value) when typeof(Godot.Range).IsAssignableFrom(type) => 10,
            nameof(Godot.LineEdit.Text) when typeof(Godot.LineEdit).IsAssignableFrom(type) => 10,
            nameof(Godot.TextEdit.Text) when typeof(Godot.TextEdit).IsAssignableFrom(type) => 10,
            nameof(Godot.BaseButton.ButtonPressed) when typeof(Godot.BaseButton).IsAssignableFrom(type) => 10,
            nameof(Godot.TabContainer.CurrentTab) when typeof(Godot.TabContainer).IsAssignableFrom(type) => 10,
            nameof(Godot.OptionButton.Selected) when typeof(Godot.OptionButton).IsAssignableFrom(type) => 10,
            nameof(Godot.TabBar.CurrentTab) when typeof(Godot.TabBar).IsAssignableFrom(type) => 10,
            nameof(Godot.ColorPicker.Color) when typeof(Godot.ColorPicker).IsAssignableFrom(type) => 10,
            nameof(Godot.ColorPickerButton.Color) when typeof(Godot.ColorPickerButton).IsAssignableFrom(type) => 10,

            _ => 0
        };
    }

    public IObservable<IObservedChange<object?, object?>> GetNotificationForProperty(
        object sender, Expression expression, string propertyName,
        bool beforeChanged = false, bool suppressWarnings = false)
    {
        return propertyName switch
        {
            nameof(Godot.Range.Value) when sender is Godot.Range range =>
                Observable.FromEvent<Godot.Range.ValueChangedEventHandler, double>(
                        h => range.ValueChanged += h,
                        h => range.ValueChanged -= h)
                    .Select(v => new ObservedChange<object?, object?>(sender, expression, v)),

            nameof(Godot.LineEdit.Text) when sender is Godot.LineEdit lineEdit =>
                Observable.FromEvent<Godot.LineEdit.TextChangedEventHandler, string>(
                        h => lineEdit.TextChanged += h,
                        h => lineEdit.TextChanged -= h)
                    .Select(v => new ObservedChange<object?, object?>(sender, expression, v)),

            nameof(Godot.TextEdit.Text) when sender is Godot.TextEdit textEdit =>
                Observable.FromEvent(
                        h => textEdit.TextChanged += h,
                        h => textEdit.TextChanged -= h)
                    .Select(_ => new ObservedChange<object?, object?>(sender, expression, textEdit.Text)),

            nameof(Godot.BaseButton.ButtonPressed) when sender is Godot.BaseButton button =>
                Observable.FromEvent<Godot.BaseButton.ToggledEventHandler, bool>(
                        h => button.Toggled += h,
                        h => button.Toggled -= h)
                    .Select(v => new ObservedChange<object?, object?>(sender, expression, v)),

            nameof(Godot.TabContainer.CurrentTab) when sender is Godot.TabContainer tabContainer =>
                Observable.FromEvent<Godot.TabContainer.TabChangedEventHandler, long>(
                        h => tabContainer.TabChanged += h,
                        h => tabContainer.TabChanged -= h)
                    .Select(idx => new ObservedChange<object?, object?>(sender, expression, (int)idx)),

            nameof(Godot.OptionButton.Selected) when sender is Godot.OptionButton optionButton =>
                Observable.FromEvent<Godot.OptionButton.ItemSelectedEventHandler, long>(
                        h => optionButton.ItemSelected += h,
                        h => optionButton.ItemSelected -= h)
                    .Select(idx => new ObservedChange<object?, object?>(sender, expression, (int)idx)),

            nameof(Godot.TabBar.CurrentTab) when sender is Godot.TabBar tabBar =>
                Observable.FromEvent<Godot.TabBar.TabChangedEventHandler, long>(
                        h => tabBar.TabChanged += h,
                        h => tabBar.TabChanged -= h)
                    .Select(tab => new ObservedChange<object?, object?>(sender, expression, (int)tab)),

            nameof(Godot.ColorPicker.Color) when sender is Godot.ColorPicker colorPicker =>
                Observable.FromEvent<Godot.ColorPicker.ColorChangedEventHandler, Godot.Color>(
                        h => colorPicker.ColorChanged += h,
                        h => colorPicker.ColorChanged -= h)
                    .Select(c => new ObservedChange<object?, object?>(sender, expression, c)),

            nameof(Godot.ColorPickerButton.Color) when sender is Godot.ColorPickerButton colorPickerButton =>
                Observable.FromEvent<Godot.ColorPickerButton.ColorChangedEventHandler, Godot.Color>(
                        h => colorPickerButton.ColorChanged += h,
                        h => colorPickerButton.ColorChanged -= h)
                    .Select(c => new ObservedChange<object?, object?>(sender, expression, c)),

            _ => Observable.Never<IObservedChange<object?, object?>>()
        };
    }
}
