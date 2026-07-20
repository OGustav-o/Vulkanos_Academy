using Microsoft.EntityFrameworkCore;
using VulkanosAcademy.Domain.Entities;

namespace VulkanosAcademy.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Course> Courses { get; set; }
    public DbSet<Module> Modules { get; set; }
    public DbSet<Lesson> Lessons { get; set; }
    public DbSet<LessonMaterial> LessonMaterials { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Enrollment> Enrollments { get; set; }
    public DbSet<Certificate> Certificates { get; set; }
    public DbSet<UserProfile> UserProfiles { get; set; }
    public DbSet<GamificationEvent> GamificationEvents { get; set; }
    public DbSet<Badge> Badges { get; set; }
    public DbSet<UserBadge> UserBadges { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>()
            .HasKey(u => u.Id);
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();
        modelBuilder.Entity<User>()
            .HasMany(u => u.CreatedCourses)
            .WithOne(c => c.Instructor)
            .HasForeignKey(c => c.InstructorId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<User>()
            .HasMany(u => u.Comments)
            .WithOne(c => c.User)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<User>()
            .HasMany(u => u.Enrollments)
            .WithOne(e => e.User)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Course configuration
        modelBuilder.Entity<Course>()
            .HasKey(c => c.Id);
        modelBuilder.Entity<Course>()
            .HasMany(c => c.Modules)
            .WithOne(m => m.Course)
            .HasForeignKey(m => m.CourseId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Course>()
            .HasMany(c => c.Enrollments)
            .WithOne(e => e.Course)
            .HasForeignKey(e => e.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        // Module configuration
        modelBuilder.Entity<Module>()
            .HasKey(m => m.Id);
        modelBuilder.Entity<Module>()
            .HasMany(m => m.Lessons)
            .WithOne(l => l.Module)
            .HasForeignKey(l => l.ModuleId)
            .OnDelete(DeleteBehavior.Cascade);

        // Lesson configuration
        modelBuilder.Entity<Lesson>()
            .HasKey(l => l.Id);
        modelBuilder.Entity<Lesson>()
            .HasMany(l => l.Materials)
            .WithOne(m => m.Lesson)
            .HasForeignKey(m => m.LessonId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Lesson>()
            .HasMany(l => l.Comments)
            .WithOne(c => c.Lesson)
            .HasForeignKey(c => c.LessonId)
            .OnDelete(DeleteBehavior.Cascade);

        // LessonMaterial configuration
        modelBuilder.Entity<LessonMaterial>()
            .HasKey(m => m.Id);

        // Comment configuration
        modelBuilder.Entity<Comment>()
            .HasKey(c => c.Id);
        modelBuilder.Entity<Comment>()
            .HasOne(c => c.ParentComment)
            .WithMany(c => c.Replies)
            .HasForeignKey(c => c.ParentCommentId)
            .OnDelete(DeleteBehavior.Restrict);

        // Enrollment configuration
        modelBuilder.Entity<Enrollment>()
            .HasKey(e => e.Id);
        modelBuilder.Entity<Enrollment>()
            .HasOne(e => e.Certificate)
            .WithOne(c => c.Enrollment)
            .HasForeignKey<Certificate>(c => c.EnrollmentId)
            .OnDelete(DeleteBehavior.Cascade);

        // Certificate configuration
        modelBuilder.Entity<Certificate>()
            .HasKey(c => c.Id);
        modelBuilder.Entity<Certificate>()
            .HasIndex(c => c.EnrollmentId)
            .IsUnique();

        // UserProfile configuration
        modelBuilder.Entity<UserProfile>()
            .HasKey(up => up.UserId);
        modelBuilder.Entity<UserProfile>()
            .HasOne(up => up.User)
            .WithOne()
            .HasForeignKey<UserProfile>(up => up.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // GamificationEvent configuration
        modelBuilder.Entity<GamificationEvent>()
            .HasKey(ge => ge.Id);
        modelBuilder.Entity<GamificationEvent>()
            .HasOne(ge => ge.User)
            .WithMany(u => u.GamificationEvents)
            .HasForeignKey(ge => ge.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Badge configuration
        modelBuilder.Entity<Badge>()
            .HasKey(b => b.Id);

        // UserBadge configuration
        modelBuilder.Entity<UserBadge>()
            .HasKey(ub => ub.Id);
        modelBuilder.Entity<UserBadge>()
            .HasOne(ub => ub.User)
            .WithMany(u => u.UserBadges)
            .HasForeignKey(ub => ub.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<UserBadge>()
            .HasOne(ub => ub.Badge)
            .WithMany(b => b.UserBadges)
            .HasForeignKey(ub => ub.BadgeId)
            .OnDelete(DeleteBehavior.Cascade);

        // Seed initial badges
        modelBuilder.Entity<Badge>().HasData(
            new Badge { Id = Guid.NewGuid(), Name = "Primeiros Passos", Description = "Concluiu a primeira aula", IconUrl = "/images/badges/first_step.png", BadgeType = "Milestone" },
            new Badge { Id = Guid.NewGuid(), Name = "Maratonista", Description = "Concluiu 5 aulas no mesmo dia", IconUrl = "/images/badges/marathon.png", BadgeType = "Milestone" },
            new Badge { Id = Guid.NewGuid(), Name = "Comunicativo", Description = "Fez 10 comentários", IconUrl = "/images/badges/communicator.png", BadgeType = "Social" },
            new Badge { Id = Guid.NewGuid(), Name = "Graduado", Description = "Concluiu o primeiro curso", IconUrl = "/images/badges/graduate.png", BadgeType = "Completion" },
            new Badge { Id = Guid.NewGuid(), Name = "Colecionador", Description = "Obteve 3 certificados", IconUrl = "/images/badges/collector.png", BadgeType = "Completion" }
        );
    }
}
