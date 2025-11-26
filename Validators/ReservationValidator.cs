using SofijaFesis_5DanaUOblacima.Data;
using SofijaFesis_5DanaUOblacima.DTOs;
using Microsoft.EntityFrameworkCore;

namespace SofijaFesis_5DanaUOblacima.Validators
{
    public class ReservationValidator
    {
        private readonly ApplicationDbContext _context;

        public ReservationValidator(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<(bool isValid, string message)> ValidateReservationAsync(CreateReservationDTO createReservationDTO)
        {
            var student = await _context.Students.FirstOrDefaultAsync(s => s.Id == createReservationDTO.StudentId);
            if (student == null)
                return (false, "Student not found");

            var canteen = await _context.Canteens
                .Include(c => c.WorkingHours)
                .Include(c => c.Reservations)
                .FirstOrDefaultAsync(c => c.Id == createReservationDTO.CanteenId);
            if (canteen == null)
                return (false, "Canteen not found");

            if (!DateTime.TryParse(createReservationDTO.Date, out var reservationDate))
                return (false, "Invalid date format. Use yyyy-MM-dd");

            if (reservationDate.Date < DateTime.Now.Date)
                return (false, "Reservation date cannot be in the past");

            if (!TimeSpan.TryParse(createReservationDTO.Time, out var reservationTime))
                return (false, "Invalid time format. Use HH:mm");

            if (reservationTime.Minutes != 0 && reservationTime.Minutes != 30)
                return (false, "Reservation time must start at 00 or 30 minutes past the hour");

            if (createReservationDTO.Duration != 30 && createReservationDTO.Duration != 60)
                return (false, "Duration must be 30 or 60 minutes");

            var workingHourMatch = canteen.WorkingHours.FirstOrDefault(wh =>
            {
                if (!TimeSpan.TryParse(wh.From, out var whStart) || !TimeSpan.TryParse(wh.To, out var whEnd))
                    return false;
                
                var endTime = reservationTime.Add(TimeSpan.FromMinutes(createReservationDTO.Duration));
                return reservationTime >= whStart && endTime <= whEnd;
            });

            if (workingHourMatch == null)
                return (false, "Reservation time is not within canteen's working hours");

            var reservationsAtThisTime = canteen.Reservations.Count(r =>
                r.Date.Date == reservationDate.Date &&
                r.Time == createReservationDTO.Time &&
                r.Status == "Active"
            );

            if (reservationsAtThisTime >= canteen.Capacity)
                return (false, "No available capacity for this time slot");
            var studentReservationAtThisTime = await _context.Reservations
                .FirstOrDefaultAsync(r =>
                    r.StudentId == createReservationDTO.StudentId &&
                    r.Date.Date == reservationDate.Date &&
                    r.Time == createReservationDTO.Time &&
                    r.Status == "Active"
                );

            if (studentReservationAtThisTime != null)
                return (false, "Student already has a reservation for this time");

            return (true, "Reservation is valid");
        }
    }
}