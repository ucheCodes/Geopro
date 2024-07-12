namespace PenPro.Data;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using System.Collections.Generic;


public class CustomRectangleAnnotation : RectangleAnnotation
{
    public HatchStyle HatchStyle { get; set; }
    public OxyColor HatchColor { get; set; } = OxyColors.Black;
    //public int FontSize {get;set;} = 14;

    public override void Render(IRenderContext rc)
    {
        base.Render(rc);

        if (HatchStyle != HatchStyle.None)
        {
            var min = new DataPoint(MinimumX, MinimumY);
            var max = new DataPoint(MaximumX, MaximumY);
            var minScreenPoint = Transform(min);
            var maxScreenPoint = Transform(max);
            var rect = new OxyRect(minScreenPoint, maxScreenPoint);

            switch (HatchStyle)
            {
                case HatchStyle.Cross:
                    DrawCrossHatch(rc, rect);
                    break;
                case HatchStyle.BackwardDiagonal:
                    DrawBackwardDiagonalHatch(rc, rect);
                    break;
                case HatchStyle.ForwardDiagonal:
                    DrawForwardDiagonalHatch(rc, rect);
                    break;
                case HatchStyle.Horizontal:
                    DrawHorizontalHatch(rc, rect);
                    break;
                case HatchStyle.Dots:
                    DrawDotsHatch(rc, rect);
                    break;
                case HatchStyle.Dashes:
                    DrawDashesHatch(rc, rect);
                    break;
                case HatchStyle.Plus:
                    DrawPlusHatch(rc, rect);
                    break;
                case HatchStyle.Mixed:
                    DrawMixedHatch(rc, rect);
                    break;
                case HatchStyle.Square:
                    Magic(rc, rect,"square");
                    break;
                case HatchStyle.X:
                    Magic(rc, rect,"X");
                    break;
                case HatchStyle.SquareX:
                    Magic(rc, rect,"both");
                    break;
            }
        }
    }

    private void DrawCrossHatch(IRenderContext rc, OxyRect rect)
    {
        for (double y = rect.Top; y <= rect.Bottom; y += 10)
        {
            rc.DrawLine(new List<ScreenPoint> { new ScreenPoint(rect.Left, y), new ScreenPoint(rect.Right, y) }, HatchColor, 1, EdgeRenderingMode.PreferSharpness);
        }
        for (double x = rect.Left; x <= rect.Right; x += 10)
        {
            rc.DrawLine(new List<ScreenPoint> { new ScreenPoint(x, rect.Top), new ScreenPoint(x, rect.Bottom) }, HatchColor, 1, EdgeRenderingMode.PreferSharpness);
        }
    }

    private void DrawBackwardDiagonalHatch(IRenderContext rc, OxyRect rect)
    {
        double step = 10;
        for (double x = rect.Left - rect.Height; x <= rect.Right; x += step)
        {
            var p1 = new ScreenPoint(x, rect.Bottom);
            var p2 = new ScreenPoint(x + rect.Height, rect.Top);
            ClipLine(ref p1, ref p2, rect);
            rc.DrawLine(new List<ScreenPoint> { p1, p2 }, HatchColor, 1, EdgeRenderingMode.PreferSharpness);
        }
    }

    private void DrawForwardDiagonalHatch(IRenderContext rc, OxyRect rect)
    {
        double step = 10;
        for (double x = rect.Left - rect.Height; x <= rect.Right; x += step)
        {
            var p1 = new ScreenPoint(x, rect.Top);
            var p2 = new ScreenPoint(x + rect.Height, rect.Bottom);
            ClipLine(ref p1, ref p2, rect);
            rc.DrawLine(new List<ScreenPoint> { p1, p2 }, HatchColor, 1, EdgeRenderingMode.PreferSharpness);
        }
    }

    private void DrawHorizontalHatch(IRenderContext rc, OxyRect rect)
    {
        for (double y = rect.Top; y <= rect.Bottom; y += 10)
        {
            rc.DrawLine(new List<ScreenPoint> { new ScreenPoint(rect.Left, y), new ScreenPoint(rect.Right, y) }, HatchColor, 1, EdgeRenderingMode.PreferSharpness);
        }
    }
    private void DrawDotsHatch(IRenderContext rc, OxyRect rect)
    {
        double step = 10; // Distance between dots
        double radius = 2; // Radius of each dot

        for (double y = rect.Top + step / 2; y <= rect.Bottom; y += step)
        {
            for (double x = rect.Left + step / 2; x <= rect.Right; x += step)
            {
                // Create a rectangle for each dot with a small size based on radius
                var dotRect = new OxyRect(x - radius, y - radius, 2 * radius, 2 * radius);
                rc.DrawEllipse(dotRect, HatchColor, OxyColors.Transparent, 1, EdgeRenderingMode.PreferSharpness);
            }
        }
        if(Text != "")
        {
            DrawText(rc, Text, 0);
        }
    }
    private void DrawText(IRenderContext rc, string text, double rotationAngle)
    {
        var midX = (MinimumX + MaximumX) / 2;
        var midY = (MinimumY + MaximumY) / 2;
        var textPosition = Transform(new DataPoint(midX, midY));

        var fontSize = 25;//this.FontSize > 0 ? this.FontSize : 16;
        OxyColor TextColor = OxyColors.Red;
        rc.DrawText(textPosition, text, TextColor, "", fontSize, 800, rotationAngle, HorizontalAlignment.Center, VerticalAlignment.Middle);
    }

    private void DrawDashesHatch0(IRenderContext rc, OxyRect rect)
    {
        double step = 20;
        for (double x = rect.Left + step / 2; x <= rect.Right; x += step)
        {
            rc.DrawLine(new List<ScreenPoint> { new ScreenPoint(x, rect.Top), new ScreenPoint(x, rect.Bottom) }, HatchColor, 1, EdgeRenderingMode.PreferSharpness);
        }
    }
    private void DrawDashesHatch(IRenderContext rc, OxyRect rect)
    {
        double step = 20; // Distance between the centers of the dashes
        double dashLength = 10; // Length of each dash
        double gapLength = 10; // Gap between dashes

        for (double y = rect.Top + step / 2; y <= rect.Bottom; y += step)
        {
            for (double x = rect.Left; x <= rect.Right; x += dashLength + gapLength)
            {
                double dashEnd = Math.Min(x + dashLength, rect.Right);
                rc.DrawLine(new List<ScreenPoint>
                {
                    new ScreenPoint(x, y),
                    new ScreenPoint(dashEnd, y)
                }, HatchColor, 1, EdgeRenderingMode.PreferSharpness);
            }
        }
        if(Text != "")
        {
            DrawText(rc, Text, 0);
        }
    }


    private void DrawPlusHatch2(IRenderContext rc, OxyRect rect)
    {
        double step = 20; // Distance between the centers of the plus signs
        double margin = 5; // Margin to create space between the intersections

        // Draw horizontal lines
        for (double y = rect.Top + step / 2; y <= rect.Bottom; y += step)
        {
            for (double x = rect.Left + margin; x <= rect.Right - margin; x += step)
            {
                rc.DrawLine(new List<ScreenPoint> 
                { 
                    new ScreenPoint(x - step / 2, y), 
                    new ScreenPoint(x + step / 2, y) 
                }, HatchColor, 1, EdgeRenderingMode.PreferSharpness);
            }
        }

        // Draw vertical lines
        for (double x = rect.Left + step / 2; x <= rect.Right; x += step)
        {
            for (double y = rect.Top + margin; y <= rect.Bottom - margin; y += step)
            {
                rc.DrawLine(new List<ScreenPoint> 
                { 
                    new ScreenPoint(x, y - step / 2), 
                    new ScreenPoint(x, y + step / 2) 
                }, HatchColor, 1, EdgeRenderingMode.PreferSharpness);
            }
        }
    }


    private void DrawPlusHatch(IRenderContext rc, OxyRect rect)
    {
        double step = 10;
        for (double y = rect.Top + step / 2; y <= rect.Bottom; y += step)
        {
            rc.DrawLine(new List<ScreenPoint> { new ScreenPoint(rect.Left, y), new ScreenPoint(rect.Right, y) }, HatchColor, 1, EdgeRenderingMode.PreferSharpness);
        }
        for (double x = rect.Left + step / 2; x <= rect.Right; x += step)
        {
            rc.DrawLine(new List<ScreenPoint> { new ScreenPoint(x, rect.Top), new ScreenPoint(x, rect.Bottom) }, HatchColor, 1, EdgeRenderingMode.PreferSharpness);
        }
    }
    private void DrawMixedHatch(IRenderContext rc, OxyRect rect)
    {
        double step = 20; // Distance between the centers of the dashes and dots
        double dashLength = 8; // Length of each dash
        double dotRadius = 2; // Radius of each dot

        for (double y = rect.Top + step / 2; y <= rect.Bottom; y += step)
        {
            for (double x = rect.Left; x <= rect.Right; x += step)
            {
                // Draw dot
                var dotRect = new OxyRect(x - dotRadius, y - dotRadius, 2 * dotRadius, 2 * dotRadius);
                rc.DrawEllipse(dotRect, HatchColor, OxyColors.Transparent, 1, EdgeRenderingMode.PreferSharpness);
                // Draw line
                double dashEnd = Math.Min(x + dashLength, rect.Right);
                rc.DrawLine(new List<ScreenPoint>
                {
                    new ScreenPoint(x, y),
                    new ScreenPoint(dashEnd, y)
                }, HatchColor, 1, EdgeRenderingMode.PreferSharpness);
            }
        }
    }

    private void Magic(IRenderContext rc, OxyRect rect, string displayType)
    {
        double step = 20; // Distance between the centers of the elements
        double squareSize = 5; // Size of the small squares

        // Define the clipping rectangle
        rc.PushClip(rect);

        for (double y = rect.Top + step / 2; y < rect.Bottom; y += step)
        {
            for (double x = rect.Left + step / 2; x < rect.Right; x += step)
            {
                MagicHelper(rc, x, y, step, squareSize,displayType);
            }
        }

        // Pop the clipping rectangle
        rc.PopClip();
    }
    private void MagicHelper(IRenderContext rc,double x, double y, double step, double squareSize, string displayType)
    {
        if (displayType == "both")
        {
            // Create a rectangle for each small square
            var squareRect = new OxyRect(x - squareSize / 2, y - squareSize / 2, squareSize, squareSize);
            rc.DrawRectangle(squareRect, HatchColor, OxyColors.Transparent, 1, EdgeRenderingMode.PreferSharpness);

            // Create bounding rect for the diagonal lines
            var lineRect = new OxyRect(x - step / 2, y - step / 2, step, step);

            // Draw diagonal lines within the clipping bounds
            rc.DrawLine(new List<ScreenPoint>
            {
                new ScreenPoint(lineRect.Left, lineRect.Top),
                new ScreenPoint(lineRect.Right, lineRect.Bottom)
            }, HatchColor, 1, EdgeRenderingMode.PreferSharpness);

            rc.DrawLine(new List<ScreenPoint>
            {
                new ScreenPoint(lineRect.Right, lineRect.Top),
                new ScreenPoint(lineRect.Left, lineRect.Bottom)
            }, HatchColor, 1, EdgeRenderingMode.PreferSharpness);
        }
        else if (displayType == "square")
        {
            // Create a rectangle for each small square
            var squareRect = new OxyRect(x - squareSize / 2, y - squareSize / 2, squareSize, squareSize);
            rc.DrawRectangle(squareRect, HatchColor, OxyColors.Transparent, 1, EdgeRenderingMode.PreferSharpness);
        }
        else if (displayType == "X")
        {
            // Create bounding rect for the diagonal lines
            var lineRect = new OxyRect(x - step / 2, y - step / 2, step, step);
            // Draw diagonal lines within the clipping bounds
            rc.DrawLine(new List<ScreenPoint>
            {
                new ScreenPoint(lineRect.Left, lineRect.Top),
                new ScreenPoint(lineRect.Right, lineRect.Bottom)
            }, HatchColor, 1, EdgeRenderingMode.PreferSharpness);

            rc.DrawLine(new List<ScreenPoint>
            {
                new ScreenPoint(lineRect.Right, lineRect.Top),
                new ScreenPoint(lineRect.Left, lineRect.Bottom)
            }, HatchColor, 1, EdgeRenderingMode.PreferSharpness);
        }
    }



    private void ClipLine(ref ScreenPoint p1, ref ScreenPoint p2, OxyRect rect)
    {
        double x0 = rect.Left;
        double x1 = rect.Right;
        double y0 = rect.Top;
        double y1 = rect.Bottom;

        // Cohen-Sutherland algorithm for line clipping
        int outcode0 = ComputeOutCode(p1, rect);
        int outcode1 = ComputeOutCode(p2, rect);

        bool accept = false;

        while (true)
        {
            if ((outcode0 | outcode1) == 0)
            {
                // Bitwise OR is 0: both points inside window; trivially accept and exit loop
                accept = true;
                break;
            }
            else if ((outcode0 & outcode1) != 0)
            {
                // Bitwise AND is not 0: both points share an outside zone (trivially reject and exit loop)
                break;
            }
            else
            {
                // Failed both tests, so calculate the line segment to clip
                double x, y;

                // At least one endpoint is outside the clip rectangle; pick it.
                int outcodeOut = outcode0 != 0 ? outcode0 : outcode1;

                // Now find the intersection point;
                if ((outcodeOut & 8) != 0)
                {
                    // Point is above the clip rectangle
                    x = p1.X + (p2.X - p1.X) * (y1 - p1.Y) / (p2.Y - p1.Y);
                    y = y1;
                }
                else if ((outcodeOut & 4) != 0)
                {
                    // Point is below the clip rectangle
                    x = p1.X + (p2.X - p1.X) * (y0 - p1.Y) / (p2.Y - p1.Y);
                    y = y0;
                }
                else if ((outcodeOut & 2) != 0)
                {
                    // Point is to the right of clip rectangle
                    y = p1.Y + (p2.Y - p1.Y) * (x1 - p1.X) / (p2.X - p1.X);
                    x = x1;
                }
                else
                {
                    // Point is to the left of clip rectangle
                    y = p1.Y + (p2.Y - p1.Y) * (x0 - p1.X) / (p2.X - p1.X);
                    x = x0;
                }

                // Now we move outside point to intersection point to clip, and get ready for next pass.
                if (outcodeOut == outcode0)
                {
                    p1 = new ScreenPoint(x, y);
                    outcode0 = ComputeOutCode(p1, rect);
                }
                else
                {
                    p2 = new ScreenPoint(x, y);
                    outcode1 = ComputeOutCode(p2, rect);
                }
            }
        }

        if (!accept)
        {
            p1 = p2; // If line is completely outside, make p1 and p2 the same
        }
    }

    private int ComputeOutCode(ScreenPoint p, OxyRect rect)
    {
        int code = 0;

        if (p.X < rect.Left) code |= 1;   // to the left of clip window
        else if (p.X > rect.Right) code |= 2;  // to the right of clip window
        if (p.Y < rect.Top) code |= 4;   // below the clip window
        else if (p.Y > rect.Bottom) code |= 8;  // above the clip window

        return code;
    }
}

public enum HatchStyle
{
    None,
    Cross,
    BackwardDiagonal,
    ForwardDiagonal,
    Horizontal,
    Dots,
    Dashes,
    Plus,
    Mixed,
    Square,
    X,
    SquareX,
}