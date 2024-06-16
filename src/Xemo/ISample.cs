using System;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Xemo
{
    public interface ISample<TSample>
    {
        ICocoon Origin();
        TSample Content();
    }
}

