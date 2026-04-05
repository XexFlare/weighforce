using System;
using System.Text.Json.Serialization;

namespace WeighForce.Dtos
{
    public class TIChangeReadDto
    {
        public long Id { get; set; }
        public UserNameDto User { get; set; }
        public CIDto Booking { get; set; }
        public TIReadDto OldValue { get; set; }
        public CIDto NewValue { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
    public class TIChangeDto
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }
        public string User { get; set; }
        public CIDto Booking { get; set; }
        public CIDto OldValue { get; set; }
        public CIDto NewValue { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class CIDto
    {
        [JsonPropertyName("id")]
        public long cId { get; set; }
    }
}