namespace VulkanosAcademy.Domain.DTOs;

public class UserProfileDto
{
    public Guid UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public int TotalXP { get; set; }
    public int CurrentLevel { get; set; }
    public string Title { get; set; } = "Iniciante";
    public IEnumerable<UserBadgeDto>? Badges { get; set; }
}

public class GamificationEventDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string EventType { get; set; } = string.Empty;
    public int XPGranted { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid? ReferenceId { get; set; }
}

public class BadgeDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string IconUrl { get; set; } = string.Empty;
    public int? RequiredXP { get; set; }
    public string BadgeType { get; set; } = string.Empty;
}

public class UserBadgeDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid BadgeId { get; set; }
    public DateTime EarnedAt { get; set; }
    public BadgeDto? Badge { get; set; }
}

public class LeaderboardEntryDto
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public int TotalXP { get; set; }
    public int CurrentLevel { get; set; }
    public string Title { get; set; } = string.Empty;
}

public class EarnXPRequestDto
{
    public Guid UserId { get; set; }
    public string EventType { get; set; } = string.Empty;
    public Guid? ReferenceId { get; set; }
}
