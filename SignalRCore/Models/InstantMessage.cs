using Newtonsoft.Json;
using System;

namespace SignalRCore.Models
{
    public class InstantMessage
    {
        public MessageActionType ActionType { get; set; }

        public MessageType MessageType { get; set; }

        public int LarId { get; set; }

        public Guid UserId { get; set; }

        public Guid AccountId { get; set; }

        public string LarName { get; set; }

        public DateTime DateTime { get; set; }

        public int ItemId { get; set; }
    }
}
