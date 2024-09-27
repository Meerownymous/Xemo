using System.Threading.Tasks;

namespace Xemo;

public interface IPatch<TContent>
{
    Task<TContent> Patch(TContent content);
}