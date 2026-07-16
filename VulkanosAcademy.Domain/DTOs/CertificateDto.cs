namespace VulkanosAcademy.Domain.DTOs;

public class CertificateDto
{
    public Guid Id { get; set; }
    public Guid EnrollmentId { get; set; }
    public DateTime IssueDate { get; set; }
    public string CertificateUrl { get; set; } = string.Empty;
    public EnrollmentDto? Enrollment { get; set; }
}

public class GenerateCertificateDto
{
    public Guid EnrollmentId { get; set; }
}

public class CertificateResponseDto
{
    public Guid Id { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
    public DateTime CompletionDate { get; set; }
    public string CertificateUrl { get; set; } = string.Empty;
}

public class GlobalMetricsDto
{
    public int TotalUsers { get; set; }
    public int TotalCourses { get; set; }
    public int TotalEnrollments { get; set; }
    public int CompletedEnrollments { get; set; }
    public double AverageCompletionRate { get; set; }
    public int ActiveStudents { get; set; }
    public decimal TotalRevenue { get; set; }
}
