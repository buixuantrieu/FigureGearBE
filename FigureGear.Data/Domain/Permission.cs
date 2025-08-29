namespace FigureGear.Data.Domain
{
    public class Permission:BaseEntity
    {
        public int Id { get; set; }

        public string Key { get; set; }

        public string? Description { get; set; }


        public User CreatedByUser { get; set; }

        public User? UpdatedByUser { get; set; }

        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}
