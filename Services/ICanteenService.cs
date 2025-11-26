using SofijaFesis_5DanaUOblacima.DTOs;

namespace SofijaFesis_5DanaUOblacima.Services
{
    public interface ICanteenService
    {
        Task<IEnumerable<CanteenDTO>> GetAllCanteensAsync();
        Task<CanteenDTO?> GetCanteenByIdAsync(string id);
        Task<CanteenDTO> CreateCanteenAsync(CreateCanteenDTO createCanteenDTO);
        Task<bool> UpdateCanteenAsync(string id, UpdateCanteenDTO updateCanteenDTO);
        Task<bool> DeleteCanteenAsync(string id);
        Task<dynamic> GetCanteensStatusAsync(string startDate, string endDate, string startTime, string endTime, int duration);
        Task<dynamic> GetCanteenStatusAsync(string canteenId, string startDate, string endDate, string startTime, string endTime, int duration);
    }
}
