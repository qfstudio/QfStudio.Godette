using System.Linq.Expressions;
using ReactiveUI;

namespace QfStudio.Godette.ReactiveUI;

public class GodotPropertyBindingHook : ICreatesObservableForProperty
{
    public int GetAffinityForObject(Type type, string propertyName, bool beforeChanged = false)
    {
        throw new NotImplementedException();
    }

    public IObservable<IObservedChange<object?, object?>> GetNotificationForProperty(object sender, Expression expression, string propertyName,
        bool beforeChanged = false, bool suppressWarnings = false)
    {
        throw new NotImplementedException();
    }
}
