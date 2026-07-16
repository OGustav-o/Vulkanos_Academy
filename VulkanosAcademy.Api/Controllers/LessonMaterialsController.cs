using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VulkanosAcademy.Data;
using VulkanosAcademy.Domain.DTOs;
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
    public async Task<ActionResult<IEnumerable<LessonMaterialDto>>> GetMaterialsByLesson(Guid lessonId)
    {
        var materials = await _context.LessonMaterials
            .Where(m => m.LessonId == lessonId)
            .Select(m => new LessonMaterialDto
            {
                Id = m.Id,
                LessonId = m.LessonId,
                Title = m.Title,
                MaterialUrl = m.MaterialUrl,
                Type = m.Type.ToString()
            })
            .ToListAsync();

        return Ok(materials);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<LessonMaterialDto>> GetMaterial(Guid id)
    {
        var material = await _context.LessonMaterials.FindAsync(id);
        if (material == null)
            return NotFound();

        return Ok(new LessonMaterialDto
        {
            Id = material.Id,
            LessonId = material.LessonId,
            Title = material.Title,
            MaterialUrl = material.MaterialUrl,
            Type = material.Type.ToString()
        });
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<LessonMaterialDto>> CreateMaterial([FromBody] CreateLessonMaterialDto createMaterialDto)
    {
        var lesson = await _context.Lessons.FindAsync(createMaterialDto.LessonId);
        if (lesson == null)
            return NotFound("Aula não encontrada");

        var materialType = Enum.Parse<MaterialType>(createMaterialDto.Type);
        var material = new LessonMaterial
        {
            LessonId = createMaterialDto.LessonId,
            Title = createMaterialDto.Title,
            MaterialUrl = createMaterialDto.MaterialUrl,
            Type = materialType
        };

        _context.LessonMaterials.Add(material);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetMaterial), new { id = material.Id }, new LessonMaterialDto
        {
            Id = material.Id,
            LessonId = material.LessonId,
            Title = material.Title,
            MaterialUrl = material.MaterialUrl,
            Type = material.Type.ToString()
        });
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateMaterial(Guid id, [FromBody] UpdateLessonMaterialDto updateMaterialDto)
    {
        var material = await _context.LessonMaterials.FindAsync(id);
        if (material == null)
            return NotFound();

        if (!string.IsNullOrEmpty(updateMaterialDto.Title))
            material.Title = updateMaterialDto.Title;
        if (!string.IsNullOrEmpty(updateMaterialDto.MaterialUrl))
            material.MaterialUrl = updateMaterialDto.MaterialUrl;
        if (!string.IsNullOrEmpty(updateMaterialDto.Type))
            material.Type = Enum.Parse<MaterialType>(updateMaterialDto.Type);

        await _context.SaveChangesAsync();
        return NoContent();
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
