using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeighForce.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        // public ICollection<Office> Offices { get; set;}
        public List<OfficeUser> OfficeUsers { get; set; }

    }

    [Table("OfficeUser")]
    public class OfficeUser
    {
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        [ForeignKey("offices")]
        public long OfficeId { get; set; }
        public Office Office { get; set; }
    }
}
