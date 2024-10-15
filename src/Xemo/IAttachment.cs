using System.IO;
using System.Threading.Tasks;

namespace Xemo;

public interface IAttachment
{
    ValueTask<TFormat> Grow<TFormat>(IMorph<Stream, TFormat> morph);
    ValueTask<IAttachment> Infuse(IPatch<Stream> patch);
}