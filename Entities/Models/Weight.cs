using System;
using WeighForce.Data;

namespace WeighForce.Models
{
    public class Weight : Syncable
    {
        public Office Office { get; set; }
        public int? Tare { get; set; }
        public int? Gross { get; set; }
        public int? Bags { get; set; }
        public int? Size { get; set; }
        public bool? Printed { get; set; }
        public string TicketNumber { get; set; }
        public DateTime? TareAt { get; set; }
        public DateTime? GrossAt { get; set; }
        public ApplicationUser TareUser { get; set; }
        public ApplicationUser GrossUser { get; set; }

        public void SetTare(ScaleWeight details, ApplicationUser user)
        {
            Tare = details.Amount;
            TareUser = user;
            TareAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
            SyncVersion++;
        }

        public void SetGross(ScaleWeight details, ApplicationUser user, bool setGrossAt = true, DateTime? grossAt = null)
        {
            Gross = details.Amount;
            Bags = details.Bags;
            Size = details.Size;
            GrossUser = user;
            if(setGrossAt) GrossAt = DateTime.UtcNow;
            else GrossAt = grossAt;
            UpdatedAt = DateTime.UtcNow;
            SyncVersion++;
        }
    }
}
