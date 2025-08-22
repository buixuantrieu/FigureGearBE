namespace FigureGear.Data.Domain
{
    public class ProductImage
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public string ImageUrl { get; set; }

        public bool IsMain { get; set; }

        public int SortOrder { get; set; }
    }
}
