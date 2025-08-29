namespace FigureGear.Data.Domain
{
    public class Series
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }

        public ICollection<Product> Products { get; set; }
    }
}
