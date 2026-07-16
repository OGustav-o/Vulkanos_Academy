namespace VulkanosAcademy.Domain.Entities;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.Student;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public string? ExternalId { get; set; }
    public string? ExternalProvider { get; set; }

    // Navigation properties
    public ICollection<Course> CreatedCourses { get; set; } = new List<Course>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}

public enum UserRole
{
    Student = 0,
    Producer = 1,
    Admin = 2
}
