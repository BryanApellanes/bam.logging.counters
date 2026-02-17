namespace Bam.Logging.Counters
{
    /// <summary>
    /// A named counter that tracks a numeric value, supporting increment, decrement, and custom count readers.
    /// </summary>
    public class Counter: Stats
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Counter"/> class with a default count reader.
        /// </summary>
        public Counter()
        {
            DefaultCountReader = () => _count;
            CountReader = DefaultCountReader;
        }

        protected Func<ulong> DefaultCountReader { get; }

        /// <summary>
        /// Gets or sets the counter value as an object. Getter returns <see cref="Count"/>, setter casts to <see cref="ulong"/>.
        /// </summary>
        public override object Value
        {
            get
            {
                return Count;
            }
            set
            {
                Count = (ulong)value;
            }
        }

        ulong _count;
        /// <summary>
        /// Gets or sets the current count value. The getter delegates to <see cref="CountReader"/>.
        /// </summary>
        public new ulong Count
        {
            get
            {
                return CountReader();
            }
            set
            {
                _count = value;
            }
        }

        /// <summary>
        /// Gets or sets the function used to read the counter value. Defaults to returning the internal count.
        /// </summary>
        public Func<ulong> CountReader { get; set; }

        /// <summary>
        /// Increments the internal counter by one. Logs a trace warning if a custom CountReader is set.
        /// </summary>
        /// <returns>This <see cref="Counter"/> instance for chaining.</returns>
        public Counter Increment()
        {
            ++_count;
            if(CountReader != DefaultCountReader)
            {
                Log.Trace("Increment called on counter ({0}) with custom CountReader; this may not behave as expected.", Name);
            }
            return this;
        }

        /// <summary>
        /// Decrements the internal counter by one. Logs a trace warning if a custom CountReader is set.
        /// </summary>
        /// <returns>This <see cref="Counter"/> instance for chaining.</returns>
        public Counter Decrement()
        {
            --_count;
            if (CountReader != DefaultCountReader)
            {
                Log.Trace("Decrement called on counter ({0}) with custom CountReader; this may not behave as expected.", Name);
            }
            return this;
        }

        /// <summary>
        /// Creates a new counter representing the difference between this counter and the specified counter.
        /// </summary>
        /// <param name="counter">The counter to subtract from this counter.</param>
        /// <returns>A new <see cref="Counter"/> with the computed difference.</returns>
        public Counter Diff(Counter counter)
        {
            return new Counter() { Name = Name, Value = Count - counter.Count };
        }
    }
}
