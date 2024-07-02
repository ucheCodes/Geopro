namespace PenPro.Data;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Annotations;
using OxyPlot.Blazor;

static class ChartInitializer
{
    public static Tuple<PlotModel,LineSeries,LinearAxis,LinearAxis> InitLinear 
    (string plotModelTitle, int xStartPosition, int xEndPosition,int yStartPosition, int yEndPosition, string xTitle, string yTitle,double strokeThickness, AxisPosition xPosition, AxisPosition yPosition, OxyColor color)
    {
        var plotModel = new PlotModel {  Title = plotModelTitle};

        var series = new LineSeries
        {
            Title = xTitle,
            MarkerType = MarkerType.Circle,
            MarkerSize = 1,
            Color = color,
            StrokeThickness = strokeThickness
        };
        plotModel.Series.Add(series);

        var XAxis = new LinearAxis(){Position = xPosition, StartPosition = xStartPosition, EndPosition = xEndPosition,IsZoomEnabled = false, IsPanEnabled = false,  MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot, Title = xTitle};
        plotModel.Axes.Add(XAxis);

        var YAxis = new LinearAxis(){Position = yPosition, StartPosition = yStartPosition, EndPosition = yEndPosition,IsZoomEnabled = false, IsPanEnabled = false,  MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot, Title = yTitle};
        plotModel.Axes.Add(YAxis);

        return new (plotModel,series,XAxis,YAxis);
    }

        public static Tuple<PlotModel,ScatterSeries,LinearAxis,LinearAxis> InitScatter
    (string plotModelTitle, int xStartPosition, int xEndPosition,int yStartPosition, int yEndPosition, string xTitle, string yTitle,double strokeThickness, AxisPosition xPosition, AxisPosition yPosition, OxyColor color)
    {
        var plotModel = new PlotModel {  Title = plotModelTitle};

        var series = new ScatterSeries
        {
            Title = xTitle,
            MarkerType = MarkerType.Circle,
            MarkerSize = 1,
            MarkerStroke = color,
            MarkerFill = color,
        };
        plotModel.Series.Add(series);

        var XAxis = new LinearAxis(){Position = xPosition, StartPosition = xStartPosition, EndPosition = xEndPosition,IsZoomEnabled = false, IsPanEnabled = false,  MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot, Title = xTitle};
        plotModel.Axes.Add(XAxis);

        var YAxis = new LinearAxis(){Position = yPosition, StartPosition = yStartPosition, EndPosition = yEndPosition,IsZoomEnabled = false, IsPanEnabled = false,  MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot, Title = yTitle};
        plotModel.Axes.Add(YAxis);

        return new (plotModel,series,XAxis,YAxis);
    }

        public static Tuple<PlotModel,LineSeries,LogarithmicAxis,LogarithmicAxis> InitLog 
    (string plotModelTitle, int xStartPosition, int xEndPosition,int yStartPosition, int yEndPosition, string xTitle, string yTitle,double strokeThickness, AxisPosition xPosition, AxisPosition yPosition, OxyColor color)
    {
        var plotModel = new PlotModel {  Title = plotModelTitle};

        var series = new LineSeries
        {
            Title = xTitle,
            MarkerType = MarkerType.Circle,
            MarkerSize = 1,
            Color = color,
            StrokeThickness = strokeThickness
        };
        plotModel.Series.Add(series);

        var XAxis = new LogarithmicAxis(){Position = xPosition, StartPosition = xStartPosition, EndPosition = xEndPosition,IsZoomEnabled = false, IsPanEnabled = false,  MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot, Title = xTitle};
        plotModel.Axes.Add(XAxis);


        var YAxis = new LogarithmicAxis(){Position = yPosition, StartPosition = yStartPosition, EndPosition = yEndPosition,IsZoomEnabled = false, IsPanEnabled = false,  MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot, Title = yTitle};
        plotModel.Axes.Add(YAxis);
        return new (plotModel,series,XAxis,YAxis);
    }
    public static (PlotModel,LineSeries,LinearAxis,LinearAxis) InitializeLinear(string title, int xStartPosition, int xEndPosition,int yStartPosition, int yEndPosition, string xTitle, string yTitle,double strokeThickness, AxisPosition xPosition, AxisPosition yPosition, OxyColor color,double xmin, double xmax)
    {
        PlotModel plotModel= new PlotModel(){Title = title};
        var XAxis = new LinearAxis(){Minimum = xmin, Maximum = xmax, Position = xPosition, StartPosition = xStartPosition, EndPosition = xEndPosition,IsZoomEnabled = false, IsPanEnabled = false,  MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot, Title = xTitle};
        plotModel.Axes.Add(XAxis);
        var YAxis = new LinearAxis(){Position = yPosition, StartPosition = yStartPosition, EndPosition = yEndPosition,IsZoomEnabled = false, IsPanEnabled = false,  MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot, Title = yTitle};
        plotModel.Axes.Add(YAxis);

        var series = new LineSeries
        {
            Title = xTitle,
            MarkerType = MarkerType.Circle,
            MarkerSize = 1,
            Color = color,
            StrokeThickness = strokeThickness
        };
        plotModel.Series.Add(series);
        return (plotModel,series, XAxis,YAxis);
    }
    public static LineSeries CreateNewLineSeries(OxyColor color, int strokeThickness)
    {
        var series = new LineSeries
        {
            //Title = xTitle,
            MarkerType = MarkerType.Circle,
            MarkerSize = 1,
            Color = color,
            StrokeThickness = strokeThickness
        };
        return series;
    }
}