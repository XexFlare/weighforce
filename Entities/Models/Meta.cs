using System.ComponentModel.DataAnnotations;

namespace WeighForce
{
    public class Meta
    {
        [Key]
        public string name { get; set; }
        public string value { get; set; }
    }
}