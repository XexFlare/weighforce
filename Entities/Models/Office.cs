using System.Collections.Generic;

namespace WeighForce.Models
{
    public class Office : Syncable
    {
        public string Name { get; set; }
        public bool IsClient { get; set; }
        public string Contacts { get; set; }
        public string Website { get; set; }
        public string Logo { get; set; }
        public long TicketPrefix { get; set; }
        public Country Country { get; set; }
        public List<OfficeUser> OfficeUsers { get; set; }
        public List<Branch> Branches { get; set; }
        public Unit Unit { get; set; }
    }
}