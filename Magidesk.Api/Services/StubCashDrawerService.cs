using System.Threading.Tasks;
using Magidesk.Application.Interfaces;

namespace Magidesk.Api.Services;

public class StubCashDrawerService : ICashDrawerService
{
    public Task OpenDrawerAsync(string printerName)
    {
        return Task.CompletedTask;
    }
}
