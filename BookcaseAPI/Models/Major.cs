using System.Text.Json.Serialization;

namespace BookcaseAPI.Models
{
    public class Major
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string UniversityName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public string GradingSystem { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;

        public int ClientId { get; set; }

        [JsonIgnore]
        public Client Client { get; set; } = null!;

        public MajorStatus Status { get; set; } = MajorStatus.Liked;

        public ICollection<Application> Applications { get; set; } = new List<Application>();
        public ICollection<MajorExam> MajorExams { get; set; } = new List<MajorExam>();
    }
}