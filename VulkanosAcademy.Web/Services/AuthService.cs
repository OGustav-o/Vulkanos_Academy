using System.Net.Http.Json;
using VulkanosAcademy.Domain.DTOs;

namespace VulkanosAcademy.Web.Services;

public interface IAuthService
{
    Task<LoginResponseDto?> LoginAsync(LoginDto loginDto);
    Task<UserDto?> RegisterAsync(CreateUserDto createUserDto);
    Task LogoutAsync();
    Task<string?> GetTokenAsync();
    Task SetTokenAsync(string token);
}

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private string? _token;

    public AuthService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<LoginResponseDto?> LoginAsync(LoginDto loginDto)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", loginDto);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsAsync<LoginResponseDto>();
                _token = result?.Token;
                return result;
            }
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao fazer login: {ex.Message}");
            return null;
        }
    }

    public async Task<UserDto?> RegisterAsync(CreateUserDto createUserDto)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/register", createUserDto);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<UserDto>();
            }
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao registrar: {ex.Message}");
            return null;
        }
    }

    public Task LogoutAsync()
    {
        _token = null;
        return Task.CompletedTask;
    }

    public Task<string?> GetTokenAsync()
    {
        return Task.FromResult(_token);
    }

    public Task SetTokenAsync(string token)
    {
        _token = token;
        return Task.CompletedTask;
    }
}
