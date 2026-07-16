using Microsoft.AspNetCore.Mvc;
using VulkanosAcademy.Api.Services;
using VulkanosAcademy.Domain.DTOs;

namespace VulkanosAcademy.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginDto loginDto)
    {
        var result = await _authService.LoginAsync(loginDto);
        if (result == null)
            return Unauthorized("Email ou senha inválidos");

        return Ok(result);
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register([FromBody] CreateUserDto createUserDto)
    {
        var result = await _authService.RegisterAsync(createUserDto);
        if (result == null)
            return BadRequest("Email já registrado");

        return CreatedAtAction(nameof(Register), result);
    }
}
