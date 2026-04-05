using System;
using WeighForce.Data;

namespace WeighForce.Dtos
{
    public class WeightDto
    {
        public long Id { get; set; }
        public OfficeNameDto Office { get; set; }
        public int Tare { get; set; }
        public DateTime? TareAt { get; set; }
        public int? Bags { get; set; }
        public int? Size { get; set; }
        public int Gross { get; set; }
        public bool? Printed { get; set; }
        public string TicketNumber { get; set; }
        public DateTime? GrossAt { get; set; }
        public UserNameDto TareUser { get; set; }
        public UserNameDto GrossUser { get; set; }
    }
    public class WeightWriteDto : SyncableDto
    {
        // public DispatchOfficeWriteDto? Office { get; set; }
        public int? Tare { get; set; }
        public DateTime? TareAt { get; set; }
        public int? Bags { get; set; }
        public int? Size { get; set; }
        public int? Gross { get; set; }
        public bool? Printed { get; set; }
        public string TicketNumber { get; set; }
        public DateTime? GrossAt { get; set; }
        public string TareUser { get; set; }
        public string GrossUser { get; set; }
    }
    public class WeightSyncDto : BaseSyncDto
    {
        public long Office { get; set; }
        public int Tare { get; set; }
        public DateTime? TareAt { get; set; }
        public int? Bags { get; set; }
        public int? Size { get; set; }
        public int Gross { get; set; }
        public bool? Printed { get; set; }
        public string TicketNumber { get; set; }
        public DateTime? GrossAt { get; set; }
        public string TareUser { get; set; }
        public string GrossUser { get; set; }
    }
    public class ClientWeight
    {
        public ScaleWeight Gross { get; set; }
        public ScaleWeight Tare { get; set; }
    }
}