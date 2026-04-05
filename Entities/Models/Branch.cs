namespace WeighForce.Models
{
    public class Branch : Syncable
    {
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public Office Office { get; set; }
    }
}