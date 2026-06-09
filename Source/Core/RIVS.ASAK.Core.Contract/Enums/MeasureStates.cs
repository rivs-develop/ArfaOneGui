using System.ComponentModel;

namespace RIVS.ASAK.Core.Contract.Enums
{
    /// <summary>
    /// Состояния сущностей
    /// </summary>
    public enum MeasureStates
    {
        /// <summary>
        /// Корректное
        /// </summary>
        [Description("Корректное")]
        Correct = 0,

        /// <summary>
        /// Пусто
        /// </summary>
        [Description("Пусто")]
        Empty = 1,

        /// <summary>
        /// Вода
        /// </summary>
        [Description("Вода")]
        Water = 2,

        /// <summary>
        /// Отклонение по критерию
        /// </summary>
        [Description("Отклонение по критерию")]
        DeviationByCriterion = 3,

        /// <summary>
        /// Меньше нижней по интенсивности
        /// </summary>
        [Description("Меньше нижней по интенсивности")]
        LessThanLowerByIntensity = 4,

        /// <summary>
        /// Больше верхней по интенсивности
        /// </summary>
        [Description("Больше верхней по интенсивности")]
        MoreThanUpperByIntensity = 5,

        /// <summary>
        /// Меньше нижней по содержанию (концентрации)
        /// </summary>
        [Description("Меньше нижней по содержанию")]
        LessThanLowerByContent = 6,

        /// <summary>
        /// Больше верхней по содержанию (концентрации)
        /// </summary>
        [Description("Больше верхней по содержанию")]
        MoreThanUpperByContent = 7,

        /// <summary>
        /// Отклонение от предыдущего вниз
        /// </summary>
        [Description("Отклонение от предыдущего вниз")]
        DeviationFromPreviousDown = 8,

        /// <summary>
        /// Отклонение от предыдущего вверх
        /// </summary>
        [Description("Отклонение от предыдущего вверх")]
        DeviationFromPreviousUp = 9,

        /// <summary>
        /// Отклонение от опорного вниз
        /// </summary>
        [Description("Отклонение от опорного вниз")]
        DeviationFromReferenceDown = 10,

        /// <summary>
        /// Отклонение от опорного вверх
        /// </summary>
        [Description("Отклонение от опорного вверх")]
        DeviationFromReferenceUp = 11,

        /// <summary>
        /// Измерение не действительно
        /// </summary>
        [Description("Измерение не действительно")]
        InvalidMeasurement = 255
    }
}
