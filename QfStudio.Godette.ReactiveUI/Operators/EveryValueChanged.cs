using System.Collections;

namespace QfStudio.Godette.ReactiveUI.Operators;

internal sealed class EveryValueChanged<TSource, TProperty>(TSource source, Func<TSource, TProperty> propertySelector, IEqualityComparer<TProperty>? equalityComparer, GodotFrameScheduler scheduler) : IObservable<TProperty>
{
    public IDisposable Subscribe(IObserver<TProperty> observer)
    {
        var initialValue = propertySelector(source);
        observer.OnNext(initialValue);

        var sink = new Sink(observer, source, propertySelector, equalityComparer ?? EqualityComparer<TProperty>.Default, initialValue);
        scheduler.Schedule(sink);
        return sink;
    }

    private sealed class Sink : FrameSink<TProperty>
    {
        private readonly TSource _source;
        private readonly Func<TSource, TProperty> _propertySelector;
        private readonly IEqualityComparer<TProperty> _comparer;
        private TProperty? _previousValue;

        public Sink(IObserver<TProperty> observer, TSource source, Func<TSource, TProperty> propertySelector, IEqualityComparer<TProperty> comparer, TProperty initialValue)
            : base(observer)
        {
            _source = source;
            _propertySelector = propertySelector;
            _comparer = comparer;
            _previousValue = initialValue;
        }

        protected override bool MoveNextCore(double delta)
        {
            var currentValue = _propertySelector(_source);
            if (!_comparer.Equals(_previousValue, currentValue))
            {
                _previousValue = currentValue;
                EmitNext(currentValue);
            }
            return true;
        }
    }
}
