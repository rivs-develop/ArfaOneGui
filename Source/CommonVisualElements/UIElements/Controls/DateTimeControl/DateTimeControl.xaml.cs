
//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Data;
//using System.Windows.Threading;

namespace RIVS.ASAK.UIElements.Controls.DateTimeControl
{
#pragma warning disable IDE0065
    using System;
    using System.Windows;
    using System.Windows.Threading;
    using System.Windows.Data;
#pragma warning restore IDE0065

    /// <summary>
    /// Контрол отображает время и дату
    /// </summary>
    public class DateTimeControl : System.Windows.Controls.Control
    {
        #region Свойства зависимостей

        public DateTime DateTime
        {
            get { return (DateTime)GetValue( DateTimeProperty ); }
            set { SetValue( DateTimeProperty, value ); }
        }

        public static readonly DependencyProperty DateTimeProperty =
            DependencyProperty.Register( "DateTime", typeof( DateTime ), typeof( DateTimeControl ) );

        #endregion

        static DateTimeControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata( typeof( DateTimeControl ), new FrameworkPropertyMetadata( typeof( DateTimeControl ) ) );
        }

        public DateTimeControl()
        {
            Loaded += DateTimeControl_Loaded;
            Unloaded += DateTimeControl_Unloaded;
            DateTime = DateTime.Now;
        }

        private readonly DispatcherTimer _dispatcherTimer = new DispatcherTimer(DispatcherPriority.Send);

        void DateTimeControl_Unloaded( object sender, RoutedEventArgs e )
        {
            _dispatcherTimer.Stop();
            _dispatcherTimer.Tick -= dispatcherTimer_Tick;
        }

        void DateTimeControl_Loaded( object sender, RoutedEventArgs e )
        {
            _dispatcherTimer.Tick += dispatcherTimer_Tick;
            _dispatcherTimer.Interval = new TimeSpan( 0, 0, 0, 0 , 500 );
            _dispatcherTimer.Start();
        }

        private void dispatcherTimer_Tick( object sender, System.EventArgs e )
        {
            DateTime = DateTime.Now;
        }
    }

    #region Конвертеры времени и даты

    /// <summary>
    /// Преобразует DateTime в строку в формате "d.MM.yy"
    /// </summary>
    [ValueConversion( typeof( System.DateTime ), typeof( string ) )]
    public class DateTimeToDateConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
        {
            if( !(value is DateTime) )
            {
                throw new ArgumentException( "DateTimeToDateConverter can only convert from DateTime" );
            }

            return ((DateTime)value).ToString( "dd.MM.yyyy" );
        }

        public object ConvertBack( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    /// <summary>
    /// Преобразует DateTime в строку в формате "h:mm:ss"
    /// </summary>
    [ValueConversion( typeof( System.DateTime ), typeof( string ) )]
    public class DateTimeToTimeConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
        {
            if( !(value is DateTime) )
            {
                throw new ArgumentException( "DateTimeToTimeConverter can only convert from DateTime" );
            }

            return ((DateTime)value).ToString( "HH:mm:ss" );
        }

        public object ConvertBack( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    #endregion
}
