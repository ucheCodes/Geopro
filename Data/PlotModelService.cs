namespace PenPro.Data;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Annotations;
using OxyPlot.Series;
using OxyPlot.Legends;

public class PlotModelService
{
    public (PlotModel Legend,PlotModel Borehole) GenerateBoreholeLogs(List<SampleInfo> sample, double yMax)
    {
        var plotModel = new PlotModel();
        plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Top, Minimum = 0, Maximum = 2, Title="Strata"});
        plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left,StartPosition = 1,EndPosition = 0, Minimum = 0,Maximum = yMax,Title="Penetration (m)"});
        PlotStratigraphy(plotModel, sample,yMax);
        var legendModel = PlotLegends();
        return (legendModel,plotModel);
    }
    private PlotModel PlotLegends()
    {
        var plotModel = new PlotModel();
        plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Top, Minimum = 0, Maximum = 2, Title="Legend"});
        plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left,StartPosition = 1,EndPosition = 0, Minimum = 0,Maximum = 10,Title="Insitu test legend"});
        List<(HatchStyle hatchStyle, OxyColor color,string strata)> legend = new  List<(HatchStyle hatchStyle, OxyColor color, string strata)>()
        {
            new (HatchStyle.ForwardDiagonal,OxyColors.Red,"CPT"),
            /*new (HatchStyle.Dashes,OxyColors.Blue,"Sand"),
            new (HatchStyle.Dots,OxyColors.Green,"Clay"),*/
        };
        //double position = 0;
        int count =0;
        foreach (var item in legend)
        {
                /*AddLegendText(plotModel, item.strata,0,2,position, position += 0.5);//legendary
                plotModel.Annotations.Add(CreateRectangle(0, 2,position, position += 1, item.color, item.hatchStyle));  */

                AddLegendText(plotModel, item.color, item.hatchStyle, item.strata,0,2,count, count + 1);
                //plotModel.Annotations.Add(CreateRectangle(0, 2,count, count + 1, item.color, item.hatchStyle)); 
                count++;
        }
        return plotModel;
    }
    private void AddLegendText(PlotModel plotModel,OxyColor color, HatchStyle hatchStyle, string text, double minX,double maxX,double minY, double maxY)
    {
        var textAnnotation = new TextAnnotation
        {
            Text = text,
            TextPosition = new ((minX + maxX) / 2, (minY + maxY) / 2),
            FontSize = 14,
            FontWeight = FontWeights.Bold,
            TextHorizontalAlignment = HorizontalAlignment.Center,
            TextVerticalAlignment = VerticalAlignment.Middle,
            TextColor = OxyColors.Black,
            Stroke = OxyColors.Transparent
        };
        plotModel.Annotations.Add(textAnnotation);
        plotModel.Annotations.Add(CreateRectangle(minX, maxX, minY, maxY, color, hatchStyle));
    }
    private void PlotStratigraphy(PlotModel plotModel, List<SampleInfo> sample, double maxDepth)
    {
        OxyColor color; HatchStyle hatchStyle;
        double gap = 0; double previousStartDepth = 0; double previousEndDepth = 0;
        double x0 = 0; double x1 = 2;
        double maxBoreholeDepth = sample.Max(x => x.BoreholeEndDepth);
        foreach (var data in sample)
        {
            gap = data.BoreholeStartDepth - previousEndDepth;
            if (gap >= 1)//indicates CPT stroke
            {
                color = OxyColors.Red; hatchStyle = HatchStyle.ForwardDiagonal;
                plotModel.Annotations.Add(CreateRectangle(x0, x1, previousEndDepth, data.BoreholeStartDepth, color, hatchStyle));
            }
            else if(gap >= 2){}
            switch (data.SampleType)
            {
                case "sand"://add others
                    color = OxyColors.Blue;
                    hatchStyle = HatchStyle.Dashes;
                break;
                case "clay":
                    color = OxyColors.Green;
                    hatchStyle = HatchStyle.Dots;
                break;
                default:
                color = OxyColors.Transparent;
                hatchStyle = HatchStyle.None;
                break;
            }
            previousStartDepth = data.BoreholeStartDepth; previousEndDepth = data.BoreholeEndDepth;
            plotModel.Annotations.Add(CreateRectangle(x0, x1, data.BoreholeStartDepth, data.BoreholeEndDepth, color, hatchStyle));
        }
        //This will plot if the composite borehole ends with CPT
        if (maxDepth > maxBoreholeDepth)
        {
            color = OxyColors.Red; hatchStyle = HatchStyle.ForwardDiagonal;
            plotModel.Annotations.Add(CreateRectangle(x0, x1, maxBoreholeDepth, maxDepth, color, hatchStyle));
        }
    }
    public PlotModel PlotConeResistance(List<double> depth,List<double> qc, List<double> qt, List<double> qnet)
    {
        var plotModel = new PlotModel {Title = "Cone Resistance"};
        plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Top,StartPosition = 0,EndPosition = 1,Title="Resistance (MPa)", MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot});
        plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left,StartPosition = 1,EndPosition = 0,Title="Penetration (m)", MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot});

        var legend = new Legend
        {
            LegendTitle = "Legend",
            LegendOrientation = LegendOrientation.Horizontal,
            LegendPosition = LegendPosition.TopRight,
            LegendPlacement = LegendPlacement.Inside,
            /*LegendBackground = OxyColors.White,*/
            LegendBorder = OxyColors.Black,
            LegendFontSize = 14,
            LegendFontWeight = FontWeights.Bold,
            LegendSymbolLength = 30,
            LegendMargin = 10 
        };
        plotModel.Legends.Add(legend);

        var qcSeries = new ScatterSeries
        {
            MarkerType = MarkerType.Circle,
            MarkerSize = 2,
            MarkerFill = OxyColors.Red,
            Title = "qc"
        };
        var qnetSeries = new ScatterSeries
        {
            MarkerType = MarkerType.Circle,
            MarkerSize = 1.5,
            MarkerFill = OxyColors.Yellow,
            Title = "qnet"
        };
        var qtSeries = new LineSeries
        {
            MarkerType = MarkerType.Square,
            MarkerSize = 1,
            MarkerFill = OxyColors.Blue,
            Title = "qt"
        };
        for (int i = 0; i < qt.Count; i++)
        {
            qcSeries.Points.Add(new ScatterPoint(qc[i],depth[i]));
            qtSeries.Points.Add(new DataPoint(qt[i],depth[i]));
            qnetSeries.Points.Add(new ScatterPoint(qnet[i],depth[i]));
        }
        plotModel.Series.Add(qcSeries);
        plotModel.Series.Add(qtSeries);
        plotModel.Series.Add(qnetSeries);
        return plotModel;  
    }
    //Testing
    public PlotModel CreatePlotModel()
    {
        var plotModel = new PlotModel {Title = "Borehole Logs"};
        plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Top, Minimum = 0, Maximum = 10, Title="Soil Stratigraphy",MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot });
        plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left,StartPosition = 1,EndPosition = 0, Minimum = 0, Maximum = 10,Title="Penetration (m)",MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot });

        plotModel.Annotations.Add(CreateRectangle(1, 2, 0, 2.5, OxyColors.Red, HatchStyle.Cross));
        plotModel.Annotations.Add(CreateRectangle(1, 2, 2.5, 5, OxyColors.Green, HatchStyle.BackwardDiagonal));
        plotModel.Annotations.Add(CreateRectangle(1, 2, 5, 7.5, OxyColors.Blue, HatchStyle.ForwardDiagonal));
        plotModel.Annotations.Add(CreateRectangle(1, 2, 7.5, 10, OxyColors.Yellow, HatchStyle.Horizontal));

        plotModel.Annotations.Add(CreateRectangle(4, 5, 0, 2.5, OxyColors.Red, HatchStyle.Cross));
        plotModel.Annotations.Add(CreateRectangle(4, 5, 2.5, 5, OxyColors.Green, HatchStyle.Dots));
        plotModel.Annotations.Add(CreateRectangle(4, 5, 5, 7.5, OxyColors.Blue, HatchStyle.Dashes));
        plotModel.Annotations.Add(CreateRectangle(4, 5, 7.5, 10, OxyColors.Yellow, HatchStyle.Plus));

        return plotModel;
    }

    public PlotModel CreateRectangleWithText(double minX, double maxX, double minY, double maxY)
    {
        var plotModel = new PlotModel { Title = "Rectangles with Hatches" };
        plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Top, Minimum = 0, Maximum = 10 });
        plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, StartPosition = 1, EndPosition = 0, Minimum = 0, Maximum = 10 });

        string text = "This is a text inside the rectangle that's mean't to stay within the bounds of the rectangle and not overflow";
        double fontSize = 16;
        double rectWidth = maxX - minX;
        double estimatedTextWidth = EstimateTextWidth(text, fontSize);

        while (estimatedTextWidth > rectWidth && fontSize > 1)
        {
            fontSize -= 0.5;
            estimatedTextWidth = EstimateTextWidth(text, fontSize);
        }

        var textAnnotation = new TextAnnotation
        {
            Text = text,
            TextPosition = new DataPoint((minX + maxX) / 2, (minY + maxY) / 2),
            FontSize = fontSize,
            TextHorizontalAlignment = HorizontalAlignment.Center,
            TextVerticalAlignment = VerticalAlignment.Middle,
            Stroke = OxyColors.Transparent
        };

        plotModel.Annotations.Add(textAnnotation);
        plotModel.Annotations.Add(CreateRectangle(minX, maxX, minY, maxY, OxyColors.Transparent, HatchStyle.None));

        return plotModel;
    }

    private double EstimateTextWidth(string text, double fontSize)
    {
        // This is a simple approximation. You may need to adjust the factor for more accuracy.
        return text.Length * fontSize * 0.6;
    }
    private CustomRectangleAnnotation CreateRectangle(double x0, double x1, double y0, double y1, OxyColor fillColor, HatchStyle hatchStyle)
    {
        return new CustomRectangleAnnotation
        {
            MinimumX = x0,
            MaximumX = x1,
            MinimumY = y0,
            MaximumY = y1,
            Fill = fillColor,
            Stroke = OxyColors.Black,
            StrokeThickness = 1,
            HatchStyle = hatchStyle,
            HatchColor = OxyColors.Black
        };
    }
}
