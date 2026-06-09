using Microsoft.Extensions.Logging;
using RIVS.ASAK.ARFA.GUI.Startup;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace RIVS.ASAK.ARFA.GUI
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        private ArfaGuiBootstrapper _bootstrapper;
        private ILogger _logger;

        public App()
        {
            AppDomain.CurrentDomain.UnhandledException += OnDomainUnhandledException;
            Current.DispatcherUnhandledException += OnDispatcherUnhandledException;
            TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            try
            {
                _bootstrapper = await ArfaGuiBootstrapper.CreateAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка запуска:\n{ex.Message}", "ARFA",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown(1);
                return;
            }

            _logger = _bootstrapper.Resolve<ILoggerFactory>()
                                   .CreateLogger<App>();

            _logger.LogInformation("ARFA started, UI thread {ThreadId}",
                Thread.CurrentThread.ManagedThreadId);


            var mainWindow = _bootstrapper.Resolve<MainWindow>();


            mainWindow.Closed += OnMainWindowClosed;
            Current.Exit += OnApplicationExit;

            mainWindow.Show();
        }

        private void OnMainWindowClosed(object sender, System.EventArgs e)
        {
            _bootstrapper?.Dispose();
        }

        private void OnApplicationExit(object sender, ExitEventArgs e)
        {
            _logger?.LogInformation("ARFA exit");
        }

        private void OnDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            _logger?.LogCritical(e.ExceptionObject as Exception, "Unhandled AppDomain exception");
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            _logger?.LogError(e.Exception, "Unhandled Dispatcher exception");
            e.Handled = true;
        }

        private void OnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            _logger?.LogError(e.Exception.GetBaseException(), "Unobserved Task exception");
            e.SetObserved();
        }
        
    }
}
