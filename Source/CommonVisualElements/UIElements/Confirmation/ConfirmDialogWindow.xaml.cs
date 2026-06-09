using System.Windows;

namespace RIVS.ASAK.UIElements.Confirmation
{
    /// <summary>
    /// Логика взаимодействия для ConfirmDialogWindow.xaml
    /// </summary>
    public partial class ConfirmDialogWindow : Window
    {
        public string DialogTitle { get; set; }
        public string Message { get; set; }

        public bool Result { get; private set; }

        public ConfirmDialogWindow(string title, string message)
        {
            InitializeComponent();

            DialogTitle = title;
            Message = message;

            DataContext = this;
        }

        private void Yes_Click(object sender, RoutedEventArgs e)
        {
            Result = true;
            DialogResult = true;
            Close();
        }

        private void No_Click(object sender, RoutedEventArgs e)
        {
            Result = false;
            DialogResult = false;
            Close();
        }

        public static bool Show(
            Window owner,
            string title,
            string message)
        {
            var dialog = new ConfirmDialogWindow(title, message)
            {
                Owner = owner
            };

            dialog.ShowDialog();

            return dialog.Result;
        }
    }
}
