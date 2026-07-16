using System.Globalization;
using Microsoft.EntityFrameworkCore;
using VulkanosAcademy.Data;
using VulkanosAcademy.Domain.DTOs;
using VulkanosAcademy.Domain.Entities;

namespace VulkanosAcademy.Api.Services;

public interface ICertificateService
{
    Task<CertificateResponseDto?> GenerateCertificateAsync(Guid enrollmentId);
    Task<CertificateDto?> GetCertificateAsync(Guid certificateId);
    Task<IEnumerable<CertificateDto>> GetUserCertificatesAsync(Guid userId);
    Task<bool> CertificateExistsAsync(Guid enrollmentId);
}

public class CertificateService : ICertificateService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<CertificateService> _logger;
    private readonly string _certificatesPath;

    public CertificateService(ApplicationDbContext context, ILogger<CertificateService> logger)
    {
        _context = context;
        _logger = logger;
        _certificatesPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "certificates");
        
        // Criar diretório se não existir
        if (!Directory.Exists(_certificatesPath))
        {
            Directory.CreateDirectory(_certificatesPath);
        }
    }

    public async Task<CertificateResponseDto?> GenerateCertificateAsync(Guid enrollmentId)
    {
        try
        {
            // Obter informações da matrícula
            var enrollment = await _context.Enrollments
                .Include(e => e.User)
                .Include(e => e.Course)
                .FirstOrDefaultAsync(e => e.Id == enrollmentId);

            if (enrollment == null)
            {
                _logger.LogWarning($"Matrícula não encontrada: {enrollmentId}");
                return null;
            }

            // Verificar se o aluno completou o curso
            if (enrollment.Progress < 100)
            {
                _logger.LogWarning($"Aluno não completou o curso. Progresso: {enrollment.Progress}%");
                return null;
            }

            // Verificar se já existe um certificado
            var existingCertificate = await _context.Certificates
                .FirstOrDefaultAsync(c => c.EnrollmentId == enrollmentId);

            if (existingCertificate != null)
            {
                return new CertificateResponseDto
                {
                    Id = existingCertificate.Id,
                    StudentName = $"{enrollment.User?.FirstName} {enrollment.User?.LastName}",
                    CourseName = enrollment.Course?.Title ?? "Curso Desconhecido",
                    CompletionDate = enrollment.CompletionDate ?? DateTime.UtcNow,
                    CertificateUrl = existingCertificate.CertificateUrl
                };
            }

            // Gerar o PDF do certificado
            var fileName = $"certificate_{enrollmentId}_{DateTime.UtcNow:yyyyMMddHHmmss}.pdf";
            var filePath = Path.Combine(_certificatesPath, fileName);

            GenerateCertificatePdf(filePath, enrollment);

            // Criar registro do certificado no banco de dados
            var certificate = new Certificate
            {
                EnrollmentId = enrollmentId,
                IssueDate = DateTime.UtcNow,
                CertificateUrl = $"/certificates/{fileName}"
            };

            _context.Certificates.Add(certificate);
            enrollment.CertificateId = certificate.Id;
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Certificado gerado com sucesso para matrícula: {enrollmentId}");

            return new CertificateResponseDto
            {
                Id = certificate.Id,
                StudentName = $"{enrollment.User?.FirstName} {enrollment.User?.LastName}",
                CourseName = enrollment.Course?.Title ?? "Curso Desconhecido",
                CompletionDate = enrollment.CompletionDate ?? DateTime.UtcNow,
                CertificateUrl = certificate.CertificateUrl
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao gerar certificado: {ex.Message}");
            return null;
        }
    }

    public async Task<CertificateDto?> GetCertificateAsync(Guid certificateId)
    {
        try
        {
            var certificate = await _context.Certificates
                .Include(c => c.Enrollment)
                .ThenInclude(e => e!.Course)
                .FirstOrDefaultAsync(c => c.Id == certificateId);

            if (certificate == null)
                return null;

            return new CertificateDto
            {
                Id = certificate.Id,
                EnrollmentId = certificate.EnrollmentId,
                IssueDate = certificate.IssueDate,
                CertificateUrl = certificate.CertificateUrl,
                Enrollment = certificate.Enrollment != null ? new EnrollmentDto
                {
                    Id = certificate.Enrollment.Id,
                    UserId = certificate.Enrollment.UserId,
                    CourseId = certificate.Enrollment.CourseId,
                    EnrollmentDate = certificate.Enrollment.EnrollmentDate,
                    CompletionDate = certificate.Enrollment.CompletionDate,
                    Progress = certificate.Enrollment.Progress,
                    CertificateId = certificate.Enrollment.CertificateId,
                    Course = certificate.Enrollment.Course != null ? new CourseDto
                    {
                        Id = certificate.Enrollment.Course.Id,
                        Title = certificate.Enrollment.Course.Title,
                        Description = certificate.Enrollment.Course.Description,
                        Price = certificate.Enrollment.Course.Price,
                        Status = certificate.Enrollment.Course.Status.ToString()
                    } : null
                } : null
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao obter certificado: {ex.Message}");
            return null;
        }
    }

    public async Task<IEnumerable<CertificateDto>> GetUserCertificatesAsync(Guid userId)
    {
        try
        {
            var certificates = await _context.Certificates
                .Include(c => c.Enrollment)
                .ThenInclude(e => e!.User)
                .Include(c => c.Enrollment)
                .ThenInclude(e => e!.Course)
                .Where(c => c.Enrollment!.UserId == userId)
                .OrderByDescending(c => c.IssueDate)
                .ToListAsync();

            return certificates.Select(c => new CertificateDto
            {
                Id = c.Id,
                EnrollmentId = c.EnrollmentId,
                IssueDate = c.IssueDate,
                CertificateUrl = c.CertificateUrl,
                Enrollment = c.Enrollment != null ? new EnrollmentDto
                {
                    Id = c.Enrollment.Id,
                    UserId = c.Enrollment.UserId,
                    CourseId = c.Enrollment.CourseId,
                    EnrollmentDate = c.Enrollment.EnrollmentDate,
                    CompletionDate = c.Enrollment.CompletionDate,
                    Progress = c.Enrollment.Progress,
                    CertificateId = c.Enrollment.CertificateId,
                    Course = c.Enrollment.Course != null ? new CourseDto
                    {
                        Id = c.Enrollment.Course.Id,
                        Title = c.Enrollment.Course.Title,
                        Description = c.Enrollment.Course.Description,
                        Price = c.Enrollment.Course.Price,
                        Status = c.Enrollment.Course.Status.ToString()
                    } : null
                } : null
            }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao obter certificados do usuário: {ex.Message}");
            return Enumerable.Empty<CertificateDto>();
        }
    }

    public async Task<bool> CertificateExistsAsync(Guid enrollmentId)
    {
        return await _context.Certificates
            .AnyAsync(c => c.EnrollmentId == enrollmentId);
    }

    private void GenerateCertificatePdf(string filePath, Enrollment enrollment)
    {
        // Implementação simplificada usando texto puro
        // Em produção, usar uma biblioteca como QuestPDF ou iTextSharp
        
        var studentName = $"{enrollment.User?.FirstName} {enrollment.User?.LastName}";
        var courseName = enrollment.Course?.Title ?? "Curso Desconhecido";
        var completionDate = (enrollment.CompletionDate ?? DateTime.UtcNow).ToString("dd/MM/yyyy", CultureInfo.GetCultureInfo("pt-BR"));
        var certificateNumber = Guid.NewGuid().ToString("N").Substring(0, 12).ToUpper();

        var certificateContent = $@"
╔════════════════════════════════════════════════════════════════════════════════╗
║                                                                                ║
║                        VULKANOS ACADEMY                                        ║
║                    CERTIFICADO DE CONCLUSÃO                                    ║
║                                                                                ║
╚════════════════════════════════════════════════════════════════════════════════╝

Certificamos que

                    {studentName}

concluiu com sucesso o curso

                    {courseName}

em {completionDate}

Número do Certificado: {certificateNumber}

Este certificado atesta que o aluno completou todas as atividades e requisitos
necessários para a conclusão do curso, demonstrando competência e dedicação
no aprendizado.

Data de Emissão: {DateTime.UtcNow:dd/MM/yyyy}

Assinado digitalmente por Vulkanos Academy

═════════════════════════════════════════════════════════════════════════════════
";

        // Salvar como arquivo de texto (em produção, seria PDF)
        File.WriteAllText(filePath.Replace(".pdf", ".txt"), certificateContent);
        
        // Para esta versão, vamos criar um PDF simples usando HTML
        CreateSimplePdfCertificate(filePath, studentName, courseName, completionDate, certificateNumber);
    }

    private void CreateSimplePdfCertificate(string filePath, string studentName, string courseName, string completionDate, string certificateNumber)
    {
        // Criar um HTML que será convertido para PDF
        var htmlContent = $@"
<!DOCTYPE html>
<html lang=""pt-BR"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Certificado de Conclusão</title>
    <style>
        body {{
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            margin: 0;
            padding: 0;
            background-color: #f5f5f5;
        }}
        .certificate {{
            max-width: 900px;
            height: 600px;
            margin: 20px auto;
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            border-radius: 10px;
            box-shadow: 0 10px 30px rgba(0, 0, 0, 0.3);
            padding: 40px;
            box-sizing: border-box;
            color: white;
            display: flex;
            flex-direction: column;
            justify-content: center;
            align-items: center;
            text-align: center;
        }}
        .header {{
            font-size: 36px;
            font-weight: bold;
            margin-bottom: 20px;
            text-transform: uppercase;
            letter-spacing: 2px;
        }}
        .subtitle {{
            font-size: 18px;
            margin-bottom: 30px;
            opacity: 0.9;
        }}
        .student-name {{
            font-size: 32px;
            font-weight: bold;
            margin: 20px 0;
            text-decoration: underline;
        }}
        .course-name {{
            font-size: 24px;
            margin: 20px 0;
            font-style: italic;
        }}
        .details {{
            margin-top: 30px;
            font-size: 14px;
            opacity: 0.95;
        }}
        .certificate-number {{
            margin-top: 20px;
            font-size: 12px;
            opacity: 0.8;
        }}
        .footer {{
            margin-top: 40px;
            font-size: 12px;
            border-top: 1px solid rgba(255, 255, 255, 0.3);
            padding-top: 20px;
        }}
    </style>
</head>
<body>
    <div class=""certificate"">
        <div class=""header"">Vulkanos Academy</div>
        <div class=""subtitle"">Certificado de Conclusão</div>
        
        <div style=""margin: 30px 0;"">
            Certificamos que
        </div>
        
        <div class=""student-name"">{studentName}</div>
        
        <div style=""margin: 20px 0;"">
            concluiu com sucesso o curso
        </div>
        
        <div class=""course-name"">{courseName}</div>
        
        <div class=""details"">
            <p>em {completionDate}</p>
            <p>Este certificado atesta que o aluno completou todas as atividades e requisitos necessários para a conclusão do curso.</p>
        </div>
        
        <div class=""certificate-number"">
            Número do Certificado: {certificateNumber}
        </div>
        
        <div class=""footer"">
            Assinado digitalmente por Vulkanos Academy em {DateTime.UtcNow:dd/MM/yyyy}
        </div>
    </div>
</body>
</html>";

        // Salvar como arquivo HTML (em produção, converteria para PDF)
        var htmlPath = filePath.Replace(".pdf", ".html");
        File.WriteAllText(htmlPath, htmlContent);
        
        // Para agora, vamos salvar o PDF como um arquivo de texto com o conteúdo
        File.WriteAllText(filePath, htmlContent);
    }
}
