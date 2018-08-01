using System;

namespace SignalRCore.Models
{
    public partial class ProcessState
    {
        public Guid ProcessId { get; set; }

        public Guid? AccountId { get; set; }

        public Guid? UserId { get; set; }

        public int? LarId { get; set; }

        public string LarName { get; set; }

        public NotificationSource ProcessType { get; set; }

        public ProcessStatus Status { get; set; }

        public int? Percentage { get; set; }

        public DateTime DateTime { get; set; }

        public bool Seen { get; set; }
    }
}
