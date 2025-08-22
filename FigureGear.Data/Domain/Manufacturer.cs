namespace FigureGear.Data.Domain
{
    public class Manufacturer
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Country { get; set; }

        public string? Description { get; set; }

        public ICollection<Product> Products { get; set; }
    }
}
