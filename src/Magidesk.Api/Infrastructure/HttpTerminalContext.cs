using Magidesk.Application.Interfaces;

namespace Magidesk.Api.Infrastructure;

public class HttpTerminalContext : ITerminalContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private string? _terminalIdentity;
    private Guid? _terminalId;

    public HttpTerminalContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? TerminalIdentity
    {
        get
        {
            if (_terminalIdentity != null) return _terminalIdentity;
            
            var header = _httpContextAccessor.HttpContext?.Request.Headers["X-Terminal-Name"].FirstOrDefault();
            return header ?? "WPA-Web-Client"; // Default fallback
        }
    }

    public Guid? TerminalId
    {
        get
        {
             if (_terminalId.HasValue) return _terminalId;

             var header = _httpContextAccessor.HttpContext?.Request.Headers["X-Terminal-Id"].FirstOrDefault();
             if (Guid.TryParse(header, out var guid))
             {
                 return guid;
             }

             // Fallback or Null. 
             // IF strictly required, returning null might cause issues in handlers that assume valid terminal.
             // Returning a 'Web Terminal' GUID constant might be safer if registered in DB.
             // For now, return null & let handler validate.
             return null;
        }
    }

    public void SetTerminalIdentity(string terminalIdentity, Guid terminalId)
    {
        // For testing or manual override during request processing
        _terminalIdentity = terminalIdentity;
        _terminalId = terminalId;
    }
}
