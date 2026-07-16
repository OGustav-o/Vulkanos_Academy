namespace VulkanosAcademy.Domain.DTOs;

public class EnrollmentDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid CourseId { get; set; }
    public DateTime EnrollmentDate { get; set; }
    public DateTime? CompletionDate { get; set; }
    public decimal Progress { get; set; }
    public Guid? CertificateId { get; set; }
    public CourseDto? Course { get; set; }
}

public class CreateEnrollmentDto
{
    public Guid CourseId { get; set; }
}

public class UpdateProgressDto
{
    public decimal Progress { get; set; }
}

public class CourseStatsDto
{
    public Guid CourseId { get; set; }
    public int TotalEnrollments { get; set; }
    public int CompletedEnrollments { get; set; }
    public double AverageProgress { get; set; }
    public int ActiveStudents { get; set; }
    public decimal CompletionRate { get; set; }
}
