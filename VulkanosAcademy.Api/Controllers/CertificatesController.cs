using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VulkanosAcademy.Api.Services;
using VulkanosAcademy.Data;
using VulkanosAcademy.Domain.DTOs;
using VulkanosAcademy.Domain.Entities;

namespace VulkanosAcademy.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CertificatesController : ControllerBase
{
    private readonly ICertificateService _certificateService;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<CertificatesController> _logger;

    public CertificatesController(
        ICertificateService certificateService,
        ApplicationDbContext context,
        ILogger<CertificatesController> logger)
    {
        _certificateService = certificateService;
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Gera um certificado de conclusão para uma matrícula
    /// </summary>
    [HttpPost("generate")]
    public async Task<ActionResult<CertificateResponseDto>> GenerateCertificate([FromBody] GenerateCertificateDto generateCertificateDto)
    {
        try
        {
            // Validar se a matrícula existe
            var enrollment = await _context.Enrollments
                .Include(e => e.User)
                .Include(e => e.Course)
                .FirstOrDefaultAsync(e => e.Id == generateCertificateDto.EnrollmentId);

            if (enrollment == null)
                return NotFound("Matrícula não encontrada");

            // Validar se o usuário é o dono da matrícula ou um administrador
            var userIdClaim = User.FindFirst("sub")?.Value ?? User.FindFirst("nameid")?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var user = await _context.Users.FindAsync(userId);
            if (enrollment.UserId != userId && user?.Role != UserRole.Admin)
                return Forbid();

            // Validar se o aluno completou o curso
            if (enrollment.Progress < 100)
                return BadRequest("Aluno não completou o curso ainda");

            // Gerar o certificado
            var certificate = await _certificateService.GenerateCertificateAsync(generateCertificateDto.EnrollmentId);

            if (certificate == null)
                return BadRequest("Erro ao gerar certificado");

            _logger.LogInformation($"Certificado gerado para matrícula: {generateCertificateDto.EnrollmentId}");

            return Ok(certificate);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao gerar certificado: {ex.Message}");
            return StatusCode(500, "Erro ao gerar certificado");
        }
    }

    /// <summary>
    /// Obtém um certificado específico
    /// </summary>
    [HttpGet("{certificateId}")]
    public async Task<ActionResult<CertificateDto>> GetCertificate(Guid certificateId)
    {
        try
        {
            var certificate = await _certificateService.GetCertificateAsync(certificateId);

            if (certificate == null)
                return NotFound();

            // Validar se o usuário é o dono do certificado ou um administrador
            var userIdClaim = User.FindFirst("sub")?.Value ?? User.FindFirst("nameid")?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var user = await _context.Users.FindAsync(userId);
            if (certificate.Enrollment?.UserId != userId && user?.Role != UserRole.Admin)
                return Forbid();

            return Ok(certificate);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao obter certificado: {ex.Message}");
            return StatusCode(500, "Erro ao obter certificado");
        }
    }

    /// <summary>
    /// Obtém todos os certificados de um usuário
    /// </summary>
    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<CertificateDto>>> GetUserCertificates(Guid userId)
    {
        try
        {
            // Validar se o usuário está consultando seus próprios dados ou é um admin
            var userIdClaim = User.FindFirst("sub")?.Value ?? User.FindFirst("nameid")?.Value;
            if (!Guid.TryParse(userIdClaim, out var currentUserId))
                return Unauthorized();

            var user = await _context.Users.FindAsync(currentUserId);
            if (currentUserId != userId && user?.Role != UserRole.Admin)
                return Forbid();

            var certificates = await _certificateService.GetUserCertificatesAsync(userId);

            return Ok(certificates);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao obter certificados do usuário: {ex.Message}");
            return StatusCode(500, "Erro ao obter certificados");
        }
    }

    /// <summary>
    /// Verifica se um certificado existe para uma matrícula
    /// </summary>
    [HttpGet("enrollment/{enrollmentId}/exists")]
    public async Task<ActionResult<bool>> CertificateExists(Guid enrollmentId)
    {
        try
        {
            var exists = await _certificateService.CertificateExistsAsync(enrollmentId);
            return Ok(exists);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao verificar existência do certificado: {ex.Message}");
            return StatusCode(500, "Erro ao verificar certificado");
        }
    }

    /// <summary>
    /// Faz download do certificado em PDF
    /// </summary>
    [HttpGet("{certificateId}/download")]
    public async Task<IActionResult> DownloadCertificate(Guid certificateId)
    {
        try
        {
            var certificate = await _certificateService.GetCertificateAsync(certificateId);

            if (certificate == null)
                return NotFound();

            // Validar se o usuário é o dono do certificado ou um administrador
            var userIdClaim = User.FindFirst("sub")?.Value ?? User.FindFirst("nameid")?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var user = await _context.Users.FindAsync(userId);
            if (certificate.Enrollment?.UserId != userId && user?.Role != UserRole.Admin)
                return Forbid();

            // Construir o caminho do arquivo
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", certificate.CertificateUrl.TrimStart('/'));

            if (!System.IO.File.Exists(filePath))
                return NotFound("Arquivo do certificado não encontrado");

            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            var fileName = $"certificado_{certificateId}.pdf";

            return File(fileBytes, "application/pdf", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao fazer download do certificado: {ex.Message}");
            return StatusCode(500, "Erro ao fazer download do certificado");
        }
    }
}
