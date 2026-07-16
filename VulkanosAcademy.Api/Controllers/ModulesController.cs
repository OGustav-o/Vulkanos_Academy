using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VulkanosAcademy.Data;
using VulkanosAcademy.Domain.DTOs;
using VulkanosAcademy.Domain.Entities;

namespace VulkanosAcademy.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ModulesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ModulesController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("course/{courseId}")]
    public async Task<ActionResult<IEnumerable<ModuleDto>>> GetModulesByCourse(Guid courseId)
    {
        var modules = await _context.Modules
            .Where(m => m.CourseId == courseId)
            .OrderBy(m => m.Order)
            .Select(m => new ModuleDto
            {
                Id = m.Id,
                CourseId = m.CourseId,
                Title = m.Title,
                Description = m.Description,
                Order = m.Order
            })
            .ToListAsync();

        return Ok(modules);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ModuleDto>> GetModule(Guid id)
    {
        var module = await _context.Modules.FindAsync(id);
        if (module == null)
            return NotFound();

        return Ok(new ModuleDto
        {
            Id = module.Id,
            CourseId = module.CourseId,
            Title = module.Title,
            Description = module.Description,
            Order = module.Order
        });
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<ModuleDto>> CreateModule([FromBody] CreateModuleDto createModuleDto)
    {
        var course = await _context.Courses.FindAsync(createModuleDto.CourseId);
        if (course == null)
            return NotFound("Curso não encontrado");

        var module = new Module
        {
            CourseId = createModuleDto.CourseId,
            Title = createModuleDto.Title,
            Description = createModuleDto.Description,
            Order = createModuleDto.Order
        };

        _context.Modules.Add(module);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetModule), new { id = module.Id }, new ModuleDto
        {
            Id = module.Id,
            CourseId = module.CourseId,
            Title = module.Title,
            Description = module.Description,
            Order = module.Order
        });
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateModule(Guid id, [FromBody] CreateModuleDto updateModuleDto)
    {
        var module = await _context.Modules.FindAsync(id);
        if (module == null)
            return NotFound();

        module.Title = updateModuleDto.Title;
        module.Description = updateModuleDto.Description;
        module.Order = updateModuleDto.Order;
        module.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteModule(Guid id)
    {
        var module = await _context.Modules.FindAsync(id);
        if (module == null)
            return NotFound();

        _context.Modules.Remove(module);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
