using SofijaFesis_5DanaUOblacima.DTOs;

namespace SofijaFesis_5DanaUOblacima.Services
{
    public interface IReservationService
    {
        Task<IEnumerable<ReservationDTO>> GetAllReservationsAsync();
        Task<ReservationDTO?> GetReservationByIdAsync(string id);
        Task<IEnumerable<ReservationDTO>> GetReservationsByStudentIdAsync(string studentId);
        Task<IEnumerable<ReservationDTO>> GetReservationsByCanteenIdAsync(string canteenId);
        Task<(bool success, string message, ReservationDTO? reservation)> CreateReservationAsync(CreateReservationDTO createReservationDTO);
        Task<bool> CancelReservationAsync(string id);
    }
}
