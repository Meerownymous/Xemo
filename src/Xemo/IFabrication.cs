using System.Threading.Tasks;

namespace Xemo;

public interface IFabrication<in TContent, TShape>
{
    ValueTask<TShape> Fabricate(TContent content);
}