using RIVS.ASAK.Core.Contract.Enums;

namespace RIVS.ASAK.Core.Infrastructure.Configuration
{
    public class AppConfig
    {
        public AsakSystemPartType SystemPartType { get; set; }
        public AppMode Mode { get; set; }
        public string Name { get; set; }
        public bool Trace { get; set; }
    }
}
