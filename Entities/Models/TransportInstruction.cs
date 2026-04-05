namespace WeighForce.Models
{
    public class TransportInstruction : Syncable
    {
        public Contract Contract { get; set; }
        public Product Product { get; set; }
        public Office FromLocation { get; set; }
        public Office ToLocation { get; set; }
        public string KineticRef { get; set; }
        public bool Closed { get; set; }
        public bool OneTime { get; set; }
    }
}