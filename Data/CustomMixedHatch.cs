namespace PenPro.Data;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using System.Collections.Generic;

public class CustomMixedHatch : Annotation
{
    public double MinimumX { get; set; }
    public double MaximumX { get; set; }
    public double MinimumY { get; set; }
    public double MaximumY { get; set; }
    public OxyColor Fill { get; set; } = OxyColors.Transparent; // Default to transparent if not set
    public OxyColor HatchColor { get; set; } = OxyColors.Black;

    private List<Tuple<bool, OxyRect>> hatchElements;

    public CustomMixedHatch()
    {
        this.hatchElements = new List<Tuple<bool, OxyRect>>();
    }

    public override void Render(IRenderContext rc)
    {
        var rect = new OxyRect(Transform(new DataPoint(MinimumX, MaximumY)), Transform(new DataPoint(MaximumX, MinimumY)));
        rc.PushClip(rect);
        // Fill the entire rectangle with the fill color
        rc.DrawRectangle(rect, Fill, OxyColors.Transparent, 0, EdgeRenderingMode.PreferSharpness);

        if (hatchElements.Count == 0)
        {
            GenerateHatchPattern(rect);
        }

        foreach (var element in hatchElements)
        {
            if (element.Item1)
            {
                rc.DrawEllipse(element.Item2, HatchColor, OxyColors.Transparent, 1, EdgeRenderingMode.PreferSharpness);
            }
            else
            {
                rc.DrawLine(new List<ScreenPoint>
                {
                    new ScreenPoint(element.Item2.Left, element.Item2.Top),
                    new ScreenPoint(element.Item2.Right, element.Item2.Top)
                }, HatchColor, 1, EdgeRenderingMode.PreferSharpness);
            }
        }
        rc.PopClip();
    }

    private void GenerateHatchPattern(OxyRect rect)
    {
        double step = 20; // Distance between the centers of the dashes and dots
        double dashLength = 10; // Length of each dash
        double dotRadius = 2; // Radius of each dot
        double dotProbability = 0.8; // 80% probability for dots

        Random rand = new Random();

        for (double y = rect.Top + step / 2; y <= rect.Bottom; y += step)
        {
            for (double x = rect.Left; x <= rect.Right; x += step)
            {
                if (rand.NextDouble() < dotProbability)
                {
                    // Precompute dot
                    var dotRect = new OxyRect(x - dotRadius, y - dotRadius, 2 * dotRadius, 2 * dotRadius);
                    hatchElements.Add(new Tuple<bool, OxyRect>(true, dotRect));
                }
                else
                {
                    // Precompute line
                    double dashEnd = Math.Min(x + dashLength, rect.Right);
                    var lineRect = new OxyRect(x, y, dashEnd - x, 0);
                    hatchElements.Add(new Tuple<bool, OxyRect>(false, lineRect));
                }
            }
        }
    }
}
