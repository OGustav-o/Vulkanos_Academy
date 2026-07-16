using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VulkanosAcademy.Data;
using VulkanosAcademy.Domain.Entities;

namespace VulkanosAcademy.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LessonMaterialsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public LessonMaterialsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("lesson/{lessonId}")]
    public async Task<ActionResult<IEnumerable<LessonMaterial>>> GetMaterialsByLesson(Guid lessonId)
    {
        var materials = await _context.LessonMaterials
            .Where(m => m.LessonId == lessonId)
            .ToListAsync();

        return Ok(materials);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<LessonMaterial>> GetMaterial(Guid id)
    {
        var material = await _context.LessonMaterials.FindAsync(id);
        if (material == null)
            return NotFound();

        return Ok(material);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<LessonMaterial>> CreateMaterial([FromBody] LessonMaterial createMaterialDto)
    {
        var lesson = await _context.Lessons.FindAsync(createMaterialDto.LessonId);
        if (lesson == null)
            return NotFound("Aula não encontrada");

        var material = new LessonMaterial
        {
            LessonId = createMaterialDto.LessonId,
            Title = createMaterialDto.Title,
            MaterialUrl = createMaterialDto.MaterialUrl,
            Type = createMaterialDto.Type
        };

        _context.LessonMaterials.Add(material);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetMaterial), new { id = material.Id }, material);
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMaterial(Guid id)
    {
        var material = await _context.LessonMaterials.FindAsync(id);
        if (material == null)
            return NotFound();

        _context.LessonMaterials.Remove(material);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
