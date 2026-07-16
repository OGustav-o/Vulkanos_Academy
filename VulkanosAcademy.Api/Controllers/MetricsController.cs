using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VulkanosAcademy.Api.Services;
using VulkanosAcademy.Data;
using VulkanosAcademy.Domain.DTOs;
using VulkanosAcademy.Domain.Entities;

namespace VulkanosAcademy.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MetricsController : ControllerBase
{
    private readonly IMetricsService _metricsService;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<MetricsController> _logger;

    public MetricsController(
        IMetricsService metricsService,
        ApplicationDbContext context,
        ILogger<MetricsController> logger)
    {
        _metricsService = metricsService;
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Obtém as métricas globais da plataforma (apenas para administradores)
    /// </summary>
    [HttpGet("global")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<GlobalMetricsDto>> GetGlobalMetrics()
    {
        try
        {
            // Validar se o usuário é um administrador
            var userIdClaim = User.FindFirst("sub")?.Value ?? User.FindFirst("nameid")?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var user = await _context.Users.FindAsync(userId);
            if (user?.Role != UserRole.Admin)
                return Forbid();

            var metrics = await _metricsService.GetGlobalMetricsAsync();

            _logger.LogInformation("Métricas globais recuperadas com sucesso");

            return Ok(metrics);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao obter métricas globais: {ex.Message}");
            return StatusCode(500, "Erro ao obter métricas globais");
        }
    }

    /// <summary>
    /// Obtém as métricas para um produtor específico
    /// </summary>
    [HttpGet("producer/{producerId}")]
    public async Task<ActionResult<GlobalMetricsDto>> GetProducerMetrics(Guid producerId)
    {
        try
        {
            // Validar se o usuário é o produtor ou um administrador
            var userIdClaim = User.FindFirst("sub")?.Value ?? User.FindFirst("nameid")?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var user = await _context.Users.FindAsync(userId);
            if (userId != producerId && user?.Role != UserRole.Admin)
                return Forbid();

            var metrics = await _metricsService.GetMetricsForProducerAsync(producerId);

            _logger.LogInformation($"Métricas do produtor {producerId} recuperadas com sucesso");

            return Ok(metrics);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao obter métricas do produtor: {ex.Message}");
            return StatusCode(500, "Erro ao obter métricas do produtor");
        }
    }

    /// <summary>
    /// Obtém as estatísticas dos cursos de um produtor
    /// </summary>
    [HttpGet("producer/{producerId}/courses")]
    public async Task<ActionResult<IEnumerable<CourseStatsDto>>> GetProducerCoursesStats(Guid producerId)
    {
        try
        {
            // Validar se o usuário é o produtor ou um administrador
            var userIdClaim = User.FindFirst("sub")?.Value ?? User.FindFirst("nameid")?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var user = await _context.Users.FindAsync(userId);
            if (userId != producerId && user?.Role != UserRole.Admin)
                return Forbid();

            var stats = await _metricsService.GetProducerCoursesStatsAsync(producerId);

            _logger.LogInformation($"Estatísticas dos cursos do produtor {producerId} recuperadas com sucesso");

            return Ok(stats);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao obter estatísticas dos cursos: {ex.Message}");
            return StatusCode(500, "Erro ao obter estatísticas dos cursos");
        }
    }
}
