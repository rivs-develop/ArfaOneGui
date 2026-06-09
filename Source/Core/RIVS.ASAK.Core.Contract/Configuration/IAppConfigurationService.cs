using System;
using RIVS.ASAK.Core.Contract.Enums;

namespace RIVS.ASAK.Core.Contract.Configuration
{

    public interface IAppConfigurationService
    {
        bool IsProduction();
        bool IsDevelopment();
        bool CanTrace();
        AsakSystemPartType GetSystemPartType();
        Guid GetSystemId();
        string GetSystemName();
    }
}
