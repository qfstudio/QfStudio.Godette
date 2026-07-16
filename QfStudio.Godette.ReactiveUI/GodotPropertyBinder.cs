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
            _ => 0
        };
    }

    public IObservable<IObservedChange<object?, object?>> GetNotificationForProperty(
        object sender, System.Linq.Expressions.Expression expression, string propertyName,
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

            nameof(Godot.BaseButton.ButtonPressed) when sender is Godot.BaseButton button =>
                Observable.FromEvent<Godot.BaseButton.ToggledEventHandler, bool>(
                        h => button.Toggled += h,
                        h => button.Toggled -= h)
                    .Select(v => new ObservedChange<object?, object?>(sender, expression, v)),

            _ => Observable.Never<IObservedChange<object?, object?>>()
        };
    }
}
