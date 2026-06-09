using System;
using Microsoft.Extensions.Configuration;
using RIVS.ASAK.Core.Contract.Configuration;
using RIVS.ASAK.Core.Contract.Enums;
using RIVS.ASAK.Core.Infrastructure.Helpers;

namespace RIVS.ASAK.Core.Infrastructure.Configuration
{
    public class AppConfigurationService : IAppConfigurationService
    {
        private readonly IConfiguration _configuration;
        private AppConfig _application; // Assuming AppConfig is moved to Infrastructure.Enums or similar

        public AppConfigurationService(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        private AppConfig Application
        {
            get
            {
                if (_application != null)
                {
                    return _application;
                }

                _application = _configuration.GetSection("appConfig").Get<AppConfig>();
                if (_application == null)
                {
                    // Log error or throw exception - depends on design choice
                    // Use injected logger or just throw
                    throw new InvalidOperationException("appConfig section is not found in configuration.");
                }
                return _application;
            }
        }

        public bool IsProduction() => Application.Mode == AppMode.Production;
        public bool IsDevelopment() => Application.Mode == AppMode.Development;
        public bool CanTrace() => Application.Trace;
        public AsakSystemPartType GetSystemPartType() => Application.SystemPartType;

        public Guid GetSystemId() => AsakSystemHelper.GetAsakSystemId(GetSystemPartType());
        public string GetSystemName() => AsakSystemHelper.GetAsakSystemName(GetSystemPartType());

    }
}
