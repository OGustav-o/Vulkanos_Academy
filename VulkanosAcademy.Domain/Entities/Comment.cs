namespace VulkanosAcademy.Domain.Entities;

public class Comment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid LessonId { get; set; }
    public Guid UserId { get; set; }
    public Guid? ParentCommentId { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public bool IsModerated { get; set; } = false;

    // Navigation properties
    public Lesson? Lesson { get; set; }
    public User? User { get; set; }
    public Comment? ParentComment { get; set; }
    public ICollection<Comment> Replies { get; set; } = new List<Comment>();
}
