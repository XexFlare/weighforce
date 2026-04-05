using AutoMapper;
using WeighForce.Filters;

namespace WeighForce.Wrappers
{
    public class PagedResponse<TE, T> : Response<T>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalRecords { get; set; }
        public PagedResponse(PagedQuery<TE> data, PaginationFilter filter, IMapper mapper)
        {
            this.PageNumber = filter.Page;
            this.PageSize = filter.Size;
            this.Data = mapper.Map<T>(data.Records);
            this.Message = null;
            this.Success = true;
            this.Errors = null;
            this.TotalPages = data.TotalPages;
            this.TotalRecords = data.TotalRecords;
        }
    }
    public class PagedQuery<T>
    {
        public int TotalRecords { get; set; }
        public int TotalPages { get; set; }
        public T Records { get; set; }
    }
}