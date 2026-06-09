using RIVS.ASAK.Core.Contract;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using RIVS.ASAK.Core.Contract.DTO;

namespace RIVS.ASAK.UIElements.Authentication
{
    /// <summary>
    /// Логика взаимодействия для AuthenticationWindow.xaml
    /// </summary>
    public partial class AuthenticationWindow : Window, INotifyPropertyChanged
    {
        public RelayCommand AsyncAuthCommand { get; }
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly IAuthorizationService _authorizationService;

        public Guid actionId { get; set; }
        public Guid objectId { get; set; }

        private bool _isExecuting;
        public bool IsExecuting
        {
            get => _isExecuting;
            private set
            {
                if (_isExecuting != value)
                {
                    _isExecuting = value;
                    OnPropertyChanged(nameof(IsExecuting));
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        private string _authenticationMessage;
        public string AuthenticationMessage
        {
            get => _authenticationMessage;
            set
            {
                if (_authenticationMessage != value)
                {
                    _authenticationMessage = value;
                    OnPropertyChanged(nameof(AuthenticationMessage));
                }
            }
        }

        private string _okButtonContent;
        public string OkButtonContent
        {
            get => _okButtonContent;
            set
            {
                _okButtonContent = value;
                OnPropertyChanged(nameof(OkButtonContent));
            }
        }

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
        private string _login;
        private bool _loginIsEnabled;
        private string _password;
        public string Login
        {
            get => _login;
            set
            {
                _login = value;
                OnPropertyChanged(nameof(Login));
            }
        }
        public bool LoginIsEnabled
        {
            get => _loginIsEnabled;
            set
            {
                _loginIsEnabled = value;
                OnPropertyChanged(nameof(LoginIsEnabled));
            }
        }
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        public AuthenticationWindow(IAuthorizationService authorizationService)
        {
            InitializeComponent();
            DataContext = this;
            _authorizationService = authorizationService;

            LoginIsEnabled = true;
            OkButtonContent = "Войти";
            TitleText = "Вход";

            AsyncAuthCommand = new RelayCommand(ExecuteAsyncAuthCommand, CanExecuteMyCommand);

            UserLoginTextBox.Focus();

            // Подписываемся на событие RequestClose
            AsyncAuthCommand.RequestClose += (sender, args) =>
            {
                // Закрываем окно с результатом true
                DialogResult = true;
                Close();
            };

            // Обновляем пароль в ViewModel при изменении текста в PasswordBox
            UserPasswordBox.PasswordChanged += (sender, args) =>
            {
                SetPassword(UserPasswordBox.Password);
            };
        }

        private async Task<bool> ExecuteAsyncAuthCommand(object parameter)
        {
            IsExecuting = true;

            try
            {
                AuthorizationResult authResult;

                if (actionId != Guid.Empty && objectId != Guid.Empty)
                {
                    authResult = await _authorizationService.AuthorizationAsync(_login, _password, actionId, objectId);
                }
                else
                {
                    authResult = await _authorizationService.AuthorizationAsync(_login, _password);
                }

                if (authResult == null)
                {
                    AuthenticationMessage = "Нет связи с сервером!";
                    return false;
                }

                if (authResult.Result != 0)
                {
                    AuthenticationMessage = authResult.Message;
                    return false;
                }

                return true;
            }
            finally
            {
                IsExecuting = false;
            }

        }

        public void SetPassword(string password)
        {
            Password = password;
        }


        private bool CanExecuteMyCommand(object parameter)
        {
            return !string.IsNullOrEmpty(Login) && !string.IsNullOrEmpty(Password)
                && !IsExecuting;
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        //private async void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    OkButton.IsEnabled = false;

        //    //если результат успешный (0), закрываем окно
        //    if (res.Item1 == 0)
        //    {
        //        DialogResult = true;
        //        return;
        //    }
        //    //в противном случае выводим в интерфейс сообщение
        //    ResultMessage.Content = res.Item2;

        //    //Разблокировка кнопки
        //    OkButton.IsEnabled = true;

        //}

        #region titleBar

        /// <summary>
        /// Обработчик кнопки закрытия приложения
        /// </summary>
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
            //Application.Current.Shutdown();
        }

        ///// <summary>
        ///// Обработчик кнопки сворачивания окна
        ///// </summary>
        //private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        //{

        //    this.WindowState = WindowState.Minimized;
        //}

        ///// <summary>
        ///// Обработчик кнопки разворачивания окна
        ///// </summary>
        //private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        //{
        //    if (this.WindowState != WindowState.Maximized)
        //    {
        //        this.WindowState = WindowState.Maximized;
        //    }
        //    else
        //    {
        //        this.WindowState = WindowState.Normal;
        //    }
        //}

        ///// <summary>
        ///// Обработчик нажатия кнопки по TitleBar для перемещения окна
        ///// </summary>
        //private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    DragMove();
        //}

        #endregion

        private void UserLoginTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter & (sender as TextBox).AcceptsReturn == false)
            {
                MoveToNextUIElement(e);
            }

        }

        void MoveToNextUIElement(KeyEventArgs e)
        {
            // Creating a FocusNavigationDirection object and setting it to a
            // local field that contains the direction selected.
            FocusNavigationDirection focusDirection = FocusNavigationDirection.Next;

            // MoveFocus takes a TraveralReqest as its argument.
            TraversalRequest request = new TraversalRequest(focusDirection);

            // Change keyboard focus.
            if (Keyboard.FocusedElement is UIElement elementWithFocus)
            {
                if (elementWithFocus.MoveFocus(request))
                {
                    e.Handled = true;
                }
            }
        }

        private void UserPasswordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && AsyncAuthCommand.CanExecute(OkButton) && UserPasswordBox.IsKeyboardFocused)
            {
                AsyncAuthCommand.Execute(OkButton);
            }
        }
    }
}
