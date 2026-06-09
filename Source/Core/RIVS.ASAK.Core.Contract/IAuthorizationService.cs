using System;
using System.Threading.Tasks;
using RIVS.ASAK.Core.Contract.DTO;

namespace RIVS.ASAK.Core.Contract
{
    public interface IAuthorizationService
    {
        /// <summary>
        /// Проверка того, что пользователь был авторизован в системе 
        /// </summary>
        /// <returns></returns>
        bool IsUserAuthorized();

        /// <summary>
        /// Получение логина пользователя, авторизовавшегося в системе
        /// </summary>
        /// <returns></returns>
        string GetCurrentUserLogin();

        /// <summary>
        /// Проверка, что пользователь авторизовался как Разработчик (Developer)
        /// </summary>
        /// <returns></returns>
        bool IsAuthorizedAsDeveloper();

        /// <summary>
        /// Текущий пользователь (надо посмотреть - надо ли наружу давать интерфейс).
        /// </summary>
        IUserInfo CurrentUser { get; }

        /// <summary>
        /// Событие о добавлении пользователя.
        /// </summary>
        event EventHandler CurrentUserChanged;

        /// <summary>
        /// Авторизация
        /// </summary>
        /// <param name="login"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<AuthorizationResult> AuthorizationAsync(string login, string password);

        /// <summary>
        /// Авторизация
        /// </summary>
        /// <param name="login"></param>
        /// <param name="password"></param>
        /// <param name="actionId"></param>
        /// <param name="objectId"></param>
        /// <returns></returns>
        Task<AuthorizationResult> AuthorizationAsync(string login, string password, Guid actionId, Guid objectId);

        /// <summary>
        /// Запрос разрешений для текущего пользователя
        /// </summary>
        /// <param name="actionId"></param>
        /// <returns></returns>
        Task<AuthorizationResult> GetPermissionCurrentUserAsync(Guid actionId);

        /// <summary>
        /// Запрос разрешений
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="actionId"></param>
        /// <param name="objectId"></param>
        /// <returns></returns>
        Task<AuthorizationResult> GetPermissionAsync(Guid userId, Guid actionId, Guid objectId);


    }

    
}
