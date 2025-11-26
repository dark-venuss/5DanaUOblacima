namespace SofijaFesis_5DanaUOblacima.DTOs
{
    public class StudentDTO
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsAdmin { get; set; }
    }

    public class CreateStudentDTO
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsAdmin { get; set; }
    }

    public class UpdateStudentDTO
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public bool? IsAdmin { get; set; }
    }
}