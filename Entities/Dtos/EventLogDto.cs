using System;
using System.Text.Json.Serialization;

namespace WeighForce.Dtos
{
    public class EventLogDto
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }
        public string User { get; set; }
        public string Resource { get; set; }
        public long ResourceId { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}