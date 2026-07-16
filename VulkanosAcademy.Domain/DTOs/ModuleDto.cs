namespace VulkanosAcademy.Domain.DTOs;

public class ModuleDto
{
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Order { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
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
