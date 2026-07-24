using System.Reactive;
using System.Reactive.Linq;
using QfStudio.Godette.ReactiveUI.Operators;

namespace QfStudio.Godette.ReactiveUI;

public static class FrameObservableExtensions
{
    extension(Observable)
    {
        public static IObservable<Unit> EveryUpdate(
            GodotFrameScheduler? scheduler = null)
        {
            scheduler ??= GodotSchedulers.ProcessFrameScheduler;
            return new EveryUpdate(scheduler);
        }

        public static IObservable<Unit> AfterFrame(int dueFrameCount,
            GodotFrameScheduler? scheduler = null)
        {
            scheduler ??= GodotSchedulers.ProcessFrameScheduler;
            return new AfterFrame(dueFrameCount, scheduler);
        }

        public static IObservable<Unit> IntervalFrame(int periodFrameCount,
            GodotFrameScheduler? scheduler = null)
        {
            scheduler ??= GodotSchedulers.ProcessFrameScheduler;
            return new IntervalFrame(0, periodFrameCount, scheduler);
        }

        public static IObservable<T> ReturnFrame<T>(T value, int dueFrameCount,
            GodotFrameScheduler? scheduler = null)
        {
            scheduler ??= GodotSchedulers.ProcessFrameScheduler;
            return new ReturnFrame<T>(value, dueFrameCount, scheduler);
        }
    }

    extension<T>(IObservable<T> source)
    {
        public IObservable<T> DelayFrame(int frameCount, GodotFrameScheduler? scheduler = null)
        {
            scheduler ??= GodotSchedulers.ProcessFrameScheduler;
            return new DelayFrame<T>(source, frameCount, scheduler);
        }

        public IObservable<T> DebounceFrame(int frameCount, GodotFrameScheduler? scheduler = null)
        {
            scheduler ??= GodotSchedulers.ProcessFrameScheduler;
            return new DebounceFrame<T>(source, frameCount, scheduler);
        }

        public IObservable<T> ThrottleFirstFrame(int frameCount, GodotFrameScheduler? scheduler = null)
        {
            scheduler ??= GodotSchedulers.ProcessFrameScheduler;
            return new ThrottleFirstFrame<T>(source, frameCount, scheduler);
        }

        public IObservable<System.Collections.Generic.IList<T>> ChunkFrame(int frameCount,
            GodotFrameScheduler? scheduler = null)
        {
            scheduler ??= GodotSchedulers.ProcessFrameScheduler;
            return new ChunkFrame<T>(source, frameCount, scheduler);
        }
    }
}
