using System.Reactive.Disposables;
using Godot;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Numerics;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using System.Reflection;
using ReactiveUI;

namespace QfStudio.Godotte.ReactiveUI;

public static class ActivatableExtensions
{
    extension(IActivatable activation)
    {
        public IDisposable Bind<TModel, TProperty>(TModel model, Expression<Func<TModel, TProperty>> propExpr, Godot.Range view)
            where TModel : INotifyPropertyChanged
            where TProperty : INumber<TProperty>
        {
            var disposable = new CompositeDisposable();

            var memberExpr = (MemberExpression)propExpr.Body;
            var propInfo = (PropertyInfo)memberExpr.Member;

            Observable.FromEvent<Godot.Range.ValueChangedEventHandler, double>(
                    h => view.ValueChanged += h,
                    h => view.ValueChanged -= h)
                .Subscribe(v =>
                {
                    propInfo.SetValue(model, v);
                })
                .DisposeWith(disposable);

            model.WhenAnyValue(propExpr)
                .ObserveOn(activation.UiContext)
                .Subscribe(v =>
                {
                    view.Value = double.CreateSaturating<TProperty>(v);
                })
                .DisposeWith(disposable);

            return disposable;
        }

        public IDisposable BindTo<TModel, TProperty>(TModel model, Expression<Func<TModel, TProperty>> propExpr, Godot.Label view)
            where TModel : INotifyPropertyChanged
            where TProperty : IConvertible
        {
            var disposable = new CompositeDisposable();

            model.WhenAnyValue(propExpr)
                .ObserveOn(activation.UiContext)
                .Subscribe(v =>
                {
                    view.Text = v.ToString();
                })
                .DisposeWith(disposable);

            return disposable;
        }

        public IDisposable Bind<TModel, TProperty>(TModel model, Expression<Func<TModel, TProperty>> propExpr, Godot.LineEdit view)
            where TModel : INotifyPropertyChanged
            where TProperty : IConvertible
        {
            var disposable = new CompositeDisposable();

            var memberExpr = (MemberExpression)propExpr.Body;
            var propInfo = (PropertyInfo)memberExpr.Member;

            Observable.FromEvent<Godot.LineEdit.TextChangedEventHandler, string>(
                    h => view.TextChanged += h,
                    h => view.TextChanged -= h)
                .Subscribe(v =>
                {
                    GD.Print("text changed!");
                    propInfo.SetValue(model, v);
                })
                .DisposeWith(disposable);

            model.WhenAnyValue(propExpr)
                .ObserveOn(activation.UiContext)
                .Where(v => view.Text != v.ToString())
                .Subscribe(v =>
                {
                    view.Text = v.ToString();
                })
                .DisposeWith(disposable);

            return disposable;
        }
    }
}
