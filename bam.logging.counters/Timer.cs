namespace Bam.Logging.Counters
{
    /// <summary>
    /// A named timer that measures elapsed time in milliseconds between a start and end instant.
    /// </summary>
    public class Timer: Stats
    {
        /// <summary>
        /// Gets or sets the timer value. Returns <see cref="Duration"/> as the value.
        /// </summary>
        public override object Value
        {
            get
            {
                return Duration;
            }
            set
            {
                Duration = (int)Value;
            }
        }

        /// <summary>
        /// Gets or sets the instant when the timer was started.
        /// </summary>
        public Instant StartTime { get; set; } = null!;

        /// <summary>
        /// Gets or sets the instant when the timer was ended.
        /// </summary>
        public Instant EndTime { get; set; } = null!;
        int _duration;
        /// <summary>
        /// Gets or sets the elapsed time in milliseconds. Computed from <see cref="StartTime"/> and <see cref="EndTime"/> if both are set.
        /// </summary>
        public int Duration
        {
            get
            {
                if(StartTime != null && EndTime != null)
                {
                    _duration = StartTime.DiffInMilliseconds(EndTime);
                }
                return _duration;
            }
            set => _duration = value;
        }
        
        /// <summary>
        /// Gets a value indicating whether this timer has been ended.
        /// </summary>
        public bool Ended => EndTime != null;

        /// <summary>
        /// Times the execution of the specified action with a random name.
        /// </summary>
        /// <param name="action">The action to time.</param>
        /// <returns>The elapsed time in milliseconds.</returns>
        public static int Time(Action action)
        {
            return Time(8.RandomLetters(), action);
        }

        /// <summary>
        /// Times the execution of the specified action with the given name.
        /// </summary>
        /// <param name="name">The name of the timer.</param>
        /// <param name="action">The action to time.</param>
        /// <returns>The elapsed time in milliseconds.</returns>
        public static int Time(string name, Action action)
        {
            Timer timer = Start(name);
            action();
            return timer.End();
        }

        /// <summary>
        /// Starts this timer by setting the <see cref="StartTime"/> to the current instant.
        /// </summary>
        /// <returns>This <see cref="Timer"/> instance for chaining.</returns>
        public Timer Start()
        {
            StartTime = new Instant();
            return this;
        }

        /// <summary>
        /// Ends this instance by setting the EndTime and Duration.
        /// </summary>
        /// <returns>Duration in milliseconds</returns>
        public int End()
        {
            if(EndTime == null)
            {
                EndTime = new Instant();
                Duration = StartTime.DiffInMilliseconds(EndTime);
                Value = Duration;
            }
            return Duration;
        }

        /// <summary>
        /// Gets the number of milliseconds that have passed since this timer was started.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">
        /// Timer already ended
        /// or
        /// Timer not started
        /// </exception>
        public int SoFar()
        {
            if(Ended)
            {
                throw new InvalidOperationException("Timer already ended");
            }

            if(StartTime == null)
            {
                throw new InvalidOperationException("Timer not started");
            }

            return StartTime.DiffInMilliseconds(new Instant());
        }

        /// <summary>
        /// Returns a string representation of this timer in the format "Name: Durationms".
        /// </summary>
        /// <returns>A string containing the timer name and duration.</returns>
        public override string ToString()
        {
            return $"{Name}: {Duration}ms";
        }

        /// <summary>
        /// Ends the specified timer and returns its duration.
        /// </summary>
        /// <param name="timer">The timer to end.</param>
        /// <returns>The elapsed time in milliseconds.</returns>
        public static int End(Timer timer)
        {
            timer.Duration = timer.End();
            return timer.Duration;
        }

        /// <summary>
        /// Creates and starts a new timer with the specified name.
        /// </summary>
        /// <param name="name">The name of the timer.</param>
        /// <returns>A new started <see cref="Timer"/>.</returns>
        public new static Timer Start(string name)
        {
            return new Timer
            {
                Name = name,
                StartTime = new Instant()
            };
        }
    }
}
