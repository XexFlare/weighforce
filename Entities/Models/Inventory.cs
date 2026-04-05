namespace WeighForce.Models
{
    public class Inventory : Syncable
    {
        public Office Office { get; set; }
        public Product Product { get; set; }
        public int Weight { get; set; }
        public string Status { get; set; }
    }
}