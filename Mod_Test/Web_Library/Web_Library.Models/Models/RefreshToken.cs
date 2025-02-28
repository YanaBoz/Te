namespace Web_Library.Models
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string Token { get; set; } = null!;
        public DateTime Expires { get; set; }
        public bool IsExpired => DateTime.UtcNow >= Expires;
        public string UserId { get; set; } = null!;
        public User User { get; set; } = null!;
    }

}
