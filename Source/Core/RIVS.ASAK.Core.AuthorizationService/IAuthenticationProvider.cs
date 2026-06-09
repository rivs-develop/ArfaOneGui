using System;
using System.Threading;
using System.Threading.Tasks;
using RIVS.ASAK.Core.Contract.DTO;

namespace RIVS.ASAK.Core.AuthorizationService
{
    /// <summary>
    /// Интерфейс провайдера.
    /// Используется ВНУТРИ сервиса AuthorizationService.
    /// </summary>
    public interface IAuthenticationProvider
    {
        Task<UserAccess2DescriptorDTO> AuthorizationAsync(string login, string password, CancellationToken ct = default);
        Task<UserAccess2DescriptorDTO> AuthorizationAsync(string login, string password, Guid actionId, Guid objectId, CancellationToken ct = default);
        Task<UserPermissionDTO> GetPermissionAsync(Guid userId, Guid actionId, CancellationToken ct = default);
        Task<UserPermissionDTO> GetPermissionAsync(Guid userId, Guid actionId, Guid objectId, CancellationToken ct = default);
    }
}
