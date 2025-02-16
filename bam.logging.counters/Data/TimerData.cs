using Bam.Data.Repositories;

namespace Bam.Logging.Counters.Data
{
    [Serializable]
    public class TimerData: CompositeKeyAuditRepoData
    {
        public TimerData() { }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
