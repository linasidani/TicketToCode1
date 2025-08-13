namespace TicketToCode.Core.Models;

public class Booking
{
    public int Id { get; set; }
    public int EventId { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string EventName { get; set; } = string.Empty;
    public int NumberOfTickets { get; set; }
    public DateTime BookingDate { get; set; } = DateTime.UtcNow;
    public decimal TotalPrice { get; set; }
    public BookingStatus Status { get; set; } = BookingStatus.Active;
}

public enum BookingStatus
{
    Active = 0,
    Cancelled = 1
}