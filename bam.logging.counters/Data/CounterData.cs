using Bam.Data.Repositories;

namespace Bam.Logging.Counters.Data
{
    [Serializable]
    public class CounterData : CompositeKeyAuditRepoData
    {
        public CounterData() { }

        [CompositeKey]
        public string UserName { get; set; }

        [CompositeKey]
        public string CounterName { get; set; }

        public string Value { get; set; }
    }
}
