using System.Net.Http.Json;
using VulkanosAcademy.Domain.DTOs;

namespace VulkanosAcademy.Web.Services;

public interface IMetricsService
{
    Task<GlobalMetricsDto?> GetGlobalMetricsAsync();
    Task<GlobalMetricsDto?> GetProducerMetricsAsync(Guid producerId);
    Task<IEnumerable<CourseStatsDto>?> GetProducerCoursesStatsAsync(Guid producerId);
}

public class MetricsService : IMetricsService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<MetricsService> _logger;

    public MetricsService(HttpClient httpClient, ILogger<MetricsService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<GlobalMetricsDto?> GetGlobalMetricsAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<GlobalMetricsDto>("api/metrics/global");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao obter métricas globais: {ex.Message}");
            return null;
        }
    }

    public async Task<GlobalMetricsDto?> GetProducerMetricsAsync(Guid producerId)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<GlobalMetricsDto>($"api/metrics/producer/{producerId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao obter métricas do produtor: {ex.Message}");
            return null;
        }
    }

    public async Task<IEnumerable<CourseStatsDto>?> GetProducerCoursesStatsAsync(Guid producerId)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<CourseStatsDto>>($"api/metrics/producer/{producerId}/courses");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao obter estatísticas dos cursos: {ex.Message}");
            return null;
        }
    }
}
