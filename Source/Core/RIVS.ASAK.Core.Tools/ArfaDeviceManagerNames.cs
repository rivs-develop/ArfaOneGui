namespace RIVS.ASAK.Core.Tools
{
    /// <summary>
    /// Класс имен Xml элементов для создания устройств, входящих в АРФА
    /// </summary>
    public static class ArfaDeviceManagerNames
    {
        /// <summary>
        /// XML элементы. 
        /// </summary>
        public static class Elements
        {
            /// <summary>
            /// Корневой элемент.
            /// </summary>
            public const string Root = "ArfaDeviceManager";
            /// <summary>
            /// Трубка.
            /// </summary>
            public const string XRayTube = "RIVS.ASAK.ARFA.ArfaDeviceManager.SpectrometricDevices.XRay, RIVS.ASAK.ARFA.SpectrometricDevices";
            /// <summary>
            /// Детектор.
            /// </summary>
            public const string Detector = "RIVS.ASAK.ARFA.DeviceManager.SpectrometricDevices.Detector, RIVS.ASAK.ARFA.SpectrometricDevices";

            /// <summary>
            /// Привод
            /// </summary>
            public const string Motor = "RIVS.ASAK.ARFA.CVE.CveController,RIVS.ASAK.ARFA.Cve";
            //public const string Motor = "RIVS.ASAK.ARFA.DeviceManager.SpectrometricDevices.Motor, RIVS.ASAK.ARFA.SpectrometricDevices";

        }

        /// <summary>
        /// XML Свойства. 
        /// </summary>
        public static class Properties
        {
            /// <summary>
            /// Имя.
            /// </summary>
            public const string Name = "Name";
            /// <summary>
            /// ID устройства.
            /// </summary>
            public const string DeviceId = "Id";

            /// <summary>
            /// ID драйвера.
            /// </summary>
            public const string DriverId = "DriverId";
            /// <summary>
            /// IP адрес.
            /// </summary>
            public const string IpAddress = "IpAddress";
            /// <summary>
            /// Порт.
            /// </summary>
            public const string Port = "Port";

            public const string Voltage = "Voltage";
            public const string Current = "Current";

        }
    }
}
