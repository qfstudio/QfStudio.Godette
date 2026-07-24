namespace QfStudio.Godette.ReactiveUI.Operators;

internal sealed class EveryValueChanged<TSource, TProperty>(TSource source, Func<TSource, TProperty> propertySelector, IEqualityComparer<TProperty>? equalityComparer, GodotFrameScheduler scheduler) : IObservable<TProperty>
{
    public IDisposable Subscribe(IObserver<TProperty> observer)
    {
        var sink = new Sink(observer, source, propertySelector, equalityComparer ?? EqualityComparer<TProperty>.Default);
        scheduler.Schedule(sink);
        return sink;
    }

    private sealed class Sink : FrameSink<TProperty>
    {
        private readonly TSource _source;
        private readonly Func<TSource, TProperty> _propertySelector;
        private readonly IEqualityComparer<TProperty> _comparer;
        private TProperty? _previousValue;

        public Sink(IObserver<TProperty> observer, TSource source, Func<TSource, TProperty> propertySelector, IEqualityComparer<TProperty> comparer)
            : base(observer)
        {
            _source = source;
            _propertySelector = propertySelector;
            _comparer = comparer;
            _previousValue = propertySelector(source); // fail-fast on subscribe
        }

        protected override bool MoveNextCore(double delta)
        {
            TProperty currentValue;
            try
            {
                currentValue = _propertySelector(_source);
            }
            catch (Exception ex)
            {
                Error(ex);
                return false;
            }

            if (!_comparer.Equals(_previousValue, currentValue))
            {
                _previousValue = currentValue;
                EmitNext(currentValue);
            }

            return true;
        }
    }
}
