using WeighForce.Data;

namespace WeighForce.Models
{
    public class DispatchFirstWeightDto : Syncable
    {
        public Booking Booking { get; set; }
        public string Status { get; set; }
        public Weight InitialWeight { get; set; }
        public Weight ReceivalWeight { get; set; }
        public Weight FirstWeight { get; set; }
    }

    public class Dispatch : Syncable
    {
        public Booking Booking { get; set; }
        public string Status { get; set; }
        public Weight InitialWeight { get; set; }
        public Weight ReceivalWeight { get; set; }
        public bool ToEmail { get; set; }

        public void SetInitialTare(ScaleWeight details, ApplicationUser user)
        {
            if (InitialWeight == null)
                InitialWeight = new Weight { };
            if (InitialWeight.Tare == 0 || InitialWeight.Tare == null)
            {
                InitialWeight.SetTare(details, user);
                Booking.SetTare(details.Amount, user);
            }
            Status = "Dispatching";
        }
        public void SetInitialGross(ScaleWeight details, ApplicationUser user, bool setStatus = true)
        {
            if (InitialWeight.Gross == 0 || InitialWeight.Gross == null)
            {
                if (setStatus) InitialWeight.SetGross(details, user, setStatus);
                else InitialWeight.SetGross(details, user, setStatus, Booking.CreatedAt);

                if (!setStatus) return;
                if (Booking.TransportInstruction.ToLocation.IsClient)
                    Status = "Processed";
                else Status = "Transit";
            }
        }
        public void SetReceivalGross(ScaleWeight details, ApplicationUser user)
        {
            if (Booking.DeliveryNoteNumber == null && (details.DeliveryNoteNumber != null))
            {
                Booking.DeliveryNoteNumber = details.DeliveryNoteNumber;
            }
            ReceivalWeight ??= new Weight { };
            if (ReceivalWeight.Gross == 0 || ReceivalWeight.Gross == null)
                ReceivalWeight.SetGross(details, user);
        }
        public void SetReceivalTare(ScaleWeight details, ApplicationUser user)
        {
            if (Booking.DeliveryNoteNumber == null && (details.DeliveryNoteNumber != null))
            {
                Booking.DeliveryNoteNumber = details.DeliveryNoteNumber;
            }
            if (ReceivalWeight.Tare == 0 || ReceivalWeight.Tare == null)
            {
                ReceivalWeight.SetTare(details, user);
                if (Status != "Temp")
                    Status = "Processed";
            }
        }
        public void Print()
        {
            if (Status == Data.EF.DispatchStatus.TRANSIT)
                InitialWeight.Printed = true;
            if (Status == Data.EF.DispatchStatus.PROCESSED)
                ReceivalWeight.Printed = true;
        }
    }
}
