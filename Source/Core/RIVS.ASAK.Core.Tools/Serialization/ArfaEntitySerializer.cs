using System;
using System.Collections.Generic;
using System.Xml.Linq;
using RIVS.ASAK.Core.Contract.DTO;
using RIVS.ASAK.Core.Contract.Enums;

namespace RIVS.ASAK.Core.Tools.Serialization
{
    public interface IDeviceOptions
    { }

    public class TubeDeviceOptions : IDeviceOptions
    {
        public ArfaDeviceType DeviceType { get; set; }
        public ArfaDriverType DriverType { get; set; }
        public int Voltage { get; set; } = 0;
        public int Current { get; set; } = 0;
    }

    public class DetectorDeviceOptions : IDeviceOptions
    {
        public ArfaDeviceType DeviceType { get; set; }
        public ArfaDriverType DriverType { get; set; }
    }

    public class MotorDeviceOptions : IDeviceOptions
    {
        public ArfaDeviceType DeviceType { get; set; }
        public ArfaDriverType DriverType { get; set; }
        public string Ip { get; set; }
        public string Port { get; set; }
    }

    /// <summary>
    /// Сериализатор сущностей.
    /// </summary>
    public static class ArfaEntitySerializer
    {
        //private static Dictionary<ArfaDeviceType, Guid> ArfaDeviceTypeToId
        //{
        //    get
        //    {
        //        return new Dictionary<ArfaDeviceType, Guid>
        //        {
        //            { ArfaDeviceType.Tube, new Guid("11000001-0000-0000-0000-000000000000")},
        //            { ArfaDeviceType.Detector, new Guid("22000002-0000-0000-0000-000000000000")},
        //            { ArfaDeviceType.Motor, new Guid("33000003-0000-0000-0000-000000000000")},
        //        };
        //    }
        //}

        private static Dictionary<ArfaDriverType, Guid> ArfaDriverTypeToId
        {
            get
            {
                return new Dictionary<ArfaDriverType, Guid>
                {
                    { ArfaDriverType.MiniX1, new Guid("F52C4E28-60E2-453F-8976-63BDB4E9EF33")},
                    { ArfaDriverType.MiniX2WinUSB, new Guid("1F110FDF-7B72-4B4B-A7A1-B6E906D12D45")},
                    { ArfaDriverType.MiniX2LibUSB, new Guid("62024648-5130-422A-912E-1E259DA0E361")},
                    { ArfaDriverType.DP5WinUSB, new Guid("5D5D4062-C6F5-4288-9948-8ED4A604B406")},
                    { ArfaDriverType.DP5LibUSB, new Guid("4DC4DC4F-D711-46FC-A93B-E6C03D45EDFA")},
                    { ArfaDriverType.FESTO, new Guid("CC670BDB-F84F-4BA7-B120-106D872BE584")}
                };
            }
        }

        /// <summary>
        /// Метод сериализации для Трубки.
        /// </summary>
        /// <param name="tube">Tube.</param>
        /// <returns>Возвращает созданный XML.</returns>
        public static XElement SerializeToXml(TubeDeviceOptions tube)
        {
            var deviceId = ManagedDeviceTypes.Tube.ID;
            var driverId = ArfaDriverTypeToId[tube.DriverType];

            return new XElement("XRayTube",
                // имя класса
                new XAttribute(CommonNames.Elements.AssemblyQualifiedName, ArfaDeviceManagerNames.Elements.XRayTube),
                // Name
                new XElement(ArfaDeviceManagerNames.Properties.Name, "XRayTube"),
                // DeviceId
                new XElement(ArfaDeviceManagerNames.Properties.DeviceId, deviceId),
                // DriverId
                new XElement(ArfaDeviceManagerNames.Properties.DriverId, driverId),
                new XElement(ArfaDeviceManagerNames.Properties.Voltage, tube.Voltage),
                new XElement(ArfaDeviceManagerNames.Properties.Current, tube.Current)
            );
        }

        /// <summary>
        /// Метод сериализации для Детектора.
        /// </summary>
        /// <param name="detector">Detector.</param>
        /// <returns>Возвращает созданный XML.</returns>
        public static XElement SerializeToXml(DetectorDeviceOptions detector)
        {
            var deviceId = ManagedDeviceTypes.Detector.ID;
            var driverId = ArfaDriverTypeToId[detector.DriverType];

            return new XElement("Detector",
                // имя класса
                new XAttribute(CommonNames.Elements.AssemblyQualifiedName, ArfaDeviceManagerNames.Elements.Detector),
                // Name
                new XElement(ArfaDeviceManagerNames.Properties.Name, "Detector"),
                // DeviceId
                new XElement(ArfaDeviceManagerNames.Properties.DeviceId, deviceId),
                // DriverId
                new XElement(ArfaDeviceManagerNames.Properties.DriverId, driverId)
            );
        }

        /// <summary>
        /// Метод сериализации для Привода.
        /// </summary>
        /// <param name="motor">Motor.</param>
        /// <returns>Возвращает созданный XML.</returns>
        public static XElement SerializeToXml(MotorDeviceOptions motor)
        {
            var deviceId = ManagedDeviceTypes.Motor.ID;
            var driverId = ArfaDriverTypeToId[motor.DriverType];

            return new XElement("Motor",
                // имя класса
                new XAttribute(CommonNames.Elements.AssemblyQualifiedName, ArfaDeviceManagerNames.Elements.Motor),
                // Name
                new XElement(ArfaDeviceManagerNames.Properties.Name, "Motor"),
                // DeviceId
                new XElement(ArfaDeviceManagerNames.Properties.DeviceId, deviceId),
                // DriverId
                new XElement(ArfaDeviceManagerNames.Properties.DriverId, driverId),
                new XElement(ArfaDeviceManagerNames.Properties.IpAddress, motor.Ip),
                new XElement(ArfaDeviceManagerNames.Properties.Port, motor.Port)
            );
        }

    }
}
