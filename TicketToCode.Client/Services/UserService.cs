namespace TicketToCode.Client.Services;

public interface IUserService
{
    bool IsLoggedIn { get; }
    string? CurrentUsername { get; }
    string? CurrentRole { get; }
    int? CurrentUserId { get; }
    bool IsAdmin { get; }
    
    void SetUser(string username, string role, int userId);
    void Logout();
}

public class UserService : IUserService
{
    private string? _currentUsername;
    private string? _currentRole;
    private int? _currentUserId;

    public bool IsLoggedIn => !string.IsNullOrEmpty(_currentUsername);
    public string? CurrentUsername => _currentUsername;
    public string? CurrentRole => _currentRole;
    public int? CurrentUserId => _currentUserId;
    public bool IsAdmin => _currentRole == "Admin";

    public void SetUser(string username, string role, int userId)
    {
        _currentUsername = username;
        _currentRole = role;
        _currentUserId = userId;
        
        Console.WriteLine("=== UserService.SetUser DEBUG ===");
        Console.WriteLine($"Username: '{username}'");
        Console.WriteLine($"Role: '{role}'");
        Console.WriteLine($"UserId: {userId}");
        Console.WriteLine($"IsLoggedIn: {IsLoggedIn}");
        Console.WriteLine($"IsAdmin: {IsAdmin}");
    }

    public void Logout()
    {
        Console.WriteLine("=== UserService.Logout DEBUG ===");
        _currentUsername = null;
        _currentRole = null;
        _currentUserId = null;
    }
}