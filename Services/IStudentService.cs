using SofijaFesis_5DanaUOblacima.DTOs;

namespace SofijaFesis_5DanaUOblacima.Services
{
    public interface IStudentService
    {
        Task<IEnumerable<StudentDTO>> GetAllStudentsAsync();
        Task<StudentDTO?> GetStudentByIdAsync(string id);
        Task<StudentDTO> CreateStudentAsync(CreateStudentDTO createStudentDTO);
        Task<bool> UpdateStudentAsync(string id, UpdateStudentDTO updateStudentDTO);
        Task<bool> DeleteStudentAsync(string id);
    }
}
