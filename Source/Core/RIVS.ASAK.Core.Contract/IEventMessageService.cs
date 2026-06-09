using System.Threading.Tasks;

namespace RIVS.ASAK.Core.Contract
{
    public interface IEventMessageService
    {
        Task SendAsync<T>(T message);
    }
}
