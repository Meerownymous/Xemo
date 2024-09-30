using System;
using System.Threading.Tasks;

namespace Xemo.Fabing;

/// <summary>
///     Fab func as Fabing.
/// </summary>
public sealed class AsFabrication<TContent, TShape>(Func<TContent, Task<TShape>> Fab) :
    IFabrication<TContent, TShape>
{
    public ValueTask<TShape> Fabricate(TContent content)
    {
        return new ValueTask<TShape>(Fab(content));
    }
}