using System.Collections.Generic;

namespace WeighForce.Models
{
    public class Contract : Syncable
    {
        public string ERPCompany { get; set; }
        public string ContractNumber { get; set; }
        public string UOM { get; set; }
        public string ShipToLocation { get; set; }
        public string OriginPort { get; set; }
        public string Supplier { get; set; }
        public string LoadType { get; set; }
        public string Item { get; set; }
        public string BillOfLadingQuantity { get; set; }
        public string Total { get; set; }
        public string PaymentTerms { get; set; }
        public string PurchasePrice { get; set; }
        public string MethodOfTransport { get; set; }
        public string Vessel { get; set; }
        public string ShippingPeriodTo { get; set; }
        public string EstimatedTimeOfArrival { get; set; }
        public string AllocatedQuantity { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedDate { get; set; }
        public string Territory { get; set; }
        public string AssignedUser { get; set; }
        public string AccpacPort { get; set; }
        public string AccpacDestination { get; set; }
        public bool IsHidden { get; set; }
    }
}