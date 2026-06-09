using System;

namespace RIVS.ASAK.Core.Contract.DTO
{
    public class CyclogramDeviceTimingDTO
    {
        /// <summary>
        /// Идентификатор контура.
        /// </summary>
        public int ContourId { get; set; }

        /// <summary>
        /// GUID контура.
        /// </summary>
        public Guid ContourGuid { get; set; }

        /// <summary>
        /// Идентификатор циклограммы.
        /// </summary>
        public int CyclogramId { get; set; }

        /// <summary>
        /// GUID циклограммы.
        /// </summary>
        public Guid CyclogramGuid { get; set; }

        /// <summary>
        /// ID типа устройства (из device_type).
        /// Например, 1 для ARFA, 2 для AKCP, 3 для AKP, 4 для APK.
        /// </summary>
        public int DeviceTypeId { get; set; }

        /// <summary>
        /// Время запуска устройства в циклограмме.
        /// </summary>
        public int StartTime { get; set; }

        /// <summary>
        /// GUID устройства (из device_contour и device).
        /// </summary>
        public Guid DeviceGuid { get; set; }

        /// <summary>
        /// Номер линии, связанной с устройством и контуром.
        /// </summary>
        public int Line { get; set; }

        /// <summary>
        /// Имя устройства (из device).
        /// Например, "АКП №1", "АПК №2".
        /// </summary>
        public string DeviceName { get; set; } = string.Empty;

    }
}