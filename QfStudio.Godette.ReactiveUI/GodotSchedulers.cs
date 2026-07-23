using ReactiveUI;

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

public static class RxSchedulersExtensions
{
    extension(RxSchedulers)
    {
        public static GodotMainThreadScheduler GodotMainThreadScheduler => GodotSchedulers.MainThreadScheduler;
        public static GodotFrameScheduler ProcessFrameScheduler => GodotSchedulers.ProcessFrameScheduler;
        public static GodotFrameScheduler PhysicsFrameScheduler => GodotSchedulers.PhysicsFrameScheduler;
    }
}
