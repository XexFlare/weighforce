using System.ComponentModel.DataAnnotations;

namespace WeighForce.Dtos
{
    public class UnitReadDto
    {
        public long Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
    public class UnitSyncDto : BaseSyncDto
    {
        public string Name { get; set; }
    }
    public class UnitWriteDto : SyncableDto
    {
        public string Name { get; set; }
    }
}