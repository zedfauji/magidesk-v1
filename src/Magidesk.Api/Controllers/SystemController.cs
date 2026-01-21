using Microsoft.AspNetCore.Mvc;
using Magidesk.Application.DTOs.SystemConfig;
using Magidesk.Application.Interfaces;

namespace Magidesk.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SystemController : ControllerBase
{
    private readonly IBackupService _backupService;
    private readonly ILogger<SystemController> _logger;

    public SystemController(IBackupService backupService, ILogger<SystemController> logger)
    {
        _backupService = backupService;
        _logger = logger;
    }

    [HttpPost("backup")]
    public async Task<IActionResult> CreateBackup()
    {
        try
        {
            var fileName = await _backupService.CreateBackupAsync();
            return Ok(new { Message = "Backup created successfully", FileName = fileName });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Backup failed");
            return StatusCode(500, new { Message = "Backup failed", Error = ex.Message });
        }
    }

    [HttpGet("backups")]
    public async Task<ActionResult<List<BackupInfoDto>>> ListBackups()
    {
        var backups = await _backupService.ListBackupsAsync();
        return Ok(backups);
    }

    [HttpPost("restore/{fileName}")]
    public async Task<IActionResult> RestoreBackup(string fileName)
    {
        // Require specific permission?
        try
        {
            await _backupService.RestoreBackupAsync(fileName);
            return Ok(new { Message = "Database restored successfully" });
        }
        catch (FileNotFoundException)
        {
            return NotFound(new { Message = "Backup file not found" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Restore failed");
            return StatusCode(500, new { Message = "Restore failed", Error = ex.Message });
        }
    }
}
