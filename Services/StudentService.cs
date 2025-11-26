using SofijaFesis_5DanaUOblacima.Data;
using SofijaFesis_5DanaUOblacima.DTOs;
using SofijaFesis_5DanaUOblacima.Models;
using Microsoft.EntityFrameworkCore;

namespace SofijaFesis_5DanaUOblacima.Services
{
    public class StudentService : IStudentService
    {
        private readonly ApplicationDbContext _context;

        public StudentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<StudentDTO>> GetAllStudentsAsync()
        {
            var students = await _context.Students.ToListAsync();
            return students.Select(s => MapToDTO(s)).ToList();
        }

        public async Task<StudentDTO?> GetStudentByIdAsync(string id)
        {
            var student = await _context.Students.FirstOrDefaultAsync(s => s.Id == id);
            return student == null ? null : MapToDTO(student);
        }

        public async Task<StudentDTO> CreateStudentAsync(CreateStudentDTO createStudentDTO)
        {
            var student = new Student
            {
                Name = createStudentDTO.Name,
                Email = createStudentDTO.Email,
                IsAdmin = createStudentDTO.IsAdmin
            };

            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            return MapToDTO(student);
        }

        public async Task<bool> UpdateStudentAsync(string id, UpdateStudentDTO updateStudentDTO)
        {
            var student = await _context.Students.FirstOrDefaultAsync(s => s.Id == id);
            if (student == null)
                return false;

            if (!string.IsNullOrEmpty(updateStudentDTO.Name))
                student.Name = updateStudentDTO.Name;
            if (!string.IsNullOrEmpty(updateStudentDTO.Email))
                student.Email = updateStudentDTO.Email;
            if (updateStudentDTO.IsAdmin.HasValue)
                student.IsAdmin = updateStudentDTO.IsAdmin.Value;

            _context.Students.Update(student);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteStudentAsync(string id)
        {
            var student = await _context.Students.FirstOrDefaultAsync(s => s.Id == id);
            if (student == null)
                return false;

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();

            return true;
        }

        private static StudentDTO MapToDTO(Student student)
        {
            return new StudentDTO
            {
                Id = student.Id,
                Name = student.Name,
                Email = student.Email,
                IsAdmin = student.IsAdmin
            };
        }
    }
}
