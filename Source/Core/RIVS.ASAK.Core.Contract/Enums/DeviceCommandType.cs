namespace RIVS.ASAK.Core.Contract.Enums
{
    public enum DeviceCmdTypeInCyclogram
    {
        /// <summary>
        /// Обычная команда старт (измерение для кювета)
        /// </summary>
        Start = 1,

        /// <summary>
        /// Команда измерения репера 1
        /// </summary>
        MeasureRep1,

        /// <summary>
        /// Команда измерения репера 2
        /// </summary>
        MeasureRep2,

        /// <summary>
        /// Команда GoHome
        /// </summary>
        GoHome
    }
}
