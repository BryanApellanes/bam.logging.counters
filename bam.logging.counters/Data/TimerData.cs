using Bam.Data.Repositories;

namespace Bam.Logging.Counters.Data
{
    /// <summary>
    /// Persistable data class representing a timer value associated with a user.
    /// </summary>
    [Serializable]
    public class TimerData: CompositeKeyAuditRepoData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TimerData"/> class.
        /// </summary>
        public TimerData() { }

        /// <summary>
        /// Gets or sets the user name that owns this timer.
        /// </summary>
        public string UserName { get; set; } = null!;

        /// <summary>
        /// Gets or sets the name of the timer.
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Gets or sets the string representation of the timer value.
        /// </summary>
        public string Value { get; set; } = null!;
    }
}
