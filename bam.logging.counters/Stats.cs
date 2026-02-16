using System.Collections.Concurrent;

namespace Bam.Logging.Counters
{
    /// <summary>
    /// Provides static methods for creating and managing named counters and timers for performance tracking.
    /// </summary>
    public class Stats
    {
        /// <summary>
        /// Gets or sets the name of this stats instance.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the value of this stats instance.
        /// </summary>
        public virtual object Value { get; set; }

        static ConcurrentDictionary<string, Stats> _stats = new ConcurrentDictionary<string, Stats>();

        /// <summary>
        /// Starts a new timer with the specified name.
        /// </summary>
        /// <param name="name">The name of the timer.</param>
        /// <returns>The started <see cref="Timer"/>.</returns>
        public static Timer Start(string name)
        {
            Timer timer = GetStats<Timer>(name, 0);
            timer.StartTime = new Instant();
            return timer;
        }

        /// <summary>
        /// Ends the specified timer and optionally invokes an end handler asynchronously.
        /// </summary>
        /// <param name="timer">The timer to end.</param>
        /// <param name="endHandler">An optional callback invoked asynchronously after the timer ends.</param>
        /// <returns>The ended <see cref="Timer"/>.</returns>
        public static Timer End(Timer timer, Action<Timer> endHandler = null)
        {
            End(timer.Name);
            timer.End();
            if(endHandler != null)
            {
                Task.Run(() => 
                endHandler(timer));
            }
            return timer;
        }

        /// <summary>
        /// Ends and removes the timer with the specified name.
        /// </summary>
        /// <param name="name">The name of the timer to end.</param>
        /// <returns>The ended <see cref="Timer"/>.</returns>
        public static Timer End(string name)
        {
            Timer timer = GetStats<Timer>(name, 0);
            timer.End();
            if(!_stats.TryRemove(name, out Stats value))
            {
                Log.Trace("Failed to remove timer {0}", name);
            }
            return timer;
        }

        /// <summary>
        /// Increments the counter with the specified name, creating it if it does not exist.
        /// </summary>
        /// <param name="name">The name of the counter.</param>
        /// <returns>The incremented <see cref="Counter"/>.</returns>
        public static Counter Increment(string name)
        {
            return GetCounter(name).Increment();
        }

        /// <summary>
        /// Decrements the counter with the specified name, creating it if it does not exist.
        /// </summary>
        /// <param name="name">The name of the counter.</param>
        /// <returns>The decremented <see cref="Counter"/>.</returns>
        public static Counter Decrement(string name)
        {
            return GetCounter(name).Decrement();
        }

        /// <summary>
        /// Gets or creates a counter with the specified name initialized to zero.
        /// </summary>
        /// <param name="name">The name of the counter.</param>
        /// <returns>The <see cref="Counter"/> instance.</returns>
        public static Counter Count(string name)
        {
            return GetCounter(name, 0);
        }

        /// <summary>
        /// Gets or creates a counter with the specified name and sets its value.
        /// </summary>
        /// <param name="name">The name of the counter.</param>
        /// <param name="setValueTo">The value to set the counter to.</param>
        /// <returns>The <see cref="Counter"/> instance.</returns>
        public static Counter Count(string name, long setValueTo)
        {
            Counter counter = GetCounter(name);
            counter.Value = setValueTo;
            return counter;
        }
        
        /// <summary>
        /// Gets or creates a counter with the specified name and assigns a custom count reader function.
        /// </summary>
        /// <param name="name">The name of the counter.</param>
        /// <param name="countReader">A function that provides the counter's value when read.</param>
        /// <returns>The <see cref="Counter"/> instance.</returns>
        public static Counter Count(string name, Func<ulong> countReader)
        {
            Counter counter = GetCounter(name);
            counter.CountReader = countReader;
            return counter;
        }

        /// <summary>
        /// Computes the difference between the named counter and a counter provided by the specified function.
        /// </summary>
        /// <param name="name">The name of the counter to diff against.</param>
        /// <param name="counter">A function that returns the counter to subtract.</param>
        /// <returns>A new <see cref="Counter"/> representing the difference.</returns>
        public static Counter Diff(string name, Func<Counter> counter)
        {
            return Diff(name, counter());
        }

        /// <summary>
        /// Computes the difference between the named counter and the specified counter.
        /// </summary>
        /// <param name="name">The name of the counter to diff against.</param>
        /// <param name="counter">The counter to subtract.</param>
        /// <returns>A new <see cref="Counter"/> representing the difference.</returns>
        public static Counter Diff(string name, Counter counter)
        {
            Counter named = GetCounter(name);
            return named.Diff(counter);
        }

        /// <summary>
        /// Gets or creates a counter with the specified name and initial value.
        /// </summary>
        /// <param name="name">The name of the counter.</param>
        /// <param name="initialValue">The initial value if the counter is newly created. Defaults to 1.</param>
        /// <returns>The <see cref="Counter"/> instance.</returns>
        public static Counter GetCounter(string name, long initialValue = 1)
        {
            return GetStats<Counter>(name, initialValue);
        }
        
        private static T GetStats<T>(string name, object initialValue) where T : Stats, new()
        { 
            if (!_stats.ContainsKey(name))
            {
                T stats = new T { Name = name, Value = initialValue };
                _stats.TryAdd(name, stats);
                return stats;
            }
            else if (_stats.TryGetValue(name, out Stats value))
            {
                T stats = (T)value;
                return stats;
            }
            else
            {
                Log.Trace("Failed to get stats instance of type {0} named {1}", typeof(T).Name, name);
                return new T { Value = initialValue };
            }
        }
    }
}
