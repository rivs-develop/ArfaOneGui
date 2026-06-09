using System;
using RIVS.ASAK.Core.Contract;

namespace RIVS.ASAK.Core.AuthorizationService
{
    /// <summary>
    /// Класс аргументов события о добавлении нового пользователя.
    /// </summary>
    public class UserChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Новый пользователь.
        /// </summary>
        public IUserInfo User { get; private set; }

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="newUser">Новый пользователь.</param>
        public UserChangedEventArgs(IUserInfo newUser)
        {
            User = newUser;
        }
    }
}
