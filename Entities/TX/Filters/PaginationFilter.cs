using System;

namespace WeighForce.Filters
{
    public class PaginationFilter
    {
        public int Page { get; set; }
        public int Size { get; set; }
        public string Type { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public PaginationFilter()
        {
            this.Page = 1;
            this.Size = 50;
            this.From = DateTime.UtcNow.AddDays(-15);
            this.To = DateTime.UtcNow;
            this.Type = "";
        }
        public PaginationFilter(int page, int size)
        {
            this.Page = page < 1 ? 1 : page;
            this.Size = size > 100 ? 100 : size;
            this.From = DateTime.UtcNow.AddDays(-15);
            this.To = DateTime.UtcNow;
            this.Type = "";
        }
        public PaginationFilter(int page, int size, DateTime from, DateTime to)
        {
            this.Page = page < 1 ? 1 : page;
            this.Size = size > 100 ? 100 : size;
            this.From = from;
            this.To = to;
            this.Type = "";
        }
        public PaginationFilter(int page, int size, DateTime from, DateTime to, string type)
        {
            this.Page = page < 1 ? 1 : page;
            this.Size = size > 100 ? 100 : size;
            this.From = from;
            this.To = to;
            this.Type = type == null ? "" : type;
        }
    }
}