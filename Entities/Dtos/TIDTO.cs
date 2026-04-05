using System.ComponentModel.DataAnnotations;

namespace WeighForce.Dtos
{
    public class TIReadDto
    {
        public long Id { get; set; }
        public long cId { get; set; }
        public ContractReadDto Contract { get; set; }
        public ProductReadDto Product { get; set; }
        public OfficeReadDto FromLocation { get; set; }
        public OfficeReadDto ToLocation { get; set; }
        public string KineticRef { get; set; }
        public bool Closed { get; set; }
    }
    public class TIPutDto
    {
        public long Id { get; set; }
        [Required]
        public IDto Contract { get; set; }
        [Required]
        public IDto Product { get; set; }
        [Required]
        public IDto FromLocation { get; set; }
        [Required]
        public IDto ToLocation { get; set; }
        public string KineticRef { get; set; }
        public bool Closed { get; set; }
    }
    public class TIWriteDto : SyncableDto
    {
        public SyncableDto Contract { get; set; }
        public SyncableDto Product { get; set; }
        public SyncableDto FromLocation { get; set; }
        public SyncableDto ToLocation { get; set; }
        public string KineticRef { get; set; }
        public bool? Closed { get; set; }
    }
    public class TISyncDto : BaseSyncDto
    {
        public long Contract { get; set; }
        public long Product { get; set; }
        public long FromLocation { get; set; }
        public long ToLocation { get; set; }
        public string KineticRef { get; set; }
        public bool? Closed { get; set; }
    }
}