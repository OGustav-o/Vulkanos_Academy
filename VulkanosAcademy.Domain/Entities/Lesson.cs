namespace VulkanosAcademy.Domain.Entities;

public class Lesson
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ModuleId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string VideoUrl { get; set; } = string.Empty;
    public int Order { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public Module? Module { get; set; }
    public ICollection<LessonMaterial> Materials { get; set; } = new List<LessonMaterial>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}
