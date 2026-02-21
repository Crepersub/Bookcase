namespace BookcaseAPI.Models
{
    public class ApplicationExam
    {
        public int ApplicationId { get; set; }
        public Application Application { get; set; } = null!;
        public int ExamId { get; set; }
        public Exam Exam { get; set; } = null!;
    }
}