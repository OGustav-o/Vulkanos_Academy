using System.Net.Http.Json;
using VulkanosAcademy.Domain.DTOs;

namespace VulkanosAcademy.Web.Services;

public interface IEnrollmentService
{
    Task<IEnumerable<EnrollmentDto>?> GetUserEnrollmentsAsync(Guid userId);
    Task<EnrollmentDto?> GetEnrollmentAsync(Guid id);
    Task<EnrollmentDto?> CreateEnrollmentAsync(CreateEnrollmentDto createEnrollmentDto);
    Task<bool> UpdateProgressAsync(Guid id, UpdateProgressDto updateProgressDto);
    Task<bool> DeleteEnrollmentAsync(Guid id);
    Task<CourseStatsDto?> GetCourseStatsAsync(Guid courseId);
}

public class EnrollmentService : IEnrollmentService
{
    private readonly HttpClient _httpClient;

    public EnrollmentService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<EnrollmentDto>?> GetUserEnrollmentsAsync(Guid userId)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<EnrollmentDto>>($"api/enrollments/user/{userId}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao buscar matrículas: {ex.Message}");
            return null;
        }
    }

    public async Task<EnrollmentDto?> GetEnrollmentAsync(Guid id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<EnrollmentDto>($"api/enrollments/{id}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao buscar matrícula: {ex.Message}");
            return null;
        }
    }

    public async Task<EnrollmentDto?> CreateEnrollmentAsync(CreateEnrollmentDto createEnrollmentDto)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/enrollments", createEnrollmentDto);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<EnrollmentDto>();
            }
            Console.WriteLine($"Erro ao criar matrícula: {response.StatusCode}");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao criar matrícula: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> UpdateProgressAsync(Guid id, UpdateProgressDto updateProgressDto)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"api/enrollments/{id}/progress", updateProgressDto);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao atualizar progresso: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> DeleteEnrollmentAsync(Guid id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"api/enrollments/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao deletar matrícula: {ex.Message}");
            return false;
        }
    }

    public async Task<CourseStatsDto?> GetCourseStatsAsync(Guid courseId)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<CourseStatsDto>($"api/enrollments/course/{courseId}/stats");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao buscar estatísticas: {ex.Message}");
            return null;
        }
    }
}
