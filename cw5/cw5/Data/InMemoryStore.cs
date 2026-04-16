using cw5.Models;

namespace cw5.Data;

public class InMemoryStore
{
    public static List<Room> Rooms { get; set; } = new()
    {
        new Room { Id = 1, Name = "Lab 101", BuildingCode = "A", Floor = 1, Capacity = 30, HasProjector = true, IsActive = true },
        new Room { Id = 2, Name = "Sala Wykładowa 1", BuildingCode = "A", Floor = 1, Capacity = 150, HasProjector = true, IsActive = true },
        new Room { Id = 3, Name = "Pokój Spotkań 1", BuildingCode = "B", Floor = 2, Capacity = 8, HasProjector = false, IsActive = true },
        new Room { Id = 4, Name = "Magazyn", BuildingCode = "C", Floor = -1, Capacity = 0, HasProjector = false, IsActive = false }
    };

    public static List<Reservation> Reservations { get; set; } = new()
    {
        new Reservation { Id = 1, RoomId = 1, OrganizerName = "Jan Kowalski", Topic = "Wstęp do C#", Date = new DateOnly(2026, 5, 10), StartTime = new TimeSpan(8, 30, 0), EndTime = new TimeSpan(10, 0, 0), Status = "confirmed" },
        new Reservation { Id = 2, RoomId = 1, OrganizerName = "Jan Kowalski", Topic = "Wstęp do C# cz.2", Date = new DateOnly(2026, 5, 10), StartTime = new TimeSpan(10, 15, 0), EndTime = new TimeSpan(11, 45, 0), Status = "confirmed" },
        new Reservation { Id = 3, RoomId = 2, OrganizerName = "Anna Nowak", Topic = "Wykład Architektura", Date = new DateOnly(2026, 5, 11), StartTime = new TimeSpan(14, 0, 0), EndTime = new TimeSpan(15, 30, 0), Status = "planned" },
        new Reservation { Id = 4, RoomId = 3, OrganizerName = "Piotr Wiśniewski", Topic = "Konsultacje projektowe", Date = new DateOnly(2026, 5, 12), StartTime = new TimeSpan(8, 30, 0), EndTime = new TimeSpan(10, 0, 0), Status = "confirmed" }
    };
};
