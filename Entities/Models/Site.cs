using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeighForce.Models
{
    public class OT_Site
    {
        [Key]
        public int Site_ID { get; set; }
        public string Site_Name { get; set; }
        [ForeignKey("Site_ID")]
        public List<OT_Train> Trains { get; set; }
    }
}
