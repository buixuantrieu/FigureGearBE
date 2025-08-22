namespace FigureGear.Data.Domain
{
    public class City
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public ICollection<District> Districts { get; set; }
    }
}
