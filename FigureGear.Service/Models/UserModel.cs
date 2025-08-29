namespace FigureGear.Service.Models
{
    public class UserModel
    {
        public Guid? Id { get; set; }

        public string? UserName { get; set; }

        public string? Email { get; set; }

        public string? Password { get; set; }

        public string? SecurityStamp { get; set; } = Guid.NewGuid().ToString();

        public bool IsActive { get; set; } = false;

        public bool IsDeleted { get; set; } = false;

        public bool EmailConfirmed { get; set; } = false;

        public int? FailedLoginCount { get; set; }

        public DateTime? LockoutEnd { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? LastLoginDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public string? RefreshToken { get; set; }

        public DateTime? RefreshTokenExpiryTime { get; set; }
    }
}
