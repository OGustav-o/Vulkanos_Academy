namespace VulkanosAcademy.Domain.Entities;

public class LessonMaterial
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid LessonId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string MaterialUrl { get; set; } = string.Empty;
    public MaterialType Type { get; set; } = MaterialType.Link;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Lesson? Lesson { get; set; }
}

public enum MaterialType
{
    PDF = 0,
    Link = 1
}
