namespace WeighForce.Models
{
    public class Client : Syncable
    {
        public string Name { get; set; }
        public Country Country { get; set; }
    }
}