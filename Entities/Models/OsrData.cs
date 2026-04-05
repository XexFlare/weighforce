using System;

namespace WeighForce.Models
{
    public class OsrData : Syncable
    {
        public long? gdnId { get; set; }
        public DateTime Date { get; set; }
        public string Product { get; set; }
        public string Vessel { get; set; }
        public string Wagon { get; set; }
        public double Tare { get; set; }
        public double Gross { get; set; }
        public string CRMReleaseNo { get; set; }
        public string Warehouse { get; set; }
        public string TeamStats { get; set; }
        public string Bl { get; set; }
        public string GdnValis { get; set; }
        public string WagonType { get; set; }
        public string SealNumber { get; set; }
        public int Bags { get; set; }
        public double Unit { get; set; }
        public double ProductWeight { get; set; }
        public string RailedAtTrain { get; set; }
        public DateTime ArrivalDate { get; set; }
        public int BagsReceived { get; set; }
        public double Wet { get; set; }
        public double QtyReceived { get; set; }
        public double Diff { get; set; }
    }
}