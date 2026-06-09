using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Prism.Events;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using RIVS.ASAK.ARFA.GUI.Services;
using RIVS.ASAK.ARFA.GUI.View;
using RIVS.ASAK.ARFA.GUI.ViewModels;
using RIVS.ASAK.Core.Contract;
using RIVS.ASAK.Core.Logging.NLog;
using RIVS.ASAK.UIElements.Authentication;
using RIVS.ASAK.Core.Contract.Configuration;
using ILogger = RIVS.ASAK.Core.Contract.ILogger;
using ILoggerFactory = RIVS.ASAK.Core.Contract.ILoggerFactory;
using RIVS.ASAK.Core.AuthorizationService;
using RIVS.ASAK.Core.Infrastructure.Configuration;

namespace RIVS.ASAK.ARFA.GUI.Startup
{
    // ArfaGuiBootstrapper.cs
    public sealed class ArfaGuiBootstrapper : IDisposable
    {
        private readonly IContainer _container;
        private readonly ILogger _logger;
        private ArfaGuiBootstrapper(IContainer container, ILogger logger)
        {
            _container = container;
            _logger = logger;
        }

        public MainViewModel RootVisual => _container.Resolve<MainViewModel>();

        // ── Фабричный метод ───────────────────────────────────────────

        public static async Task<ArfaGuiBootstrapper> CreateAsync()
        {
            var configuration = LoadConfiguration();
            var container = BuildContainer(configuration);
            var logger = container.Resolve<ILoggerFactory>().Create(typeof(ArfaGuiBootstrapper));

            var bootstrapper = new ArfaGuiBootstrapper(container, logger);
            
            return bootstrapper;
        }

        public void Dispose()
        {
            _logger.Info("ARFA stopping");
            _container.Dispose();
        }

        // ── Конфигурация контейнера ───────────────────────────────────

        private static IConfiguration LoadConfiguration()
        {
#if _DEVELOPMENT
        const string settingsFile = "ArfaSettingsDevelopment.json";
#else
            const string settingsFile = "ARFASettings.json";
#endif
            return new ConfigurationBuilder()
                .AddJsonFile(settingsFile, optional: true)
                .Build();
        }

        private static IContainer BuildContainer(IConfiguration configuration)
        {
            var builder = new ContainerBuilder();

            RegisterLogging(builder, configuration);
            RegisterInfrastructure(builder);
            RegisterViewModels(builder);
            RegisterHttpClient(builder);

            builder.RegisterInstance(configuration).As<IConfiguration>().SingleInstance();
            return builder.Build();
        }

        private static void RegisterLogging(ContainerBuilder builder, IConfiguration configuration)
        {
            var options = configuration
                .GetSection(nameof(CoreNLogOptions))
                .Get<CoreNLogOptions>();

            if (options == null)
                return;

            EnsureLogDirectoryExists();

            var factory = new NLoggerFactory(options);
            builder.RegisterInstance(factory).As<ILoggerFactory>().SingleInstance();
        }

        private static void EnsureLogDirectoryExists()
        {
            var logDir = Path.Combine(Environment.CurrentDirectory, "log");

            if (!Directory.Exists(logDir))
                Directory.CreateDirectory(logDir);
        }

        private static void RegisterInfrastructure(ContainerBuilder builder)
        {
            builder.RegisterType<EventAggregator>()
                .As<IEventAggregator>()
                .SingleInstance();

            //builder.RegisterType<ArfaMeasuringResultSaverService>()
            //    .As<IArfaMeasuringResultSaverService>()
            //    .SingleInstance();

            builder.RegisterType<AuthorizationService>()
                .As<IAuthorizationService>()
                .SingleInstance();

            builder.RegisterType<AuthenticationProvider>()
                .As<IAuthenticationProvider>()
                .SingleInstance();

            builder.RegisterType<AppConfigurationService>()
                .As<IAppConfigurationService>()
                .SingleInstance();

            builder.RegisterType<ArfaConfigurationResolver>()
                .As<IArfaConfigurationResolver>()
                .SingleInstance();

            // SIGNALR КЛИЕНТ
            builder.Register(c =>
                {
                    var config = c.Resolve<IConfiguration>();
                    var eventAggregator = c.Resolve<IEventAggregator>();
                    var loggerFactory = c.Resolve<ILoggerFactory>();

                    var signalRUrl = config.GetSection("SignalR")?["ClientUrl"] ?? "http://localhost:10003";

                    return new SignalRService(eventAggregator, loggerFactory, signalRUrl);
                })
                .As<ISignalRService>()
                .SingleInstance();
            // =======================================
        }

        private static void RegisterViewModels(ContainerBuilder builder)
        {
            var assembly = Assembly.GetExecutingAssembly();

            builder.RegisterAssemblyTypes(assembly)
                .Where(t => typeof(IViewModel).IsAssignableFrom(t))
                .AsImplementedInterfaces();

            
            
            
            

            builder.RegisterType<MainWindow>().AsSelf();
            builder.RegisterType<MainViewModel>().AsSelf().SingleInstance();

            builder.RegisterType<MeasurementResultsView>().AsSelf();
            builder.RegisterType<MeasurementResultsViewModel>().AsSelf().SingleInstance();

            //TODO: тут можно придумать, как определять конфигурацию кол-ва кювет. Пока строго прописываем 1 кювету.
            builder.RegisterType<CuvetteBlock1View>().AsSelf();
            builder.RegisterType<CuvetteBlock1ViewModel>().As<CuvetteBlockViewModelBase>().SingleInstance();

            builder.RegisterType<ConfigView>().AsSelf();
            builder.RegisterType<ConfigViewModel>().AsSelf().SingleInstance();

            builder.RegisterType<DateTimeViewModel>().As<IDateTimeViewModel>().SingleInstance();

            builder.RegisterType<ParametersView>().AsSelf();
            builder.RegisterType<ParametersViewModel>().AsSelf().SingleInstance();

            builder.RegisterType<AuthenticationWindow>().AsSelf();
        }

        private static void RegisterHttpClient(ContainerBuilder builder)
        {
            var services = new ServiceCollection();
            services.AddHttpClient();
            builder.Populate(services);
        }


        public T Resolve<T>() => _container.Resolve<T>();
    }
}
