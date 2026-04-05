using System;

namespace WeighForce.Models
{
    public class EventLog
    {
        public long Id { get; set; }
        public ApplicationUser User { get; set; }
        public string Resource { get; set; }
        public long ResourceId { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}