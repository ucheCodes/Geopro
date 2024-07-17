namespace PenPro.Data;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Annotations;
using OxyPlot.Series;
using OxyPlot.Legends;
using Microsoft.AspNetCore.HttpLogging;

public class PlotModelService
{
    public (PlotModel Legend,PlotModel Borehole) GenerateBoreholeLogs(List<SampleInfo> sample, double yMin, double yMax)
    {
        var plotModel = new PlotModel();//TitlePosition = 0.5,AxisTitleDistance = 10
        plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Top, Minimum = 0, Maximum = 2,IsZoomEnabled = false, IsPanEnabled = false, FontSize = 14, Title="Soil Strata",TitleFontWeight = FontWeights.Bold});
        plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left,StartPosition = 1,EndPosition = 0,IsZoomEnabled = false, IsPanEnabled = false, Minimum = yMin,Maximum = yMax,Title="Penetration (m)",TitleFontWeight = FontWeights.Bold,AxisTitleDistance = 15,FontSize = 14});
        PlotStratigraphy(plotModel, sample,yMax);
        var legendModel = PlotLegends();
        return (legendModel,plotModel);
    }
    private Dictionary<string,(HatchStyle hatchStyle, OxyColor color,string strata)> GetAllLegend()
    {
        Dictionary<string,(HatchStyle hatchStyle, OxyColor color,string strata)> legend = new Dictionary<string, (HatchStyle hatchStyle, OxyColor color, string strata)>()
        {
            {"N/R", new (HatchStyle.Horizontal,OxyColors.Yellow,"N/R")},//no recovery
            {"sand", new (HatchStyle.Dots,OxyColors.Transparent,"Sand")},
            {"clay", new (HatchStyle.Dashes,OxyColors.Transparent,"Clay")},
            {"clayey sand", new (HatchStyle.Dots,OxyColors.RoyalBlue,"Clayey Sand")},
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
        plotModel.Annotations.Add(CreateRectangle("",10,minX, maxX, minY, maxY, color, hatchStyle));
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
       // plotModel.Annotations.Add(textAnnotation);
    }
    public PlotModel PlotStrata(List<SampleInfo> sample, double yMin, double yMax)
    {
        var plotModel = new PlotModel();
        plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Top, Minimum = 0, Maximum = 60,IsZoomEnabled = false, IsPanEnabled = false, FontSize = 14, Title="Soil Strata / Lithology Description",TitleFontWeight = FontWeights.Bold});
        plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left,StartPosition = 1,EndPosition = 0,IsZoomEnabled = false, IsPanEnabled = false, Minimum = yMin,Maximum = yMax,MajorStep = 0.5, Title="Penetration (m)",TitleFontWeight = FontWeights.Bold,AxisTitleDistance = 15,FontSize = 14});
        (HatchStyle hatchStyle, OxyColor color,string strata) legend = new (HatchStyle.None,OxyColors.Transparent,"");
        double x0 = 0; double x1 = 10;
        foreach (var data in sample)
        {
            if(data.BoreholeLogList.Count > 0)
            {
                foreach(var log in data.BoreholeLogList)
                {
                    legend = GetLegend(log.strata);//data.SampleDesc
                    string desc = DissectTextForRectangle(log.description,50);
                    plotModel.Annotations.Add(CreateRectangle(log.strata, 20, x0, x1, log.startDepth, log.endDepth, legend.color, legend.hatchStyle));
                    plotModel.Annotations.Add(CreateRectangle(desc,14, 10, 60, log.startDepth, log.endDepth, OxyColors.Transparent, HatchStyle.None));
                }
            }
            //Console.WriteLine(data.SampleDesc.Length);
            //Console.WriteLine($"{data.BoreholeStartDepth} - {data.BoreholeEndDepth} - {data.SampleDesc}");
        }
        return plotModel;
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
                plotModel.Annotations.Add(CreateRectangle("",10,x0, x1, previousEndDepth, data.BoreholeStartDepth, legend.color, legend.hatchStyle));
            }
            legend = GetLegend(data.SampleType);
            previousStartDepth = data.BoreholeStartDepth; previousEndDepth = data.BoreholeEndDepth;
            plotModel.Annotations.Add(CreateRectangle("",10,x0, x1, data.BoreholeStartDepth, data.BoreholeEndDepth, legend.color, legend.hatchStyle));
        }
        //This will plot if the composite borehole ends with CPT
        if (maxDepth > maxBoreholeDepth)
        {
            legend = GetLegend("cpt");
            plotModel.Annotations.Add(CreateRectangle("",10,x0, x1, maxBoreholeDepth, maxDepth, legend.color, legend.hatchStyle));
        }
    }
    public PlotModel PlotUnitWeight(List<SampleInfo> sample, double yMin, double yMax)
    {
        var plotModel = new PlotModel (); double xMax = 0;
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
                if(uw.depth <= yMax && uw.bulkUnitWeightInKNm3 > xMax){xMax = Math.Round(uw.bulkUnitWeightInKNm3,2);}
            }
        }
        double majorStep = Math.Round(xMax / 2,1);
        plotModel.Axes.Add(new LinearAxis {Minimum = 0, Maximum = xMax,MajorStep = majorStep, Position = AxisPosition.Top,StartPosition = 0,EndPosition = 1,IsZoomEnabled = false, IsPanEnabled = false, Title="Unit Weight (kN/m3)", MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot,TitleFontWeight = FontWeights.Bold,AxisTitleDistance = 5,FontSize = 13});
        plotModel.Axes.Add(new LinearAxis {Minimum = yMin, Maximum = yMax, MinorStep = 0.1, MajorStep = 0.5, Position = AxisPosition.Left,StartPosition = 1,EndPosition = 0,IsZoomEnabled = false, IsPanEnabled = false, Title="Penetration (m)", MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot,TitleFontWeight = FontWeights.Bold,AxisTitleDistance = 10,FontSize = 13});
        //Adding series
        plotModel.Series.Add(wetSeries);
        plotModel.Series.Add(drySeries);
        plotModel.Series.Add(subSeries);
        return plotModel;  
    }
    public PlotModel PlotWaterContent(List<SampleInfo> sample, double yMin, double yMax)
    {
        var plotModel = new PlotModel ();//StringFormat = "0.######",//for correct formatting of decimal places
        plotModel.Axes.Add(new LinearAxis {Minimum = 0, Maximum = 100, MajorStep = 50, Position = AxisPosition.Top,StartPosition = 0,EndPosition = 1,IsZoomEnabled = false, IsPanEnabled = false, Title="Water Content,%", MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot,TitleFontWeight = FontWeights.Bold,AxisTitleDistance = 5,FontSize = 13});
        plotModel.Axes.Add(new LinearAxis {Minimum = yMin, Maximum = yMax,MajorStep = 0.5,MinorStep = 0.1, Position = AxisPosition.Left,StartPosition = 1,EndPosition = 0,IsZoomEnabled = false, IsPanEnabled = false, Title="Penetration (m)", MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot,TitleFontWeight = FontWeights.Bold,AxisTitleDistance = 10,FontSize = 13});
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
    public PlotModel PlotConeResistance(List<double> depth,List<double> qc, List<double> qt, List<double> qnet, double yMin, double yMax)
    {
        var plotModel = new PlotModel ();
        double xMax = 0;
       /* double xmax = new List<List<double>> {qc,}//,qnet}
                     .SelectMany(x => x)
                     .Max();
        double xMax = Math.Round(xmax,1);*/

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
            MarkerSize = 0.7,
            MarkerStroke = OxyColors.Blue,
            MarkerFill = OxyColors.Blue,
            Title = "qc"
        };
        var qnetSeries = new ScatterSeries
        {
            MarkerType = MarkerType.Circle,
            MarkerSize = 0.5,
            MarkerFill = OxyColors.Yellow,
            Title = "qnet"
        };
        var qtSeries = new ScatterSeries
        {
            MarkerType = MarkerType.Square,
            MarkerSize = 0.4,
            MarkerStroke = OxyColors.Red,
            MarkerFill = OxyColors.Red,
            Title = "qt"
        };
        for (int i = 0; i < qt.Count; i++)
        {
            qtSeries.Points.Add(new ScatterPoint(qt[i],depth[i]));
            qcSeries.Points.Add(new ScatterPoint(qc[i],depth[i]));
            qnetSeries.Points.Add(new ScatterPoint(qnet[i],depth[i]));
            if(depth[i] <= yMax && qc[i] > xMax){xMax = Math.Round(qc[i],1);}//keep track of the highest value in each call
        }

        double majorStep = xMax / 2;//Math.Round(xMax / 2,1);
        plotModel.Axes.Add(new LinearAxis {Minimum = 0, Maximum = xMax, MajorStep = majorStep, Position = AxisPosition.Top,StartPosition = 0,EndPosition = 1,IsZoomEnabled = false, IsPanEnabled = false, Title="Tip Resistance (MPa)", MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot,TitleFontWeight = FontWeights.Bold,AxisTitleDistance = 5,FontSize = 13});
        plotModel.Axes.Add(new LinearAxis {Minimum = yMin, Maximum = yMax,MajorStep = 0.5, MinorStep = 0.1,  Position = AxisPosition.Left,StartPosition = 1,EndPosition = 0,IsZoomEnabled = false, IsPanEnabled = false, Title="Penetration (m)", MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot,TitleFontWeight = FontWeights.Bold,AxisTitleDistance = 10,FontSize = 13});
        //adding series
        plotModel.Series.Add(qtSeries);
        plotModel.Series.Add(qcSeries);
        //plotModel.Series.Add(qnetSeries);
        return plotModel;  
    }
    private Legend CreateLegend()
    {
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
        return legend;
    }
    private ScatterSeries CreateScatterSeries(string title, int index)
    {
        ScatterSeries series = new ScatterSeries();
        Dictionary<int,(OxyColor color, MarkerType markerType)> oxyType = new Dictionary<int,(OxyColor color, MarkerType markerType)>()
        {
            {1, new (OxyColors.Red,MarkerType.Circle)},
            {2, new (OxyColors.Blue,MarkerType.Square)},
            {3, new (OxyColors.Green,MarkerType.Diamond)},
            {4, new (OxyColors.Yellow,MarkerType.Circle)},
            {5, new (OxyColors.Red,MarkerType.Square)},
        };
        if(oxyType.ContainsKey(index))
        {
            series = new ScatterSeries
            {
                MarkerType = oxyType[index].markerType,
                MarkerSize = 2,
                MarkerStroke = oxyType[index].color,
                MarkerFill = oxyType[index].color,
                Title = title
            };
        }
        return series;
    }
    public PlotModel PlotSuParameters(Dictionary<string,List<double>> SuParameters, double yMin, double yMax)
    {
        var plotModel = new PlotModel() { };//get the x min and maximum with AI
        var xMax = SuParameters.Values.Max(v => v.Max());
        var xMin = SuParameters.Values.Min(v => v.Min());
        //Console.WriteLine($"from su plot {xMin} - {xMax}");//Minimum = xMin, Maximum = xMax,
        double majorStep = Math.Round(xMax / 2,2); 
        plotModel.Axes.Add(new LinearAxis {Minimum = 0, Maximum = xMax, MajorStep = majorStep, Position = AxisPosition.Top,StartPosition = 0,EndPosition = 1,IsZoomEnabled = false, IsPanEnabled = false, Title="Shear Strength (Su),kPa", MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot,TitleFontWeight = FontWeights.Bold,AxisTitleDistance = 5,FontSize = 13});
        plotModel.Axes.Add(new LinearAxis {Minimum = yMin,Maximum = yMax, MajorStep = 0.5, MinorStep = 0.1, Position = AxisPosition.Left,StartPosition = 1,EndPosition = 0,IsZoomEnabled = false, IsPanEnabled = false, Title="Penetration (m)", MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot,TitleFontWeight = FontWeights.Bold,AxisTitleDistance = 10,FontSize = 13});

        int index = 0;
        List<double> depthList = new List<double>();
        foreach(var kvp in SuParameters)
        {
            if(index > 0)
            {
                var key = kvp.Key;
                var series = CreateScatterSeries(key,index);
                var legend = CreateLegend();
                if(depthList.Count > 0 && depthList.Count >= kvp.Value.Count)
                {
                    for(int v = 0; v < depthList.Count; v++)
                    {
                       if(kvp.Value[v] > 0 && depthList[v] > 0)
                       {
                         series.Points.Add(new ScatterPoint(kvp.Value[v],depthList[v]));
                       }
                    }
                    plotModel.Series.Add(series);
                    plotModel.Legends.Add(legend);      
                }
            }
            else
            {
                depthList = kvp.Value;             
            }
            index++;
        }
        return plotModel;
    }
    public PlotModel PlotSleeveFriction(List<double> depth,List<double> fs, double yMin, double yMax)
    {
        var plotModel = new PlotModel() { };
        double xMax = 0;

        var fsSeries = new ScatterSeries
        {
            MarkerType = MarkerType.Square,
            MarkerSize = 0.5,
            MarkerFill = OxyColors.Red,
            MarkerStroke = OxyColors.Red,
            Title = "fs"
        };
        for (int i = 0; i < depth.Count; i++)
        {
            fsSeries.Points.Add(new ScatterPoint(fs[i],depth[i]));
            if(depth[i] <= yMax && fs[i] > xMax){xMax = Math.Round(fs[i],2);}
        }

        double majorStep = xMax / 2;//Math.Round(xMax / 2,2); 
        plotModel.Axes.Add(new LinearAxis {Maximum = xMax,MajorStep = majorStep, Position = AxisPosition.Top,StartPosition = 0,EndPosition = 1,IsZoomEnabled = false, IsPanEnabled = false, Title="Sleeve Fr,MPa", MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot,TitleFontWeight = FontWeights.Bold,AxisTitleDistance = 5,FontSize = 13});
        plotModel.Axes.Add(new LinearAxis {Minimum = yMin,Maximum = yMax, MinorStep = 0.1, MajorStep = 0.5, Position = AxisPosition.Left,StartPosition = 1,EndPosition = 0,IsZoomEnabled = false, IsPanEnabled = false, Title="Penetration (m)", MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot,TitleFontWeight = FontWeights.Bold,AxisTitleDistance = 10,FontSize = 13});

        //Adding sereis
        plotModel.Series.Add(fsSeries);
        return plotModel;  

        /*
        //alternative code for truncated line
        double interval = 0;
        LineSeries ls = LineSeriesHelper(OxyColors.Red,1);
        double prevDepth = 0;
        for (int i = 0; i < depth.Count; i++)
        {
            interval = depth[i] - prevDepth;
            if(interval >= 0.1)
            {
                double z = interval;
                interval = 0;
                plotModel.Series.Add(ls);
                ls = LineSeriesHelper(OxyColors.Red,1);
            }
            ls.Points.Add(new DataPoint(fs[i],depth[i]));
            if(depth[i] <= yMax && fs[i] > xMax){xMax = fs[i];}
            prevDepth = depth[i];
        }
        plotModel.Series.Add(ls);
        */
    }
    private LineSeries LineSeriesHelper(OxyColor color, double lineThickness)
    {
        var lineSeries = new LineSeries
        {
            MarkerStroke = color,
            MarkerSize = 0.8,
            StrokeThickness = lineThickness,
            Color = color,
        };
        return lineSeries;
    }
    public PlotModel PlotPorePressure(List<double> depth,List<double> u2, double yMin, double yMax)
    {
        var plotModel = new PlotModel() { };
        double xMax = 0; double xMin = 0;

        double interval = 0;
        LineSeries ls = LineSeriesHelper(OxyColors.Blue,1);
        double prevDepth = 0;
        for (int i = 0; i < depth.Count; i++)
        {
            interval = depth[i] - prevDepth;
            if(interval >= 0.05)
            {
                plotModel.Series.Add(ls);
                ls = LineSeriesHelper(OxyColors.Blue,1);
                interval = 0;
            }
            ls.Points.Add(new DataPoint(u2[i],depth[i]));
            if(depth[i] <= yMax && u2[i] > xMax){xMax = u2[i];}
            if(depth[i] <= yMax && u2[i] < xMin){xMin = u2[i];}
            prevDepth = depth[i];
        }

        double majorStep = Math.Round((xMax + xMin) / 2,2); 
        if(majorStep < 0){majorStep = Math.Round(xMax / 2,2); }
        //Console.WriteLine($"{xMin} <--> {xMax} <---> {majorStep}");
        plotModel.Axes.Add(new LinearAxis {Minimum = xMin, Maximum = xMax,MajorStep = majorStep, Position = AxisPosition.Top,StartPosition = 0,EndPosition = 1,IsZoomEnabled = false, IsPanEnabled = false, Title="Pore Pressure,MPa", MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot,TitleFontWeight = FontWeights.Bold,AxisTitleDistance = 5,FontSize = 13});
        plotModel.Axes.Add(new LinearAxis {Minimum = yMin,Maximum = yMax, MajorStep = 0.5, MinorStep = 0.1, Position = AxisPosition.Left,StartPosition = 1,EndPosition = 0,IsZoomEnabled = false, IsPanEnabled = false, Title="Penetration (m)", MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot,TitleFontWeight = FontWeights.Bold,AxisTitleDistance = 10,FontSize = 13});

        //Adding sereis
        plotModel.Series.Add(ls);
        return plotModel;  
    }


    //Testing
    public PlotModel CreateRectangleWithText2(double minX, double maxX, double minY, double maxY)
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
        plotModel.Annotations.Add(CreateRectangle("",16,minX, maxX, minY, maxY, OxyColors.Transparent, HatchStyle.None));

        return plotModel;
    }

    private double EstimateTextWidth(string text, double fontSize)
    {
        return text.Length * fontSize * 0.6;
    }
    private CustomRectangleAnnotation CreateRectangle(string text, int fontsize, double x0, double x1, double y0, double y1, OxyColor fillColor, HatchStyle hatchStyle)
    {
        return new CustomRectangleAnnotation
        {
            MinimumX = x0,
            MaximumX = x1,
            MinimumY = y0,
            MaximumY = y1,
            Text = text,
            Fill = fillColor,
            FontSize = fontsize,
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
    private string DissectTextForRectangle(string text, int textRange)
    {
        string formattedText = "";
        int count  = 0;
        if(text.Length > 10)
        {
            foreach(Char t in text)
            {
                if(count < textRange)
                {
                    formattedText += t;
                }
                else
                {
                    formattedText += $"{t}\n";
                    count = 0;
                }
                count ++;
            }
            //Console.Write($"-{formattedText}-");
            return formattedText;
        }
        return text;
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
