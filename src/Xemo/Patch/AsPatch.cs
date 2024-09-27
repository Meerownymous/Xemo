using System;
using System.Threading.Tasks;

namespace Xemo.Patch;

/// <summary>
///     Func as patch.
/// </summary>
public sealed class AsPatch<TContent>(Func<TContent, TContent> patch) : IPatch<TContent>
{
    public async Task<TContent> Patch(TContent content)
    {
        return await Task.Run(() => patch(content));
    }
}