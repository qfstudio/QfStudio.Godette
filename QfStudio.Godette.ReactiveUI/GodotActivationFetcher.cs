using ReactiveUI;

namespace QfStudio.Godette.ReactiveUI;

public class GodotActivationFetcher : IActivationForViewFetcher
{
    public int GetAffinityForView(Type view)
    {
        throw new NotImplementedException();
    }

    public IObservable<bool> GetActivationForView(IActivatableView view)
    {
        throw new NotImplementedException();
    }
}
