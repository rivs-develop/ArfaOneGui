using Prism.Events;

namespace RIVS.ASAK.ARFA.GUI.ViewModels
{
    /// <summary>
    /// Реализация ViewModel для 1 кюветника
    /// </summary>
    public class CuvetteBlock1ViewModel : CuvetteBlockViewModelBase/*BaseViewModel, ICuvetteBlockViewModel*/
    {
        public CuvetteBlock1ViewModel(IEventAggregator eventAggregator)
            : base(eventAggregator)
        {
        }

    }
}
