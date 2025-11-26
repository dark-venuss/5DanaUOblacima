namespace SofijaFesis_5DanaUOblacima.DTOs
{
    public class WorkingHourDTO
    {
        public string Meal { get; set; } = string.Empty;
        public string From { get; set; } = string.Empty;
        public string To { get; set; } = string.Empty;
    }

    public class CanteenDTO
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public ICollection<WorkingHourDTO> WorkingHours { get; set; } = new List<WorkingHourDTO>();
    }

    public class CreateCanteenDTO
    {
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public ICollection<WorkingHourDTO> WorkingHours { get; set; } = new List<WorkingHourDTO>();
    }

    public class UpdateCanteenDTO
    {
        public string? Name { get; set; }
        public string? Location { get; set; }
        public int? Capacity { get; set; }
        public ICollection<WorkingHourDTO>? WorkingHours { get; set; }
    }
}