namespace RIVS.ASAK.Core.Tools.SettingsParams
{
    /// <summary>
    /// Отладочные параметры
    /// Имя параметра соответствует имени параметра в файле
    /// </summary>
    public enum DebugParams
    {        
        /// <summary>
        /// Периодически формировать ошибку работоспособности Proxy
        /// </summary>
        [BehaviourParams(DefaultValue = false)] OccasionallyProxyFail,

        /// <summary>
        /// Пинговать proxy ip и логгировать результаты
        /// </summary>
        [BehaviourParams(DefaultValue = false)] IsNeedTestPing,

        /// <summary>
        /// Запуск RIVSASAK в режиме автотеста
        /// </summary>
        [BehaviourParams(DefaultValue = false)] Autotest,
    
    }
}
