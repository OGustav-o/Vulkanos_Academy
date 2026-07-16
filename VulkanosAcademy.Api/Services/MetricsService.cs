using Microsoft.EntityFrameworkCore;
using VulkanosAcademy.Data;
using VulkanosAcademy.Domain.DTOs;
using VulkanosAcademy.Domain.Entities;

namespace VulkanosAcademy.Api.Services;

public interface IMetricsService
{
    Task<GlobalMetricsDto> GetGlobalMetricsAsync();
    Task<GlobalMetricsDto> GetMetricsForProducerAsync(Guid producerId);
    Task<IEnumerable<CourseStatsDto>> GetProducerCoursesStatsAsync(Guid producerId);
}

public class MetricsService : IMetricsService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<MetricsService> _logger;

    public MetricsService(ApplicationDbContext context, ILogger<MetricsService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<GlobalMetricsDto> GetGlobalMetricsAsync()
    {
        try
        {
            var totalUsers = await _context.Users.CountAsync();
            var totalCourses = await _context.Courses.CountAsync();
            var totalEnrollments = await _context.Enrollments.CountAsync();
            var completedEnrollments = await _context.Enrollments
                .Where(e => e.Progress >= 100)
                .CountAsync();
            
            var averageCompletionRate = totalEnrollments > 0
                ? (completedEnrollments * 100.0) / totalEnrollments
                : 0;

            var activeStudents = await _context.Enrollments
                .Where(e => e.Progress > 0 && e.Progress < 100)
                .Select(e => e.UserId)
                .Distinct()
                .CountAsync();

            var totalRevenue = await _context.Enrollments
                .Include(e => e.Course)
                .SumAsync(e => e.Course != null ? e.Course.Price : 0);

            _logger.LogInformation("Métricas globais calculadas com sucesso");

            return new GlobalMetricsDto
            {
                TotalUsers = totalUsers,
                TotalCourses = totalCourses,
                TotalEnrollments = totalEnrollments,
                CompletedEnrollments = completedEnrollments,
                AverageCompletionRate = averageCompletionRate,
                ActiveStudents = activeStudents,
                TotalRevenue = totalRevenue
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao calcular métricas globais: {ex.Message}");
            throw;
        }
    }

    public async Task<GlobalMetricsDto> GetMetricsForProducerAsync(Guid producerId)
    {
        try
        {
            // Validar se o produtor existe
            var producer = await _context.Users.FindAsync(producerId);
            if (producer == null || producer.Role != UserRole.Producer)
            {
                _logger.LogWarning($"Produtor não encontrado ou não autorizado: {producerId}");
                throw new UnauthorizedAccessException("Produtor não encontrado ou não autorizado");
            }

            // Obter cursos do produtor
            var producerCourses = await _context.Courses
                .Where(c => c.InstructorId == producerId)
                .Select(c => c.Id)
                .ToListAsync();

            // Calcular métricas para os cursos do produtor
            var totalCourses = producerCourses.Count;
            
            var enrollments = await _context.Enrollments
                .Where(e => producerCourses.Contains(e.CourseId))
                .ToListAsync();

            var totalEnrollments = enrollments.Count;
            var completedEnrollments = enrollments.Count(e => e.Progress >= 100);
            
            var averageCompletionRate = totalEnrollments > 0
                ? (completedEnrollments * 100.0) / totalEnrollments
                : 0;

            var activeStudents = enrollments
                .Where(e => e.Progress > 0 && e.Progress < 100)
                .Select(e => e.UserId)
                .Distinct()
                .Count();

            var totalRevenue = await _context.Enrollments
                .Include(e => e.Course)
                .Where(e => producerCourses.Contains(e.CourseId))
                .SumAsync(e => e.Course != null ? e.Course.Price : 0);

            _logger.LogInformation($"Métricas do produtor {producerId} calculadas com sucesso");

            return new GlobalMetricsDto
            {
                TotalUsers = enrollments.Select(e => e.UserId).Distinct().Count(),
                TotalCourses = totalCourses,
                TotalEnrollments = totalEnrollments,
                CompletedEnrollments = completedEnrollments,
                AverageCompletionRate = averageCompletionRate,
                ActiveStudents = activeStudents,
                TotalRevenue = totalRevenue
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao calcular métricas do produtor: {ex.Message}");
            throw;
        }
    }

    public async Task<IEnumerable<CourseStatsDto>> GetProducerCoursesStatsAsync(Guid producerId)
    {
        try
        {
            // Validar se o produtor existe
            var producer = await _context.Users.FindAsync(producerId);
            if (producer == null || producer.Role != UserRole.Producer)
            {
                _logger.LogWarning($"Produtor não encontrado ou não autorizado: {producerId}");
                throw new UnauthorizedAccessException("Produtor não encontrado ou não autorizado");
            }

            // Obter cursos do produtor com estatísticas
            var coursesStats = await _context.Courses
                .Where(c => c.InstructorId == producerId)
                .Select(c => new
                {
                    Course = c,
                    Enrollments = _context.Enrollments.Where(e => e.CourseId == c.Id).ToList()
                })
                .ToListAsync();

            var result = new List<CourseStatsDto>();

            foreach (var courseData in coursesStats)
            {
                var enrollments = courseData.Enrollments;
                var totalEnrollments = enrollments.Count;
                var completedEnrollments = enrollments.Count(e => e.Progress >= 100);
                var averageProgress = totalEnrollments > 0 ? enrollments.Average(e => (double)e.Progress) : 0;
                var activeStudents = enrollments.Count(e => e.Progress > 0 && e.Progress < 100);
                var completionRate = totalEnrollments > 0 ? (completedEnrollments * 100.0m) / totalEnrollments : 0;

                result.Add(new CourseStatsDto
                {
                    CourseId = courseData.Course.Id,
                    TotalEnrollments = totalEnrollments,
                    CompletedEnrollments = completedEnrollments,
                    AverageProgress = averageProgress,
                    ActiveStudents = activeStudents,
                    CompletionRate = completionRate
                });
            }

            _logger.LogInformation($"Estatísticas dos cursos do produtor {producerId} calculadas com sucesso");

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao calcular estatísticas dos cursos do produtor: {ex.Message}");
            throw;
        }
    }
}
