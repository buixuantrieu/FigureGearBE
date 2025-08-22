namespace FigureGear.Data.Domain
{
    public class Tag
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string? Color { get; set; }

        public ICollection<ProductTag> ProductTags { get; set; }
    }
}
