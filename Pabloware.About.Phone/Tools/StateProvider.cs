using System.Collections.Generic;

namespace Pabloware.About.Tools
{
    public interface StateProvider
    {
        IDictionary<string, object> State { get; }
        bool IsOpened { get; }
    }
}
