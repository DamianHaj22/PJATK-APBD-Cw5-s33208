using cw5.Data;
using cw5.Models;
using Microsoft.AspNetCore.Mvc;

namespace cw5.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReservationsController : ControllerBase
{
    [HttpGet]
    public IActionResult GetReservations([FromQuery] DateOnly? date, [FromQuery] string? status, [FromQuery] int? roomId)
    {
        var query = InMemoryStore.Reservations.AsQueryable();

        if (date.HasValue)
            query = query.Where(r => r.Date == date.Value);
            
        if (!string.IsNullOrEmpty(status))
            query = query.Where(r => r.Status.Equals(status, StringComparison.OrdinalIgnoreCase));
            
        if (roomId.HasValue)
            query = query.Where(r => r.RoomId == roomId.Value);

        return Ok(query.ToList());
    }
    
    [HttpGet("{id}")]
    public IActionResult GetReservationById(int id)
    {
        var reservation = InMemoryStore.Reservations.FirstOrDefault(r => r.Id == id);
        if (reservation == null) return NotFound($"Reservation with ID {id} not found.");
        
        return Ok(reservation);
    }
    
    [HttpPost]
    public IActionResult CreateReservation([FromBody] Reservation reservation)
    {
        // sprawdzenie sali
        var room = InMemoryStore.Rooms.FirstOrDefault(r => r.Id == reservation.RoomId);
        if (room == null) 
            return NotFound($"Room with ID {reservation.RoomId} does not exist.");
            
        if (!room.IsActive) 
            return BadRequest($"Room with ID {reservation.RoomId} is not active.");

        // sprawdzenie konfliktów czasowych
        bool isOverlap = InMemoryStore.Reservations.Any(r =>
            r.RoomId == reservation.RoomId &&
            r.Date == reservation.Date &&
            r.StartTime < reservation.EndTime && 
            r.EndTime > reservation.StartTime);

        if (isOverlap)
            return Conflict("The reservation conflicts with an existing reservation for this room on the given date and time.");

        reservation.Id = InMemoryStore.Reservations.Count > 0 ? InMemoryStore.Reservations.Max(r => r.Id) + 1 : 1;
        InMemoryStore.Reservations.Add(reservation);
        
        return CreatedAtAction(nameof(GetReservationById), new { id = reservation.Id }, reservation);
    }
    
    [HttpPut("{id}")]
    public IActionResult UpdateReservation(int id, [FromBody] Reservation updatedReservation)
    {
        var existingReservation = InMemoryStore.Reservations.FirstOrDefault(r => r.Id == id);
        if (existingReservation == null) return NotFound($"Reservation with ID {id} not found.");

        var room = InMemoryStore.Rooms.FirstOrDefault(r => r.Id == updatedReservation.RoomId);
        if (room == null) return NotFound($"Room with ID {updatedReservation.RoomId} does not exist.");
        if (!room.IsActive) return BadRequest("Can't assign reservation to an inactive room.");
        
        bool isOverlap = InMemoryStore.Reservations.Any(r =>
            r.Id != id &&
            r.RoomId == updatedReservation.RoomId &&
            r.Date == updatedReservation.Date &&
            r.StartTime < updatedReservation.EndTime && 
            r.EndTime > updatedReservation.StartTime);

        if (isOverlap)
            return Conflict("The updated reservation time conflicts with another existing reservation.");

        existingReservation.RoomId = updatedReservation.RoomId;
        existingReservation.OrganizerName = updatedReservation.OrganizerName;
        existingReservation.Topic = updatedReservation.Topic;
        existingReservation.Date = updatedReservation.Date;
        existingReservation.StartTime = updatedReservation.StartTime;
        existingReservation.EndTime = updatedReservation.EndTime;
        existingReservation.Status = updatedReservation.Status;

        return Ok(existingReservation);
    }
    
    [HttpDelete("{id}")]
    public IActionResult DeleteReservation(int id)
    {
        var reservation = InMemoryStore.Reservations.FirstOrDefault(r => r.Id == id);
        if (reservation == null) return NotFound($"Reservation with ID {id} not found.");

        InMemoryStore.Reservations.Remove(reservation);
        return NoContent();
    }
}