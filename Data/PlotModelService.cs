namespace PenPro.Data;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Annotations;
using OxyPlot.Series;
using OxyPlot.Legends;
using Microsoft.AspNetCore.HttpLogging;

public class PlotModelService
{
    public (PlotModel Legend,PlotModel Borehole) GenerateBoreholeLogs(List<SampleInfo> sample, double yMax)
    {
        var plotModel = new PlotModel();//TitlePosition = 0.5,AxisTitleDistance = 10
        plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Top, Minimum = 0, Maximum = 2,IsZoomEnabled = false, IsPanEnabled = false, FontSize = 14, Title="Soil Strata",TitleFontWeight = FontWeights.Bold});
        plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left,StartPosition = 1,EndPosition = 0,IsZoomEnabled = false, IsPanEnabled = false, Minimum = 0,Maximum = yMax,Title="Penetration (m)",TitleFontWeight = FontWeights.Bold,AxisTitleDistance = 15,FontSize = 14});
        PlotStratigraphy(plotModel, sample,yMax);
        var legendModel = PlotLegends();
        return (legendModel,plotModel);
    }
    private Dictionary<string,(HatchStyle hatchStyle, OxyColor color,string strata)> GetAllLegend()
    {
        Dictionary<string,(HatchStyle hatchStyle, OxyColor color,string strata)> legend = new Dictionary<string, (HatchStyle hatchStyle, OxyColor color, string strata)>()
        {
            {"cpt", new (HatchStyle.Horizontal,OxyColors.Red,"CPT")},
            {"sand", new (HatchStyle.Dashes,OxyColors.Teal,"Sand")},
            {"clayey sand", new (HatchStyle.Dots,OxyColors.RoyalBlue,"Clayey Sand")},
            {"clay", new (HatchStyle.X,OxyColors.ForestGreen,"Clay")},
            {"sandy clay", new (HatchStyle.SquareX,OxyColors.DarkGreen,"Sandy Clay")},
            {"silt", new (HatchStyle.Mixed,OxyColors.Tan,"Silt")},
            {"granite", new (HatchStyle.ForwardDiagonal,OxyColors.DarkRed,"Granite")},
            {"boulders", new (HatchStyle.Square,OxyColors.Navy,"Boulders")},
            {"rock", new (HatchStyle.Cross,OxyColors.YellowGreen,"Rock")},
        };
        return legend;
    }
    private  (HatchStyle hatchStyle, OxyColor color,string strata) GetLegend(string key)
    {
        var legend  = GetAllLegend();
        if (legend.ContainsKey(key))
        {
            return legend[key];
        }
        return new();
    }
    private PlotModel PlotLegends()
    {
        var plotModel = new PlotModel();
        plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Top, Minimum = 0, Maximum = 2,IsZoomEnabled = false, IsPanEnabled = false, Title="Legend",TitleFontWeight = FontWeights.Bold,AxisTitleDistance = 10,FontSize = 13,MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot});
        plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left,StartPosition = 1,EndPosition = 0,IsZoomEnabled = false, IsPanEnabled = false, Minimum = 0,Maximum = 9,Title="Insitu test legend",TitleFontWeight = FontWeights.Bold,AxisTitleDistance = 10,FontSize = 13,MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot});
        var legend = GetAllLegend();

        int count = 0;
        foreach (var item in legend.Values)
        {
                /*AddLegendText(plotModel, item.strata,0,2,position, position += 0.5);//legendary //double position = 0;
                plotModel.Annotations.Add(CreateRectangle(0, 2,position, position += 1, item.color, item.hatchStyle));  */
                AddLegendText(plotModel, item.color, item.hatchStyle, item.strata,0,2,count, count + 1);
                count++;
        }
        return plotModel;
    }
    private void AddLegendText(PlotModel plotModel,OxyColor color, HatchStyle hatchStyle, string text, double minX,double maxX,double minY, double maxY)
    {
        plotModel.Annotations.Add(CreateRectangle(minX, maxX, minY, maxY, color, hatchStyle));
        var textAnnotation = new TextAnnotation
        {
            Text = text,
            TextPosition = new ((minX + maxX) / 2, (minY + maxY) / 2),
            FontSize = 14,
            FontWeight = FontWeights.Bold,
            TextHorizontalAlignment = HorizontalAlignment.Center,
            TextVerticalAlignment = VerticalAlignment.Middle,
            TextColor = OxyColors.Wheat,
            Stroke = OxyColors.Transparent
        };
        plotModel.Annotations.Add(textAnnotation);
    }
    private void PlotStratigraphy(PlotModel plotModel, List<SampleInfo> sample, double maxDepth)
    {
        double gap = 0; double previousStartDepth = 0; double previousEndDepth = 0;
        double x0 = 0; double x1 = 2;
        double maxBoreholeDepth = sample.Max(x => x.BoreholeEndDepth);
        (HatchStyle hatchStyle, OxyColor color,string strata) legend = new (HatchStyle.None,OxyColors.Transparent,"");
        foreach (var data in sample)
        {
            gap = data.BoreholeStartDepth - previousEndDepth;
            if (gap >= 1)//indicates CPT stroke
            {
                legend = GetLegend("cpt");
                plotModel.Annotations.Add(CreateRectangle(x0, x1, previousEndDepth, data.BoreholeStartDepth, legend.color, legend.hatchStyle));
            }
            legend = GetLegend(data.SampleType);
            previousStartDepth = data.BoreholeStartDepth; previousEndDepth = data.BoreholeEndDepth;
            plotModel.Annotations.Add(CreateRectangle(x0, x1, data.BoreholeStartDepth, data.BoreholeEndDepth, legend.color, legend.hatchStyle));
        }
        //This will plot if the composite borehole ends with CPT
        if (maxDepth > maxBoreholeDepth)
        {
            legend = GetLegend("cpt");
            plotModel.Annotations.Add(CreateRectangle(x0, x1, maxBoreholeDepth, maxDepth, legend.color, legend.hatchStyle));
        }
    }
    public PlotModel PlotUnitWeight(List<SampleInfo> sample)
    {
        var plotModel = new PlotModel ();
        plotModel.Axes.Add(new LinearAxis {Minimum = 0, Position = AxisPosition.Top,StartPosition = 0,EndPosition = 1,IsZoomEnabled = false, IsPanEnabled = false, Title="Unit Weight (kN/m3)", MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot,TitleFontWeight = FontWeights.Bold,AxisTitleDistance = 5,FontSize = 13});
        plotModel.Axes.Add(new LinearAxis {Minimum = 0, Position = AxisPosition.Left,StartPosition = 1,EndPosition = 0,IsZoomEnabled = false, IsPanEnabled = false, Title="Penetration (m)", MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot,TitleFontWeight = FontWeights.Bold,AxisTitleDistance = 10,FontSize = 13});
        var legend = new Legend
        {
            LegendTitle = "Legend",
            LegendOrientation = LegendOrientation.Horizontal,
            LegendPosition = LegendPosition.TopCenter,
            LegendPlacement = LegendPlacement.Outside,
            /*LegendBackground = OxyColors.White,*/
            LegendBorder = OxyColors.Black,
            LegendFontSize = 14,
            LegendFontWeight = FontWeights.Bold,
            LegendSymbolLength = 30,
            LegendMargin = 10 
        };
        plotModel.Legends.Add(legend);
        var wetSeries = new ScatterSeries
        {
            MarkerType = MarkerType.Star,
            MarkerSize = 2,
            MarkerStroke = OxyColors.Blue,
            MarkerFill = OxyColors.Blue,
            Title = "wet uw"
        };
        var drySeries = new ScatterSeries
        {
            MarkerType = MarkerType.Circle,
            MarkerSize = 2,
            MarkerStroke = OxyColors.Green,
            MarkerFill = OxyColors.Green,
            Title = "dry uw"
        };
        var subSeries = new ScatterSeries
        {
            MarkerType = MarkerType.Diamond,
            MarkerSize = 2,
            MarkerStroke = OxyColors.Red,
            MarkerFill = OxyColors.Red,
            Title = "sub uw"
        };
        foreach (var sotr in sample)
        {
            foreach (var uw in sotr.Calculus.UnitWeightParamList)
            {
                wetSeries.Points.Add(new ScatterPoint(uw.bulkUnitWeightInKNm3,uw.depth));
                drySeries.Points.Add(new ScatterPoint(uw.dryUnitWeightInKNm3,uw.depth));
                subSeries.Points.Add(new ScatterPoint(uw.submergedDensityInKNm3,uw.depth));
            }
        }
        plotModel.Series.Add(wetSeries);
        plotModel.Series.Add(drySeries);
        plotModel.Series.Add(subSeries);
        return plotModel;  
    }
    public PlotModel PlotWaterContent(List<SampleInfo> sample)
    {
        var plotModel = new PlotModel ();
        plotModel.Axes.Add(new LinearAxis {Minimum = 0, Position = AxisPosition.Top,StartPosition = 0,EndPosition = 1,IsZoomEnabled = false, IsPanEnabled = false, Title="Water Content (%)", MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot,TitleFontWeight = FontWeights.Bold,AxisTitleDistance = 5,FontSize = 13});
        plotModel.Axes.Add(new LinearAxis {Minimum = 0, Position = AxisPosition.Left,StartPosition = 1,EndPosition = 0,IsZoomEnabled = false, IsPanEnabled = false, Title="Penetration (m)", MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot,TitleFontWeight = FontWeights.Bold,AxisTitleDistance = 10,FontSize = 13});
        var wcSeries = new ScatterSeries
        {
            MarkerType = MarkerType.Square,
            MarkerSize = 2,
            MarkerStroke = OxyColors.DarkBlue,
            MarkerFill = OxyColors.DarkBlue,
            Title = "wc (%)"
        };
        foreach (var sotr in sample)
        {
            foreach (var wc in sotr.Calculus.MoistureContentParamList)
            {
                wcSeries.Points.Add(new ScatterPoint(wc.result,wc.depth));
            }
        }
        plotModel.Series.Add(wcSeries);
        return plotModel;  
    }
    public PlotModel PlotConeResistance(List<double> depth,List<double> qc, List<double> qt, List<double> qnet)
    {
        var plotModel = new PlotModel ();
        double xmax = new List<List<double>> {qc,qt,qnet}
                     .SelectMany(x => x)
                     .Max();
        double xMax = Math.Round(xmax,1);
        double yMax = depth.Max();
        plotModel.Axes.Add(new LinearAxis {Minimum = 0, Maximum = xMax, Position = AxisPosition.Top,StartPosition = 0,EndPosition = 1,IsZoomEnabled = false, IsPanEnabled = false, Title="Resistance (MPa)", MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot,TitleFontWeight = FontWeights.Bold,AxisTitleDistance = 5,FontSize = 13});
        plotModel.Axes.Add(new LinearAxis {Minimum = 0, Maximum = yMax, Position = AxisPosition.Left,StartPosition = 1,EndPosition = 0,IsZoomEnabled = false, IsPanEnabled = false, Title="Penetration (m)", MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot,TitleFontWeight = FontWeights.Bold,AxisTitleDistance = 10,FontSize = 13});

        var legend = new Legend
        {
            LegendTitle = "Legend",
            LegendOrientation = LegendOrientation.Horizontal,
            LegendPosition = LegendPosition.RightTop,
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
            Color = OxyColors.Blue,
            MarkerStroke = OxyColors.Blue,
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
    public PlotModel PlotPorePressure(List<double> depth,List<double> u2)
    {
        var plotModel = new PlotModel() { };
        double xMin = Math.Round(u2.Min(),1); double xMax =Math.Round(u2.Max(),1);
        plotModel.Axes.Add(new LinearAxis {Minimum = xMin, Maximum = xMax, Position = AxisPosition.Top,StartPosition = 0,EndPosition = 1,IsZoomEnabled = false, IsPanEnabled = false, Title="Pore Pressure,MPa", MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot,TitleFontWeight = FontWeights.Bold,AxisTitleDistance = 5,FontSize = 13});
        plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left,StartPosition = 1,EndPosition = 0,IsZoomEnabled = false, IsPanEnabled = false, Title="Penetration (m)", MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot,TitleFontWeight = FontWeights.Bold,AxisTitleDistance = 10,FontSize = 13});

        var u2Series = new LineSeries
        {
            MarkerType = MarkerType.Circle,
            MarkerSize = 1,
            Color = OxyColors.Blue,
            Title = "u2"
        };
        for (int i = 0; i < depth.Count; i++)
        {
            u2Series.Points.Add(new DataPoint(u2[i],depth[i]));
        }
        plotModel.Series.Add(u2Series);
        return plotModel;  
    }


    //Testing
    public PlotModel CreatePlotModel()
    {
        var plotModel = new PlotModel {Title = "Borehole Logs"};
        plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Top, Minimum = 0, Maximum = 10, Title="Soil Stratigraphy",MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot });
        plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left,StartPosition = 1,EndPosition = 0, Minimum = 0, Maximum = 15,Title="Penetration (m)",MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot });

        plotModel.Annotations.Add(CreateRectangle(1, 2, 0, 2.5, OxyColors.Red, HatchStyle.Cross));
        plotModel.Annotations.Add(CreateRectangle(1, 2, 2.5, 5, OxyColors.Green, HatchStyle.BackwardDiagonal));
        plotModel.Annotations.Add(CreateRectangle(1, 2, 5, 7.5, OxyColors.Blue, HatchStyle.ForwardDiagonal));
        plotModel.Annotations.Add(CreateRectangle(1, 2, 7.5, 10, OxyColors.Brown, HatchStyle.Horizontal));
        plotModel.Annotations.Add(CreateMixedHatch(1, 2, 10, 14,OxyColors.HotPink));

        plotModel.Annotations.Add(CreateRectangle(4, 5, 0, 2.5, OxyColors.Red, HatchStyle.Cross));
        plotModel.Annotations.Add(CreateRectangle(4, 5, 2.5, 5, OxyColors.Green, HatchStyle.Dots));
        plotModel.Annotations.Add(CreateRectangle(4, 5, 5, 7.5, OxyColors.Blue, HatchStyle.Dashes));
        plotModel.Annotations.Add(CreateRectangle(4, 5, 7.5, 10, OxyColors.Yellow, HatchStyle.Plus));
        plotModel.Annotations.Add(CreateRectangle(4, 5, 10, 14, OxyColors.Brown, HatchStyle.SquareX));
        //plotModel.Annotations.Add(CreateDiagonalSquare(4, 5, 10, 14,OxyColors.Gold));

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
    private CustomMixedHatch CreateMixedHatch(double x0, double x1, double y0, double y1, OxyColor color)
    {
        return new CustomMixedHatch
        {
            MinimumX = x0,
            MaximumX = x1,
            MinimumY = y0,
            MaximumY = y1,
            Fill = color,
            HatchColor = OxyColors.Black
        };
    }
    private DiagonalSquaresHatch CreateDiagonalSquare(double x0, double x1, double y0, double y1, OxyColor color)
    {
        return new DiagonalSquaresHatch
        {
            MinimumX = x0,
            MaximumX = x1,
            MinimumY = y0,
            MaximumY = y1,
            HatchColor = OxyColors.Black
        };
    }
    /*
                double xMax = new List<List<double>> { u2,bq}
                     .SelectMany(x => x)
                     .Max();
        double xMin = new List<List<double>> { u2,bq}
                     .SelectMany(x => x)
                     .Min();
    */
}
