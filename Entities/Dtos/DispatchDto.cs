using System;

namespace WeighForce.Dtos
{
    public class DispatchReadDto
    {
        public long Id { get; set; }
        public OfficeNameDto FromOffice { get; set; }
        public OfficeNameDto ToOffice { get; set; }
        public BookingDto Booking { get; set; }
        public ContractReadDto Contract { get; set; }
        public ProductReadDto Product { get; set; }
        public WeightDto InitialWeight { get; set; }
        public WeightDto ReceivalWeight { get; set; }
        public WeightDto FirstWeight { get; set; }

        public string VehicleType { get; set; }
        public string Status { get; set; }
        public Diff Diff
        {
            get
            {
                int? value = null;
                if (ReceivalWeight == null || ReceivalWeight.Gross == 0) return new Diff { };
                if (InitialWeight == null || InitialWeight.Gross == 0) return new Diff { };
                if (ReceivalWeight.Tare == ReceivalWeight.Gross) return new Diff { };
                if (InitialWeight.Tare == InitialWeight.Gross) return new Diff { };
                if (ReceivalWeight.Tare == 0 && ReceivalWeight.Gross > 0
                && ReceivalWeight.Gross < InitialWeight.Gross) return new Diff
                {
                    Value = value,
                    Percentage = Math.Round(Convert.ToDouble(ReceivalWeight.Gross - InitialWeight.Gross) / InitialWeight.Gross * 100, 2)
                };
                return new Diff
                {
                    Value = ReceivalWeight.Gross - ReceivalWeight.Tare - (InitialWeight.Gross - InitialWeight.Tare),
                    Percentage = Math.Round(
                        Convert.ToDouble(ReceivalWeight.Gross - ReceivalWeight.Tare - (InitialWeight.Gross - InitialWeight.Tare)) / (InitialWeight.Gross - InitialWeight.Tare)
                        * 100, 2)
                };
            }
        }
    }
    public class Diff
    {
        public int? Value { get; set; }
        public double? Percentage { get; set; }
    }
    public class DispatchSyncDto : BaseSyncDto
    {
        public long Booking { get; set; }
        public long InitialWeight { get; set; }
        public long ReceivalWeight { get; set; }
        public string Status { get; set; }
    }
    public class DispatchFullSyncDto : BaseSyncDto
    {
        public BookingSyncDto Booking { get; set; }
        public WeightSyncDto InitialWeight { get; set; }
        public WeightSyncDto ReceivalWeight { get; set; }
        public string Status { get; set; }
    }
    public class DispatchWriteDto : SyncableDto
    {
        public BookingWriteDto Booking { get; set; }
        public WeightWriteDto InitialWeight { get; set; }
        public WeightWriteDto ReceivalWeight { get; set; }
        public string Status { get; set; }
    }
}