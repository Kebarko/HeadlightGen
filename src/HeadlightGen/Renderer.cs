using System.Drawing;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace KE.MSTS.HeadlightGen;

public class Renderer
{
    public void Render(Canvas canvas, float centerX, float centerY, int circles, float maxRadius, int increment)
    {
        // Calculate points
        var segmentCalculator = new SegmentCalculator();
        var points = segmentCalculator.Calculate(new PointF(centerX, centerY), circles, maxRadius, increment);

        // Calculate bounds of original points
        float minX = points.Min(p => p.X);
        float maxX = points.Max(p => p.X);
        float minY = points.Min(p => p.Y);
        float maxY = points.Max(p => p.Y);
        float pointsWidth  = maxX - minX;
        float pointsHeight = maxY - minY;

        // Get the canvas size
        float canvasWidth  = (float)canvas.ActualWidth;
        float canvasHeight = (float)canvas.ActualHeight;

        // Calculate scale to fit points within canvas
        float scaleX = canvasWidth / pointsWidth;
        float scaleY = canvasHeight / pointsHeight;

        // Use the smaller scale to maintain aspect ratio
        float scale = Math.Min(scaleX, scaleY);

        // Calculate scaled dimensions
        float scaledWidth  = pointsWidth  * scale;
        float scaledHeight = pointsHeight * scale;

        // Calculate offsets to center the drawing
        float offsetX = (canvasWidth  - scaledWidth)  / 2;
        float offsetY = (canvasHeight - scaledHeight) / 2;

        // Transform points to canvas coordinates
        var scaledPoints = points.Select(p => new PointF
        {
            X = (p.X - minX) * scale + offsetX,
            Y = (p.Y - minY) * scale + offsetY
        }).ToList();

        // Draw points on canvas
        bool first = true;
        foreach (PointF point in scaledPoints)
        {
            var ellipse = new Ellipse
            {
                Width = 6,
                Height = 6,
                Fill = first ? Brushes.Red : Brushes.Green
            };

            Canvas.SetLeft(ellipse, point.X - ellipse.Width / 2);
            Canvas.SetTop(ellipse, point.Y - ellipse.Height / 2);
            canvas.Children.Add(ellipse);

            first = false;
        }
    }
}
