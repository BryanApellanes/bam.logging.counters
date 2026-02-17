using Bam.Data.Repositories;

namespace Bam.Logging.Counters.Data
{
    /// <summary>
    /// Persistable data class representing a counter value associated with a user, keyed by user name and counter name.
    /// </summary>
    [Serializable]
    public class CounterData : CompositeKeyAuditRepoData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CounterData"/> class.
        /// </summary>
        public CounterData() { }

        /// <summary>
        /// Gets or sets the user name that owns this counter. Part of the composite key.
        /// </summary>
        [CompositeKey]
        public string UserName { get; set; } = null!;

        /// <summary>
        /// Gets or sets the name of the counter. Part of the composite key.
        /// </summary>
        [CompositeKey]
        public string CounterName { get; set; } = null!;

        /// <summary>
        /// Gets or sets the string representation of the counter value.
        /// </summary>
        public string Value { get; set; } = null!;
    }
}
