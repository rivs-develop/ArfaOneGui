using Prism.Events;

namespace RIVS.ASAK.ARFA.GUI.ViewModels
{
    /// <summary>
    /// Реализация ViewModel для 10 кюветника
    /// </summary>
    public class CuvetteBlock10ViewModel : CuvetteBlockViewModelBase/*BaseViewModel, ICuvetteBlockViewModel*/
    {
        public CuvetteBlock10ViewModel(IEventAggregator eventAggregator)
            : base(eventAggregator)
        {
        }

    }
}
