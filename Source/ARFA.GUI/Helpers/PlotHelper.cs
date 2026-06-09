using System.Collections.Generic;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using RIVS.ASAK.ARFA.Application;

namespace RIVS.ASAK.ARFA.GUI.Helpers
{
    internal static class PlotHelper
    {
        static readonly List<OxyColor> graphColors = new List<OxyColor>
        {
            OxyColors.Green
            , OxyColors.Turquoise
            , OxyColors.Blue
            , OxyColors.Yellow
            , OxyColors.Violet
            , OxyColors.DarkSalmon
            , OxyColors.Bisque
        };

        internal static PlotModel CreateModel()
        {
            var plot = new PlotModel();
            plot.PlotAreaBorderColor = OxyColors.White;
            plot.LegendTextColor = OxyColors.White;
            plot.TextColor = OxyColors.White;

            plot.Axes.Add(new LinearAxis
            {
                Minimum = 0,
                //  Maximum = 800, //GURA
                Position = AxisPosition.Left,
                TicklineColor = OxyColors.White

            });

            plot.Axes.Add(new LinearAxis
            {
                Minimum = 0,
                Maximum = 4096,
                // Minimum = 500, //GURA
                //  Maximum = 2600, //GURA
                Position = AxisPosition.Bottom,
                TicklineColor = OxyColors.White

            });

            plot.InvalidatePlot(true);
            return plot;
        }

        internal static AreaSeries CreateSeries(uint[] specterBuffer, int n = 4096)
        {
            var ls = new AreaSeries();

            for (var i = 0; i < n; i++)
            {
                ls.Points.Add(new DataPoint(i, specterBuffer[i]));
            }
            ls.Color = OxyColors.Red;
            ls.Fill = OxyColors.Red;

            return ls;
        }

        internal static IEnumerable<AreaSeries> CreateDemoSeries(UpdateGraphData data)
        {
            var areaSeriesList = new List<AreaSeries>();
            var idxColor = 0;
            foreach (var elem in data.ArfaGraphElement)
            {
                var color = graphColors[idxColor];
                var areaSeries = new AreaSeries { };
                for (var i = elem.Start; i <= elem.Stop; i++)
                {
                    areaSeries.Points.Add(new DataPoint(i, data.Buffer[i]));
                }
                areaSeries.Color = color;
                areaSeries.Fill = color;
                areaSeries.Title = elem.Name + " " + elem.Line;

                areaSeriesList.Add(areaSeries);
                idxColor++;
            }

            return areaSeriesList;
        }
    }
}
