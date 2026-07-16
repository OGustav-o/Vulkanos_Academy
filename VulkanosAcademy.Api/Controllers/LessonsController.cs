using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VulkanosAcademy.Data;
using VulkanosAcademy.Domain.DTOs;
using VulkanosAcademy.Domain.Entities;

namespace VulkanosAcademy.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LessonsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public LessonsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("module/{moduleId}")]
    public async Task<ActionResult<IEnumerable<LessonDto>>> GetLessonsByModule(Guid moduleId)
    {
        var lessons = await _context.Lessons
            .Where(l => l.ModuleId == moduleId)
            .OrderBy(l => l.Order)
            .Select(l => new LessonDto
            {
                Id = l.Id,
                ModuleId = l.ModuleId,
                Title = l.Title,
                Description = l.Description,
                VideoUrl = l.VideoUrl,
                Order = l.Order
            })
            .ToListAsync();

        return Ok(lessons);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<LessonDto>> GetLesson(Guid id)
    {
        var lesson = await _context.Lessons
            .Include(l => l.Materials)
            .Include(l => l.Comments)
            .FirstOrDefaultAsync(l => l.Id == id);

        if (lesson == null)
            return NotFound();

        return Ok(new LessonDto
        {
            Id = lesson.Id,
            ModuleId = lesson.ModuleId,
            Title = lesson.Title,
            Description = lesson.Description,
            VideoUrl = lesson.VideoUrl,
            Order = lesson.Order
        });
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<LessonDto>> CreateLesson([FromBody] CreateLessonDto createLessonDto)
    {
        var module = await _context.Modules.FindAsync(createLessonDto.ModuleId);
        if (module == null)
            return NotFound("Módulo não encontrado");

        var lesson = new Lesson
        {
            ModuleId = createLessonDto.ModuleId,
            Title = createLessonDto.Title,
            Description = createLessonDto.Description,
            VideoUrl = createLessonDto.VideoUrl,
            Order = createLessonDto.Order
        };

        _context.Lessons.Add(lesson);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetLesson), new { id = lesson.Id }, new LessonDto
        {
            Id = lesson.Id,
            ModuleId = lesson.ModuleId,
            Title = lesson.Title,
            Description = lesson.Description,
            VideoUrl = lesson.VideoUrl,
            Order = lesson.Order
        });
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateLesson(Guid id, [FromBody] CreateLessonDto updateLessonDto)
    {
        var lesson = await _context.Lessons.FindAsync(id);
        if (lesson == null)
            return NotFound();

        lesson.Title = updateLessonDto.Title;
        lesson.Description = updateLessonDto.Description;
        lesson.VideoUrl = updateLessonDto.VideoUrl;
        lesson.Order = updateLessonDto.Order;
        lesson.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteLesson(Guid id)
    {
        var lesson = await _context.Lessons.FindAsync(id);
        if (lesson == null)
            return NotFound();

        _context.Lessons.Remove(lesson);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
