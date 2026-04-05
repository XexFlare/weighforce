using System;

namespace WeighForce.Models
{
    public abstract class Syncable
    {
        public long Id { get; set; }
        public long cId { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int SyncVersion { get; set; }
        public bool IsDeleted { get; set; }
        public string DeletedBy { get; set; }
        public bool Delete(string userId)
        {
            this.IsDeleted = true;
            this.DeletedBy = userId;
            this.UpdatedAt = DateTime.UtcNow;
            this.SyncVersion++;
            return true;
        }
    }
}
