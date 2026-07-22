using Godot;
using ReactiveUI;

namespace QfStudio.Godette.ReactiveUI;

public class GodotViewLocator : IViewLocator
{
    private readonly Dictionary<Type, Registration> _registrations = [];

    public IViewFor<TViewModel>? ResolveView<TViewModel>(string? contract = null) where TViewModel : class
    {
        if (_registrations.TryGetValue(typeof(TViewModel), out var registration))
        {
            var scene = GD.Load<PackedScene>(registration.SceneFilePath);
            return scene.Instantiate<IViewFor<TViewModel>>();
        }

        return null;
    }

    public IViewFor? ResolveView(object? instance, string? contract = null)
    {
        if (instance == null) return null;

        if (_registrations.TryGetValue(instance.GetType(), out var registration))
        {
            var scene = GD.Load<PackedScene>(registration.SceneFilePath);
            return scene.Instantiate<IViewFor>();
        }

        return null;
    }

    public void RegisterView<TView, TViewModel>(string sceneFilePath) where TView : IViewFor
    {
        _registrations[typeof(TViewModel)] = new Registration(sceneFilePath);
    }

    public void RegisterView<TViewModel>(string sceneFilePath)
    {
        _registrations[typeof(TViewModel)] = new Registration(sceneFilePath);
    }

    private record Registration(string SceneFilePath);
}
