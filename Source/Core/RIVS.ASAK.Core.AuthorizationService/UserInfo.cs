using System;
using System.Runtime.InteropServices;
using System.Security;
using RIVS.ASAK.Core.Contract;

namespace RIVS.ASAK.Core.AuthorizationService
{
    /// <summary>
    /// Класс-описатель пользователя.
    /// </summary>
    internal sealed class UserInfo : IUserInfo
    {
        // блокировка
        private readonly object _lock = new object();

        // Хэш пароля.
        private SecureString _passwordHash = new SecureString();

        /// <summary>
        /// Логин.
        /// </summary>
        public string Login { get; internal set; }

        /// <summary>
        /// Имя пользователя.
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Идентификатор пользователя.
        /// </summary>
        public Guid UserId { get; internal set; }

        /// <summary>
        /// Роль пользователя.
        /// </summary>
        public Guid RoleId { get; internal set; }

        /// <summary>
        /// Конструктор.
        /// </summary>
        internal UserInfo()
        {
            UserId = Guid.Empty;
            RoleId = Guid.Empty;
            Name = Login = string.Empty;
        }

        /// <summary>
        /// Проверка того, что авторизовался специфический пользователь
        /// (которого мы уважаем)
        /// </summary>
        /// <returns>Возвращает булевый результат проверки.</returns>
        public bool IsSpecificUser()
        {
            return UserId == Permissions.SystemUserID
                   || UserId == Permissions.DeveloperUserID;
        }

        /// <summary>
        /// Проверка того, что пользователь авторизован.
        /// </summary>
        /// <param name="login">Логин пользователя.</param>
        /// <param name="passwordHash">Хэш пароля пользователя.</param>
        /// <returns>Возвращает булевый результат проверки.</returns>
        public bool IsUserIdentificated(string login, string passwordHash)
        {
            return !string.IsNullOrEmpty(login) &&
                   (string.Equals(Login, login, StringComparison.OrdinalIgnoreCase) && passwordHash == GetPassHash());
        }

        /// <summary>
        /// Получение хэша пароля.
        /// </summary>
        /// <returns>Хэш пароля.</returns>
        private string GetPassHash()
        {
            string pass;
            IntPtr bstr = Marshal.SecureStringToBSTR(_passwordHash);
            try
            {
                pass = Marshal.PtrToStringBSTR(bstr);
            }
            catch
            {
                pass = string.Empty;
            }
            finally
            {
                Marshal.ZeroFreeBSTR(bstr);
            }
            return pass;
        }


        public void SetPassHash(string passHash)
        {
            _passwordHash = new SecureString();
            foreach (var ch in passHash.ToCharArray())
            {
                _passwordHash.AppendChar(ch);
            }
        }

        public bool IsAuthorizedAsDeveloper()
        {
            return UserId == Permissions.DeveloperUserID
                && RoleId == Permissions.DeveloperRoleID;
        }

        public void SetIdentificationInfo(string login, string passwordHash, Guid userId, Guid roleId, string fullName)
        {
            UserId = userId;
            RoleId = roleId;
            Login = login;
            Name = fullName;

            SetPassHash(passwordHash);
        }
    }
}
