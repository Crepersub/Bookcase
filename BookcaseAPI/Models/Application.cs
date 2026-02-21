namespace BookcaseAPI.Models
{
    public class Application
    {
        public int Id { get; set; }
        public int MajorId { get; set; }
        public Major Major { get; set; } = null!;
        public int StudentId { get; set; }
        public Client Student { get; set; } = null!;
        public DateTime Deadline { get; set; }
        public string Stage { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public ICollection<ApplicationExam> ApplicationExams { get; set; } = new List<ApplicationExam>();
    }
}