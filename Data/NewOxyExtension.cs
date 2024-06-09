namespace PenPro.Data;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using System.Collections.Generic;

public class DiagonalSquaresHatch : Annotation
{
    public double MinimumX { get; set; }
    public double MaximumX { get; set; }
    public double MinimumY { get; set; }
    public double MaximumY { get; set; }
    public OxyColor HatchColor { get; set; } = OxyColors.Black;

    private List<Tuple<bool, OxyRect>> hatchElements;

    public  DiagonalSquaresHatch()
    {
        this.hatchElements = new List<Tuple<bool, OxyRect>>();
    }

    public override void Render(IRenderContext rc)
    {
        var rect = new OxyRect(Transform(new DataPoint(MinimumX, MaximumY)), Transform(new DataPoint(MaximumX, MinimumY)));

        if (hatchElements.Count == 0)
        {
            GenerateHatchPattern(rect);
        }

        foreach (var element in hatchElements)
        {
            if (element.Item1)
            {
                rc.DrawRectangle(element.Item2, HatchColor, OxyColors.Transparent, 1, EdgeRenderingMode.PreferSharpness);
            }
            else
            {
                rc.DrawLine(new List<ScreenPoint>
                {
                    new ScreenPoint(element.Item2.Left, element.Item2.Top),
                    new ScreenPoint(element.Item2.Right, element.Item2.Bottom)
                }, HatchColor, 1, EdgeRenderingMode.PreferSharpness);

                rc.DrawLine(new List<ScreenPoint>
                {
                    new ScreenPoint(element.Item2.Right, element.Item2.Top),
                    new ScreenPoint(element.Item2.Left, element.Item2.Bottom)
                }, HatchColor, 1, EdgeRenderingMode.PreferSharpness);
            }
        }
    }

    private void GenerateHatchPattern(OxyRect rect)
    {
        double step = 20; // Distance between the centers of the elements
        double squareSize = 5; // Size of the small squares

        for (double y = rect.Top + step / 2; y <= rect.Bottom; y += step)
        {
            for (double x = rect.Left + step / 2; x <= rect.Right; x += step)
            {
                // Create a rectangle for each small square
                var squareRect = new OxyRect(x - squareSize / 2, y - squareSize / 2, squareSize, squareSize);
                hatchElements.Add(new Tuple<bool, OxyRect>(true, squareRect));

                // Create bounding rect for the diagonal lines
                var lineRect = new OxyRect(x - step / 2, y - step / 2, step, step);
                hatchElements.Add(new Tuple<bool, OxyRect>(false, lineRect));
            }
        }
    }
}
