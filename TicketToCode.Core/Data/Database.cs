using static BCrypt.Net.BCrypt;
using TicketToCode.Core.Models;

namespace TicketToCode.Core.Data;

public interface IDatabase
{
    List<Event> Events { get; set; }
    List<User> Users { get; set; }
    List<Booking> Bookings { get; set; }
}

public class Database : IDatabase
{
    public List<Event> Events { get; set; } = new List<Event>();
    public List<User> Users { get; set; } = new List<User>();
    public List<Booking> Bookings { get; set; } = new List<Booking>();

    public Database()
    {
        SeedData();
    }

    private void SeedData()
    {
        Users.Add(new User("admin", HashPassword("admin123"))
        {
            Id = 1,
            Role = UserRoles.Admin
        });

        Users.Add(new User("testuser", HashPassword("test123"))
        {
            Id = 2,
            Role = UserRoles.User
        });

        Events.AddRange(new[]
        {
            new Event
            {
                Id = 1,
                Name = "Webbutveckling Workshop",
                Description = "Intensiv workshop i modern webbutveckling med ASP.NET Core och Blazor",
                Type = EventType.Other,
                StartTime = DateTime.Now.AddDays(14),
                EndTime = DateTime.Now.AddDays(14).AddHours(6),
                MaxAttendees = 50
            },
            new Event
            {
                Id = 2,
                Name = "Rock Konsert - The Developers",
                Description = "En episk konsert med bandet The Developers som spelar hits om kodning",
                Type = EventType.Concert,
                StartTime = DateTime.Now.AddDays(30),
                EndTime = DateTime.Now.AddDays(30).AddHours(3),
                MaxAttendees = 200
            },
            new Event
            {
                Id = 3,
                Name = "Tech Festival 2025",
                Description = "Största tech-festivalen i Norden med föreläsningar, workshops och networking",
                Type = EventType.Festival,
                StartTime = DateTime.Now.AddDays(60),
                EndTime = DateTime.Now.AddDays(62),
                MaxAttendees = 1000
            },
            new Event
            {
                Id = 4,
                Name = "Teater: Hamlet 2.0",
                Description = "Shakespeares klassiker i modern tappning med AI och robotar",
                Type = EventType.Theatre,
                StartTime = DateTime.Now.AddDays(45),
                EndTime = DateTime.Now.AddDays(45).AddHours(2.5),
                MaxAttendees = 100
            }
        });

        Bookings.AddRange(new[]
        {
            new Booking
            {
                Id = 1,
                EventId = 1,
                UserId = 2,
                UserName = "testuser",
                EventName = "Webbutveckling Workshop",
                NumberOfTickets = 2,
                TotalPrice = 598.00m,
                BookingDate = DateTime.Now.AddDays(-5)
            },
            new Booking
            {
                Id = 2,
                EventId = 2,
                UserId = 2,
                UserName = "testuser",
                EventName = "Rock Konsert - The Developers",
                NumberOfTickets = 1,
                TotalPrice = 450.00m,
                BookingDate = DateTime.Now.AddDays(-2)
            }
        });
    }
}