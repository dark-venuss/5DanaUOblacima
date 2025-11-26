using Microsoft.AspNetCore.Mvc;
using SofijaFesis_5DanaUOblacima.Models;
using SofijaFesis_5DanaUOblacima.DTOs;
using SofijaFesis_5DanaUOblacima.Services;

namespace SofijaFesis_5DanaUOblacima.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservationsController : ControllerBase
    {
        private readonly IReservationService _reservationService;

        public ReservationsController(IReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReservationDTO>>> GetAllReservations()
        {
            var reservations = await _reservationService.GetAllReservationsAsync();
            return Ok(reservations);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ReservationDTO>> GetReservationById(string id)
        {
            var reservation = await _reservationService.GetReservationByIdAsync(id);
            if (reservation == null)
                return NotFound(new { message = "Reservation not found" });

            return Ok(reservation);
        }

        [HttpGet("student/{studentId}")]
        public async Task<ActionResult<IEnumerable<ReservationDTO>>> GetReservationsByStudentId(string studentId)
        {
            var reservations = await _reservationService.GetReservationsByStudentIdAsync(studentId);
            return Ok(reservations);
        }

        [HttpGet("canteen/{canteenId}")]
        public async Task<ActionResult<IEnumerable<ReservationDTO>>> GetReservationsByCanteenId(string canteenId)
        {
            var reservations = await _reservationService.GetReservationsByCanteenIdAsync(canteenId);
            return Ok(reservations);
        }

        [HttpPost]
        public async Task<ActionResult<ReservationDTO>> CreateReservation([FromBody] CreateReservationDTO createReservationDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _reservationService.CreateReservationAsync(createReservationDTO);
            if (!result.success)
                return BadRequest(new { message = result.message });

            return CreatedAtAction(nameof(GetReservationById), new { id = result.reservation!.Id }, result.reservation);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ReservationDTO>> CancelReservation(
            string id,
            [FromHeader(Name = "studentId")] string? studentId)
        {
            if (string.IsNullOrEmpty(studentId))
                return BadRequest(new { message = "studentId header is required" });

            var reservation = await _reservationService.GetReservationByIdAsync(id);
            if (reservation == null)
                return NotFound(new { message = "Reservation not found" });

            if (reservation.StudentId != studentId)
                return Forbid();

            var updated = await _reservationService.CancelReservationAsync(id);
            if (!updated)
                return NotFound(new { message = "Reservation not found" });

            var updatedReservation = await _reservationService.GetReservationByIdAsync(id);
            return Ok(updatedReservation);
        }
    }
}
