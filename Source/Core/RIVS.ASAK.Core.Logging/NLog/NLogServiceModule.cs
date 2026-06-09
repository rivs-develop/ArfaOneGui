using Autofac;
using Microsoft.Extensions.Configuration;
using RIVS.ASAK.Core.Contract;

namespace RIVS.ASAK.Core.Logging.NLog
{
    public class NLogServiceModule
    {
        private readonly CoreNLogOptions _options;
        private readonly NLoggerFactory _nLoggerFactory;
        public NLogServiceModule(IConfigurationRoot appSettings)
        {
            _options = appSettings.GetSection(typeof(CoreNLogOptions).Name).Get<CoreNLogOptions>();
            if (_options != null)
            {
                _nLoggerFactory = new NLoggerFactory(_options);
            }
        }

        public NLogServiceModule(NLoggerFactory nLoggerFactory)
        {
            if (nLoggerFactory != null)
            {
                _nLoggerFactory = nLoggerFactory;
            }
        }

        public void DoRegister(ContainerBuilder componentContainerBuilder)
        {
            if (_options != null)
            {
                componentContainerBuilder.RegisterInstance(_nLoggerFactory).As<ILoggerFactory>().SingleInstance();
            }
        }

        public void DoInit(IContainer container)
        {
            var loggerFactory = container.Resolve<ILoggerFactory>();
            var logger = loggerFactory.Create(GetType());
            logger.Info($"[config]use NLog logger");
        }
    }
}
