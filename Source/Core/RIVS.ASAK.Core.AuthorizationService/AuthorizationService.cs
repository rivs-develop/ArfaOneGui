using System;
using System.Threading.Tasks;
using RIVS.ASAK.Core.Contract;
using RIVS.ASAK.Core.Contract.DTO;
using RIVS.ASAK.Core.Tools.Strings;

namespace RIVS.ASAK.Core.AuthorizationService
{
    public class AuthorizationService : IAuthorizationService
    {

        private readonly ILogger _logger;
        private readonly IAuthenticationProvider _authenticationProvider;

        // блокировка
        private readonly object _lock = new object();
        private IUserInfo _currentUser;

        /// <summary>
        /// Максимальное время ожидания завершения задачи запроса инфы по авторизации/разрешениям 
        /// </summary>
        private readonly TimeSpan _taskFinishedTimeOut = new TimeSpan(0, 0, 0, 10);

        public AuthorizationService(ILoggerFactory loggerFactory,
            IAuthenticationProvider authenticationProvider)
        {
            _currentUser = new UserInfo();

            _logger = loggerFactory?.Create();
            _authenticationProvider = authenticationProvider;
        }

        #region Command Processing

        ///// <summary>
        ///// Команда на авторизацию
        ///// </summary>
        ///// <param name="command"></param>
        ///// <param name="confirmationdata"></param>
        //private void OnAuthorization(Command command, ref object confirmationdata)
        //{
        //    if (command == null || command.CommandData == null)
        //    {
        //        confirmationdata = new ErrorResult(
        //            new ApplicationException("AuthorizationService.OnAuthorization: Нет необходимых данных для команды"));
        //        return;
        //    }

        //    if (_authenticationProvider == null)
        //    {
        //        confirmationdata = new ErrorResult(
        //            new Exception("AuthorizationService.OnAuthorization: AuthenticationProvider == null"));
        //        return;
        //    }

        //    try
        //    {
        //        if (command.CommandData is not AuthorizationDataCommand dataCommand)
        //        {
        //            confirmationdata = new ErrorResult(new Exception("AuthorizationService.OnAuthorizationFull: command.CommandData is not AuthorizationDataCommand"));
        //            return;
        //        }
        //        var task = Task.Run(() => AuthorizationInternal(dataCommand.Login, dataCommand.Password));
        //        var taskResult = Task.WhenAny(task, Task.Delay(_taskFinishedTimeOut));
        //        if (taskResult.IsCompleted && taskResult.Result != null)
        //        {
        //            confirmationdata = task.Result;
        //        }
        //        else
        //        {
        //            confirmationdata = new ErrorResult(new Exception("AuthorizationService.OnAuthorization: Timeout"));
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        confirmationdata = new ErrorResult(e);
        //    }
        //}

        //private void OnAuthorizationFull(Command command, ref object confirmationdata)
        //{
        //    if (command == null || command.CommandData == null)
        //    {
        //        confirmationdata = new ErrorResult(
        //            new ApplicationException("AuthorizationService.OnAuthorizationFull: Нет необходимых данных для команды"));
        //        return;
        //    }

        //    if (_authenticationProvider == null)
        //    {
        //        confirmationdata = new ErrorResult(
        //            new Exception("AuthorizationService.OnAuthorizationFull: AuthenticationProvider == null"));
        //        return;
        //    }

        //    try
        //    {
        //        if (command.CommandData is not AuthorizationFullDataCommand dataCommand)
        //        {
        //            confirmationdata = new ErrorResult(new Exception("AuthorizationService.OnAuthorizationFull: command.CommandData is not AuthorizationFullDataCommand"));
        //            return;
        //        }
        //        var task = Task.Run(() => AuthorizationFullInternal(dataCommand.Login, dataCommand.Password, dataCommand.ActionId, dataCommand.ObjectId));
        //        var taskResult = Task.WhenAny(task, Task.Delay(_taskFinishedTimeOut));
        //        if (taskResult.IsCompleted && taskResult.Result != null)
        //        {
        //            confirmationdata = task.Result;
        //        }
        //        else
        //        {
        //            confirmationdata = new ErrorResult(new Exception("AuthorizationService.OnAuthorizationFull: Timeout"));
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        confirmationdata = new ErrorResult(e);
        //    }
        //}

        //private void OnGetPermission(Command command, ref object confirmationdata)
        //{
        //    if (command == null || command.CommandData == null)
        //    {
        //        confirmationdata = new ErrorResult(
        //            new ApplicationException("AuthorizationService.OnGetPermission: Нет необходимых данных для команды"));
        //        return;
        //    }

        //    if (_authenticationProvider == null)
        //    {
        //        confirmationdata = new ErrorResult(
        //            new Exception("AuthorizationService.OnGetPermission: AuthenticationProvider == null"));
        //        return;
        //    }

        //    try
        //    {
        //        if (command.CommandData is not PermissionDataCommand dataCommand)
        //        {
        //            confirmationdata = new ErrorResult(new Exception("AuthorizationService.OnGetPermission: command.CommandData is not PermissionDataCommand"));
        //            return;
        //        }
        //        var task = Task.Run(() => GetPermissionInternal(dataCommand.ActionId));
        //        var taskResult = Task.WhenAny(task, Task.Delay(_taskFinishedTimeOut));
        //        if (taskResult.IsCompleted && taskResult.Result != null)
        //        {
        //            confirmationdata = task.Result;
        //        }
        //        else
        //        {
        //            confirmationdata = new ErrorResult(new Exception("AuthorizationService.OnGetPermission: Timeout"));
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        confirmationdata = new ErrorResult(e);
        //    }
        //}

        //private void OnGetPermissionFull(Command command, ref object confirmationdata)
        //{
        //    if (command == null || command.CommandData == null)
        //    {
        //        confirmationdata = new ErrorResult(
        //            new ApplicationException("AuthorizationService.OnGetPermissionFull: Нет необходимых данных для команды"));
        //        return;
        //    }

        //    if (_authenticationProvider == null)
        //    {
        //        confirmationdata = new ErrorResult(
        //            new Exception("AuthorizationService.OnGetPermissionFull: AuthenticationProvider == null"));
        //        return;
        //    }

        //    try
        //    {
        //        if (command.CommandData is not PermissionFullDataCommand dataCommand)
        //        {
        //            confirmationdata = new ErrorResult(new Exception("AuthorizationService.OnGetPermissionFull: command.CommandData is not PermissionFullDataCommand"));
        //            return;
        //        }
        //        var task = Task.Run(() => GetPermissionFullInternal(dataCommand.UserId, dataCommand.ActionId, dataCommand.ObjectId));
        //        var taskResult = Task.WhenAny(task, Task.Delay(_taskFinishedTimeOut));
        //        if (taskResult.IsCompleted && taskResult.Result != null)
        //        {
        //            confirmationdata = task.Result;
        //        }
        //        else
        //        {
        //            confirmationdata = new ErrorResult(new Exception("AuthorizationService.OnGetPermissionFull: Timeout"));
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        confirmationdata = new ErrorResult(e);
        //    }
        //}


        #endregion Command Processing

        #region IAuthorizationService Members

        public async Task<AuthorizationResult> AuthorizationAsync(string login, string password)
        {
            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                _logger.Error("AuthorizationService.AuthorizationAsync: Нет необходимых данных для команды");
                return null;
            }
            return await AuthorizationInternal(login, password);
        }

        public async Task<AuthorizationResult> AuthorizationAsync(string login, string password, Guid actionId, Guid objectId)
        {
            if (string.IsNullOrEmpty(login) 
                || string.IsNullOrEmpty(password)
                || actionId == Guid.Empty
                || objectId == Guid.Empty)
            {
                _logger.Error("AuthorizationService.AuthorizationAsync: Нет необходимых данных для команды");
                return null;
            }
            return await AuthorizationFullInternal(login, password, actionId, objectId);
        }

        public async Task<AuthorizationResult> GetPermissionCurrentUserAsync(Guid actionId)
        {
            if (actionId == Guid.Empty)
            {
                _logger.Error("AuthorizationService.GetPermissionCurrentUserAsync: Нет необходимых данных для команды");
                return null;
            }
            return await GetPermissionInternal(actionId);
        }

        public async Task<AuthorizationResult> GetPermissionAsync(Guid userId, Guid actionId, Guid objectId)
        {
            if (userId == Guid.Empty
                || actionId == Guid.Empty
                || objectId == Guid.Empty)
            {
                _logger.Error("AuthorizationService.GetPermissionAsync: Нет необходимых данных для команды");
                return null;
            }
            return await GetPermissionFullInternal(userId, actionId, objectId);
        }

        public bool IsUserAuthorized()
        {
            IUserInfo checkUser = null;
            if (CurrentUser != null)
            {
                checkUser = CurrentUser;
            }

            return checkUser != null;
        }

        public string GetCurrentUserLogin()
        {
            IUserInfo checkUser = null;
            if (CurrentUser != null)
            {
                checkUser = CurrentUser;
            }

            return checkUser != null ? checkUser.Login : null;
        }

        public bool IsAuthorizedAsDeveloper()
        {
            IUserInfo checkUser = null;
            if (CurrentUser != null)
            {
                checkUser = CurrentUser;
            }

            return checkUser != null && ((UserInfo)checkUser).IsAuthorizedAsDeveloper();
        }

        /// <summary>
        /// Текущий пользователь.
        /// </summary>
        public IUserInfo CurrentUser
        {
            get
            {
                lock (_lock)
                {
                    return _currentUser;
                }
            }
            private set
            {
                //т.к. режима перелогина нет - не учитываем, что были уже какие-то данные
                lock (_lock)
                {
                    _currentUser = value;
                }
                OnCurrentUserChanged(value);
            }
        }

        public event EventHandler CurrentUserChanged;

        #endregion

        /// <summary>
        /// Метод вызова события о добавлении нового пользователя.
        /// </summary>
        /// <param name="value">Новый пользователь.</param>
        private void OnCurrentUserChanged(IUserInfo value)
        {
            CurrentUserChanged?.Invoke(this, new UserChangedEventArgs(value));
        }

        /// <summary>
        /// Идентификация пользователя.
        /// </summary>
        /// <param name="login">Логин пользователя.</param>
        /// <param name="passwordHash">Хэш пароля пользователя.</param>
        /// <param name="userId"></param>
        /// <param name="roleId"></param>
        /// <param name="fullName"></param>
        /// <returns>Возвращает булевый результат проверки.</returns>
        private void SetIdentificationInfo(string login, string passwordHash, Guid userId, Guid roleId, string fullName)
        {
            if (!string.IsNullOrEmpty(login))
            {
                IUserInfo loginUser = _currentUser;
                ((UserInfo)loginUser).SetIdentificationInfo(login, passwordHash, userId, roleId, fullName);
                CurrentUser = loginUser;
            }
        }

        /// <summary>
        /// Метод логина под системным пользователем.
        /// Оставлен до лучших времен.
        /// </summary>
        public void LoginAsSystem()
        {
            CurrentUser = new UserInfo()
            {
                Login = "System",
                Name = "system",
                UserId = Permissions.SystemUserID,
                RoleId = Permissions.SystemRoleID,
            };
        }

        /// <summary>
        /// На всякий случай, если вдруг понадобится "обнулить" инфу о пользователе
        /// </summary>
        internal void CloseUserSession()
        {
            CurrentUser = null;
        }

        /// <summary>
        /// Авторизация.
        /// Внутренний метод, для вызова метода провайдера
        /// </summary>
        /// <param name="login"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        private async Task<AuthorizationResult> AuthorizationInternal(string login, string password)
        {
            if (_authenticationProvider == null)
            {
                _logger.Error("AuthorizationService.AuthorizationAsync: AuthenticationProvider == null");
                return null;
            }
            var passwordHash = CryptoProvider.ComputeHashHex(password);
            var result = await _authenticationProvider.AuthorizationAsync(login, password);
            if (result == null)
            {
                return null;
            }

            if (result.Result == 0 && result.UserId != null && result.RoleId != null)
            {
                //перезаписываем в любом случае
                SetIdentificationInfo(login, passwordHash, result.UserId.Value, result.RoleId.Value, result.FullName);
            }

            var authorizationResult = new AuthorizationResult { Result = result.Result, Message = result.Message };
            return authorizationResult;
        }

        /// <summary>
        /// Авторизация.
        /// Внутренний метод, для вызова метода провайдера
        /// </summary>
        /// <param name="login"></param>
        /// <param name="password"></param>
        /// <param name="actionId"></param>
        /// <param name="objectId"></param>
        /// <returns></returns>
        private async Task<AuthorizationResult> AuthorizationFullInternal(string login, string password, Guid actionId, Guid objectId)
        {
            if (_authenticationProvider == null)
            {
                _logger.Error("AuthorizationService.AuthorizationAsync: AuthenticationProvider == null");
                return null;
            }
            var passwordHash = CryptoProvider.ComputeHashHex(password);
            var result = await _authenticationProvider.AuthorizationAsync(login, password, actionId, objectId);
            if (result == null)
            {
                return null;
            }

            if (result.Result == 0 && result.UserId != null && result.RoleId != null)
            {
                //перезаписываем в любом случае
                SetIdentificationInfo(login, passwordHash, result.UserId.Value, result.RoleId.Value, result.FullName);
            }

            var authorizationResult = new AuthorizationResult { Result = result.Result, Message = result.Message };
            return authorizationResult;
        }

        private async Task<AuthorizationResult> GetPermissionInternal(Guid actionId)
        {
            if (CurrentUser == null)
            {
                _logger.Error("AuthorizationService.GetPermissionInternal: проверка разрешений для неавторизованного пользователя");
                return new AuthorizationResult { Result = 1, Message = "Пользователь не авторизован" };
            }

            var checkUser = CurrentUser;
            var specificUser = ((UserInfo)checkUser).IsSpecificUser();
            if (specificUser)
            {
                return new AuthorizationResult { Result = 0, Message = "Доступ разрешен" };
            }

            if (_authenticationProvider == null)
            {
                _logger.Error("AuthorizationService.AuthorizationAsync: AuthenticationProvider == null");
                return null;
            }

            var result = await _authenticationProvider.GetPermissionAsync(checkUser.UserId, actionId);
            return result == null ? null : new AuthorizationResult { Result = result.Result, Message = result.Message };
        }

        private async Task<AuthorizationResult> GetPermissionFullInternal(Guid userId, Guid actionId, Guid objectId)
        {
            if (_authenticationProvider == null)
            {
                _logger.Error("AuthorizationService.AuthorizationAsync: AuthenticationProvider == null");
                return null;
            }
            var result = await _authenticationProvider.GetPermissionAsync(userId, actionId, objectId);
            return result == null ? null : new AuthorizationResult { Result = result.Result, Message = result.Message };
        }



    }
}
