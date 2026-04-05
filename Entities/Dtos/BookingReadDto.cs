using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WeighForce.Dtos
{
    public class BookingDto
    {
        public long Id { get; set; }
        public long wId { get; set; }
        public long gdnId { get; set; }
        public string VehicleType { get; set; }
        public string NumberPlate { get; set; }
        public string TrailerNumber { get; set; }
        public string DriverName { get; set; }
        public string PassportNumber { get; set; }
        public string Transporter { get; set; }
        public string PhoneNumber { get; set; }
        public string Speed { get; set; }
        public string WagonNo { get; set; }
        public string DeliveryNoteNumber { get; set; }
        public string LoadingAuthorityNumber { get; set; }
        public string OtherTransporter { get; set; }
        public string LPO { get; set; }
        public string Description { get; set; }
        public int TareWeight { get; set; }
        public TIReadDto TransportInstruction { get; set; }
        public BranchNameDto Branch { get; set; }
        public UserNameDto TareUser { get; set; }
        public DateTime? TareAt { get; set; }
        public DateTime? CreatedAt { get; set; }
        public List<TIChangeReadDto> TIChanges { get; set; }
    }
    public class BookingPostDto
    {
        public string VehicleType { get; set; }
        [Required]
        public string DriverName { get; set; }
        [Required]
        public string NumberPlate { get; set; }
        public string TrailerNumber { get; set; }
        public string PassportNumber { get; set; }
        public string PhoneNumber { get; set; }
        public string TempTicketNumber { get; set; }
        public string DeliveryNoteNumber { get; set; }
        public string LoadingAuthorityNumber { get; set; }
        public string OtherTransporter { get; set; }
        public string LPO { get; set; }
        public string Description { get; set; }
        [Required]
        public IDto Transporter { get; set; }
        public IDto Branch { get; set; }
        [Required]
        public TIPutDto TransportInstruction { get; set; }
    }
    public class BookingSyncDto : BaseSyncDto
    {
        public long TransportInstruction { get; set; }
        public string VehicleType { get; set; }
        public string NumberPlate { get; set; }
        public string TrailerNumber { get; set; }
        public string DriverName { get; set; }
        public string PassportNumber { get; set; }
        public long Transporter { get; set; }
        public long Branch { get; set; }
        public string PhoneNumber { get; set; }
        public string DeliveryNoteNumber { get; set; }
        public string LoadingAuthorityNumber { get; set; }
        public string OtherTransporter { get; set; }
        public string LPO { get; set; }
        public string Description { get; set; }
        public int TareWeight { get; set; }
        public string TareUser { get; set; }
        public DateTime? TareAt { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class BookingWriteDto : SyncableDto
    {
        public long? wId { get; set; }
        public long? gdnId { get; set; }
        public string VehicleType { get; set; }
        public string NumberPlate { get; set; }
        public string TrailerNumber { get; set; }
        public string DriverName { get; set; }
        public string PassportNumber { get; set; }
        public long Transporter { get; set; }
        public long Branch { get; set; }
        public string PhoneNumber { get; set; }
        public string DeliveryNoteNumber { get; set; }
        public string LoadingAuthorityNumber { get; set; }
        public string OtherTransporter { get; set; }
        public string LPO { get; set; }
        public string Description { get; set; }
        public int? TareWeight { get; set; }
        public string TareUser { get; set; }
        public long TransportInstruction { get; set; }
        public DateTime? TareAt { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}