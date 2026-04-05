using System.ComponentModel.DataAnnotations;

namespace WeighForce.Dtos
{
    public class BranchDto
    {
        public long Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public OfficeNameDto Office { get; set; }
    }
    public class BranchNameDto
    {
        public long Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
    }
    public class BranchSyncDto : BaseSyncDto
    {
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public long Office { get; set; }
    }
    public class BranchWriteDto : SyncableDto
    {
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public DispatchOfficeWriteDto Office { get; set; }
    }
}