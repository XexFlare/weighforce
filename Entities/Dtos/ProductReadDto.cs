using System.ComponentModel.DataAnnotations;

namespace WeighForce.Dtos
{
    public class ProductReadDto
    {
        public long Id { get; set; }
        [Required]
        public string Name { get; set; }
        public UnitReadDto Unit { get; set; }
    }
    public class ProductSyncDto : BaseSyncDto
    {
        public string Name { get; set; }
        public long Unit { get; set; }
    }
    public class ProductWriteDto : SyncableDto
    {
        public string Name { get; set; }
        public SyncableDto Unit { get; set; }
    }
}