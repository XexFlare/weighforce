using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WeighForce.Dtos
{
    public class OfficeNameDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }
    public class OfficeWriteDto : SyncableDto
    {
        public string Name { get; set; }
        public bool? IsClient { get; set; }
        public string Contacts { get; set; }
        public string Website { get; set; }
        public string Logo { get; set; }
        public long? TicketPrefix { get; set; }
        public CountryWriteDto Country { get; set; }
        public UnitWriteDto Unit { get; set; }
    }
    public class OfficePostDto
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }
        [Required]
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [Required]
        [JsonPropertyName("unit")]
        public IDto Unit { get; set; }
        [JsonPropertyName("country")]
        public IDto Country { get; set; }
        [JsonPropertyName("ticketPrefix")]
        public long? TicketPrefix { get; set; }
        [JsonPropertyName("contracts")]
        public string Contacts { get; set; }
        [JsonPropertyName("website")]
        public string Website { get; set; }
        [JsonPropertyName("logo")]
        public string Logo { get; set; }
        [JsonPropertyName("isClient")]
        public bool? IsClient { get; set; }
    }
    public class OfficeSyncDto : BaseSyncDto
    {
        public string Contacts { get; set; }
        public bool? IsClient { get; set; }
        
        public string Website { get; set; }
        public string Logo { get; set; }
        public long TicketPrefix { get; set; }
        public long Country { get; set; }
        public long Unit { get; set; }
        public string Name { get; set; }
    }
    public class DispatchOfficeWriteDto : SyncableDto
    {
        public string Name { get; set; }
    }
    public class OfficeReadDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public bool IsClient { get; set; }
        public CountryReadDto Country { get; set; }
        public UnitReadDto Unit { get; set; }
        public string Contacts { get; set; }
        public string Website { get; set; }
        public string Logo { get; set; }
        public long TicketPrefix { get; set; }
    }
    public class BranchedOfficeDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public bool IsClient { get; set; }
        public IEnumerable<BranchNameDto> Branches { get; set; }
        public UnitReadDto Unit { get; set; }
        public CountryReadDto Country { get; set; }
        public string Contacts { get; set; }
        public string Website { get; set; }
        public string Logo { get; set; }
        public long TicketPrefix { get; set; }
    }
}