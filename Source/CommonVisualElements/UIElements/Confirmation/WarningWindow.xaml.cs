using System.Windows;

namespace RIVS.ASAK.UIElements.Confirmation
{
    /// <summary>
    /// Логика взаимодействия для WarningWindow.xaml
    /// </summary>
    public partial class WarningWindow : Window
    {
        public string Message { get; set; }

        public WarningWindow(string message)
        {
            InitializeComponent();

            Message = message;
            DataContext = this;
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        public static void Show(Window owner, string message)
        {
            var dialog = new WarningWindow(message)
            {
                Owner = owner
            };

            dialog.ShowDialog();
        }
    }
}
