namespace BookcaseAPI.Models
{
    public class Client
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = "User"; // "Admin" or "User" 
        public ICollection<Application> Applications { get; set; } = new List<Application>();
    }
}