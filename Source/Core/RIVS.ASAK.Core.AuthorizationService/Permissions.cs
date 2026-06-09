using System;

namespace RIVS.ASAK.Core.AuthorizationService
{
    /// <summary>
    /// Список разрешений.
    /// </summary>
    public static class Permissions
    {

        #region Данные о пользователях и ролях

        // Вопрос о системном пользователе/роли пока оставил открытым
        // Когда запускается сервис, производятся какие-то действия,
        // то от имени системного пользователя 

        /// <summary>
        /// Идентификатор системного пользователя.
        /// </summary>
        public static readonly Guid SystemUserID = new Guid("9C08104D-E218-4915-8ACE-DE6EAF4AF1CC");

        /// <summary>
        /// Идентификатор роли системного пользователя.
        /// </summary>
        public static readonly Guid SystemRoleID = new Guid("B1C23D21-5509-4FB2-98A0-1A6732A7DF93");

        /// <summary>
        /// Идентификатор разработчика.
        /// </summary>
        public static readonly Guid DeveloperUserID = new Guid("D13D446B-A433-458E-9415-6DAC7D84A2F5");

        /// <summary>
        /// Идентификатор роли разработчика.
        /// </summary>
        public static readonly Guid DeveloperRoleID = new Guid("E69428C5-FE6D-460F-9D8F-2D9D76476D3C");

        #endregion

        #region Функциональные права

        /// <summary>
        /// Авторизация.
        /// </summary>
        public static readonly Guid Authorization = new Guid("D170A7AC-E826-4BA9-90D2-4DA338644262");

        /// <summary>
        /// Старт/стоп части системы (АРФА1-АРФА4, ЦСУ, Диспетчер).
        /// </summary>
        public static readonly Guid SystemOnOff = new Guid("F5BBF113-98D6-413E-BEB7-80756975684D");

        /// <summary>
        /// Переключение режима работы ручной/авто
        /// </summary>
        public static readonly Guid SwitchModeManualAuto = new Guid("A92F8A05-7208-466A-AB4B-7FC6F130DA69");

        /// <summary>
        /// Старт/стоп циклограммы
        /// </summary>
        public static readonly Guid CyclogramStartStop = new Guid("7D99189B-C269-4E02-87E1-CA251A33E039");

        /// <summary>
        /// Изменение количества отсечек
        /// </summary>
        public static readonly Guid ChangeCutoffs = new Guid("75A3A44C-69D4-11EF-962D-9009D03822CF");

        /// <summary>
        /// Доступ к изменению параметров
        /// </summary>
        public static readonly Guid ChangeConfig = new Guid("7482EF88-577A-471D-987F-ECF6F03D57D2");


        #endregion
    }
}
