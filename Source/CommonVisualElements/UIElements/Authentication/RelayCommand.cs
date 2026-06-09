using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RIVS.ASAK.UIElements.Authentication
{
    public class RelayCommand : ICommand
    {
        private readonly Func<object, Task<bool>> _execute;
        private readonly Func<object, bool> _canExecute;

        public event EventHandler RequestClose;

        public RelayCommand(Func<object, Task<bool>> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public async void Execute(object parameter)
        {
            var result = _execute(parameter);
            if (await result)
            {
                // Уведомляем о необходимости закрыть окно
                RequestClose?.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

    }
}
