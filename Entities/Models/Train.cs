using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeighForce.Models
{
    public class OT_Train
    {
        [Key]
        public int Message_ID { get; set; }
        public OT_Site Site { get; set; }
        public DateTime Post_Date { get; set; }
        public string Direction { get; set; }
        [ForeignKey("Message_ID")]
        public List<OT_WagonData> Wagons { get; set; }
    }
}
