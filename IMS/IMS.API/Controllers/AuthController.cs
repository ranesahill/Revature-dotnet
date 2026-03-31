using IMS.App.DTOs;
using IMS.Core.Entities;
using IMS.Core.Enums;
using IMS.Core.Interfaces;
using IMS.App.Services;
using Microsoft.AspNetCore.Mvc;

namespace IMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserRepository _userRepo;
    private readonly IJwtService _jwtService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IUserRepository userRepo,
        IJwtService jwtService,
        ILogger<AuthController> logger)
    {
        _userRepo = userRepo;
        _jwtService = jwtService;
        _logger = logger;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto dto)
    {
        // Check if username already exists
        var existingUser = await _userRepo.GetByUsernameAsync(dto.Username);
        if (existingUser != null)
        {
            _logger.LogWarning("Registration failed: username '{Username}' already exists.", dto.Username);
            return Conflict(new { message = "Username already exists." });
        }

        // Parse the role
        if (!Enum.TryParse<UserRole>(dto.Role, ignoreCase: true, out var role))
        {
            return BadRequest(new { message = $"Invalid role '{dto.Role}'. Valid roles: FinanceUser, FinanceManager, Admin." });
        }

        // Create new user with hashed password
        var user = new User
        {
            Username = dto.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = role,
            CreatedDate = DateTime.UtcNow
        };

        await _userRepo.AddAsync(user);
        await _userRepo.SaveChangesAsync();

        // Generate JWT token
        var token = _jwtService.GenerateToken(user);

        _logger.LogInformation("User '{Username}' registered successfully with role '{Role}'.", user.Username, user.Role);

        return CreatedAtAction(nameof(Register), new AuthResponseDto
        {
            Token = token,
            Username = user.Username,
            Role = user.Role.ToString(),
            Expiration = DateTime.UtcNow.AddHours(8)
        });
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto dto)
    {
        var user = await _userRepo.GetByUsernameAsync(dto.Username);
        if (user == null)
        {
            _logger.LogWarning("Login failed: user '{Username}' not found.", dto.Username);
            return Unauthorized(new { message = "Invalid username or password." });
        }

        // Verify BCrypt hash
        bool validPassword = false;
        try
        {
            validPassword = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
        }
        catch
        {
            validPassword = false;
        }

        if (!validPassword)
        {
            _logger.LogWarning("Login failed: invalid password for user '{Username}'.", dto.Username);
            return Unauthorized(new { message = "Invalid username or password." });
        }

        var token = _jwtService.GenerateToken(user);

        _logger.LogInformation("User '{Username}' logged in successfully.", dto.Username);

        return Ok(new AuthResponseDto
        {
            Token = token,
            Username = user.Username,
            Role = user.Role.ToString(),
            Expiration = DateTime.UtcNow.AddHours(8)
        });
    }
}
