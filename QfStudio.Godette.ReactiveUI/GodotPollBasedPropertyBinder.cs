using System.Collections.Concurrent;
using System.Reactive.Linq;
using System.Reflection;
using System.Linq.Expressions;
using ReactiveUI;

namespace QfStudio.Godette.ReactiveUI;

public sealed class GodotPollBasedPropertyBinder : ICreatesObservableForProperty
{
    private static readonly ConcurrentDictionary<(Type Type, string Property), Func<object, object?>?> GetterCache = new();

    public int GetAffinityForObject(Type type, string propertyName, bool beforeChanged = false)
    {
        if (beforeChanged) return 0;
        return typeof(Godot.GodotObject).IsAssignableFrom(type) ? 2 : 0;
    }

    public IObservable<IObservedChange<object?, object?>> GetNotificationForProperty(
        object sender, Expression expression, string propertyName,
        bool beforeChanged = false, bool suppressWarnings = false)
    {
        var getter = GetterCache.GetOrAdd((sender.GetType(), propertyName), static key =>
        {
            var property = key.Type.GetProperty(
                key.Property,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (property is null || !property.CanRead)
                return null;

            // (object o) => (object)((Type)o).Property
            var param = Expression.Parameter(typeof(object), "o");
            var cast = Expression.Convert(param, key.Type);
            var access = Expression.Property(cast, property);
            var boxed = Expression.Convert(access, typeof(object));
            return Expression.Lambda<Func<object, object?>>(boxed, param).Compile();
        });

        if (getter is null)
            return Observable.Never<IObservedChange<object?, object?>>();

        return Observable.PollEveryUpdate<object, object?>(
                sender,
                getter)
            .Select(value => new ObservedChange<object?, object?>(sender, expression, value));
    }
}
