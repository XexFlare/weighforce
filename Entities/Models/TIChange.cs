using System;

namespace WeighForce.Models
{
    public class TIChange
    {
        public long Id { get; set; }
        public ApplicationUser User { get; set; }
        public Booking Booking { get; set; }
        public TransportInstruction OldValue { get; set; }
        public TransportInstruction NewValue { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}