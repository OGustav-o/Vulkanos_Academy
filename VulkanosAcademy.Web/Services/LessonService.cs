using System.Net.Http.Json;
using VulkanosAcademy.Domain.DTOs;

namespace VulkanosAcademy.Web.Services;

public interface ILessonService
{
    Task<IEnumerable<LessonDto>?> GetLessonsByModuleAsync(Guid moduleId);
    Task<LessonDto?> GetLessonAsync(Guid id);
    Task<LessonDto?> CreateLessonAsync(CreateLessonDto createLessonDto);
    Task<bool> UpdateLessonAsync(Guid id, CreateLessonDto updateLessonDto);
    Task<bool> DeleteLessonAsync(Guid id);
}

public class LessonService : ILessonService
{
    private readonly HttpClient _httpClient;

    public LessonService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<LessonDto>?> GetLessonsByModuleAsync(Guid moduleId)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<LessonDto>>($"api/lessons/module/{moduleId}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao buscar aulas: {ex.Message}");
            return null;
        }
    }

    public async Task<LessonDto?> GetLessonAsync(Guid id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<LessonDto>($"api/lessons/{id}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao buscar aula: {ex.Message}");
            return null;
        }
    }

    public async Task<LessonDto?> CreateLessonAsync(CreateLessonDto createLessonDto)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/lessons", createLessonDto);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<LessonDto>();
            }
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao criar aula: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> UpdateLessonAsync(Guid id, CreateLessonDto updateLessonDto)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"api/lessons/{id}", updateLessonDto);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao atualizar aula: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> DeleteLessonAsync(Guid id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"api/lessons/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao deletar aula: {ex.Message}");
            return false;
        }
    }
}
