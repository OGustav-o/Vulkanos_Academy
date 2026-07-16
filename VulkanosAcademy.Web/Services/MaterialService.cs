using System.Net.Http.Json;
using VulkanosAcademy.Domain.DTOs;
using VulkanosAcademy.Domain.Entities;

namespace VulkanosAcademy.Web.Services;

public interface IMaterialService
{
    Task<IEnumerable<LessonMaterialDto>?> GetMaterialsByLessonAsync(Guid lessonId);
    Task<LessonMaterialDto?> GetMaterialAsync(Guid id);
    Task<LessonMaterialDto?> CreateMaterialAsync(CreateLessonMaterialDto createMaterialDto);
    Task<bool> UpdateMaterialAsync(Guid id, UpdateLessonMaterialDto updateMaterialDto);
    Task<bool> DeleteMaterialAsync(Guid id);
}

public class MaterialService : IMaterialService
{
    private readonly HttpClient _httpClient;

    public MaterialService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<LessonMaterialDto>?> GetMaterialsByLessonAsync(Guid lessonId)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<LessonMaterialDto>>($"api/lessonmaterials/lesson/{lessonId}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao buscar materiais: {ex.Message}");
            return null;
        }
    }

    public async Task<LessonMaterialDto?> GetMaterialAsync(Guid id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<LessonMaterialDto>($"api/lessonmaterials/{id}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao buscar material: {ex.Message}");
            return null;
        }
    }

    public async Task<LessonMaterialDto?> CreateMaterialAsync(CreateLessonMaterialDto createMaterialDto)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/lessonmaterials", createMaterialDto);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<LessonMaterialDto>();
            }
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao criar material: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> UpdateMaterialAsync(Guid id, UpdateLessonMaterialDto updateMaterialDto)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"api/lessonmaterials/{id}", updateMaterialDto);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao atualizar material: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> DeleteMaterialAsync(Guid id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"api/lessonmaterials/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao deletar material: {ex.Message}");
            return false;
        }
    }
}
