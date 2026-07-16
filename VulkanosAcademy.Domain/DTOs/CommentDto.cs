namespace VulkanosAcademy.Domain.DTOs;

public class CommentDto
{
    public Guid Id { get; set; }
    public Guid LessonId { get; set; }
    public Guid UserId { get; set; }
    public Guid? ParentCommentId { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsModerated { get; set; }
    public UserDto? User { get; set; }
    public List<CommentDto> Replies { get; set; } = new();
}

public class CreateCommentDto
{
    public Guid LessonId { get; set; }
    public Guid? ParentCommentId { get; set; }
    public string Content { get; set; } = string.Empty;
}

public class UpdateCommentDto
{
    public string? Content { get; set; }
}
