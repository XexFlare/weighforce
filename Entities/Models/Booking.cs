using System;
using System.Collections.Generic;

namespace WeighForce.Models
{
    public class Booking : Syncable
    {
        public long? gdnId { get; set; }
        public long? wId { get; set; }
        public DateTime? TareAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public TransportInstruction TransportInstruction { get; set; }
        public string VehicleType { get; set; }
        public string NumberPlate { get; set; }
        public string DriverName { get; set; }
        public string TrailerNumber { get; set; }
        public Transporter Transporter { get; set; }
        public string PhoneNumber { get; set; }
        public string PassportNumber { get; set; }
        public string TempTicketNumber { get; set; }
        public string DeliveryNoteNumber { get; set; }
        public string LoadingAuthorityNumber { get; set; }
        public string OtherTransporter { get; set; }
        public Branch Branch { get; set; }
        public int TareWeight { get; set; }
        public string LPO { get; set; }
        public string Description { get; set; }
        public ApplicationUser TareUser { get; set; }
        public List<TIChange> TIChanges { get; set; }

        public void SetTare(int value, ApplicationUser user)
        {
            this.TareWeight = value;
            this.TareUser = user;
            this.TareAt = DateTime.UtcNow;
            this.UpdatedAt = DateTime.UtcNow;
            this.SyncVersion++;
        }
    }
}