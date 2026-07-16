using Godot;

namespace QfStudio.Godette.ReactiveUI;

public static class ActivationContextProvider
{
    private static GodotSynchronizationContext? _uiContext;

    public static GodotSynchronizationContext UiContext => _uiContext ?? throw new NullReferenceException();

    public static void RegisterGodotContext(GodotSynchronizationContext context)
    {
        _uiContext = context;
    }
}
