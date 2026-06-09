using System.Windows.Input;
using Prism.Commands;
using Prism.Events;

namespace RIVS.ASAK.ARFA.GUI.ViewModels
{
    /// <summary>
    /// Реализует базовый функционал для 10 и 15 кюветных ViewModel
    /// </summary>
    public class CuvetteBlockViewModelBase : BaseViewModel, ICuvetteBlockViewModel
    {
        public ICommand CuvetteClickCommand { get; }

        public readonly IEventAggregator eventAggregator;

        public bool IsButtonsEnabled { get; set; }

        public CuvetteBlockViewModelBase(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;

            CuvetteClickCommand = new DelegateCommand<object>(OnCuvetteClickCommand);
            IsButtonsEnabled = true;

        }

        private void OnCuvetteClickCommand(object obj)
        {
            string parameter = obj as string;
            if (int.TryParse(parameter, out int number) && IsButtonsEnabled)
            {
                //TODO: тут отправляем комманду на перемещение и измерение
            }
        }

    }
}
