namespace WeighForce.Models
{
    public class Product : Syncable
    {
        public string Name { get; set; }
        public Unit Unit { get; set; }
    }
}