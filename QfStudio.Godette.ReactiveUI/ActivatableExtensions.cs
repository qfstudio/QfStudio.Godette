using System.Reactive.Disposables;
using Godot;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Numerics;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using System.Reflection;
using ReactiveUI;

namespace QfStudio.Godette.ReactiveUI;

// TODO migrate to GodotPropertyBindingHook
public static class ActivatableExtensions
{
    public static IDisposable Bind<TModel, TProperty>(TModel model, Expression<Func<TModel, TProperty>> propExpr, Godot.Range view)
        where TModel : INotifyPropertyChanged
        where TProperty : INumber<TProperty>
    {
        var disposable = new CompositeDisposable();
        var setter = CompileSetter(propExpr);

        Observable.FromEvent<Godot.Range.ValueChangedEventHandler, double>(
                h => view.ValueChanged += h,
                h => view.ValueChanged -= h)
            .Subscribe(v => setter(model, TProperty.CreateSaturating(v)))
            .DisposeWith(disposable);

        model.WhenAnyValue(propExpr)
            .ObserveOn(RxSchedulers.MainThreadScheduler)
            .Subscribe(v => view.Value = double.CreateSaturating(v))
            .DisposeWith(disposable);

        return disposable;
    }

    public static IDisposable BindTo<TModel, TProperty>(TModel model, Expression<Func<TModel, TProperty>> propExpr, Godot.Label view)
        where TModel : INotifyPropertyChanged
        where TProperty : IConvertible
    {
        var disposable = new CompositeDisposable();

        model.WhenAnyValue(propExpr)
            .ObserveOn(RxSchedulers.MainThreadScheduler)
            .Subscribe(v => view.Text = v.ToString())
            .DisposeWith(disposable);

        return disposable;
    }

    public static IDisposable Bind<TModel, TProperty>(TModel model, Expression<Func<TModel, TProperty>> propExpr, Godot.LineEdit view)
        where TModel : INotifyPropertyChanged
        where TProperty : IConvertible
    {
        var disposable = new CompositeDisposable();
        var setter = CompileSetter(propExpr);

        Observable.FromEvent<Godot.LineEdit.TextChangedEventHandler, string>(
                h => view.TextChanged += h,
                h => view.TextChanged -= h)
            .Subscribe(v => setter(model, (TProperty)Convert.ChangeType(v, typeof(TProperty))))
            .DisposeWith(disposable);

        model.WhenAnyValue(propExpr)
            .ObserveOn(RxSchedulers.MainThreadScheduler)
            .Where(v => view.Text != v.ToString())
            .Subscribe(v => view.Text = v.ToString())
            .DisposeWith(disposable);

        return disposable;
    }

    private static Action<TModel, TProperty> CompileSetter<TModel, TProperty>(Expression<Func<TModel, TProperty>> propExpr)
    {
        var memberExpr = (MemberExpression)propExpr.Body;
        var propInfo = (PropertyInfo)memberExpr.Member;
        var param = System.Linq.Expressions.Expression.Parameter(typeof(TModel), "obj");
        var valueParam = System.Linq.Expressions.Expression.Parameter(typeof(TProperty), "value");
        return System.Linq.Expressions.Expression.Lambda<Action<TModel, TProperty>>(
            System.Linq.Expressions.Expression.Assign(
                System.Linq.Expressions.Expression.Property(param, propInfo), valueParam),
            param, valueParam).Compile();
    }
}
