using Microsoft.EntityFrameworkCore;
using VulkanosAcademy.Data;
using VulkanosAcademy.Domain.DTOs;
using VulkanosAcademy.Domain.Entities;

namespace VulkanosAcademy.Api.Services;

public interface IGamificationService
{
    Task<UserProfileDto> GetUserProfileAsync(Guid userId);
    Task<IEnumerable<LeaderboardEntryDto>> GetLeaderboardAsync();
    Task ProcessGamificationEventAsync(Guid userId, string eventType, Guid? referenceId = null);
    Task<bool> AwardBadgeAsync(Guid userId, string badgeName);
}

public class GamificationService : IGamificationService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<GamificationService> _logger;

    public GamificationService(ApplicationDbContext context, ILogger<GamificationService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<UserProfileDto> GetUserProfileAsync(Guid userId)
    {
        var userProfile = await _context.UserProfiles
            .Include(up => up.UserBadges)
                .ThenInclude(ub => ub.Badge)
            .FirstOrDefaultAsync(up => up.UserId == userId);

        if (userProfile == null)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) throw new Exception("User not found.");
            userProfile = new UserProfile { UserId = userId, TotalXP = 0, CurrentLevel = 1, Title = "Iniciante", User = user };
            _context.UserProfiles.Add(userProfile);
            await _context.SaveChangesAsync();
        }

        return new UserProfileDto
        {
            UserId = userProfile.UserId,
            FirstName = userProfile.User.FirstName,
            LastName = userProfile.User.LastName,
            TotalXP = userProfile.TotalXP,
            CurrentLevel = userProfile.CurrentLevel,
            Title = userProfile.Title,
            Badges = userProfile.UserBadges.Select(ub => new UserBadgeDto
            {
                Id = ub.Id,
                UserId = ub.UserId,
                BadgeId = ub.BadgeId,
                EarnedAt = ub.EarnedAt,
                Badge = new BadgeDto
                {
                    Id = ub.Badge.Id,
                    Name = ub.Badge.Name,
                    Description = ub.Badge.Description,
                    IconUrl = ub.Badge.IconUrl,
                    RequiredXP = ub.Badge.RequiredXP,
                    BadgeType = ub.Badge.BadgeType
                }
            }).ToList()
        };
    }

    public async Task<IEnumerable<LeaderboardEntryDto>> GetLeaderboardAsync()
    {
        return await _context.UserProfiles
            .Include(up => up.User)
            .OrderByDescending(up => up.TotalXP)
            .Take(100) // Top 100 users
            .Select(up => new LeaderboardEntryDto
            {
                UserId = up.UserId,
                UserName = $"{up.User.FirstName} {up.User.LastName}",
                TotalXP = up.TotalXP,
                CurrentLevel = up.CurrentLevel,
                Title = up.Title
            })
            .ToListAsync();
    }

    public async Task ProcessGamificationEventAsync(Guid userId, string eventType, Guid? referenceId = null)
    {
        var userProfile = await _context.UserProfiles.FirstOrDefaultAsync(up => up.UserId == userId);
        if (userProfile == null)
        {
            userProfile = new UserProfile { UserId = userId, TotalXP = 0, CurrentLevel = 1, Title = "Iniciante" };
            _context.UserProfiles.Add(userProfile);
        }

        var xpGranted = GetXPForEvent(eventType);
        userProfile.TotalXP += xpGranted;

        // Check for level up
        var newLevel = CalculateLevel(userProfile.TotalXP);
        if (newLevel > userProfile.CurrentLevel)
        {
            _logger.LogInformation($"User {userId} leveled up from {userProfile.CurrentLevel} to {newLevel}");
            userProfile.CurrentLevel = newLevel;
            userProfile.Title = GetTitleForLevel(newLevel);
            // Award level-up badge if applicable
        }

        _context.GamificationEvents.Add(new GamificationEvent
        {
            UserId = userId,
            EventType = eventType,
            XPGranted = xpGranted,
            CreatedAt = DateTime.UtcNow,
            ReferenceId = referenceId
        });

        await _context.SaveChangesAsync();
        _logger.LogInformation($"Gamification event {eventType} processed for user {userId}. XP granted: {xpGranted}");

        // Check for badges after saving changes
        await CheckAndAwardBadgesAsync(userId, eventType, referenceId);
    }

    public async Task<bool> AwardBadgeAsync(Guid userId, string badgeName)
    {
        var badge = await _context.Badges.FirstOrDefaultAsync(b => b.Name == badgeName);
        if (badge == null)
        {
            _logger.LogWarning($"Badge '{badgeName}' not found.");
            return false;
        }

        var userHasBadge = await _context.UserBadges.AnyAsync(ub => ub.UserId == userId && ub.BadgeId == badge.Id);
        if (userHasBadge)
        {
            _logger.LogInformation($"User {userId} already has badge '{badgeName}'.");
            return false;
        }

        _context.UserBadges.Add(new UserBadge
        {
            UserId = userId,
            BadgeId = badge.Id,
            EarnedAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
        _logger.LogInformation($"Badge '{badgeName}' awarded to user {userId}.");
        return true;
    }

    private int GetXPForEvent(string eventType)
    {
        return eventType switch
        {
            "LessonCompleted" => 10,
            "ModuleCompleted" => 50,
            "CourseCompleted" => 200,
            "CommentPosted" => 5,
            "CommentLiked" => 2,
            _ => 0,
        };
    }

    private int CalculateLevel(int totalXP)
    {
        if (totalXP < 101) return 1;
        if (totalXP < 301) return 2;
        if (totalXP < 601) return 3;
        if (totalXP < 1001) return 4;
        return 5; // Mestre
    }

    private string GetTitleForLevel(int level)
    {
        return level switch
        {
            1 => "Iniciante",
            2 => "Aprendiz",
            3 => "Estudioso",
            4 => "Especialista",
            5 => "Mestre",
            _ => "Desconhecido",
        };
    }

    private async Task CheckAndAwardBadgesAsync(Guid userId, string eventType, Guid? referenceId)
    {
        // "Primeiros Passos": Concluir a primeira aula
        if (eventType == "LessonCompleted")
        {
            var completedLessonsCount = await _context.GamificationEvents
                .CountAsync(ge => ge.UserId == userId && ge.EventType == "LessonCompleted");
            if (completedLessonsCount == 1)
            {
                await AwardBadgeAsync(userId, "Primeiros Passos");
            }
        }

        // "Maratonista": Concluir 5 aulas no mesmo dia (simplificado para 5 aulas no total por enquanto)
        if (eventType == "LessonCompleted")
        {
            var lessonsToday = await _context.GamificationEvents
                .Where(ge => ge.UserId == userId && ge.EventType == "LessonCompleted" && ge.CreatedAt.Date == DateTime.UtcNow.Date)
                .CountAsync();
            if (lessonsToday >= 5)
            {
                await AwardBadgeAsync(userId, "Maratonista");
            }
        }

        // "Comunicativo": Fazer 10 comentários
        if (eventType == "CommentPosted")
        {
            var commentsCount = await _context.GamificationEvents
                .CountAsync(ge => ge.UserId == userId && ge.EventType == "CommentPosted");
            if (commentsCount >= 10)
            {
                await AwardBadgeAsync(userId, "Comunicativo");
            }
        }

        // "Graduado": Concluir o primeiro curso
        if (eventType == "CourseCompleted")
        {
            var completedCoursesCount = await _context.GamificationEvents
                .CountAsync(ge => ge.UserId == userId && ge.EventType == "CourseCompleted");
            if (completedCoursesCount == 1)
            {
                await AwardBadgeAsync(userId, "Graduado");
            }
        }

        // "Colecionador": Obter 3 certificados
        if (eventType == "CertificateGenerated") // Assumindo que um evento é disparado ao gerar certificado
        {
            var certificatesCount = await _context.Certificates
                .CountAsync(c => c.Enrollment!.UserId == userId);
            if (certificatesCount >= 3)
            {
                await AwardBadgeAsync(userId, "Colecionador");
            }
        }
    }
}
