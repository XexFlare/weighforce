using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace WeighForce.Dtos
{
    public class CountryReadDto
    {
        public long Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
    public class CountrySyncDto : BaseSyncDto
    {
        public string Name { get; set; }
    }
    public class CountryWriteDto : SyncableDto
    {
        public string Name { get; set; }
        public ICollection<OfficeNameDto> Offices { get; set; }
    }
}
