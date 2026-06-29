using System.Collections.Generic;
using System.Threading.Tasks;

namespace HostAgent.Display.DisplayEnumeration
{
    public interface IDisplayEnumerator
    {
        Task<IReadOnlyList<DisplayInfo>> GetDisplaysAsync();
    }
}