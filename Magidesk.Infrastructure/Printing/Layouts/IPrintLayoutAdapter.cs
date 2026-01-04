using System.Threading.Tasks;

namespace Magidesk.Infrastructure.Printing.Layouts
{
    public interface IPrintLayoutAdapter
    {
        Task<string> GenerateLayoutAsync(object data);
    }
}
