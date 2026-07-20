using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VulkanosAcademy.Data;
using VulkanosAcademy.Domain.DTOs;
using VulkanosAcademy.Domain.Entities;
using VulkanosAcademy.Api.Services;

namespace VulkanosAcademy.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EnrollmentsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IGamificationService _gamificationService;

    public EnrollmentsController(ApplicationDbContext context, IGamificationService gamificationService)
    {
        _context = context;
        _gamificationService = gamificationService;
    }

    /// <summary>
    /// Obtém todas as matrículas de um usuário
    /// </summary>
    [Authorize]
    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<EnrollmentDto>>> GetUserEnrollments(Guid userId)
    {
        // Validar se o usuário está consultando seus próprios dados ou é um admin
        var userIdClaim = User.FindFirst("sub")?.Value ?? User.FindFirst("nameid")?.Value;
        if (!Guid.TryParse(userIdClaim, out var currentUserId))
            return Unauthorized();

        var user = await _context.Users.FindAsync(currentUserId);
        if (currentUserId != userId && user?.Role != UserRole.Admin)
            return Forbid();

        var enrollments = await _context.Enrollments
            .Where(e => e.UserId == userId)
            .Include(e => e.Course)
            .Include(e => e.Certificate)
            .OrderByDescending(e => e.EnrollmentDate)
            .Select(e => new EnrollmentDto
            {
                Id = e.Id,
                UserId = e.UserId,
                CourseId = e.CourseId,
                EnrollmentDate = e.EnrollmentDate,
                CompletionDate = e.CompletionDate,
                Progress = e.Progress,
                CertificateId = e.CertificateId,
                Course = new CourseDto
                {
                    Id = e.Course!.Id,
                    Title = e.Course.Title,
                    Description = e.Course.Description,
                    Price = e.Course.Price,
                    Status = e.Course.Status.ToString()
                }
            })
            .ToListAsync();

        return Ok(enrollments);
    }

    /// <summary>
    /// Obtém uma matrícula específica
    /// </summary>
    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult<EnrollmentDto>> GetEnrollment(Guid id)
    {
        var enrollment = await _context.Enrollments
            .Include(e => e.Course)
            .Include(e => e.Certificate)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (enrollment == null)
            return NotFound();

        var enrollmentDto = new EnrollmentDto
        {
            Id = enrollment.Id,
            UserId = enrollment.UserId,
            CourseId = enrollment.CourseId,
            EnrollmentDate = enrollment.EnrollmentDate,
            CompletionDate = enrollment.CompletionDate,
            Progress = enrollment.Progress,
            CertificateId = enrollment.CertificateId,
            Course = new CourseDto
            {
                Id = enrollment.Course!.Id,
                Title = enrollment.Course.Title,
                Description = enrollment.Course.Description,
                Price = enrollment.Course.Price,
                Status = enrollment.Course.Status.ToString()
            }
        };

        return Ok(enrollmentDto);
    }

    /// <summary>
    /// Cria uma nova matrícula
    /// </summary>
    [Authorize]
    [HttpPost]
    public async Task<ActionResult<EnrollmentDto>> CreateEnrollment([FromBody] CreateEnrollmentDto createEnrollmentDto)
    {
        // Validar se o curso existe
        var course = await _context.Courses.FindAsync(createEnrollmentDto.CourseId);
        if (course == null)
            return NotFound("Curso não encontrado");

        // Obter ID do usuário autenticado
        var userIdClaim = User.FindFirst("sub")?.Value ?? User.FindFirst("nameid")?.Value;
        if (!Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized("Usuário não identificado");

        // Verificar se o usuário já está matriculado
        var existingEnrollment = await _context.Enrollments
            .FirstOrDefaultAsync(e => e.UserId == userId && e.CourseId == createEnrollmentDto.CourseId);

        if (existingEnrollment != null)
            return BadRequest("Usuário já está matriculado neste curso");

        var enrollment = new Enrollment
        {
            UserId = userId,
            CourseId = createEnrollmentDto.CourseId,
            Progress = 0
        };

        _context.Enrollments.Add(enrollment);
        await _context.SaveChangesAsync();

        var enrollmentDto = new EnrollmentDto
        {
            Id = enrollment.Id,
            UserId = enrollment.UserId,
            CourseId = enrollment.CourseId,
            EnrollmentDate = enrollment.EnrollmentDate,
            CompletionDate = enrollment.CompletionDate,
            Progress = enrollment.Progress,
            CertificateId = enrollment.CertificateId,
            Course = new CourseDto
            {
                Id = course.Id,
                Title = course.Title,
                Description = course.Description,
                Price = course.Price,
                Status = course.Status.ToString()
            }
        };

        return CreatedAtAction(nameof(GetEnrollment), new { id = enrollment.Id }, enrollmentDto);
    }

    /// <summary>
    /// Atualiza o progresso de uma matrícula
    /// </summary>
    [Authorize]
    [HttpPut("{id}/progress")]
    public async Task<IActionResult> UpdateProgress(Guid id, [FromBody] UpdateProgressDto updateProgressDto)
    {
        var enrollment = await _context.Enrollments.FindAsync(id);
        if (enrollment == null)
            return NotFound();

        // Validar se o progresso está entre 0 e 100
        if (updateProgressDto.Progress < 0 || updateProgressDto.Progress > 100)
            return BadRequest("Progresso deve estar entre 0 e 100");

        enrollment.Progress = updateProgressDto.Progress;

        // Se o progresso atingiu 100%, marcar como completo
        if (updateProgressDto.Progress >= 100)
            {
                enrollment.CompletionDate = DateTime.UtcNow;
                // Processar evento de gamificação para conclusão de curso
                await _gamificationService.ProcessGamificationEventAsync(enrollment.UserId, "CourseCompleted", enrollment.CourseId);
            }
            // Processar evento de gamificação para conclusão de aula (simplificado para cada atualização de progresso)
            // Em um cenário real, isso seria mais granular, talvez por aula específica
            await _gamificationService.ProcessGamificationEventAsync(enrollment.UserId, "LessonCompleted", enrollment.CourseId); // Usando CourseId como referência temporária para aula

        await _context.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Deleta uma matrícula
    /// </summary>
    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEnrollment(Guid id)
    {
        var enrollment = await _context.Enrollments.FindAsync(id);
        if (enrollment == null)
            return NotFound();

        // Validar se o usuário é o dono ou um administrador
        var userIdClaim = User.FindFirst("sub")?.Value ?? User.FindFirst("nameid")?.Value;
        if (!Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        var user = await _context.Users.FindAsync(userId);
        if (enrollment.UserId != userId && user?.Role != UserRole.Admin)
            return Forbid();

        _context.Enrollments.Remove(enrollment);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Obtém estatísticas de um curso (para produtores)
    /// </summary>
    [Authorize]
    [HttpGet("course/{courseId}/stats")]
    public async Task<ActionResult<CourseStatsDto>> GetCourseStats(Guid courseId)
    {
        var course = await _context.Courses.FindAsync(courseId);
        if (course == null)
            return NotFound();

        // Validar se o usuário é o instrutor ou um admin
        var userIdClaim = User.FindFirst("sub")?.Value ?? User.FindFirst("nameid")?.Value;
        if (!Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        var user = await _context.Users.FindAsync(userId);
        if (course.InstructorId != userId && user?.Role != UserRole.Admin)
            return Forbid();

        var enrollments = await _context.Enrollments
            .Where(e => e.CourseId == courseId)
            .ToListAsync();

        var totalEnrollments = enrollments.Count;
        var completedEnrollments = enrollments.Count(e => e.Progress >= 100);
        var averageProgress = totalEnrollments > 0 ? enrollments.Average(e => e.Progress) : 0;
        var activeStudents = enrollments.Count(e => e.Progress > 0 && e.Progress < 100);

        var stats = new CourseStatsDto
        {
            CourseId = courseId,
            TotalEnrollments = totalEnrollments,
            CompletedEnrollments = completedEnrollments,
            AverageProgress = averageProgress,
            ActiveStudents = activeStudents,
            CompletionRate = totalEnrollments > 0 ? (completedEnrollments * 100.0m) / totalEnrollments : 0
        };

        return Ok(stats);
    }
}
