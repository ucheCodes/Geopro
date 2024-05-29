namespace PenPro.Data;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Annotations;
using OxyPlot.Blazor;

static class ChartInitializer
{
    public static Tuple<PlotModel,LineSeries> Init //PlotModel plotModel, LineSeries series,
    (string plotModelTitle, bool isXAxisLog,
     bool isYAxisLog, int xStartPosition, int xEndPosition,int yStartPosition, int yEndPosition, string xTitle, string yTitle,double strokeThickness, AxisPosition xPosition, AxisPosition yPosition, OxyColor color)
    {
        var plotModel = new PlotModel {  Title = plotModelTitle};
        if (isXAxisLog)
        {
            var XAxis = new LogarithmicAxis(){Position = xPosition, StartPosition = xStartPosition, EndPosition = xEndPosition,  MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot, Title = xTitle};
            plotModel.Axes.Add(XAxis);
        }
        else
        {
            var XAxis = new LinearAxis(){Position = xPosition, StartPosition = xStartPosition, EndPosition = xEndPosition,  MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot, Title = xTitle};
            plotModel.Axes.Add(XAxis);
        }
        if (isYAxisLog)
        {
            var YAxis = new LogarithmicAxis(){Position = yPosition, StartPosition = yStartPosition, EndPosition = yEndPosition,  MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot, Title = yTitle};
            plotModel.Axes.Add(YAxis);
        }
        else
        {
            var YAxis = new LinearAxis(){Position = yPosition, StartPosition = yStartPosition, EndPosition = yEndPosition,  MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot, Title = yTitle};
            plotModel.Axes.Add(YAxis);
        }
        var series = new LineSeries
        {
            Title = xTitle,
            MarkerType = MarkerType.Circle,
            MarkerSize = 1,
            Color = color,
            StrokeThickness = strokeThickness
        };
        plotModel.Series.Add(series);
        return new (plotModel,series);
    }
}