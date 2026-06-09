using System.Linq;
using OxyPlot;
using RIVS.ASAK.ARFA.Application;
using RIVS.ASAK.ARFA.GUI.AuxiliaryClass;
using RIVS.ASAK.ARFA.GUI.Helpers;
using RIVS.ASAK.UIElements.Tools;

namespace RIVS.ASAK.ARFA.GUI.ViewModels
{
    public class MeasurementResultsViewModel : BaseViewModel, IMeasurementResultsViewModel
    {

        //private readonly object _measureDataEventDataReceivedLock = new object();
        //private IReceiverChannel _measureDataInputChannel;
        //Guid _measureDataSubscribeCookie;

        public PlotModel XrayPlot { get; set; }

        private readonly int _maxCapacity = 20;

        private CustomObservableCollection<IConcentrationDescription> _concentration;

        public CustomObservableCollection<IConcentrationDescription> Concentration
        {
            get { return _concentration ?? (_concentration = new CustomObservableCollection<IConcentrationDescription>()); }
        }

        public MeasurementResultsViewModel()
        {
            XrayPlot = PlotHelper.CreateModel();

            //TODO: заменить подписки
            //SubscribeEventAddNewMeasureData();
        }

        //private void SubscribeEventAddNewMeasureData()
        //{
        //    var conditionEvent = new EventFilter();
        //    conditionEvent.Types.Add(EventTypes.AddNewMeasureDataToTable);
        //    conditionEvent.Types.Add(EventTypes.ArfaUpdateGraph);

        //    AppSettings.Resolve<IBlackboardWrapper>().Subscribe(
        //        conditionEvent,
        //        out _measureDataInputChannel,
        //        OnMeasureDataResultReceived,
        //        out _measureDataSubscribeCookie);

        //    _measureDataInputChannel.DataReceived += OnMeasureDataResultReceived;
        //}

        //private void OnMeasureDataResultReceived(IReceiverChannel channel)
        //{
        //    //lock (_measureDataEventDataReceivedLock)
        //    {
        //        channel.GetData(out var chData);
        //        if (chData is not Event eventData)
        //        {
        //            return;
        //        }

        //        if (eventData.Type == EventTypes.AddNewMeasureDataToTable)
        //        {
        //            if (eventData.ExtData != null && eventData.ExtData is MeasureResult2Table measureResult)
        //            {
        //                var sbTable = new StringBuilder();
        //                foreach (var elem in measureResult.concentrationsElementList)
        //                {
        //                    sbTable.Append($"{elem.Name,-2}  Cc={elem.Cc,5:F2},  ");
        //                }
        //                sbTable.Append($"TFcp={measureResult.TFcp,5:F}");
        //                //sbTable.Append(", ");
        //                //sbTable.Append($"TFcc={measureResult.TFcc,5:F}");

        //                var concentrationDescription
        //                    = new ConcentrationDescription(
        //                        measureResult.Time.ToString("HH:mm:ss"),
        //                        measureResult.Kuvet.ToString(),
        //                        measureResult.Prod.ToString(),
        //                        sbTable.ToString()
        //                    );
        //                DispatcherHelper.RunAsync(() => AddConcentration(concentrationDescription), DispatcherPriority.Background);
        //            }
        //        }

        //        if (eventData.Type == EventTypes.ArfaUpdateGraph)
        //        {
        //            if (eventData.ExtData != null && eventData.ExtData is UpdateGraphData data)
        //            {
        //                Task.Run(()=>UpdateSpecterBuffer(data));
        //            }

        //        }

        //    }
        //}

        private void AddConcentration(ConcentrationDescription concentrationDescription)
        {
            //добавляем в начало коллекции
            _concentration.Insert(0, concentrationDescription);
            //_concentration.Add(concentrationDescription);
            ResortConcentrationLists();
        }

        private void ResortConcentrationLists()
        {
            //var delta = _concentration.Count - _maxCapacity;
            // возможно понадобиться отсортировать
            //if (delta > 0)
            //{
                _concentration.RemoveRange(_concentration.Skip(_maxCapacity));
            //}
        }

        private void UpdateSpecterBuffer(UpdateGraphData data)
        {
            var areaSeries = PlotHelper.CreateSeries(data.Buffer.ToArray());
            XrayPlot.Series.Clear();
            XrayPlot.Series.Add(areaSeries);

            var areaSeriesList = PlotHelper.CreateDemoSeries(data);
            foreach (var elem in areaSeriesList)
            {
                XrayPlot.Series.Add(elem);
            }

            XrayPlot.InvalidatePlot(true);
        } 
    }
}
