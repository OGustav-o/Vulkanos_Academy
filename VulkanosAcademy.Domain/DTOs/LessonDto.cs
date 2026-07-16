namespace VulkanosAcademy.Domain.DTOs;

public class LessonDto
{
    public Guid Id { get; set; }
    public Guid ModuleId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string VideoUrl { get; set; } = string.Empty;
    public int Order { get; set; }
}

public class CreateLessonDto
{
    public Guid ModuleId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string VideoUrl { get; set; } = string.Empty;
    public int Order { get; set; }
}

public class ModuleDto
{
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Order { get; set; }
}

public class CreateModuleDto
{
    public Guid CourseId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Order { get; set; }
}
