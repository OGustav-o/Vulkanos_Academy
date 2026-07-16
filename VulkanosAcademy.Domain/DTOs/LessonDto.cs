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

public class UpdateLessonDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? VideoUrl { get; set; }
    public int? Order { get; set; }
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

public class UpdateModuleDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public int? Order { get; set; }
}

public class LessonMaterialDto
{
    public Guid Id { get; set; }
    public Guid LessonId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string MaterialUrl { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
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
