namespace FigureGear.Service.Models
{
    public class RoleModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public List<int> PermissionIds { get; set; } = new List<int>();

        public List<PermissionModel> Permissions { get; set; } = new List<PermissionModel>();
    }
}
