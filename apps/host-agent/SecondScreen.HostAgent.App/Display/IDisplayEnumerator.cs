using System.Collections.Generic;
using System.Threading.Tasks;

namespace SecondScreen.HostAgent.App.Display.DisplayEnumeration
{
    public interface IDisplayEnumerator
    {
        Task<IReadOnlyList<DisplayInfo>> GetDisplaysAsync();
    }
}