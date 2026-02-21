using System.Text.Json.Serialization;

namespace BookcaseAPI.Models
{
    public class MajorExam
    {
        public int MajorId { get; set; }

        [JsonIgnore]
        public Major Major { get; set; } = null!;

        public int ExamId { get; set; }
        public Exam Exam { get; set; } = null!;
    }
}