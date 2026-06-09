using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using RIVS.ASAK.Core.Contract.Enums;

namespace RIVS.ASAK.Core.Infrastructure.Helpers
{
    public static class AsakSystemHelper
    {
        private static Dictionary<AsakSystemPartType, Guid> AsakSystemTypeToGuid
        {
            get
            {
                return new Dictionary<AsakSystemPartType, Guid>
                {
                    { AsakSystemPartType.None, new Guid("00000000-0000-0000-0000-000000000000") },
                    { AsakSystemPartType.Dispatcher, new Guid("c9438abb-ef9a-419a-a6bf-dda6c7b0d4ea") },
                    { AsakSystemPartType.Csu, new Guid("be8802ba-1a6f-4a3a-aa3c-0c9e30541976") },
                    { AsakSystemPartType.Arfa1, new Guid("af010000-0000-0000-0000-000000000000") },
                    { AsakSystemPartType.Arfa2, new Guid("af020000-0000-0000-0000-000000000000") },
                    { AsakSystemPartType.Arfa3, new Guid("af030000-0000-0000-0000-000000000000") },
                    { AsakSystemPartType.Arfa4, new Guid("af040000-0000-0000-0000-000000000000") },
                    { AsakSystemPartType.ARMO, new Guid("ed68e54e-9e4f-46d5-a436-cb7835b5f847") },
                    { AsakSystemPartType.ARMN, new Guid("67a664db-dab6-4f17-a734-fda67af6090a") },
                    { AsakSystemPartType.ARMA, new Guid("0c5602d2-6929-403e-83a3-9ec05ac1472b") },
                    { AsakSystemPartType.AuthentificationRedactorTool, new Guid("eecb2db0-bcbd-49b5-8023-243fac95e236") }
                };
            }
        }

        public static short GetAsakSystem(Guid id)
        {
            foreach (var keyVar in AsakSystemTypeToGuid.Keys.Where(keyVar => AsakSystemTypeToGuid[keyVar] == id))
            {
                return (short)keyVar;
            }

            return 0;
        }

        public static Guid GetAsakSystemId(AsakSystemPartType type)
        {
            if (AsakSystemTypeToGuid.ContainsKey(type))
            {
                return AsakSystemTypeToGuid[type];
            }

            return new Guid("00000000-0000-0000-0000-000000000000");
        }

        public static string GetAsakSystemName(AsakSystemPartType systemPartType)
        {
            var defValueAttr = EnumHelper.GetAttribute<DescriptionAttribute, AsakSystemPartType>(systemPartType);
            return defValueAttr != null ? defValueAttr.Description : string.Empty;
        }
    }
}
