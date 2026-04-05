using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeighForce.Models
{
    [Table("Wagon_Data")]
    public class OT_WagonData
    {
        [Key]
        public int Wagon_Data_ID { get; set; }
        public short Wagon_No { get; set; }
        public float Speed { get; set; }
        public OT_Train Train { get; set; }
        public string Vehicle_Type { get; set; }
        public float Mass { get; set; }
        public float Tare_Mass { get; set; }
        [ForeignKey("Tag_Data_ID_1")]
        public Tag Tag { get; set; }
        public bool Exclude { get; set; }
        public bool isValid()
        {
            return !Exclude;
        }
        public string Number()
        {
            return Tag != null ? Tag.Vehicle_Number : null;
        }
    }
    public class Tag
    {
        [Key]
        public int Tag_Data_ID { get; set; }
        public string Vehicle_Number { get; set; }
    }
}


