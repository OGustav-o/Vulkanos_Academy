using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VulkanosAcademy.Data;
using VulkanosAcademy.Domain.DTOs;
using VulkanosAcademy.Domain.Entities;

namespace VulkanosAcademy.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommentsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CommentsController(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Obtém todos os comentários de uma aula específica
    /// </summary>
    [HttpGet("lesson/{lessonId}")]
    public async Task<ActionResult<IEnumerable<CommentDto>>> GetCommentsByLesson(Guid lessonId)
    {
        var comments = await _context.Comments
            .Where(c => c.LessonId == lessonId)
            .Include(c => c.User)
            .Include(c => c.Replies)
            .ThenInclude(r => r.User)
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => new CommentDto
            {
                Id = c.Id,
                LessonId = c.LessonId,
                UserId = c.UserId,
                ParentCommentId = c.ParentCommentId,
                Content = c.Content,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt,
                IsModerated = c.IsModerated,
                User = new UserDto
                {
                    Id = c.User!.Id,
                    FirstName = c.User.FirstName,
                    LastName = c.User.LastName,
                    Email = c.User.Email,
                    Role = c.User.Role.ToString()
                },
                Replies = c.Replies.Select(r => new CommentDto
                {
                    Id = r.Id,
                    LessonId = r.LessonId,
                    UserId = r.UserId,
                    ParentCommentId = r.ParentCommentId,
                    Content = r.Content,
                    CreatedAt = r.CreatedAt,
                    UpdatedAt = r.UpdatedAt,
                    IsModerated = r.IsModerated,
                    User = new UserDto
                    {
                        Id = r.User!.Id,
                        FirstName = r.User.FirstName,
                        LastName = r.User.LastName,
                        Email = r.User.Email,
                        Role = r.User.Role.ToString()
                    }
                }).ToList()
            })
            .ToListAsync();

        return Ok(comments);
    }

    /// <summary>
    /// Obtém um comentário específico
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<CommentDto>> GetComment(Guid id)
    {
        var comment = await _context.Comments
            .Include(c => c.User)
            .Include(c => c.Replies)
            .ThenInclude(r => r.User)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (comment == null)
            return NotFound();

        var commentDto = new CommentDto
        {
            Id = comment.Id,
            LessonId = comment.LessonId,
            UserId = comment.UserId,
            ParentCommentId = comment.ParentCommentId,
            Content = comment.Content,
            CreatedAt = comment.CreatedAt,
            UpdatedAt = comment.UpdatedAt,
            IsModerated = comment.IsModerated,
            User = new UserDto
            {
                Id = comment.User!.Id,
                FirstName = comment.User.FirstName,
                LastName = comment.User.LastName,
                Email = comment.User.Email,
                Role = comment.User.Role.ToString()
            },
            Replies = comment.Replies.Select(r => new CommentDto
            {
                Id = r.Id,
                LessonId = r.LessonId,
                UserId = r.UserId,
                ParentCommentId = r.ParentCommentId,
                Content = r.Content,
                CreatedAt = r.CreatedAt,
                UpdatedAt = r.UpdatedAt,
                IsModerated = r.IsModerated,
                User = new UserDto
                {
                    Id = r.User!.Id,
                    FirstName = r.User.FirstName,
                    LastName = r.User.LastName,
                    Email = r.User.Email,
                    Role = r.User.Role.ToString()
                }
            }).ToList()
        };

        return Ok(commentDto);
    }

    /// <summary>
    /// Cria um novo comentário ou resposta
    /// </summary>
    [Authorize]
    [HttpPost]
    public async Task<ActionResult<CommentDto>> CreateComment([FromBody] CreateCommentDto createCommentDto)
    {
        // Validar se a aula existe
        var lesson = await _context.Lessons.FindAsync(createCommentDto.LessonId);
        if (lesson == null)
            return NotFound("Aula não encontrada");

        // Se for uma resposta, validar se o comentário pai existe
        if (createCommentDto.ParentCommentId.HasValue)
        {
            var parentComment = await _context.Comments.FindAsync(createCommentDto.ParentCommentId);
            if (parentComment == null)
                return NotFound("Comentário pai não encontrado");
        }

        // Obter ID do usuário autenticado
        var userIdClaim = User.FindFirst("sub")?.Value ?? User.FindFirst("nameid")?.Value;
        if (!Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized("Usuário não identificado");

        var comment = new Comment
        {
            LessonId = createCommentDto.LessonId,
            UserId = userId,
            ParentCommentId = createCommentDto.ParentCommentId,
            Content = createCommentDto.Content,
            IsModerated = false
        };

        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();

        var user = await _context.Users.FindAsync(userId);
        var commentDto = new CommentDto
        {
            Id = comment.Id,
            LessonId = comment.LessonId,
            UserId = comment.UserId,
            ParentCommentId = comment.ParentCommentId,
            Content = comment.Content,
            CreatedAt = comment.CreatedAt,
            UpdatedAt = comment.UpdatedAt,
            IsModerated = comment.IsModerated,
            User = new UserDto
            {
                Id = user!.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = user.Role.ToString()
            }
        };

        return CreatedAtAction(nameof(GetComment), new { id = comment.Id }, commentDto);
    }

    /// <summary>
    /// Atualiza um comentário existente
    /// </summary>
    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateComment(Guid id, [FromBody] UpdateCommentDto updateCommentDto)
    {
        var comment = await _context.Comments.FindAsync(id);
        if (comment == null)
            return NotFound();

        // Validar se o usuário é o autor ou um administrador
        var userIdClaim = User.FindFirst("sub")?.Value ?? User.FindFirst("nameid")?.Value;
        if (!Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        var user = await _context.Users.FindAsync(userId);
        if (comment.UserId != userId && user?.Role != UserRole.Admin)
            return Forbid();

        if (!string.IsNullOrEmpty(updateCommentDto.Content))
            comment.Content = updateCommentDto.Content;

        comment.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Deleta um comentário
    /// </summary>
    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteComment(Guid id)
    {
        var comment = await _context.Comments.FindAsync(id);
        if (comment == null)
            return NotFound();

        // Validar se o usuário é o autor ou um administrador
        var userIdClaim = User.FindFirst("sub")?.Value ?? User.FindFirst("nameid")?.Value;
        if (!Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        var user = await _context.Users.FindAsync(userId);
        if (comment.UserId != userId && user?.Role != UserRole.Admin)
            return Forbid();

        // Deletar todas as respostas também
        var replies = await _context.Comments.Where(c => c.ParentCommentId == id).ToListAsync();
        _context.Comments.RemoveRange(replies);
        _context.Comments.Remove(comment);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Marca um comentário como moderado (apenas para admins)
    /// </summary>
    [Authorize]
    [HttpPut("{id}/moderate")]
    public async Task<IActionResult> ModerateComment(Guid id)
    {
        var comment = await _context.Comments.FindAsync(id);
        if (comment == null)
            return NotFound();

        // Validar se o usuário é um administrador
        var userIdClaim = User.FindFirst("sub")?.Value ?? User.FindFirst("nameid")?.Value;
        if (!Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        var user = await _context.Users.FindAsync(userId);
        if (user?.Role != UserRole.Admin)
            return Forbid();

        comment.IsModerated = true;
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
