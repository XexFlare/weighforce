using System;

namespace WeighForce.Filters
{
    public class ReportFilter
    {
        public long? OfficeId { get; set; }
        public DateTime? Date { get; set; }
        public DateTime? ToDate { get; set; }
        public ReportFilter()
        {
            this.OfficeId = null;
            this.Date = null;
        }
        public ReportFilter(DateTime date)
        {
            this.Date = date;
        }
        public ReportFilter(long officeId)
        {
            this.OfficeId = officeId;
        }
        public ReportFilter(long officeId, DateTime date)
        {
            this.OfficeId = officeId;
            this.Date = date;
        }
        public ReportFilter(long officeId, DateTime date, DateTime toDate)
        {
            this.OfficeId = officeId;
            this.Date = date;
            this.ToDate = toDate;
        }
    }
}