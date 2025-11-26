namespace SofijaFesis_5DanaUOblacima.DTOs
{
    public class ReservationDTO
    {
        public string Id { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string StudentId { get; set; } = string.Empty;
        public string? StudentName { get; set; }
        public string CanteenId { get; set; } = string.Empty;
        public string? CanteenName { get; set; }
        public string Date { get; set; } = string.Empty; // yyyy-MM-dd
        public string Time { get; set; } = string.Empty; // HH:mm
        public int Duration { get; set; } // 30 to 60 minutes
    }

    public class CreateReservationDTO
    {
        public string StudentId { get; set; } = string.Empty;
        public string CanteenId { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty; // yyyy-MM-dd
        public string Time { get; set; } = string.Empty; // HH:mm
        public int Duration { get; set; } // 30 or 60
    }

    public class UpdateReservationDTO
    {
        public string? Status { get; set; }
        public string? Time { get; set; }
        public int? Duration { get; set; }
    }
}