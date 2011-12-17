using System.Collections.Generic;

namespace Dietphone.Tools
{
    public interface StateProvider
    {
        IDictionary<string, object> State { get; }
        bool IsOpened { get; }
    }
}
