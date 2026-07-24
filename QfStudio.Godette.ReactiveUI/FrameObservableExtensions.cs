using System.Reactive;
using System.Reactive.Linq;
using QfStudio.Godette.ReactiveUI.Operators;

namespace QfStudio.Godette.ReactiveUI;

public static class FrameObservableExtensions
{
    extension(Observable)
    {
        /// <summary>Emits <see cref="Unit.Default"/> every frame. Never completes.</summary>
        public static IObservable<Unit> EveryUpdate(
            GodotFrameScheduler? scheduler = null)
        {
            scheduler ??= GodotSchedulers.ProcessFrameScheduler;
            return new EveryUpdate(scheduler);
        }

        /// <summary>
        /// Emits <see cref="Unit.Default"/> once after <paramref name="dueFrameCount"/> frames, then completes.
        /// A value of 0 is treated as 1.
        /// </summary>
        public static IObservable<Unit> AfterFrame(uint dueFrameCount,
            GodotFrameScheduler? scheduler = null)
        {
            scheduler ??= GodotSchedulers.ProcessFrameScheduler;
            return new AfterFrame(dueFrameCount, scheduler);
        }

        /// <summary>
        /// Emits <see cref="Unit.Default"/> on the next frame, then every <paramref name="periodFrameCount"/> frames.
        /// Never completes.
        /// </summary>
        public static IObservable<Unit> IntervalFrame(uint periodFrameCount,
            GodotFrameScheduler? scheduler = null)
        {
            scheduler ??= GodotSchedulers.ProcessFrameScheduler;
            return new IntervalFrame(0u, periodFrameCount, scheduler);
        }

        /// <summary>
        /// Emits <paramref name="value"/> once after <paramref name="dueFrameCount"/> frames, then completes.
        /// A value of 0 is treated as 1.
        /// </summary>
        public static IObservable<T> ReturnFrame<T>(T value, uint dueFrameCount,
            GodotFrameScheduler? scheduler = null)
        {
            scheduler ??= GodotSchedulers.ProcessFrameScheduler;
            return new ReturnFrame<T>(value, dueFrameCount, scheduler);
        }

        /// <summary>
        /// Polls <paramref name="propertySelector"/> on <paramref name="source"/> every frame and emits when the value changes. 
        /// The initial value read at subscribe time is not emitted. 
        /// If the selector throws at subscribe time, the exception propagates synchronously; 
        /// if it throws during polling, the error is forwarded to the observer and the stream terminates.
        /// </summary>
        public static IObservable<TProperty> PollEveryUpdate<TSource, TProperty>(
            TSource source,
            Func<TSource, TProperty> propertySelector,
            IEqualityComparer<TProperty>? equalityComparer = null,
            GodotFrameScheduler? scheduler = null)
        {
            scheduler ??= GodotSchedulers.ProcessFrameScheduler;
            return new EveryValueChanged<TSource, TProperty>(source, propertySelector, equalityComparer, scheduler);
        }
    }

    extension<T>(IObservable<T> source)
    {
        /// <summary>
        /// Delays each upstream value by <paramref name="frameCount"/> frames.
        /// Completes after upstream completes and all delayed values are flushed.
        /// Upstream errors are forwarded immediately, discarding pending values.
        /// </summary>
        public IObservable<T> DelayFrame(uint frameCount, GodotFrameScheduler? scheduler = null)
        {
            scheduler ??= GodotSchedulers.ProcessFrameScheduler;
            return new DelayFrame<T>(source, frameCount, scheduler);
        }

        /// <summary>
        /// Emits the latest upstream value after <paramref name="frameCount"/> frames of quiet (no new values); new values reset the countdown.
        /// On upstream completion, flushes the latest pending value (if any) before completing.
        /// Upstream errors are forwarded immediately and any pending value is dropped.
        /// </summary>
        /// <remarks>Equivalent to <c>Throttle</c> in Rx.NET or <c>debounceTime</c> in RxJS.</remarks>
        public IObservable<T> DebounceFrame(uint frameCount, GodotFrameScheduler? scheduler = null)
        {
            scheduler ??= GodotSchedulers.ProcessFrameScheduler;
            return new DebounceFrame<T>(source, frameCount, scheduler);
        }

        /// <summary>
        /// Emits the first upstream value in each <paramref name="frameCount"/>-frame window, ignoring subsequent values until the window expires.
        /// On upstream completion, completes immediately and any captured but unemitted value is lost if the window has not expired.
        /// Upstream errors are forwarded immediately.
        /// </summary>
        public IObservable<T> ThrottleFirstFrame(uint frameCount, GodotFrameScheduler? scheduler = null)
        {
            scheduler ??= GodotSchedulers.ProcessFrameScheduler;
            return new ThrottleFirstFrame<T>(source, frameCount, scheduler);
        }

        /// <summary>
        /// Collects upstream values and emits them as <see cref="IList{T}"/> every <paramref name="frameCount"/> frames.
        /// Empty windows produce no emission. On upstream completion, the remaining batch is flushed at the next window boundary before completing.
        /// Upstream errors discard buffered data and forward the error immediately.
        /// </summary>
        public IObservable<IList<T>> ChunkFrame(uint frameCount, GodotFrameScheduler? scheduler = null)
        {
            scheduler ??= GodotSchedulers.ProcessFrameScheduler;
            return new ChunkFrame<T>(source, frameCount, scheduler);
        }
    }
}
