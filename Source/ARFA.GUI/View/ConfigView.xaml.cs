using System.Windows;
using RIVS.ASAK.ARFA.GUI.ViewModels;

namespace RIVS.ASAK.ARFA.GUI.View
{
    /// <summary>
    /// Логика взаимодействия для ConfigView.xaml
    /// </summary>
    public partial class ConfigView : Window
    {
        private readonly ConfigViewModel _configViewModel;

        public ConfigView(ConfigViewModel configViewModel)
        {
            InitializeComponent();
            _configViewModel = configViewModel;
            DataContext = _configViewModel;

        }

        private void ReturnToMainWindowClick(object sender, RoutedEventArgs e)
        {
            _configViewModel.CloseConfigWindow();
        }


        private void PowerPageShowButton_Click(object sender, RoutedEventArgs e)
        {
            _configViewModel.SlideFrame = new PowerConfigPage(_configViewModel);
        }

        private void ConfigPageShow_Click(object sender, RoutedEventArgs e)
        {
            _configViewModel.SlideFrame = new DeveloperConfigPage(_configViewModel);
        }
        private void AuthWindowButton_Click(object sender, RoutedEventArgs e)
        {
            _configViewModel.OpenAuthWindow();
        }

        private void Window_Closed(object sender, System.EventArgs e)
        {
            _configViewModel.CloseConfigWindow();
        }
    }
}
