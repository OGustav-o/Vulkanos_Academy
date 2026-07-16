namespace VulkanosAcademy.Domain.Entities;

public class Enrollment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public Guid CourseId { get; set; }
    public DateTime EnrollmentDate { get; set; } = DateTime.UtcNow;
    public DateTime? CompletionDate { get; set; }
    public decimal Progress { get; set; } = 0;
    public Guid? CertificateId { get; set; }

    // Navigation properties
    public User? User { get; set; }
    public Course? Course { get; set; }
    public Certificate? Certificate { get; set; }
}
