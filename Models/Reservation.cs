namespace SofijaFesis_5DanaUOblacima.Models
{
    public class Reservation
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Status { get; set; } = "Active";
        public string StudentId { get; set; } = string.Empty;
        public string CanteenId { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Time { get; set; } = string.Empty;

        public int Duration { get; set; } = 30;
        public Student? Student { get; set; }
        public Canteen? Canteen { get; set; }
    }
}