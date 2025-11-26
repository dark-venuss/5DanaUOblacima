using Microsoft.AspNetCore.Mvc;
using SofijaFesis_5DanaUOblacima.Models;
using SofijaFesis_5DanaUOblacima.DTOs;
using SofijaFesis_5DanaUOblacima.Services;

namespace SofijaFesis_5DanaUOblacima.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CanteensController : ControllerBase
    {
        private readonly ICanteenService _canteenService;
        private readonly IStudentService _studentService;

        public CanteensController(ICanteenService canteenService, IStudentService studentService)
        {
            _canteenService = canteenService;
            _studentService = studentService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CanteenDTO>>> GetAllCanteens()
        {
            var canteens = await _canteenService.GetAllCanteensAsync();
            return Ok(canteens);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<CanteenDTO>> GetCanteenById(string id)
        {
            var canteen = await _canteenService.GetCanteenByIdAsync(id);
            if (canteen == null)
                return NotFound(new { message = "Canteen not found" });

            return Ok(canteen);
        }

        [HttpPost]
        public async Task<ActionResult<CanteenDTO>> CreateCanteen(
            [FromBody] CreateCanteenDTO createCanteenDTO,
            [FromHeader(Name = "studentId")] string? studentId)
        {
            if (string.IsNullOrEmpty(studentId))
                return BadRequest(new { message = "studentId header is required" });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var student = await _studentService.GetStudentByIdAsync(studentId);
            if (student == null)
                return NotFound(new { message = "Student not found" });

            if (!student.IsAdmin)
                return Forbid();

            var canteen = await _canteenService.CreateCanteenAsync(createCanteenDTO);
            return CreatedAtAction(nameof(GetCanteenById), new { id = canteen.Id }, canteen);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<CanteenDTO>> UpdateCanteen(
            string id,
            [FromBody] UpdateCanteenDTO updateCanteenDTO,
            [FromHeader(Name = "studentId")] string? studentId)
        {
            if (string.IsNullOrEmpty(studentId))
                return BadRequest(new { message = "studentId header is required" });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var student = await _studentService.GetStudentByIdAsync(studentId);
            if (student == null)
                return NotFound(new { message = "Student not found" });

            if (!student.IsAdmin)
                return Forbid();

            var canteen = await _canteenService.GetCanteenByIdAsync(id);
            if (canteen == null)
                return NotFound(new { message = "Canteen not found" });

            var updated = await _canteenService.UpdateCanteenAsync(id, updateCanteenDTO);
            var updatedCanteen = await _canteenService.GetCanteenByIdAsync(id);
            return Ok(updatedCanteen);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCanteen(
            string id,
            [FromHeader(Name = "studentId")] string? studentId)
        {
            if (string.IsNullOrEmpty(studentId))
                return BadRequest(new { message = "studentId header is required" });

            var student = await _studentService.GetStudentByIdAsync(studentId);
            if (student == null)
                return NotFound(new { message = "Student not found" });

            if (!student.IsAdmin)
                return Forbid();

            var result = await _canteenService.DeleteCanteenAsync(id);
            if (!result)
                return NotFound(new { message = "Canteen not found" });

            return NoContent();
        }

        [HttpGet("status")]
        public async Task<ActionResult> GetCanteensStatus(
            [FromQuery] string startDate,
            [FromQuery] string endDate,
            [FromQuery] string startTime,
            [FromQuery] string endTime,
            [FromQuery] int duration = 30)
        {
            try
            {
                var status = await _canteenService.GetCanteensStatusAsync(startDate, endDate, startTime, endTime, duration);
                return Ok(status);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}/status")]
        public async Task<ActionResult> GetCanteenStatus(
            string id,
            [FromQuery] string startDate,
            [FromQuery] string endDate,
            [FromQuery] string startTime,
            [FromQuery] string endTime,
            [FromQuery] int duration = 30)
        {
            try
            {
                var canteen = await _canteenService.GetCanteenByIdAsync(id);
                if (canteen == null)
                    return NotFound(new { message = "Canteen not found" });

                var status = await _canteenService.GetCanteenStatusAsync(id, startDate, endDate, startTime, endTime, duration);
                return Ok(status);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
