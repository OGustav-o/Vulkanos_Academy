using System.Net.Http.Json;
using VulkanosAcademy.Domain.Entities;

namespace VulkanosAcademy.Web.Services;

public interface IMaterialService
{
    Task<IEnumerable<LessonMaterial>?> GetMaterialsByLessonAsync(Guid lessonId);
    Task<LessonMaterial?> GetMaterialAsync(Guid id);
    Task<LessonMaterial?> CreateMaterialAsync(LessonMaterial createMaterialDto);
    Task<bool> DeleteMaterialAsync(Guid id);
}

public class MaterialService : IMaterialService
{
    private readonly HttpClient _httpClient;

    public MaterialService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<LessonMaterial>?> GetMaterialsByLessonAsync(Guid lessonId)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<LessonMaterial>>($"api/lessonmaterials/lesson/{lessonId}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao buscar materiais: {ex.Message}");
            return null;
        }
    }

    public async Task<LessonMaterial?> GetMaterialAsync(Guid id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<LessonMaterial>($"api/lessonmaterials/{id}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao buscar material: {ex.Message}");
            return null;
        }
    }

    public async Task<LessonMaterial?> CreateMaterialAsync(LessonMaterial createMaterialDto)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/lessonmaterials", createMaterialDto);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<LessonMaterial>();
            }
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao criar material: {ex.Message}");
            return null;
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
