namespace VulkanosAcademy.Domain.Entities;

public class Certificate
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid EnrollmentId { get; set; }
    public DateTime IssueDate { get; set; } = DateTime.UtcNow;
    public string CertificateUrl { get; set; } = string.Empty;

    // Navigation properties
    public Enrollment? Enrollment { get; set; }
}
