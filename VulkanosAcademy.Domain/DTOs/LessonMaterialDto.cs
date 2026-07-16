namespace VulkanosAcademy.Domain.DTOs;

public class LessonMaterialDto
{
    public Guid Id { get; set; }
    public Guid LessonId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string MaterialUrl { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class CreateLessonMaterialDto
{
    public Guid LessonId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string MaterialUrl { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
}

public class UpdateLessonMaterialDto
{
    public string? Title { get; set; }
    public string? MaterialUrl { get; set; }
    public string? Type { get; set; }
}
