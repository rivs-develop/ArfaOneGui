using System.Threading.Tasks;
using RIVS.ASAK.Core.Contract.DTO;

namespace RIVS.ASAK.Core.Contract
{
    public interface ISendEventService
    {
        Task Send(EventBusMessage ev);
    }
}
