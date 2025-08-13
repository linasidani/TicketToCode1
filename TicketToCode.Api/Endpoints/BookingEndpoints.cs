using TicketToCode.Core.Data;
using TicketToCode.Core.Models;

namespace TicketToCode.Api.Endpoints;

public static class BookingEndpoints
{
    public static void MapBookingEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/bookings").WithTags("Bookings");

        group.MapPost("/", CreateBooking)
            .WithName("CreateBooking")
            .WithSummary("Book tickets for an event");

        group.MapGet("/user/{userId}", GetUserBookings)
            .WithName("GetUserBookings")
            .WithSummary("Get all bookings for a user");

        group.MapGet("/", GetAllBookings)
            .WithName("GetAllBookings")
            .WithSummary("Get all bookings (admin only)");

        group.MapDelete("/{id}", CancelBooking)
            .WithName("CancelBooking")
            .WithSummary("Cancel a booking");

        group.MapGet("/event/{eventId}", GetEventBookings)
            .WithName("GetEventBookings")
            .WithSummary("Get all bookings for an event (admin only)");
    }

    private static IResult CreateBooking(BookingRequest request, IDatabase database)
    {
        try
        {
            Console.WriteLine("=== CreateBooking API endpoint called ===");
            Console.WriteLine($"EventId: {request.EventId}, UserId: {request.UserId}, UserName: {request.UserName}, Tickets: {request.NumberOfTickets}");
            
            var eventItem = database.Events.FirstOrDefault(e => e.Id == request.EventId);
            if (eventItem == null)
            {
                Console.WriteLine("Event not found");
                return Results.NotFound("Event not found");
            }

            var existingBookings = database.Bookings
                .Where(b => b.EventId == request.EventId && b.Status == BookingStatus.Active)
                .Sum(b => b.NumberOfTickets);

            if (existingBookings + request.NumberOfTickets > eventItem.MaxAttendees)
            {
                Console.WriteLine("Not enough tickets available");
                return Results.BadRequest("Not enough tickets available");
            }

            var newId = database.Bookings.Any() ? database.Bookings.Max(b => b.Id) + 1 : 1;
            
            var booking = new Booking
            {
                Id = newId,
                EventId = request.EventId,
                UserId = request.UserId,
                UserName = request.UserName,
                EventName = eventItem.Name,
                NumberOfTickets = request.NumberOfTickets,
                TotalPrice = request.NumberOfTickets * 299m,
                BookingDate = DateTime.UtcNow,
                Status = BookingStatus.Active
            };

            database.Bookings.Add(booking);
            Console.WriteLine($"Booking created successfully with ID: {booking.Id}");

            return Results.Ok(booking);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating booking: {ex.Message}");
            return Results.Problem($"Error creating booking: {ex.Message}");
        }
    }

    private static IResult GetUserBookings(int userId, IDatabase database)
    {
        var bookings = database.Bookings
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.BookingDate)
            .ToList();

        return Results.Ok(bookings);
    }

    private static IResult GetAllBookings(IDatabase database)
    {
        var bookings = database.Bookings
            .OrderByDescending(b => b.BookingDate)
            .ToList();

        return Results.Ok(bookings);
    }

    private static IResult CancelBooking(int id, IDatabase database)
    {
        Console.WriteLine($"=== API CancelBooking DEBUG ===");
        Console.WriteLine($"Attempting to cancel booking ID: {id}");
        
        var booking = database.Bookings.FirstOrDefault(b => b.Id == id);
        if (booking == null)
        {
            Console.WriteLine($"Booking {id} not found");
            return Results.NotFound("Booking not found");
        }

        Console.WriteLine($"Found booking: {booking.EventName} for user {booking.UserName}");
        
        database.Bookings.Remove(booking);
        
        Console.WriteLine($"Booking {id} successfully removed from database");
        Console.WriteLine($"Remaining bookings in database: {database.Bookings.Count}");
        
        return Results.Ok(new { message = "Booking cancelled successfully", bookingId = id });
    }

    private static IResult GetEventBookings(int eventId, IDatabase database)
    {
        var bookings = database.Bookings
            .Where(b => b.EventId == eventId)
            .OrderByDescending(b => b.BookingDate)
            .ToList();

        return Results.Ok(bookings);
    }
}

public record BookingRequest(int EventId, int UserId, string UserName, int NumberOfTickets);