using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VulkanosAcademy.Domain.Entities;

public class UserProfile
{
    [Key]
    public Guid UserId { get; set; }
    [ForeignKey("UserId")]
    public User User { get; set; } = null!;

    public int TotalXP { get; set; }
    public int CurrentLevel { get; set; }
    public string Title { get; set; } = "Iniciante";

    public ICollection<UserBadge> UserBadges { get; set; } = new List<UserBadge>();
    public ICollection<GamificationEvent> GamificationEvents { get; set; } = new List<GamificationEvent>();
}
