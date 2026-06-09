using System.Threading;
using System.Threading.Tasks;

namespace RIVS.ASAK.ARFA.GUI.Services
{
    public interface IApplicationStateService
    {
        Task<bool> StartArfaAsync(CancellationToken ct = default);
        Task<bool> StopArfaAsync(CancellationToken ct = default);
    }
}
