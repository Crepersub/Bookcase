namespace BookcaseAPI.Models.Dto
{
    public class UpdateExamDto
    {
        public DateTime Date { get; set; }
        public string Address { get; set; } = string.Empty;
        public string TestName { get; set; } = string.Empty;
    }
}