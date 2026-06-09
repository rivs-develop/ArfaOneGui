using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using RIVS.ASAK.Core.Contract.Enums;

namespace RIVS.ASAK.Core.Tools
{
    public class ManagedDeviceType
    {
        public Guid ID { get; private set; }
        public ArfaDeviceType DeviceType { get; private set; }
        public string Name { get; private set; }

        public ManagedDeviceType(Guid id, ArfaDeviceType deviceType, string name)
        {
            Debug.Assert(id != null && id != Guid.Empty, "Должен быть id");
            Debug.Assert(!string.IsNullOrEmpty(name), "Имя не может быть пустым");
            ID = id;
            DeviceType = deviceType;
            Name = name;
        }
    }

    /// <summary>
    /// Идентификаторы типов сущност
    /// </summary>
    public static class ManagedDeviceTypes
    {
        private static List<ManagedDeviceType> _allTypes;
        private static Dictionary<Guid, ManagedDeviceType> _typeById;

        static ManagedDeviceTypes()
        {
        }

        private static void InitTypes()
        {
            var types = typeof(ManagedDeviceTypes).GetFields()
                                                   .Where(x => x.FieldType == typeof(ManagedDeviceType))
                                                   .Select(x => x.GetValue(null))
                                                   .OfType<ManagedDeviceType>()
                                                   .ToList();
            ModuleEnvironment<FillAllManagedEntityTypes>.Run(types);
            _allTypes = new List<ManagedDeviceType>();
            _typeById = new Dictionary<Guid, ManagedDeviceType>();
            foreach (var type in types)
            {
                _allTypes.Add(type);
                _typeById[type.ID] = type;
            }
        }

        public static ManagedDeviceType GetMEType(Guid id)
        {
            if (_typeById == null)
            {
                InitTypes();
            }
            var contains = _typeById.ContainsKey(id);
            if (!contains)
            {
                //Logger.Error("ManagedEntityTypes: Can't find type " + id + new StackTrace());
            }
            return _typeById[id];
        }

        public static IEnumerable<ManagedDeviceType> METypes
        {
            get
            {
                if (_allTypes == null)
                {
                    InitTypes();
                }
                return _allTypes;
            }
        }

        public static bool ContainsType(Guid typeId)
        {
            if (_typeById == null)
            {
                InitTypes();
            }
            return _typeById.ContainsKey(typeId);
        }

        public static readonly ManagedDeviceType Tube =
            new ManagedDeviceType(new Guid("11000001-0000-0000-0000-000000000000")
            , ArfaDeviceType.Tube
            , "Рентгеновская трубка"
            );

        public static readonly ManagedDeviceType Detector =
            new ManagedDeviceType(new Guid("22000002-0000-0000-0000-000000000000")
                , ArfaDeviceType.Detector
                , "Детектор"
            );

        public static readonly ManagedDeviceType Motor =
        new ManagedDeviceType(new Guid("33000003-0000-0000-0000-000000000000")
            , ArfaDeviceType.Motor
        , "Привод"
        );
    }
}
