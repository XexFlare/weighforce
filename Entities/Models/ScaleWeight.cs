using System.ComponentModel.DataAnnotations;

namespace WeighForce.Data
{
    public class ScaleWeight
    {
        [Required]
        [Range(1, 999999)]
        public int Amount { get; set; }
        public int? Bags { get; set; }
        public int? Size { get; set; } 
        public string DeliveryNoteNumber { get; set; } 
    }
    public class TIUpdate
    {
        public long contractId { get; set; }
        public long productId { get; set; }
    }
    public class ReassignPost
    {
        public long product { get; set; }
        public string numberPlate { get; set; }
    }
}
