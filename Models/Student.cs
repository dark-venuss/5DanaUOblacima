namespace SofijaFesis_5DanaUOblacima.Models
{
    public class Student
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsAdmin { get; set; } = false;

        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}