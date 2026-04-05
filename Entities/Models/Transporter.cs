namespace WeighForce.Models
{
    public class Transporter : Syncable
    {
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public Country Country { get; set; }
    }
}