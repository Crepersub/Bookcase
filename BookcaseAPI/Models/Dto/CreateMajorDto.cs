namespace BookcaseAPI.Models.Dto
{
    public class CreateMajorDto
    {
        public string Name { get; set; } = string.Empty;
        public string UniversityName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public string GradingSystem { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public string Status { get; set; } = "Liked";
    }
}