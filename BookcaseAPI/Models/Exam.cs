using System.Text.Json.Serialization;

namespace BookcaseAPI.Models
{
    public class Exam
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Address { get; set; } = string.Empty;
        public string TestName { get; set; } = string.Empty;

        public int ClientId { get; set; }

        [JsonIgnore]
        public Client Client { get; set; } = null!;

        [JsonIgnore]
        public ICollection<ApplicationExam> ApplicationExams { get; set; } = new List<ApplicationExam>();

        [JsonIgnore]
        public ICollection<MajorExam> MajorExams { get; set; } = new List<MajorExam>();
    }
}
