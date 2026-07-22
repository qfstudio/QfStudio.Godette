namespace QfStudio.Godette.ReactiveUI;

public static class GodotSchedulers
{
    public static GodotMainThreadScheduler MainThreadScheduler
    {
        get => field ?? throw new NullReferenceException();
        set;
    }

    public static GodotFrameScheduler ProcessFrameScheduler
    {
        get => field ?? throw new NullReferenceException();
        set;
    }

    public static GodotFrameScheduler PhysicsFrameScheduler
    {
        get => field ?? throw new NullReferenceException();
        set;
    }
}
