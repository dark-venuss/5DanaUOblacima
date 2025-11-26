using SofijaFesis_5DanaUOblacima.Data;
using SofijaFesis_5DanaUOblacima.DTOs;
using SofijaFesis_5DanaUOblacima.Models;
using SofijaFesis_5DanaUOblacima.Validators;
using Microsoft.EntityFrameworkCore;

namespace SofijaFesis_5DanaUOblacima.Services
{
    public class ReservationService : IReservationService
    {
        private readonly ApplicationDbContext _context;
        private readonly ReservationValidator _validator;

        public ReservationService(ApplicationDbContext context, ReservationValidator validator)
        {
            _context = context;
            _validator = validator;
        }

        public async Task<IEnumerable<ReservationDTO>> GetAllReservationsAsync()
        {
            var reservations = await _context.Reservations
                .Include(r => r.Student)
                .Include(r => r.Canteen)
                .ToListAsync();
            return reservations.Select(r => MapToDTO(r)).ToList();
        }

        public async Task<ReservationDTO?> GetReservationByIdAsync(string id)
        {
            var reservation = await _context.Reservations
                .Include(r => r.Student)
                .Include(r => r.Canteen)
                .FirstOrDefaultAsync(r => r.Id == id);
            return reservation == null ? null : MapToDTO(reservation);
        }

        public async Task<IEnumerable<ReservationDTO>> GetReservationsByStudentIdAsync(string studentId)
        {
            var reservations = await _context.Reservations
                .Include(r => r.Student)
                .Include(r => r.Canteen)
                .Where(r => r.StudentId == studentId)
                .ToListAsync();
            return reservations.Select(r => MapToDTO(r)).ToList();
        }

        public async Task<IEnumerable<ReservationDTO>> GetReservationsByCanteenIdAsync(string canteenId)
        {
            var reservations = await _context.Reservations
                .Include(r => r.Student)
                .Include(r => r.Canteen)
                .Where(r => r.CanteenId == canteenId)
                .ToListAsync();
            return reservations.Select(r => MapToDTO(r)).ToList();
        }

        public async Task<(bool success, string message, ReservationDTO? reservation)> CreateReservationAsync(CreateReservationDTO createReservationDTO)
        {
            var validationResult = await _validator.ValidateReservationAsync(createReservationDTO);
            if (!validationResult.isValid)
                return (false, validationResult.message, null);

            if (!DateTime.TryParse(createReservationDTO.Date, out var reservationDate))
                return (false, "Invalid date format", null);

            var reservation = new Reservation
            {
                StudentId = createReservationDTO.StudentId,
                CanteenId = createReservationDTO.CanteenId,
                Date = reservationDate,
                Time = createReservationDTO.Time,
                Duration = createReservationDTO.Duration,
                Status = "Active"
            };

            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            var reservationDTO = MapToDTO(reservation);
            return (true, "Reservation created successfully", reservationDTO);
        }

        public async Task<bool> CancelReservationAsync(string id)
        {
            var reservation = await _context.Reservations.FirstOrDefaultAsync(r => r.Id == id);
            if (reservation == null)
                return false;

            reservation.Status = "Cancelled";
            _context.Reservations.Update(reservation);
            await _context.SaveChangesAsync();

            return true;
        }

        private static ReservationDTO MapToDTO(Reservation reservation)
        {
            return new ReservationDTO
            {
                Id = reservation.Id,
                StudentId = reservation.StudentId,
                StudentName = reservation.Student?.Name,
                CanteenId = reservation.CanteenId,
                CanteenName = reservation.Canteen?.Name,
                Date = reservation.Date.ToString("yyyy-MM-dd"),
                Time = reservation.Time,
                Duration = reservation.Duration,
                Status = reservation.Status
            };
        }
    }
}