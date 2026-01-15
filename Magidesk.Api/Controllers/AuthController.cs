using Magidesk.Api.Dtos.Auth;
using Magidesk.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Magidesk.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly ISecurityService _securityService;
    // Context services would be injected here
    
    public AuthController(ISecurityService securityService)
    {
        _securityService = securityService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login([FromBody] LoginRequest request)
    {
        // 1. Authenticate via Service (Direct Map)
        // Note: Real implementation would handle encryption/hashing here or in service
        var user = await _securityService.GetUserByPinAsync(request.Pin);

        if (user == null)
        {
            return Unauthorized();
        }

        // 2. Map Domain User to DTO
        return Ok(new UserDto
        {
            Id = user.Id.Value.ToString(),
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
            TerminalId = "resolved-terminal-id",
            StartedAt = DateTime.UtcNow.ToString("O")
        });
    }
}
