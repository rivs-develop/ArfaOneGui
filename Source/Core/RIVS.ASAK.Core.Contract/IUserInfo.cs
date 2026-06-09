using System;

namespace RIVS.ASAK.Core.Contract
{
    public interface IUserInfo
    {
        /// <summary>
        /// Имя пользователя (ФИО).
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Логин
        /// </summary>
        string Login { get; }

        /// <summary>
        /// Идентификатор пользователя.
        /// </summary>
        Guid UserId { get; }

        /// <summary>
        /// Роль.
        /// </summary>
        Guid RoleId { get; }

        void SetPassHash(string passHash);
    }
}
