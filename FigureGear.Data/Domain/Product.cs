namespace FigureGear.Data.Domain
{
    public class Product
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string? Description { get; set; }

        public int ManufacturerId { get; set; }            
        public Manufacturer Manufacturer { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public int MaterialId { get; set; }
        public Material Material { get; set; }

        public int SeriesId { get; set; }
        public Series Series { get; set; }

        public decimal Price { get; set; }

        public int StockQuantity { get; set; } = 0;

        public int ReservedQuantity { get; set; } = 0;

        public Guid CreatedBy { get; set; }
        public User CreatedUser { get; set; }

        public bool IsDeleted { get; set; } = false;

        public Guid UpdatedBy { get; set; }
        public User UpdatedUser { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; } = DateTime.Now;

        public ICollection<ProductImage> Images { get; set; }

        public ICollection<ProductTag> ProductTags { get; set; }
    }
}
