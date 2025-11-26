namespace SofijaFesis_5DanaUOblacima.Models
{
    public class Canteen
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public ICollection<WorkingHour> WorkingHours { get; set; } = new List<WorkingHour>();
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }

    public class WorkingHour
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Meal { get; set; } = string.Empty;
        public string From { get; set; } = string.Empty;
        public string To { get; set; } = string.Empty;
        public string CanteenId { get; set; } = string.Empty;
        public Canteen Canteen { get; set; }
    }
}
