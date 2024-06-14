namespace PenPro.Data;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using System.Collections.Generic;

public enum ArrowDirection{up,down}
public class CustomTextAnnotation : Annotation
{
    public string Text { get; set; } = "";
    public RectangleAnnotation? Rectangle { get; set; }
    public new double FontSize { get; set; } = 0;
    public new OxyColor TextColor { get; set; } =  OxyColors.Transparent;
    public int RotationAngle { get; set; }

    public override void Render(IRenderContext rc)
    {
        if (Rectangle == null) return;

        var midX = (Rectangle.MinimumX + Rectangle.MaximumX) / 2;
        var midY = (Rectangle.MinimumY + Rectangle.MaximumY) / 2;
        var textPosition = Transform(new DataPoint(midX, midY));

        var text = this.Text ?? string.Empty;
        var fontSize = this.FontSize > 0 ? this.FontSize : 12;

        rc.DrawText(textPosition, text, TextColor, "", FontSize, 500, RotationAngle, HorizontalAlignment.Center,VerticalAlignment.Middle);
    }
}

public class CustomRectangleWithArrowAndTextAnnotation : Annotation
{
    public double MinimumX { get; set; }
    public double MaximumX { get; set; }
    public double MinimumY { get; set; }
    public double MaximumY { get; set; }
    public string Text { get; set; } = string.Empty;  
    public double TextRotationAngle { get; set; }  
    public OxyColor FillColor { get; set; } = OxyColors.Black;
    public OxyColor LineColor { get; set; } = OxyColors.Red;
    public new OxyColor TextColor { get; set; } = OxyColors.White; // Changed default to white for visibility
    public double LineThickness { get; set; } = 1;
    public double ArrowHeadLength { get; set; } = 10;
    public new double FontSize { get; set; } = 12;

    public override void Render(IRenderContext rc)
    {
        var rect = new OxyRect(Transform(new DataPoint(MinimumX, MaximumY)), Transform(new DataPoint(MaximumX, MinimumY)));

        // Draw the rectangle
        rc.DrawRectangle(rect, FillColor, OxyColors.Transparent, 1, EdgeRenderingMode.PreferSharpness);

        // Calculate the center point of the rectangle
        double centerX = (MinimumX + MaximumX) / 2.05;
        double centerYTop = MaximumY;
        double centerYBottom = MinimumY;

        var screenCenterTop = Transform(new DataPoint(centerX, centerYTop));
        var screenCenterBottom = Transform(new DataPoint(centerX, centerYBottom));

        // Draw the vertical line
        rc.DrawLine(
            new List<ScreenPoint> { screenCenterTop, screenCenterBottom },
            LineColor,
            LineThickness,
            EdgeRenderingMode.PreferSharpness);

        // Draw the arrowhead at the top
        DrawArrowHead(rc, screenCenterTop, ArrowHeadLength, LineThickness);
        
        // Draw the text
        DrawText(rc, Text, TextRotationAngle);
    }

    private void DrawArrowHead(IRenderContext rc, ScreenPoint point, double length, double thickness)
    {
        double angle = 45; // Arrowhead angle
        double radians = angle * (Math.PI / 180);

        // Calculate the points for the arrowhead lines
        var leftPoint = new ScreenPoint(
            point.X - length * Math.Cos(radians),
            point.Y + length * Math.Sin(radians));

        var rightPoint = new ScreenPoint(
            point.X + length * Math.Cos(radians),
            point.Y + length * Math.Sin(radians));

        // Draw the arrowhead lines
        rc.DrawLine(
            new List<ScreenPoint> { point, leftPoint },
            LineColor,
            thickness,
            EdgeRenderingMode.PreferSharpness);

        rc.DrawLine(
            new List<ScreenPoint> { point, rightPoint },
            LineColor,
            thickness,
            EdgeRenderingMode.PreferSharpness);
    }

    private void DrawText(IRenderContext rc, string text, double rotationAngle)
    {   
        var midX = (MinimumX + MaximumX) / 2;
        var midY = (MinimumY + MaximumY) / 2;
        var textPosition = Transform(new DataPoint(midX, midY));

        rc.DrawText(
            textPosition,
            text,
            TextColor,
            "Arial", // Font family (you can specify a different one)
            FontSize,
            500,
            rotationAngle,
            HorizontalAlignment.Center,
            VerticalAlignment.Middle);
    }
}

//Let's hope this will do

public class CustomRectangleWithArrowAnnotation : Annotation
{
    public double MinimumX { get; set; }
    public double MaximumX { get; set; }
    public double MinimumY { get; set; }
    public double MaximumY { get; set; }
    public string Text { get; set; } = string.Empty;
    public double TextRotationAngle { get; set; }
    public OxyColor FillColor { get; set; } = OxyColors.Black;
    public OxyColor LineColor { get; set; } = OxyColors.Red;
    public new OxyColor TextColor { get; set; } = OxyColors.Black;
    public double LineThickness { get; set; } = 1;
    public double ArrowHeadLength { get; set; } = 10;
    public ArrowDirection ArrowDirection { get; set; }

    public override void Render(IRenderContext rc)
    {
        var rect = new OxyRect(Transform(new DataPoint(MinimumX, MaximumY)), Transform(new DataPoint(MaximumX, MinimumY)));

        // Draw the rectangle
        rc.DrawRectangle(rect, FillColor, OxyColors.Transparent, 1, EdgeRenderingMode.PreferSharpness);

        // Calculate the center point of the rectangle
        double centerX = (MinimumX + MaximumX) / 2;
        double centerYTop = MaximumY;
        double centerYBottom = MinimumY;

        var screenCenterTop = Transform(new DataPoint(centerX, centerYTop));
        var screenCenterBottom = Transform(new DataPoint(centerX, centerYBottom));

        // Ensure the line is drawn from bottom to top
        if (ArrowDirection == ArrowDirection.up)
        {
            if (screenCenterTop.Y < screenCenterBottom.Y)
            {
                var temp = screenCenterTop;
                screenCenterTop = screenCenterBottom;
                screenCenterBottom = temp;
            }

            rc.DrawLine(
                new List<ScreenPoint> { screenCenterBottom, screenCenterTop },
                LineColor,
                LineThickness,
                EdgeRenderingMode.PreferSharpness);

            DrawArrowHeadUp(rc, screenCenterTop, ArrowHeadLength, LineThickness); // Draw at the top
        }
        else if (ArrowDirection == ArrowDirection.down)
        {
            if (screenCenterTop.Y > screenCenterBottom.Y)
            {
                var temp = screenCenterTop;
                screenCenterTop = screenCenterBottom;
                screenCenterBottom = temp;
            }

            rc.DrawLine(
                new List<ScreenPoint> { screenCenterTop, screenCenterBottom },
                LineColor,
                LineThickness,
                EdgeRenderingMode.PreferSharpness);

            DrawArrowHeadDown(rc, screenCenterBottom, ArrowHeadLength, LineThickness); // Draw at the bottom
        }

        // Draw the text
        DrawText(rc, Text, TextRotationAngle);
    }

    private void DrawArrowHeadUp(IRenderContext rc, ScreenPoint point, double length, double thickness)
    {
        double angle = 45; // Arrowhead angle
        double radians = angle * (Math.PI / 180);

        // Calculate the points for the arrowhead lines
        var leftPoint = new ScreenPoint(
            point.X - length * Math.Cos(radians),
            point.Y + length * Math.Sin(radians));

        var rightPoint = new ScreenPoint(
            point.X + length * Math.Cos(radians),
            point.Y + length * Math.Sin(radians));

        // Draw the arrowhead lines
        rc.DrawLine(
            new List<ScreenPoint> { point, leftPoint },
            LineColor,
            thickness,
            EdgeRenderingMode.PreferSharpness);

        rc.DrawLine(
            new List<ScreenPoint> { point, rightPoint },
            LineColor,
            thickness,
            EdgeRenderingMode.PreferSharpness);
    }

    private void DrawArrowHeadDown(IRenderContext rc, ScreenPoint point, double length, double thickness)
    {
        double angle = 45; // Arrowhead angle
        double radians = angle * (Math.PI / 180);

        // Calculate the points for the arrowhead lines
        var leftPoint = new ScreenPoint(
            point.X - length * Math.Cos(radians),
            point.Y - length * Math.Sin(radians));

        var rightPoint = new ScreenPoint(
            point.X + length * Math.Cos(radians),
            point.Y - length * Math.Sin(radians));

        // Draw the arrowhead lines
        rc.DrawLine(
            new List<ScreenPoint> { point, leftPoint },
            LineColor,
            thickness,
            EdgeRenderingMode.PreferSharpness);

        rc.DrawLine(
            new List<ScreenPoint> { point, rightPoint },
            LineColor,
            thickness,
            EdgeRenderingMode.PreferSharpness);
    }

    private void DrawText(IRenderContext rc, string text, double rotationAngle)
    {
        var midX = (MinimumX + MaximumX) / 2;
        var midY = (MinimumY + MaximumY) / 2;
        var textPosition = Transform(new DataPoint(midX, midY));

        var fontSize = this.FontSize > 0 ? this.FontSize : 12;

        rc.DrawText(textPosition, text, TextColor, "", fontSize, 500, rotationAngle, HorizontalAlignment.Center, VerticalAlignment.Middle);
    }
}




//I am still wondering why this is malfunctioning 
//By the way, remove the else codes and it works well for arrow up

public class CustomRectangleWithArrowAnnotationPartialValid : Annotation
{
    public double MinimumX { get; set; }
    public double MaximumX { get; set; }
    public double MinimumY { get; set; }
    public double MaximumY { get; set; }
    public string Text { get; set; } = string.Empty;
    public double TextRotationAngle { get; set; }
    public OxyColor FillColor { get; set; } = OxyColors.Black;
    public OxyColor LineColor { get; set; } = OxyColors.Red;
    public new OxyColor TextColor { get; set; } = OxyColors.Black;
    public double LineThickness { get; set; } = 1;
    public double ArrowHeadLength { get; set; } = 10;
    public ArrowDirection ArrowDirection { get; set; }

    public override void Render(IRenderContext rc)
    {
        var rect = new OxyRect(Transform(new DataPoint(MinimumX, MaximumY)), Transform(new DataPoint(MaximumX, MinimumY)));

        // Draw the rectangle
        rc.DrawRectangle(rect, FillColor, OxyColors.Transparent, 1, EdgeRenderingMode.PreferSharpness);

        // Calculate the center point of the rectangle
        double centerX = (MinimumX + MaximumX) / 2;
        double centerYTop = MaximumY;
        double centerYBottom = MinimumY;

        var screenCenterTop = Transform(new DataPoint(centerX, centerYTop));
        var screenCenterBottom = Transform(new DataPoint(centerX, centerYBottom));

        // Draw the vertical line based on the arrow direction
        if (ArrowDirection == ArrowDirection.up)
        {
            if (screenCenterTop.Y < screenCenterBottom.Y)
            {
                var temp = screenCenterTop;
                screenCenterTop = screenCenterBottom;
                screenCenterBottom = temp;
            }

            rc.DrawLine(
                new List<ScreenPoint> { screenCenterBottom, screenCenterTop },
                LineColor,
                LineThickness,
                EdgeRenderingMode.PreferSharpness);

            DrawArrowHeadUp(rc, screenCenterTop, ArrowHeadLength, LineThickness);
        }
        else if (ArrowDirection == ArrowDirection.down)
        {
            if (screenCenterTop.Y > screenCenterBottom.Y)
            {
                var temp = screenCenterTop;
                screenCenterTop = screenCenterBottom;
                screenCenterBottom = temp;
            }

            rc.DrawLine(
                new List<ScreenPoint> { screenCenterTop, screenCenterBottom },
                LineColor,
                LineThickness,
                EdgeRenderingMode.PreferSharpness);

            DrawArrowHeadDown(rc, screenCenterBottom, ArrowHeadLength, LineThickness);
        }

        // Draw the text
        DrawText(rc, Text, TextRotationAngle);
    }

    private void DrawArrowHeadUp(IRenderContext rc, ScreenPoint point, double length, double thickness)
    {
        double angle = 45; // Arrowhead angle
        double radians = angle * (Math.PI / 180);

        // Calculate the points for the arrowhead lines
        var leftPoint = new ScreenPoint(
            point.X - length * Math.Cos(radians),
            point.Y + length * Math.Sin(radians));

        var rightPoint = new ScreenPoint(
            point.X + length * Math.Cos(radians),
            point.Y + length * Math.Sin(radians));

        // Draw the arrowhead lines
        rc.DrawLine(
            new List<ScreenPoint> { point, leftPoint },
            LineColor,
            thickness,
            EdgeRenderingMode.PreferSharpness);

        rc.DrawLine(
            new List<ScreenPoint> { point, rightPoint },
            LineColor,
            thickness,
            EdgeRenderingMode.PreferSharpness);
    }

    private void DrawArrowHeadDown(IRenderContext rc, ScreenPoint point, double length, double thickness)
    {
        double angle = 45; // Arrowhead angle
        double radians = angle * (Math.PI / 180);

        // Calculate the points for the arrowhead lines
        var leftPoint = new ScreenPoint(
            point.X - length * Math.Cos(radians),
            point.Y - length * Math.Sin(radians));

        var rightPoint = new ScreenPoint(
            point.X + length * Math.Cos(radians),
            point.Y - length * Math.Sin(radians));

        // Draw the arrowhead lines
        rc.DrawLine(
            new List<ScreenPoint> { point, leftPoint },
            LineColor,
            thickness,
            EdgeRenderingMode.PreferSharpness);

        rc.DrawLine(
            new List<ScreenPoint> { point, rightPoint },
            LineColor,
            thickness,
            EdgeRenderingMode.PreferSharpness);
    }

    private void DrawText(IRenderContext rc, string text, double rotationAngle)
    {
        var midX = (MinimumX + MaximumX) / 2;
        var midY = (MinimumY + MaximumY) / 2;
        var textPosition = Transform(new DataPoint(midX, midY));

        var fontSize = this.FontSize > 0 ? this.FontSize : 12;

        rc.DrawText(textPosition, text, TextColor, "", fontSize, 500, rotationAngle, HorizontalAlignment.Center, VerticalAlignment.Middle);
    }
}



public class CustomRectangleWithArrowAnnotationUpArrow : Annotation
{
    public double MinimumX { get; set; }
    public double MaximumX { get; set; }
    public double MinimumY { get; set; }
    public double MaximumY { get; set; }
    public string Text { get; set; } = string.Empty;
    public double TextRotationAngle { get; set; }
    public OxyColor FillColor { get; set; } = OxyColors.Black;
    public OxyColor LineColor { get; set; } = OxyColors.Red;
    public new OxyColor TextColor { get; set; } = OxyColors.Black;
    public double LineThickness { get; set; } = 1;
    public double ArrowHeadLength { get; set; } = 10;
    public ArrowDirection ArrowDirection { get; set; }

    public override void Render(IRenderContext rc)
    {
        var rect = new OxyRect(Transform(new DataPoint(MinimumX, MaximumY)), Transform(new DataPoint(MaximumX, MinimumY)));

        // Draw the rectangle
        rc.DrawRectangle(rect, FillColor, OxyColors.Transparent, 1, EdgeRenderingMode.PreferSharpness);

        // Calculate the center point of the rectangle
        double centerX = (MinimumX + MaximumX) / 2;
        double centerYTop = MaximumY;
        double centerYBottom = MinimumY;

        var screenCenterTop = Transform(new DataPoint(centerX, centerYTop));
        var screenCenterBottom = Transform(new DataPoint(centerX, centerYBottom));

        // Ensure the line is drawn from bottom to top
        if (screenCenterTop.Y < screenCenterBottom.Y)
        {
            var temp = screenCenterTop;
            screenCenterTop = screenCenterBottom;
            screenCenterBottom = temp;
        }

        // Draw the vertical line
        rc.DrawLine(
            new List<ScreenPoint> { screenCenterBottom, screenCenterTop },
            LineColor,
            LineThickness,
            EdgeRenderingMode.PreferSharpness);

        // Draw the arrowhead at the top
         if (ArrowDirection.Equals(ArrowDirection.up))
        {
            DrawArrowHeadUp(rc, screenCenterBottom, ArrowHeadLength, LineThickness);
        }

        // Draw the text
        DrawText(rc, Text, TextRotationAngle);
    }

    private void DrawArrowHeadUp(IRenderContext rc, ScreenPoint point, double length, double thickness)
    {
        double angle = 45; // Arrowhead angle
        double radians = angle * (Math.PI / 180);

        // Calculate the points for the arrowhead lines
        var leftPoint = new ScreenPoint(
            point.X - length * Math.Cos(radians),
            point.Y + length * Math.Sin(radians));

        var rightPoint = new ScreenPoint(
            point.X + length * Math.Cos(radians),
            point.Y + length * Math.Sin(radians));

        // Draw the arrowhead lines
        rc.DrawLine(
            new List<ScreenPoint> { point, leftPoint },
            LineColor,
            thickness,
            EdgeRenderingMode.PreferSharpness);

        rc.DrawLine(
            new List<ScreenPoint> { point, rightPoint },
            LineColor,
            thickness,
            EdgeRenderingMode.PreferSharpness);
    }

    private void DrawText(IRenderContext rc, string text, double rotationAngle)
    {
        var midX = (MinimumX + MaximumX) / 2;
        var midY = (MinimumY + MaximumY) / 2;
        var textPosition = Transform(new DataPoint(midX, midY));

        var fontSize = this.FontSize > 0 ? this.FontSize : 12;

        rc.DrawText(textPosition, text, TextColor, "", FontSize, 500, rotationAngle, HorizontalAlignment.Center, VerticalAlignment.Middle);
    }
}



//arrow head down already works here, let's try for arrow head up
public class CustomRectangleWithArrowAnnotationDownArrowValid : Annotation
{
    public double MinimumX { get; set; }
    public double MaximumX { get; set; }
    public double MinimumY { get; set; }
    public double MaximumY { get; set; }
    public string Text { get; set; } = string.Empty;
    public double TextRotationAngle { get; set; }
    public OxyColor FillColor { get; set; } = OxyColors.Black;
    public OxyColor LineColor { get; set; } = OxyColors.Red;
    public new OxyColor TextColor { get; set; } = OxyColors.Black;
    public double LineThickness { get; set; } = 1;
    public double ArrowHeadLength { get; set; } = 10;
    public ArrowDirection ArrowDirection { get; set; }

    public override void Render(IRenderContext rc)
    {
        var rect = new OxyRect(Transform(new DataPoint(MinimumX, MaximumY)), Transform(new DataPoint(MaximumX, MinimumY)));

        // Draw the rectangle
        rc.DrawRectangle(rect, FillColor, OxyColors.Transparent, 1, EdgeRenderingMode.PreferSharpness);

        // Calculate the center point of the rectangle
        double centerX = (MinimumX + MaximumX) / 2;
        double centerYTop = MaximumY;
        double centerYBottom = MinimumY;

        var screenCenterTop = Transform(new DataPoint(centerX, centerYTop));
        var screenCenterBottom = Transform(new DataPoint(centerX, centerYBottom));

        // Ensure the line is drawn from top to bottom
        if (screenCenterTop.Y > screenCenterBottom.Y)
        {
            var temp = screenCenterTop;
            screenCenterTop = screenCenterBottom;
            screenCenterBottom = temp;
        }

        // Draw the vertical line
        rc.DrawLine(
            new List<ScreenPoint> { screenCenterTop, screenCenterBottom },
            LineColor,
            LineThickness,
            EdgeRenderingMode.PreferSharpness);

        // Draw the arrowhead at the bottom
        if (ArrowDirection.Equals(ArrowDirection.down))
        {
            DrawArrowHeadDown(rc, screenCenterBottom, ArrowHeadLength, LineThickness);
        }

        // Draw the text
        DrawText(rc, Text, TextRotationAngle);
    }
     private void DrawArrowHeadUp(IRenderContext rc, ScreenPoint point, double length, double thickness)
    {
        double angle = 45; // Arrowhead angle
        double radians = angle * (Math.PI / 180);

        // Calculate the points for the arrowhead lines
        var leftPoint = new ScreenPoint(
            point.X - length * Math.Cos(radians),
            point.Y + length * Math.Sin(radians));

        var rightPoint = new ScreenPoint(
            point.X + length * Math.Cos(radians),
            point.Y + length * Math.Sin(radians));

        // Draw the arrowhead lines
        rc.DrawLine(
            new List<ScreenPoint> { point, leftPoint },
            LineColor,
            thickness,
            EdgeRenderingMode.PreferSharpness);

        rc.DrawLine(
            new List<ScreenPoint> { point, rightPoint },
            LineColor,
            thickness,
            EdgeRenderingMode.PreferSharpness);
    }


    private void DrawArrowHeadDown(IRenderContext rc, ScreenPoint point, double length, double thickness)
    {
        double angle = 45; // Arrowhead angle
        double radians = angle * (Math.PI / 180);

        // Calculate the points for the arrowhead lines
        var leftPoint = new ScreenPoint(
            point.X - length * Math.Cos(radians),
            point.Y - length * Math.Sin(radians));

        var rightPoint = new ScreenPoint(
            point.X + length * Math.Cos(radians),
            point.Y - length * Math.Sin(radians));

        // Draw the arrowhead lines
        rc.DrawLine(
            new List<ScreenPoint> { point, leftPoint },
            LineColor,
            thickness,
            EdgeRenderingMode.PreferSharpness);

        rc.DrawLine(
            new List<ScreenPoint> { point, rightPoint },
            LineColor,
            thickness,
            EdgeRenderingMode.PreferSharpness);
    }

    private void DrawText(IRenderContext rc, string text, double rotationAngle)
    {
        var midX = (MinimumX + MaximumX) / 2;
        var midY = (MinimumY + MaximumY) / 2;
        var textPosition = Transform(new DataPoint(midX, midY));

        var fontSize = this.FontSize > 0 ? this.FontSize : 12;

        rc.DrawText(textPosition, text, TextColor, "", FontSize, 500, rotationAngle, HorizontalAlignment.Center, VerticalAlignment.Middle);
    }
}



// working arrow head extension! do not tamper
/*
public class CustomRectangleWithArrowAnnotation : Annotation
{
    public double MinimumX { get; set; }
    public double MaximumX { get; set; }
    public double MinimumY { get; set; }
    public double MaximumY { get; set; }
    public ArrowDirection ArrowDirection{ get; set; }
    public OxyColor FillColor { get; set; } = OxyColors.Black;
    public OxyColor LineColor { get; set; } = OxyColors.Red;
    public double LineThickness { get; set; } = 1;
    public double ArrowHeadLength { get; set; } = 10;

    public override void Render(IRenderContext rc)
    {
        var rect = new OxyRect(Transform(new DataPoint(MinimumX, MaximumY)), Transform(new DataPoint(MaximumX, MinimumY)));

        // Draw the rectangle
        rc.DrawRectangle(rect, FillColor, OxyColors.Transparent, 1, EdgeRenderingMode.PreferSharpness);

        // Calculate the center point of the rectangle
        double centerX = (MinimumX + MaximumX) / 2;
        double centerYTop = MaximumY;
        double centerYBottom = MinimumY;

        var screenCenterTop = Transform(new DataPoint(centerX, centerYTop));
        var screenCenterBottom = Transform(new DataPoint(centerX, centerYBottom));

        // Draw the vertical line
        rc.DrawLine(
            new List<ScreenPoint> { screenCenterTop, screenCenterBottom },
            LineColor,
            LineThickness,
            EdgeRenderingMode.PreferSharpness);

        // Draw the arrowhead at the top
        switch (ArrowDirection)
        {
            case ArrowDirection.up:
                DrawArrowHeadUp(rc, screenCenterTop, ArrowHeadLength, LineThickness);
                break;
            case ArrowDirection.down:
                DrawArrowHeadDown(rc, screenCenterTop, ArrowHeadLength, LineThickness);
            break;
            default:
            break;
        }
    }

    private void DrawArrowHeadUp(IRenderContext rc, ScreenPoint point, double length, double thickness)
    {
        double angle = 45; // Arrowhead angle
        double radians = angle * (Math.PI / 180);

        // Calculate the points for the arrowhead lines
        var leftPoint = new ScreenPoint(
            point.X - length * Math.Cos(radians),
            point.Y + length * Math.Sin(radians));

        var rightPoint = new ScreenPoint(
            point.X + length * Math.Cos(radians),
            point.Y + length * Math.Sin(radians));

        // Draw the arrowhead lines
        rc.DrawLine(
            new List<ScreenPoint> { point, leftPoint },
            LineColor,
            thickness,
            EdgeRenderingMode.PreferSharpness);

        rc.DrawLine(
            new List<ScreenPoint> { point, rightPoint },
            LineColor,
            thickness,
            EdgeRenderingMode.PreferSharpness);
    }
    private void DrawArrowHeadDown(IRenderContext rc, ScreenPoint point, double length, double thickness)
{
    double angle = 45; // Arrowhead angle
    double radians = angle * (Math.PI / 180);

    // Calculate the points for the arrowhead lines
    var leftPoint = new ScreenPoint(
        point.X - length * Math.Cos(radians),
        point.Y - length * Math.Sin(radians));

    var rightPoint = new ScreenPoint(
        point.X + length * Math.Cos(radians),
        point.Y - length * Math.Sin(radians));

    // Draw the arrowhead lines
    rc.DrawLine(
        new List<ScreenPoint> { point, leftPoint },
        LineColor,
        thickness,
        EdgeRenderingMode.PreferSharpness);

    rc.DrawLine(
        new List<ScreenPoint> { point, rightPoint },
        LineColor,
        thickness,
        EdgeRenderingMode.PreferSharpness);
}

    private void DrawText(IRenderContext rc, string text, double rotationAngle)
    {   
        var midX = (MinimumX + MaximumX) / 2;
        var midY = (MinimumY + MaximumY) / 2;
        var textPosition = Transform(new DataPoint(midX, midY));

        var fontSize = this.FontSize > 0 ? this.FontSize : 12;

        rc.DrawText(textPosition, text, TextColor, "", FontSize, 500, rotationAngle, HorizontalAlignment.Center,VerticalAlignment.Middle);
    }
}
*/