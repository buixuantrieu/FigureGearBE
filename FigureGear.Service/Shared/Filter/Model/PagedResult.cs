namespace FigureGear.Service.Shared.Filter.Model
{
    public class PagedResult<T>
    {
        public List<T> Data { get; }
        public int Total { get; }
        public int Page { get; }
        public int PageSize { get; }

        public PagedResult(List<T> data, int total, int page, int pageSize)
        {
            Data = data;
            Total = total;
            Page = page;
            PageSize = pageSize;
        }
    }
}
