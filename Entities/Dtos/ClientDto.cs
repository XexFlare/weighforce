using System.Text.Json.Serialization;

namespace WeighForce.Dtos
{
    public class ClientReadDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }
    public class ClientWriteDto : SyncableDto
    {
        public string Name { get; set; }
    }
}