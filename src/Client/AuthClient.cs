using System.Net.Http.Json;
using RemindMeApp.Shared;

namespace RemindMeApp.Client;

public class AuthClient
{
    private readonly HttpClient _client;
    public AuthClient(HttpClient client)
    {
        _client = client;
    }

    public async Task<bool> LoginAsync(string? username, string? password)
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            return false;
        }

        var response = await _client.PostAsJsonAsync("authentication/login",
            new UserInfo
            {
                Username = username,
                Password = password
            });
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> CreateUserAsync(string? username, string? password)
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            return false;
        }

        var response = await _client.PostAsJsonAsync("authentication/register",
            new UserInfo
            {
                Username = username,
                Password = password
            });

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> LogoutAsync()
    {
        var response = await _client.PostAsync("authentication/logout", content: null);
        return response.IsSuccessStatusCode;
    }
}
