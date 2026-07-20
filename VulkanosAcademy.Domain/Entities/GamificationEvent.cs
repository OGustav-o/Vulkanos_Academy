using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VulkanosAcademy.Domain.Entities;

public class GamificationEvent
{
    [Key]
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    [ForeignKey("UserId")]
    public User User { get; set; } = null!;

    public string EventType { get; set; } = string.Empty; // Ex: LessonCompleted, CourseCompleted, CommentPosted
    public int XPGranted { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid? ReferenceId { get; set; } // ID da aula, curso, comentário, etc.
}
