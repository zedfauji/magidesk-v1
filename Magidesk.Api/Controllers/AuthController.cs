using Magidesk.Api.Dtos.Auth;
using Magidesk.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Magidesk.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly ISecurityService _securityService;
    private readonly IAesEncryptionService _encryptionService;
    
    public AuthController(ISecurityService securityService, IAesEncryptionService encryptionService)
    {
        _securityService = securityService;
        _encryptionService = encryptionService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login([FromBody] LoginRequest request)
    {
        // 1. Encrypt the incoming PIN using standard AES service to match DB storage
        var encryptedPin = _encryptionService.Encrypt(request.Pin);

        // 2. Authenticate
        var user = await _securityService.GetUserByPinAsync(encryptedPin);

        if (user == null)
        {
            return Unauthorized();
        }

        // 3. Map Domain User to DTO
        return Ok(new UserDto
        {
            Id = user.Id.ToString(),
            Username = user.Username, // Assuming property exists based on DTO
            FirstName = user.FirstName,
            LastName = user.LastName,
            Role = user.Role.Name // flattening
        });
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        // Stateless logout (client-side token removal)
        return NoContent();
    }

    [HttpGet("session")]
    public ActionResult<AuthSessionDto> GetSession()
    {
        // Gap: Requires IUserService / ITerminalContext to be plumbed to HttpContext
        // This is a placeholder for the "Context Resolution" step
        
        // if (User.Identity?.IsAuthenticated != true) return Unauthorized();
        
        // Mock return to satisfy contract signature until plumbing is done
        return Ok(new AuthSessionDto
        {
            Token = "current-token-placeholder",
            // User = ... map from context
            TerminalId = Environment.MachineName,
            StartedAt = DateTime.UtcNow.ToString("O")
        });
    }
}
