namespace RIVS.ASAK.Core.Tools.SettingsParams
{
    /// <summary>
    /// Интерфейс чтения и сохранения параметров в некотором хранилище
    /// </summary>
    interface IParamsFromStore
    {
        /// <summary>
        /// Получение значение параметра.
        /// </summary>
        /// <param name="valueName">Имя значения параметра.</param>
        /// <returns>Возвращает значение или null, если параметр не найден.</returns>
        string GetParameter(string valueName);

        /// <summary>
        /// Получение значение параметра, используемого при отладке.
        /// </summary>
        /// <param name="valueName">Имя значения параметра.</param>
        /// <returns>Возвращает значение или null, если параметр не найден.</returns>
        string GetDebugParameter(string valueName);

        /// <summary>
        /// Задание значение параметра.
        /// </summary>
        /// <param name="valueName">Имя значения параметра.</param>
        /// <param name="value">Значение параметра</param>
        void SetParameter(string valueName, object value);

        /// <summary>
        /// Задание значение отладочного параметра.
        /// </summary>
        /// <param name="valueName">Имя значения параметра.</param>
        /// <param name="value">Значение параметра</param>
        void SetDebugParameter(string valueName, object value);
    }


}