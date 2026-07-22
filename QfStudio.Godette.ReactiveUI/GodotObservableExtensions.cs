using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Godot;

namespace QfStudio.Godette.ReactiveUI;

public static class GodotObservableExtensions
{
    extension(GodotObject target)
    {
        private IDisposable ConnectSignal(StringName signal, Callable callable)
        {
            target.Connect(signal, callable);
            return Disposable.Create(() =>
            {
                if (GodotObject.IsInstanceValid(target))
                    target.Disconnect(signal, callable);
            });
        }

        public IObservable<Unit> ObserveSignal(StringName signal) =>
            Observable.Create<Unit>(observer =>
                target.ConnectSignal(signal, Callable.From(() => observer.OnNext(Unit.Default))));

        public IObservable<ValueTuple<T1>> ObserveSignal<T1>(StringName signal) =>
            Observable.Create<ValueTuple<T1>>(observer =>
                target.ConnectSignal(signal, Callable.From((T1 a1) => observer.OnNext(ValueTuple.Create(a1)))));

        public IObservable<ValueTuple<T1, T2>> ObserveSignal<T1, T2>(StringName signal) =>
            Observable.Create<ValueTuple<T1, T2>>(observer =>
                target.ConnectSignal(signal, Callable.From((T1 a1, T2 a2) => observer.OnNext(ValueTuple.Create(a1, a2)))));

        public IObservable<ValueTuple<T1, T2, T3>> ObserveSignal<T1, T2, T3>(StringName signal) =>
            Observable.Create<ValueTuple<T1, T2, T3>>(observer =>
                target.ConnectSignal(signal, Callable.From((T1 a1, T2 a2, T3 a3) => observer.OnNext(ValueTuple.Create(a1, a2, a3)))));

        public IObservable<ValueTuple<T1, T2, T3, T4>> ObserveSignal<T1, T2, T3, T4>(StringName signal) =>
            Observable.Create<ValueTuple<T1, T2, T3, T4>>(observer =>
                target.ConnectSignal(signal, Callable.From((T1 a1, T2 a2, T3 a3, T4 a4) => observer.OnNext(ValueTuple.Create(a1, a2, a3, a4)))));

        public IObservable<ValueTuple<T1, T2, T3, T4, T5>> ObserveSignal<T1, T2, T3, T4, T5>(StringName signal) =>
            Observable.Create<ValueTuple<T1, T2, T3, T4, T5>>(observer =>
                target.ConnectSignal(signal, Callable.From((T1 a1, T2 a2, T3 a3, T4 a4, T5 a5) => observer.OnNext(ValueTuple.Create(a1, a2, a3, a4, a5)))));

        public IObservable<ValueTuple<T1, T2, T3, T4, T5, T6>> ObserveSignal<T1, T2, T3, T4, T5, T6>(StringName signal) =>
            Observable.Create<ValueTuple<T1, T2, T3, T4, T5, T6>>(observer =>
                target.ConnectSignal(signal, Callable.From((T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6) => observer.OnNext(ValueTuple.Create(a1, a2, a3, a4, a5, a6)))));

        public IObservable<ValueTuple<T1, T2, T3, T4, T5, T6, T7>> ObserveSignal<T1, T2, T3, T4, T5, T6, T7>(StringName signal) =>
            Observable.Create<ValueTuple<T1, T2, T3, T4, T5, T6, T7>>(observer =>
                target.ConnectSignal(signal, Callable.From((T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6, T7 a7) => observer.OnNext(ValueTuple.Create(a1, a2, a3, a4, a5, a6, a7)))));
    }

    extension(BaseButton button)
    {
        public IObservable<bool> ObserveToggled() =>
            Observable.FromEvent<BaseButton.ToggledEventHandler, bool>(
                h => button.Toggled += h,
                h => button.Toggled -= h);

        public IObservable<Unit> ObservePressed() =>
            Observable.FromEvent(
                h => button.Pressed += h,
                h => button.Pressed -= h);
    }

    public static IObservable<double> ObserveValueChanged(this Godot.Range range) =>
        Observable.FromEvent<Godot.Range.ValueChangedEventHandler, double>(
            h => range.ValueChanged += h,
            h => range.ValueChanged -= h);

    extension(LineEdit lineEdit)
    {
        public IObservable<string> ObserveTextChanged() =>
            Observable.FromEvent<LineEdit.TextChangedEventHandler, string>(
                h => lineEdit.TextChanged += h,
                h => lineEdit.TextChanged -= h);

        public IObservable<string> ObserveTextSubmitted() =>
            Observable.FromEvent<LineEdit.TextSubmittedEventHandler, string>(
                h => lineEdit.TextSubmitted += h,
                h => lineEdit.TextSubmitted -= h);
    }

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

    extension(SceneTree tree)
    {
        public IObservable<Unit> ObserveProcessFrame() =>
            Observable.FromEvent(
                h => tree.ProcessFrame += h,
                h => tree.ProcessFrame -= h);

        public IObservable<Unit> ObservePhysicsFrame() =>
            Observable.FromEvent(
                h => tree.PhysicsFrame += h,
                h => tree.PhysicsFrame -= h);
    }
}
