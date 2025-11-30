using Microsoft.AspNetCore.Mvc;
using SofijaFesis_5DanaUOblacima.Models;
using SofijaFesis_5DanaUOblacima.DTOs;
using SofijaFesis_5DanaUOblacima.Services;

namespace SofijaFesis_5DanaUOblacima.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public StudentsController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<StudentDTO>>> GetAllStudents()
        {
            var students = await _studentService.GetAllStudentsAsync();
            return Ok(students);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<StudentDTO>> GetStudentById(string id)
        {
            var student = await _studentService.GetStudentByIdAsync(id);
            if (student == null)
                return NotFound(new { message = "Student not found" });

            return Ok(student);
        }

        [HttpPost]
        public async Task<ActionResult<StudentDTO>> CreateStudent([FromBody] CreateStudentDTO createStudentDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var student = await _studentService.CreateStudentAsync(createStudentDTO);
            return CreatedAtAction(nameof(GetStudentById), new { id = student.Id }, student);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStudent(string id, [FromBody] UpdateStudentDTO updateStudentDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _studentService.UpdateStudentAsync(id, updateStudentDTO);
            if (!result)
                return NotFound(new { message = "Student not found" });

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(string id)
        {
            var result = await _studentService.DeleteStudentAsync(id);
            if (!result)
                return NotFound(new { message = "Student not found" });

            return NoContent();
        }
    }
}
