namespace FigureGear.Service.Models
{
    public class UserModel
    {
        public Guid? Id { get; set; }

        public string UserName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string SecurityStamp { get; set; } = Guid.NewGuid().ToString();

        public bool IsActive { get; set; } = true;

        public bool IsDeleted { get; set; } = false;

        public bool EmailConfirmed { get; set; } = false;

        public int FailedLoginCount { get; set; } = 0;

        public DateTime? LockoutEnd { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? LastLoginDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public string? RefreshToken { get; set; }

        public DateTime? RefreshTokenExpiryTime { get; set; }
    }
}
