using System;
using System.Text.Json.Serialization;

namespace WeighForce.Dtos
{
    public class SyncableDto
    {
        [JsonPropertyName("host_id")]
        public long Id { get; set; }
        [JsonPropertyName("id")]
        public long cId { get; set; }
        [JsonPropertyName("sync_time")]
        public DateTime UpdatedAt { get; set; }
        public int? SyncVersion { get; set; }
        public bool? IsDeleted { get; set; }
        public string DeletedBy { get; set; }
        [JsonPropertyName("error")]
        public string Error { get; set; }
    }
    public class BaseSyncDto
    {
        [JsonPropertyName("id")]
        public long cId { get; set; }
        [JsonPropertyName("host_id")]
        public long Id { get; set; }
        public bool IsDeleted { get; set; }
        public string DeletedBy { get; set; }
        public int SyncVersion { get; set; }
    }
    public class IDto
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }
    }
}
