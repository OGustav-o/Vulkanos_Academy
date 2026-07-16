namespace VulkanosAcademy.Domain.Entities;

public class Course
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid InstructorId { get; set; }
    public string? ThumbnailUrl { get; set; }
    public decimal? Price { get; set; }
    public CourseStatus Status { get; set; } = CourseStatus.Draft;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public User? Instructor { get; set; }
    public ICollection<Module> Modules { get; set; } = new List<Module>();
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}

public enum CourseStatus
{
    Draft = 0,
    Published = 1,
    Archived = 2
}
