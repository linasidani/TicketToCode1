using TicketToCode.Core.Models;
using System.Text.Json;

namespace TicketToCode.Client.Services;

public interface IEventService
{
    Task<List<Event>> GetAllEventsAsync();
    Task<Event?> GetEventAsync(int id);
    Task<bool> CreateBookingAsync(int eventId, int userId, string userName, int numberOfTickets);
    Task<List<Booking>> GetUserBookingsAsync(int userId);
    Task<bool> CancelBookingAsync(int bookingId);
    Task<List<Booking>> GetAllBookingsAsync();
    Task<(bool Success, string Username, string Role, int UserId)> LoginAsync(string email, string password);
    Task<bool> CreateEventAsync(Event eventItem);
}

public class EventService : IEventService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public EventService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("http://localhost:5235/");
        
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<List<Event>> GetAllEventsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("events");
            response.EnsureSuccessStatusCode();
            
            var json = await response.Content.ReadAsStringAsync();
            var events = JsonSerializer.Deserialize<List<Event>>(json, _jsonOptions);
            
            return events ?? new List<Event>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching events: {ex.Message}");
            return new List<Event>();
        }
    }

    public async Task<Event?> GetEventAsync(int id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"events/{id}");
            response.EnsureSuccessStatusCode();
            
            var json = await response.Content.ReadAsStringAsync();
            var eventItem = JsonSerializer.Deserialize<Event>(json, _jsonOptions);
            
            return eventItem;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching event {id}: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> CreateBookingAsync(int eventId, int userId, string userName, int numberOfTickets)
    {
        try
        {
            Console.WriteLine("=== CreateBookingAsync DEBUG START ===");
            Console.WriteLine($"API Base URL: {_httpClient.BaseAddress}");
            Console.WriteLine($"EventId: {eventId}");
            Console.WriteLine($"UserId: {userId}");
            Console.WriteLine($"UserName: '{userName}'");
            Console.WriteLine($"NumberOfTickets: {numberOfTickets}");

            var bookingRequest = new
            {
                EventId = eventId,
                UserId = userId,
                UserName = userName,
                NumberOfTickets = numberOfTickets
            };

            var json = JsonSerializer.Serialize(bookingRequest);
            Console.WriteLine($"JSON payload: {json}");

            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            Console.WriteLine("Sending POST request to 'bookings' endpoint...");
            var response = await _httpClient.PostAsync("bookings", content);
            
            Console.WriteLine($"Response status: {response.StatusCode} ({(int)response.StatusCode})");
            Console.WriteLine($"Response success: {response.IsSuccessStatusCode}");

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error response content: {errorContent}");
            }
            else
            {
                var successContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Success response content: {successContent}");
            }

            Console.WriteLine("=== CreateBookingAsync DEBUG END ===");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"CreateBookingAsync exception: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            return false;
        }
    }

    public async Task<List<Booking>> GetUserBookingsAsync(int userId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"bookings/user/{userId}");
            response.EnsureSuccessStatusCode();
            
            var json = await response.Content.ReadAsStringAsync();
            var bookings = JsonSerializer.Deserialize<List<Booking>>(json, _jsonOptions);
            
            return bookings ?? new List<Booking>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching user bookings: {ex.Message}");
            return new List<Booking>();
        }
    }

    public async Task<bool> CancelBookingAsync(int bookingId)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"bookings/{bookingId}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error cancelling booking: {ex.Message}");
            return false;
        }
    }

    public async Task<List<Booking>> GetAllBookingsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("bookings");
            response.EnsureSuccessStatusCode();
            
            var json = await response.Content.ReadAsStringAsync();
            var bookings = JsonSerializer.Deserialize<List<Booking>>(json, _jsonOptions);
            
            return bookings ?? new List<Booking>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching all bookings: {ex.Message}");
            return new List<Booking>();
        }
    }

    public async Task<(bool Success, string Username, string Role, int UserId)> LoginAsync(string email, string password)
    {
        try
        {
            var loginRequest = new
            {
                Username = email,
                Password = password
            };

            var json = JsonSerializer.Serialize(loginRequest);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("auth/login", content);
            
            if (response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                var loginResponse = JsonSerializer.Deserialize<LoginResponse>(responseJson, _jsonOptions);
                
                string actualRole = loginResponse.Username == "admin" ? "Admin" : "User";
                int userId = loginResponse.Username == "admin" ? 1 : 2;
                
                return (true, loginResponse.Username, actualRole, userId);
            }
            
            return (false, "", "", 0);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during login: {ex.Message}");
            return (false, "", "", 0);
        }
    }

    public async Task<bool> CreateEventAsync(Event eventItem)
    {
        try
        {
            var json = JsonSerializer.Serialize(eventItem, _jsonOptions);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("events", content);
            
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating event: {ex.Message}");
            return false;
        }
    }
}

public class LoginResponse
{
    public string Username { get; set; } = "";
    public string Role { get; set; } = "";
}