using System.Net.Http.Json;
using VulkanosAcademy.Domain.DTOs;

namespace VulkanosAcademy.Web.Services;

public interface ICommentService
{
    Task<IEnumerable<CommentDto>?> GetCommentsByLessonAsync(Guid lessonId);
    Task<CommentDto?> GetCommentAsync(Guid id);
    Task<CommentDto?> CreateCommentAsync(CreateCommentDto createCommentDto);
    Task<bool> UpdateCommentAsync(Guid id, UpdateCommentDto updateCommentDto);
    Task<bool> DeleteCommentAsync(Guid id);
    Task<bool> ModerateCommentAsync(Guid id);
}

public class CommentService : ICommentService
{
    private readonly HttpClient _httpClient;

    public CommentService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<CommentDto>?> GetCommentsByLessonAsync(Guid lessonId)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<CommentDto>>($"api/comments/lesson/{lessonId}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao buscar comentários: {ex.Message}");
            return null;
        }
    }

    public async Task<CommentDto?> GetCommentAsync(Guid id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<CommentDto>($"api/comments/{id}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao buscar comentário: {ex.Message}");
            return null;
        }
    }

    public async Task<CommentDto?> CreateCommentAsync(CreateCommentDto createCommentDto)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/comments", createCommentDto);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<CommentDto>();
            }
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao criar comentário: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> UpdateCommentAsync(Guid id, UpdateCommentDto updateCommentDto)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"api/comments/{id}", updateCommentDto);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao atualizar comentário: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> DeleteCommentAsync(Guid id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"api/comments/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao deletar comentário: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> ModerateCommentAsync(Guid id)
    {
        try
        {
            var response = await _httpClient.PutAsync($"api/comments/{id}/moderate", null);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao moderar comentário: {ex.Message}");
            return false;
        }
    }
}
