using System.Windows;
using System.Windows.Controls;
using RIVS.ASAK.ARFA.GUI.ViewModels;

namespace RIVS.ASAK.ARFA.GUI.View
{
    /// <summary>
    /// Логика взаимодействия для PowerConfigPage.xaml
    /// </summary>
    public partial class PowerConfigPage : Page
    {
        private readonly ConfigViewModel _configViewModel;

        public PowerConfigPage(ConfigViewModel configViewModel)
        {
            InitializeComponent();
            _configViewModel = configViewModel;
            DataContext = _configViewModel;
        }

        private void RestartButton_Click(object sender, RoutedEventArgs e)
        {
            _configViewModel.ArfaRestart();
        }

        private void ShutdownButton_Click(object sender, RoutedEventArgs e)
        {
            _configViewModel.ArfaShutdown();
        }
        private void AppExitClick(object sender, RoutedEventArgs e)
        {
            _configViewModel.SlideFrame = new AppExitConfigPage();
            //FrameConfigTabs.NavigationService.Navigate();
            _configViewModel.AppExit();
        }
    }
}
