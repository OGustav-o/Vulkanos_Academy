namespace VulkanosAcademy.Domain.DTOs;

public class CourseDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid InstructorId { get; set; }
    public string? ThumbnailUrl { get; set; }
    public decimal? Price { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class CreateCourseDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public decimal? Price { get; set; }
}

public class UpdateCourseDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? ThumbnailUrl { get; set; }
    public decimal? Price { get; set; }
    public string? Status { get; set; }
}
