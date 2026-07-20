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
public class GamificationController : ControllerBase
{
    private readonly IGamificationService _gamificationService;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<GamificationController> _logger;

    public GamificationController(
        IGamificationService gamificationService,
        ApplicationDbContext context,
        ILogger<GamificationController> logger)
    {
        _gamificationService = gamificationService;
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Obtém o perfil de gamificação de um usuário
    /// </summary>
    [HttpGet("profile/{userId}")]
    public async Task<ActionResult<UserProfileDto>> GetUserProfile(Guid userId)
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

            var userProfile = await _gamificationService.GetUserProfileAsync(userId);
            return Ok(userProfile);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao obter perfil de gamificação: {ex.Message}");
            return StatusCode(500, "Erro ao obter perfil de gamificação");
        }
    }

    /// <summary>
    /// Obtém o ranking global de usuários
    /// </summary>
    [HttpGet("leaderboard")]
    public async Task<ActionResult<IEnumerable<LeaderboardEntryDto>>> GetLeaderboard()
    {
        try
        {
            var leaderboard = await _gamificationService.GetLeaderboardAsync();
            return Ok(leaderboard);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao obter leaderboard: {ex.Message}");
            return StatusCode(500, "Erro ao obter leaderboard");
        }
    }

    /// <summary>
    /// Processa um evento de gamificação (interno, para ser chamado por outros serviços)
    /// </summary>
    [HttpPost("event")]
    [ApiExplorerSettings(IgnoreApi = true)] // Esconder do Swagger/OpenAPI
    public async Task<IActionResult> ProcessGamificationEvent([FromBody] EarnXPRequestDto request)
    {
        try
        {
            // Esta rota deve ser protegida para chamadas internas ou por um token específico
            // Por simplicidade, aqui assumimos que o serviço de gamificação lida com a lógica de XP
            await _gamificationService.ProcessGamificationEventAsync(request.UserId, request.EventType, request.ReferenceId);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao processar evento de gamificação: {ex.Message}");
            return StatusCode(500, "Erro ao processar evento de gamificação");
        }
    }

    /// <summary>
    /// Concede uma medalha a um usuário (interno, para ser chamado por outros serviços)
    /// </summary>
    [HttpPost("award-badge")]
    [ApiExplorerSettings(IgnoreApi = true)] // Esconder do Swagger/OpenAPI
    public async Task<IActionResult> AwardBadge([FromBody] AwardBadgeRequestDto request)
    {
        try
        {
            // Esta rota deve ser protegida para chamadas internas ou por um token específico
            var awarded = await _gamificationService.AwardBadgeAsync(request.UserId, request.BadgeName);
            if (awarded)
            {
                return Ok();
            }
            return BadRequest("Não foi possível conceder a medalha.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao conceder medalha: {ex.Message}");
            return StatusCode(500, "Erro ao conceder medalha");
        }
    }
}

public class AwardBadgeRequestDto
{
    public Guid UserId { get; set; }
    public string BadgeName { get; set; } = string.Empty;
}
