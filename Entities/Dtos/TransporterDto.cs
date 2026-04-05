using System.ComponentModel.DataAnnotations;

namespace WeighForce.Dtos
{
    public class TransporterDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
    }
    public class TransporterSyncDto : BaseSyncDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public IDto Country { get; set; }
    }
    public class TransporterPutDto
    {
        public long Id { get; set; }
        [Required]
        public string Name { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public IDto Country { get; set; }
    }
    public class TransporterWriteDto : SyncableDto
    {
        [Required]
        public string Name { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public IDto Country { get; set; }
    }
}