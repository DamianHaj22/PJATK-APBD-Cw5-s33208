using cw5.Data;
using cw5.Models;
using Microsoft.AspNetCore.Mvc;

namespace cw5.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoomsController : ControllerBase
{
    [HttpGet]
    public IActionResult GetRooms([FromQuery] int? minCapacity, [FromQuery] bool? hasProjector, [FromQuery] bool? activeOnly)
    {
        var query = InMemoryStore.Rooms.AsQueryable();

        if (minCapacity.HasValue)
            query = query.Where(r => r.Capacity >= minCapacity.Value);
            
        if (hasProjector.HasValue)
            query = query.Where(r => r.HasProjector == hasProjector.Value);
            
        if (activeOnly.HasValue && activeOnly.Value)
            query = query.Where(r => r.IsActive);

        return Ok(query.ToList());
    }
    
    [HttpGet("{id}")]
    public IActionResult GetRoomById(int id)
    {
        var room = InMemoryStore.Rooms.FirstOrDefault(r => r.Id == id);
        if (room == null) return NotFound($"Room with ID {id} not found.");
        
        return Ok(room);
    }
    
    [HttpGet("building/{buildingCode}")]
    public IActionResult GetRoomsByBuilding(string buildingCode)
    {
        var rooms = InMemoryStore.Rooms
            .Where(r => r.BuildingCode.Equals(buildingCode, StringComparison.OrdinalIgnoreCase))
            .ToList();
            
        return Ok(rooms);
    }
    
    [HttpPost]
    public IActionResult CreateRoom([FromBody] Room room)
    {
        room.Id = InMemoryStore.Rooms.Count > 0 ? InMemoryStore.Rooms.Max(r => r.Id) + 1 : 1;
        InMemoryStore.Rooms.Add(room);
        
        return CreatedAtAction(nameof(GetRoomById), new { id = room.Id }, room);
    }
    
    [HttpPut("{id}")]
    public IActionResult UpdateRoom(int id, [FromBody] Room updatedRoom)
    {
        var existingRoom = InMemoryStore.Rooms.FirstOrDefault(r => r.Id == id);
        if (existingRoom == null) return NotFound($"Room with ID {id} not found.");

        existingRoom.Name = updatedRoom.Name;
        existingRoom.BuildingCode = updatedRoom.BuildingCode;
        existingRoom.Floor = updatedRoom.Floor;
        existingRoom.Capacity = updatedRoom.Capacity;
        existingRoom.HasProjector = updatedRoom.HasProjector;
        existingRoom.IsActive = updatedRoom.IsActive;

        return Ok(existingRoom);
    }
    
    [HttpDelete("{id}")]
    public IActionResult DeleteRoom(int id)
    {
        var room = InMemoryStore.Rooms.FirstOrDefault(r => r.Id == id);
        if (room == null) return NotFound($"Room with ID {id} not found.");

        var hasFutureReservations = InMemoryStore.Reservations.Any(res => res.RoomId == id && res.Date >= DateOnly.FromDateTime(DateTime.Now));
        if (hasFutureReservations)
        {
            return Conflict("Can't delete room.");
        }

        InMemoryStore.Rooms.Remove(room);
        return NoContent();
    }
}