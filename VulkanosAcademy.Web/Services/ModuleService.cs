using System.Net.Http.Json;
using VulkanosAcademy.Domain.DTOs;

namespace VulkanosAcademy.Web.Services;

public interface IModuleService
{
    Task<IEnumerable<ModuleDto>?> GetModulesByCourseAsync(Guid courseId);
    Task<ModuleDto?> GetModuleAsync(Guid id);
    Task<ModuleDto?> CreateModuleAsync(CreateModuleDto createModuleDto);
    Task<bool> UpdateModuleAsync(Guid id, UpdateModuleDto updateModuleDto);
    Task<bool> DeleteModuleAsync(Guid id);
}

public class ModuleService : IModuleService
{
    private readonly HttpClient _httpClient;

    public ModuleService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<ModuleDto>?> GetModulesByCourseAsync(Guid courseId)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<ModuleDto>>($"api/modules/course/{courseId}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao buscar módulos: {ex.Message}");
            return null;
        }
    }

    public async Task<ModuleDto?> GetModuleAsync(Guid id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<ModuleDto>($"api/modules/{id}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao buscar módulo: {ex.Message}");
            return null;
        }
    }

    public async Task<ModuleDto?> CreateModuleAsync(CreateModuleDto createModuleDto)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/modules", createModuleDto);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<ModuleDto>();
            }
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao criar módulo: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> UpdateModuleAsync(Guid id, UpdateModuleDto updateModuleDto)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"api/modules/{id}", updateModuleDto);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao atualizar módulo: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> DeleteModuleAsync(Guid id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"api/modules/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao deletar módulo: {ex.Message}");
            return false;
        }
    }
}
