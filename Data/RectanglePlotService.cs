namespace PenPro.Data;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Annotations;
using OxyPlot.Series;
using OxyPlot.Legends;

interface IRectanglePlotService
{
    PlotModel PlotSampleInfo(List<SampleInfo> sample, double maxBoreholeDepth);
}
class RectanglePlotService //: IRectanglePlotService
{
    public PlotModel PlotSampleInfo(List<SampleInfo> sample, double maxBoreholeDepth)
    {
        var plotModel = new PlotModel();//TitlePosition = 0.5,AxisTitleDistance = 10
        plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Top, Minimum = 0, Maximum = 7,IsZoomEnabled = false, IsPanEnabled = false, FontSize = 14, Title="Insitu Test",TitleFontWeight = FontWeights.Bold});
        plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left,StartPosition = 1,EndPosition = 0,IsZoomEnabled = false, IsPanEnabled = false, Minimum = 0,Maximum = maxBoreholeDepth,Title="Penetration (m)",TitleFontWeight = FontWeights.Bold,AxisTitleDistance = 15,FontSize = 14});
        foreach (var data in sample)
        {
            string text = $"{data.TestId} / {data.SamplingTool}";
            CreateRectangleWithText(plotModel,text, 2, 7, data.BoreholeStartDepth, data.BoreholeEndDepth,OxyColors.Transparent, OxyColors.Black);//description fot the sotr
            switch(data.SampleType)
            {
                case "sample":
                    CreateRectangle(plotModel, 1,2,data.BoreholeStartDepth,data.BoreholeEndDepth, OxyColors.DarkBlue,OxyColors.Black);
                    DrawSubSamples(plotModel,data.SubSampleInCsv,data.BoreholeStartDepth);
                break;
                case "cpt":
                    DrawRectangleArrowHead(plotModel, 1,2,data.BoreholeStartDepth,data.BoreholeEndDepth, OxyColors.LightGray, OxyColors.Black,ArrowDirection.down);
                break;
            }
            if(data.SampleDrillOut > 0)
            {
                 CreateRectangle(plotModel, 1,2,data.BoreholeEndDepth,data.BoreholeEndDepth + data.SampleDrillOut, OxyColors.Red,OxyColors.Black);
            }
        }
        return plotModel;
    }

    private void DrawSubSamples(PlotModel plotModel,string subSampleCsv, double sampleStartDepth)
    {
        try
        {
            if(subSampleCsv != "")
            {
                string[] array = subSampleCsv.Split(",");
                if(array.Length > 1)
                {
                    double startDepth = 0;
                    double.TryParse(array[0], out startDepth);
                    for(int i = 1; i < array.Length;i++)//loop from index 1 since i've set index 0 as start
                    {
                        double endDepth = 0;
                        if(double.TryParse(array[i], out endDepth))
                        {
                            CreateRectangle(plotModel, 0.1,0.99,startDepth,endDepth, OxyColors.Black,OxyColors.LightGray);
                            startDepth = endDepth;
                        }
                    }
                }
        }
        }
        catch{}
    }
    public void DrawRectangleArrowHeadAndText(PlotModel plotModel,double x0, double x1, double y0, double y1, OxyColor fillColor, OxyColor lineColor,OxyColor textColor, string text)
    {
        //valid and working
        var customAnnotation = new CustomRectangleWithArrowAndTextAnnotation
        {
            MinimumX = x0,
            MaximumX = x1,
            MinimumY = y0,
            MaximumY = y1,
            FontSize = 16,
            Text = text,
            TextRotationAngle = 90,
            FillColor = fillColor,
            LineColor = lineColor,
            TextColor = textColor,
            LineThickness = 2,
            ArrowHeadLength = 15
        };
        plotModel.Annotations.Add(customAnnotation);
    }
    private void DrawRectangleArrowHead(PlotModel plotModel,double x0, double x1, double y0, double y1, OxyColor fillColor, OxyColor lineColor, ArrowDirection direction)
    {
        //valid and working
        var customAnnotation = new CustomRectangleWithArrowAnnotation
        {
            MinimumX = x0,
            MaximumX = x1,
            MinimumY = y0,
            MaximumY = y1,
            FontSize = 16,
            FillColor = fillColor,
            LineColor = lineColor,
            LineThickness = 1.5,
            ArrowHeadLength = 9,
            ArrowDirection = direction
        };
        plotModel.Annotations.Add(customAnnotation);
    }
    private void CreateRectangle(PlotModel plotModel, double x0, double x1, double y0, double y1,OxyColor fillColor, OxyColor strokeColor)
    {
        var rectangleAnnotation = new RectangleAnnotation
        {
            MinimumX = x0,
            MaximumX = x1,
            MinimumY = y0,
            MaximumY = y1,
            Fill = fillColor,
            Stroke = strokeColor,
            StrokeThickness = 1
        };
        plotModel.Annotations.Add(rectangleAnnotation); 
    }
    public void CreateRectangleWithText(PlotModel plotModel,string text, double x0, double x1, double y0, double y1,OxyColor fillColor, OxyColor textColor)
    {
                // Add a rectangle annotation
        var rectangleAnnotation = new RectangleAnnotation
        {
            MinimumX = x0,
            MaximumX = x1,
            MinimumY = y0,
            MaximumY = y1,
            Fill = fillColor,
            FontSize = 10,
            Stroke = OxyColors.Black,
            StrokeThickness = 1
        };
        plotModel.Annotations.Add(rectangleAnnotation);

        // Add a text annotation
        var textAnnotation = new TextAnnotation
        {
            Text = text,
            TextPosition = new DataPoint((x0 + x1) / 2, 
                                         (y0 + y1) / 2),
            FontSize = 10,
            TextColor = textColor,
            TextHorizontalAlignment = HorizontalAlignment.Center,
            TextVerticalAlignment = VerticalAlignment.Middle,
            Stroke = OxyColors.Transparent
        };
        plotModel.Annotations.Add(textAnnotation);
    }
}
