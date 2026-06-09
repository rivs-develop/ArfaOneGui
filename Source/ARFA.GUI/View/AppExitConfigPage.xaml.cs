using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace RIVS.ASAK.ARFA.GUI.View
{
    /// <summary>
    /// Логика взаимодействия для AppExitConfigPage.xaml
    /// </summary>
    public partial class AppExitConfigPage : Page
    {
        private readonly DispatcherTimer _timer = new() { Interval = TimeSpan.FromSeconds(3) };

        public AppExitConfigPage()
        {
            InitializeComponent();

            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private void Timer_Tick(object sender, System.EventArgs e)
        {
            PleaseWaitTextBlock.Visibility = PleaseWaitTextBlock.Visibility == Visibility.Visible
                ? Visibility.Hidden : Visibility.Visible;
        }
    }
}
