using System.Threading;
using System.Threading.Tasks;
using Prism.Events;

namespace RIVS.ASAK.ARFA.GUI.Services
{
    public class ApplicationStateService : IApplicationStateService
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IRestCommandService _restCommandService;
        public ApplicationStateService(
            IEventAggregator eventAggregator,
            IRestCommandService restCommandService
        )
        {
            _eventAggregator = eventAggregator;
            _restCommandService = restCommandService;
        }

        public async Task<bool> StartArfaAsync(CancellationToken ct)
        {
           return await _restCommandService.StartArfaAsync(ct);
        }

        public async Task<bool> StopArfaAsync(CancellationToken ct)
        {
            return await _restCommandService.StopArfaAsync(ct);
        }
    }
}
