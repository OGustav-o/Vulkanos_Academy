using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VulkanosAcademy.Data;
using VulkanosAcademy.Domain.DTOs;
using VulkanosAcademy.Domain.Entities;

namespace VulkanosAcademy.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CoursesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public CoursesController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CourseDto>>> GetCourses()
    {
        var courses = await _context.Courses
            .Where(c => c.Status == CourseStatus.Published)
            .Select(c => new CourseDto
            {
                Id = c.Id,
                Title = c.Title,
                Description = c.Description,
                InstructorId = c.InstructorId,
                ThumbnailUrl = c.ThumbnailUrl,
                Price = c.Price,
                Status = c.Status.ToString(),
                CreatedAt = c.CreatedAt
            })
            .ToListAsync();

        return Ok(courses);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CourseDto>> GetCourse(Guid id)
    {
        var course = await _context.Courses.FindAsync(id);
        if (course == null)
            return NotFound();

        return Ok(new CourseDto
        {
            Id = course.Id,
            Title = course.Title,
            Description = course.Description,
            InstructorId = course.InstructorId,
            ThumbnailUrl = course.ThumbnailUrl,
            Price = course.Price,
            Status = course.Status.ToString(),
            CreatedAt = course.CreatedAt
        });
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<CourseDto>> CreateCourse([FromBody] CreateCourseDto createCourseDto)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var course = new Course
        {
            Title = createCourseDto.Title,
            Description = createCourseDto.Description,
            ThumbnailUrl = createCourseDto.ThumbnailUrl,
            Price = createCourseDto.Price,
            InstructorId = Guid.Parse(userId),
            Status = CourseStatus.Draft
        };

        _context.Courses.Add(course);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetCourse), new { id = course.Id }, new CourseDto
        {
            Id = course.Id,
            Title = course.Title,
            Description = course.Description,
            InstructorId = course.InstructorId,
            ThumbnailUrl = course.ThumbnailUrl,
            Price = course.Price,
            Status = course.Status.ToString(),
            CreatedAt = course.CreatedAt
        });
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCourse(Guid id, [FromBody] UpdateCourseDto updateCourseDto)
    {
        var course = await _context.Courses.FindAsync(id);
        if (course == null)
            return NotFound();

        if (!string.IsNullOrEmpty(updateCourseDto.Title))
            course.Title = updateCourseDto.Title;
        if (!string.IsNullOrEmpty(updateCourseDto.Description))
            course.Description = updateCourseDto.Description;
        if (!string.IsNullOrEmpty(updateCourseDto.ThumbnailUrl))
            course.ThumbnailUrl = updateCourseDto.ThumbnailUrl;
        if (updateCourseDto.Price.HasValue)
            course.Price = updateCourseDto.Price;
        if (!string.IsNullOrEmpty(updateCourseDto.Status))
            course.Status = Enum.Parse<CourseStatus>(updateCourseDto.Status);

        course.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCourse(Guid id)
    {
        var course = await _context.Courses.FindAsync(id);
        if (course == null)
            return NotFound();

        _context.Courses.Remove(course);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
