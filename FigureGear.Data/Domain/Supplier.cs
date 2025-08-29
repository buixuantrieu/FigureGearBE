namespace FigureGear.Data.Domain
{
    public class Supplier
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string AddressDetail { get; set; }

        public int CityId { get; set; }
        public City City { get; set; }

        public int DistrictId { get; set; }
        public District District { get; set; }

        public int WardId { get; set; }
        public Ward Ward { get; set; }

        public string Phone { get; set; }
        public string Email { get; set; }
    }
}
