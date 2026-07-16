namespace VulkanosAcademy.Domain.Entities;

public class Module
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CourseId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Order { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public Course? Course { get; set; }
    public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
}
