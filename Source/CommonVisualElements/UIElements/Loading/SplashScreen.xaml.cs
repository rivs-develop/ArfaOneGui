using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace RIVS.ASAK.UIElements.Loading
{
    /// <summary>
    /// Логика взаимодействия для SplashScreen.xaml
    /// </summary>
    public partial class SplashScreen : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _titleText;
        public string TitleText
        {
            get => _titleText;
            set
            {
                _titleText = value;
                OnPropertyChanged(nameof(TitleText));
            }
        }

        private readonly int _timeToCloseInSeconds;

        public SplashScreen(int timeToCloseInSeconds)
        {
            InitializeComponent();
            DataContext = this;
            _timeToCloseInSeconds = timeToCloseInSeconds;
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        //надо куда-то вынести эту функцию
        private double ConvertToUnixTimestamp(DateTime date)
        {
            var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            var diff = date.ToUniversalTime() - origin;
            return Math.Floor(diff.TotalSeconds);
        }

        public void UpdateProgressBar(double newValue)
        {
            try
            {
                // Создаем анимацию
                var animation = new DoubleAnimation
                {
                    From = ProgressBar.Value, // Начальное значение - текущее значение ProgressBar
                    To = newValue, // Конечное значение (например, 100%)
                    Duration = TimeSpan.FromSeconds(1), // Продолжительность анимации
                    EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut } // Плавное начало и конец
                };

                // Запускаем анимацию
                ProgressBar.BeginAnimation(ProgressBar.ValueProperty, animation);
            }
            finally
            {
                ProgressBar.Value = newValue;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var dt1 = ConvertToUnixTimestamp(DateTime.Now);
            var processSeconds = 0;
            Task.Run(() =>
            {
                try
                {

                    while (_timeToCloseInSeconds > processSeconds)
                    {
                        if (ConvertToUnixTimestamp(DateTime.Now) - dt1 > processSeconds)
                        {
                            if (Dispatcher.Invoke(() => IsLoaded && IsVisible))
                            {
                                processSeconds += 2;
                                var seconds = processSeconds;
                                Dispatcher.Invoke(() =>
                                UpdateProgressBar((double)seconds * 100 / _timeToCloseInSeconds));
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
                finally
                {
                    Dispatcher.Invoke(Close);
                }

            });
        }
    }
}
