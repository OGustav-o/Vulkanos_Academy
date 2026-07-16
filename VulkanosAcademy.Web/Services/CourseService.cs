using System.Net.Http.Json;
using VulkanosAcademy.Domain.DTOs;

namespace VulkanosAcademy.Web.Services;

public interface ICourseService
{
    Task<IEnumerable<CourseDto>?> GetCoursesAsync();
    Task<IEnumerable<CourseDto>?> GetCoursesByInstructorAsync(Guid instructorId);
    Task<CourseDto?> GetCourseAsync(Guid id);
    Task<CourseDto?> CreateCourseAsync(CreateCourseDto createCourseDto);
    Task<bool> UpdateCourseAsync(Guid id, UpdateCourseDto updateCourseDto);
    Task<bool> DeleteCourseAsync(Guid id);
}

public class CourseService : ICourseService
{
    private readonly HttpClient _httpClient;

    public CourseService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<CourseDto>?> GetCoursesAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<CourseDto>>("api/courses");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao buscar cursos: {ex.Message}");
            return null;
        }
    }

    public async Task<IEnumerable<CourseDto>?> GetCoursesByInstructorAsync(Guid instructorId)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<CourseDto>>($"api/courses/instructor/{instructorId}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao buscar cursos do instrutor: {ex.Message}");
            return null;
        }
    }

    public async Task<CourseDto?> GetCourseAsync(Guid id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<CourseDto>($"api/courses/{id}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao buscar curso: {ex.Message}");
            return null;
        }
    }

    public async Task<CourseDto?> CreateCourseAsync(CreateCourseDto createCourseDto)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/courses", createCourseDto);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<CourseDto>();
            }
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao criar curso: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> UpdateCourseAsync(Guid id, UpdateCourseDto updateCourseDto)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"api/courses/{id}", updateCourseDto);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao atualizar curso: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> DeleteCourseAsync(Guid id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"api/courses/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao deletar curso: {ex.Message}");
            return false;
        }
    }
}
