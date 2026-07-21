using System.Reactive;
using System.Reactive.Linq;
using Godot;

namespace QfStudio.Godette.ReactiveUI;

public static class GodotObservableExtensions
{
    public static IObservable<bool> ObserveToggled(this BaseButton button) =>
        Observable.FromEvent<BaseButton.ToggledEventHandler, bool>(
            h => button.Toggled += h,
            h => button.Toggled -= h);

    public static IObservable<Unit> ObservePressed(this BaseButton button) =>
        Observable.FromEvent(
            h => button.Pressed += h,
            h => button.Pressed -= h);

    public static IObservable<double> ObserveValueChanged(this Godot.Range range) =>
        Observable.FromEvent<Godot.Range.ValueChangedEventHandler, double>(
            h => range.ValueChanged += h,
            h => range.ValueChanged -= h);

    public static IObservable<string> ObserveTextChanged(this LineEdit lineEdit) =>
        Observable.FromEvent<LineEdit.TextChangedEventHandler, string>(
            h => lineEdit.TextChanged += h,
            h => lineEdit.TextChanged -= h);

    public static IObservable<string> ObserveTextSubmitted(this LineEdit lineEdit) =>
        Observable.FromEvent<LineEdit.TextSubmittedEventHandler, string>(
            h => lineEdit.TextSubmitted += h,
            h => lineEdit.TextSubmitted -= h);

    public static IObservable<Unit> ObserveTextChanged(this TextEdit textEdit) =>
        Observable.FromEvent(
            h => textEdit.TextChanged += h,
            h => textEdit.TextChanged -= h);

    public static IObservable<long> ObserveItemSelected(this ItemList itemList) =>
        Observable.FromEvent<ItemList.ItemSelectedEventHandler, long>(
            h => itemList.ItemSelected += h,
            h => itemList.ItemSelected -= h);

    public static IObservable<long> ObserveItemSelected(this OptionButton optionButton) =>
        Observable.FromEvent<OptionButton.ItemSelectedEventHandler, long>(
            h => optionButton.ItemSelected += h,
            h => optionButton.ItemSelected -= h);

    public static IObservable<long> ObserveTabChanged(this TabBar tabBar) =>
        Observable.FromEvent<TabBar.TabChangedEventHandler, long>(
            h => tabBar.TabChanged += h,
            h => tabBar.TabChanged -= h);

    public static IObservable<long> ObserveTabChanged(this TabContainer tabContainer) =>
        Observable.FromEvent<TabContainer.TabChangedEventHandler, long>(
            h => tabContainer.TabChanged += h,
            h => tabContainer.TabChanged -= h);

    public static IObservable<Color> ObserveColorChanged(this ColorPicker picker) =>
        Observable.FromEvent<ColorPicker.ColorChangedEventHandler, Color>(
            h => picker.ColorChanged += h,
            h => picker.ColorChanged -= h);

    public static IObservable<Color> ObserveColorChanged(this ColorPickerButton button) =>
        Observable.FromEvent<ColorPickerButton.ColorChangedEventHandler, Color>(
            h => button.ColorChanged += h,
            h => button.ColorChanged -= h);

    public static IObservable<Unit> ObserveItemSelected(this Tree tree) =>
        Observable.FromEvent(
            h => tree.ItemSelected += h,
            h => tree.ItemSelected -= h);

    public static IObservable<long> ObserveIdPressed(this PopupMenu popupMenu) =>
        Observable.FromEvent<PopupMenu.IdPressedEventHandler, long>(
            h => popupMenu.IdPressed += h,
            h => popupMenu.IdPressed -= h);

    public static IObservable<string> ObserveFileSelected(this FileDialog dialog) =>
        Observable.FromEvent<FileDialog.FileSelectedEventHandler, string>(
            h => dialog.FileSelected += h,
            h => dialog.FileSelected -= h);

    public static IObservable<Unit> ObserveProcessFrame(this SceneTree tree) =>
        Observable.FromEvent(
            h => tree.ProcessFrame += h,
            h => tree.ProcessFrame -= h);

    public static IObservable<Unit> ObservePhysicsFrame(this SceneTree tree) =>
        Observable.FromEvent(
            h => tree.PhysicsFrame += h,
            h => tree.PhysicsFrame -= h);
}
