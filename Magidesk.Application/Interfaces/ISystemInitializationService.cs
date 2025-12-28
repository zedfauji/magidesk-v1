using System.Threading.Tasks;

namespace Magidesk.Application.Interfaces
{
    public interface ISystemInitializationService
    {
        Task<InitializationResult> InitializeSystemAsync();
    }

    public class InitializationResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public string? TerminalId { get; set; }

        public static InitializationResult Success(string terminalId) => new() { IsSuccess = true, TerminalId = terminalId };
        public static InitializationResult Failure(string message) => new() { IsSuccess = false, Message = message };
    }
}
