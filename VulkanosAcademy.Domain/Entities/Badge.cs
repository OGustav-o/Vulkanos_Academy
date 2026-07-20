using System.ComponentModel.DataAnnotations;

namespace VulkanosAcademy.Domain.Entities;

public class Badge
{
    [Key]
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string IconUrl { get; set; } = string.Empty;
    public int? RequiredXP { get; set; }
    public string BadgeType { get; set; } = string.Empty; // Ex: "Milestone", "Social", "Completion"

    public ICollection<UserBadge> UserBadges { get; set; } = new List<UserBadge>();
}
