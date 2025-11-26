using SofijaFesis_5DanaUOblacima.Data;
using SofijaFesis_5DanaUOblacima.DTOs;
using SofijaFesis_5DanaUOblacima.Models;
using Microsoft.EntityFrameworkCore;

namespace SofijaFesis_5DanaUOblacima.Services
{
    public class CanteenService : ICanteenService
    {
        private readonly ApplicationDbContext _context;

        public CanteenService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CanteenDTO>> GetAllCanteensAsync()
        {
            var canteens = await _context.Canteens.Include(c => c.WorkingHours).ToListAsync();
            return canteens.Select(c => MapToDTO(c)).ToList();
        }

        public async Task<CanteenDTO?> GetCanteenByIdAsync(string id)
        {
            var canteen = await _context.Canteens
                .Include(c => c.WorkingHours)
                .FirstOrDefaultAsync(c => c.Id == id);
            return canteen == null ? null : MapToDTO(canteen);
        }

        public async Task<CanteenDTO> CreateCanteenAsync(CreateCanteenDTO createCanteenDTO)
        {
            var canteen = new Canteen
            {
                Name = createCanteenDTO.Name,
                Location = createCanteenDTO.Location,
                Capacity = createCanteenDTO.Capacity,
                WorkingHours = new List<WorkingHour>()
            };

            // Add working hours if provided
            if (createCanteenDTO.WorkingHours != null && createCanteenDTO.WorkingHours.Count > 0)
            {
                foreach (var wh in createCanteenDTO.WorkingHours)
                {
                    canteen.WorkingHours.Add(new WorkingHour
                    {
                        Meal = wh.Meal,
                        From = wh.From,
                        To = wh.To,
                        CanteenId = canteen.Id
                    });
                }
            }

            _context.Canteens.Add(canteen);
            await _context.SaveChangesAsync();

            return MapToDTO(canteen);
        }

        public async Task<bool> UpdateCanteenAsync(string id, UpdateCanteenDTO updateCanteenDTO)
        {
            var canteen = await _context.Canteens
                .Include(c => c.WorkingHours)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (canteen == null)
                return false;

            if (!string.IsNullOrEmpty(updateCanteenDTO.Name))
                canteen.Name = updateCanteenDTO.Name;
            if (!string.IsNullOrEmpty(updateCanteenDTO.Location))
                canteen.Location = updateCanteenDTO.Location;
            if (updateCanteenDTO.Capacity.HasValue)
                canteen.Capacity = updateCanteenDTO.Capacity.Value;
            
            if (updateCanteenDTO.WorkingHours != null)
            {
                canteen.WorkingHours.Clear();
                foreach (var wh in updateCanteenDTO.WorkingHours)
                {
                    canteen.WorkingHours.Add(new WorkingHour
                    {
                        Meal = wh.Meal,
                        From = wh.From,
                        To = wh.To,
                        CanteenId = canteen.Id
                    });
                }
            }

            _context.Canteens.Update(canteen);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteCanteenAsync(string id)
        {
            var canteen = await _context.Canteens
                .Include(c => c.Reservations)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (canteen == null)
                return false;
            foreach (var reservation in canteen.Reservations)
            {
                reservation.Status = "Cancelled";
            }

            _context.Canteens.Remove(canteen);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<dynamic> GetCanteensStatusAsync(string startDate, string endDate, string startTime, string endTime, int duration)
        {
            if (!DateTime.TryParse(startDate, out var start) || !DateTime.TryParse(endDate, out var end))
                throw new ArgumentException("Invalid date format");

            if (!TimeSpan.TryParse(startTime, out var startTimeSpan) || !TimeSpan.TryParse(endTime, out var endTimeSpan))
                throw new ArgumentException("Invalid time format");

            var canteens = await _context.Canteens
                .Include(c => c.WorkingHours)
                .Include(c => c.Reservations)
                .ToListAsync();

            var result = new List<dynamic>();

            foreach (var canteen in canteens)
            {
                var slots = CalculateCapacitySlots(canteen, start, end, startTimeSpan, endTimeSpan, duration);
                result.Add(new
                {
                    canteenId = canteen.Id,
                    slots = slots
                });
            }

            return result;
        }

        public async Task<dynamic> GetCanteenStatusAsync(string canteenId, string startDate, string endDate, string startTime, string endTime, int duration)
        {
            if (!DateTime.TryParse(startDate, out var start) || !DateTime.TryParse(endDate, out var end))
                throw new ArgumentException("Invalid date format");

            if (!TimeSpan.TryParse(startTime, out var startTimeSpan) || !TimeSpan.TryParse(endTime, out var endTimeSpan))
                throw new ArgumentException("Invalid time format");

            var canteen = await _context.Canteens
                .Include(c => c.WorkingHours)
                .Include(c => c.Reservations)
                .FirstOrDefaultAsync(c => c.Id == canteenId);

            if (canteen == null)
                return null;

            var slots = CalculateCapacitySlots(canteen, start, end, startTimeSpan, endTimeSpan, duration);

            return new
            {
                canteenId = canteen.Id,
                slots = slots
            };
        }

        private List<dynamic> CalculateCapacitySlots(Canteen canteen, DateTime startDate, DateTime endDate, TimeSpan startTime, TimeSpan endTime, int duration)
        {
            var slots = new List<dynamic>();

            for (var currentDate = startDate; currentDate <= endDate; currentDate = currentDate.AddDays(1))
            {
                var dayOfWeek = currentDate.DayOfWeek.ToString().ToLower();

                foreach (var workingHour in canteen.WorkingHours)
                {
                    if (!TimeSpan.TryParse(workingHour.From, out var whStart) || 
                        !TimeSpan.TryParse(workingHour.To, out var whEnd))
                        continue;

                    // Generate 30-minute or 1-hour slots within the working hours and requested time range
                    var slotStart = new TimeSpan(Math.Max(startTime.Ticks, whStart.Ticks));
                    var slotEnd = new TimeSpan(Math.Min(endTime.Ticks, whEnd.Ticks));

                    while (slotStart.Add(TimeSpan.FromMinutes(duration)) <= slotEnd)
                    {
                        var reservationCount = canteen.Reservations.Count(r =>
                            r.Date.Date == currentDate.Date &&
                            r.Status == "Active" &&
                            r.Time == slotStart.ToString(@"hh\:mm") &&
                            workingHour.Meal.Equals(GetMealType(r.Time, canteen.WorkingHours), StringComparison.OrdinalIgnoreCase));

                        var remainingCapacity = canteen.Capacity - reservationCount;

                        slots.Add(new
                        {
                            date = currentDate.ToString("yyyy-MM-dd"),
                            meal = workingHour.Meal,
                            startTime = slotStart.ToString(@"hh\:mm"),
                            remainingCapacity = Math.Max(0, remainingCapacity)
                        });

                        slotStart = slotStart.Add(TimeSpan.FromMinutes(30));
                    }
                }
            }

            return slots;
        }

        private string GetMealType(string time, ICollection<WorkingHour> workingHours)
        {
            if (!TimeSpan.TryParse(time, out var timeSpan))
                return "";

            foreach (var wh in workingHours)
            {
                if (TimeSpan.TryParse(wh.From, out var from) && TimeSpan.TryParse(wh.To, out var to))
                {
                    if (timeSpan >= from && timeSpan < to)
                        return wh.Meal;
                }
            }

            return "";
        }

        private static CanteenDTO MapToDTO(Canteen canteen)
        {
            return new CanteenDTO
            {
                Id = canteen.Id,
                Name = canteen.Name,
                Location = canteen.Location,
                Capacity = canteen.Capacity,
                WorkingHours = canteen.WorkingHours.Select(w => new WorkingHourDTO
                {
                    Meal = w.Meal,
                    From = w.From,
                    To = w.To
                }).ToList()
            };
        }
    }
}
