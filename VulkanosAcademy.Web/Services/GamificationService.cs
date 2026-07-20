using System.Net.Http.Json;
using VulkanosAcademy.Domain.DTOs;

namespace VulkanosAcademy.Web.Services;

public interface IGamificationService
{
    Task<UserProfileDto?> GetUserProfileAsync(Guid userId);
    Task<IEnumerable<LeaderboardEntryDto>?> GetLeaderboardAsync();
    Task<bool> ProcessGamificationEventAsync(Guid userId, string eventType, Guid? referenceId = null);
}

public class GamificationService : IGamificationService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<GamificationService> _logger;

    public GamificationService(HttpClient httpClient, ILogger<GamificationService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<UserProfileDto?> GetUserProfileAsync(Guid userId)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<UserProfileDto>($"api/gamification/profile/{userId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao obter perfil de gamificação: {ex.Message}");
            return null;
        }
    }

    public async Task<IEnumerable<LeaderboardEntryDto>?> GetLeaderboardAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<LeaderboardEntryDto>>("api/gamification/leaderboard");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao obter leaderboard: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> ProcessGamificationEventAsync(Guid userId, string eventType, Guid? referenceId = null)
    {
        try
        {
            var request = new EarnXPRequestDto
            {
                UserId = userId,
                EventType = eventType,
                ReferenceId = referenceId
            };
            var response = await _httpClient.PostAsJsonAsync("api/gamification/event", request);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao processar evento de gamificação: {ex.Message}");
            return false;
        }
    }
}
