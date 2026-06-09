using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using RIVS.ASAK.ARFA.GUI.ViewModels;

namespace RIVS.ASAK.ARFA.GUI.View
{
    /// <summary>
    /// Логика взаимодействия для DeveloperConfigPage.xaml
    /// </summary>
    public partial class DeveloperConfigPage : Page
    {
        private readonly ConfigViewModel _configViewModel;
        public DeveloperConfigPage(ConfigViewModel configViewModel)
        {
            InitializeComponent();
            _configViewModel = configViewModel;
            DataContext = _configViewModel;
        }

        private void UpdateApButton_Click(object sender, RoutedEventArgs e)
        {
            _configViewModel.UpdateAp();
        }

        private void UpdateElementsButton_Click(object sender, RoutedEventArgs e)
        {
            _configViewModel.UpdateElements();
        }

        private void ListView_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;
        }
    }
}
