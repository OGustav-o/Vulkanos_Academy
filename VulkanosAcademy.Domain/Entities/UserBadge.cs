using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VulkanosAcademy.Domain.Entities;

public class UserBadge
{
    [Key]
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    [ForeignKey("UserId")]
    public User User { get; set; } = null!;

    public Guid BadgeId { get; set; }
    [ForeignKey("BadgeId")]
    public Badge Badge { get; set; } = null!;

    public DateTime EarnedAt { get; set; }
}
