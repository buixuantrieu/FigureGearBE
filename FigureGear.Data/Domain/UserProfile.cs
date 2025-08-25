namespace FigureGear.Data.Domain
{
    public class UserProfile
    {
        public Guid UserId { get; set; }

        public User User { get; set; }

        public string? FullName { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }

        public string? AvatarUrl { get; set; }
    }
}
