namespace FigureGear.Service.Shared.Filter.Model
{
    public class PagedFilterRequest
    {
        public string? FilterText { get; set; }   // Field[Op]"Value"&Field2[Op]"Value2"
        public string? SearchGlobal { get; set; } // Name,Description[Like]"abc"
        public string? Sort { get; set; }    // ví dụ: "Name.Desc"
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
