using System.Drawing;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace KE.MSTS.HeadlightGen;

/// <summary>
/// Renders calculated light points to a WPF canvas with automatic scaling and centering.
/// Handles transformation of calculated coordinates to canvas display coordinates.
/// </summary>
public class Renderer
{
    /// <summary>
    /// Renders light points on a canvas with automatic scaling to fit the available space.
    /// The center point is drawn in red, while all other points are drawn in green.
    /// </summary>
    /// <param name="canvas">The WPF canvas to render the points on.</param>
    /// <param name="centerX">The X coordinate of the light center point.</param>
    /// <param name="centerY">The Y coordinate of the light center point.</param>
    /// <param name="circles">The number of concentric circles to render.</param>
    /// <param name="maxRadius">The maximum radius as a percentage.</param>
    /// <param name="increment">The base increment value for segment calculation.</param>
    /// <param name="baseAngle">The starting angle in degrees for segment orientation.</param>
    public void Render(Canvas canvas, float centerX, float centerY, int circles, float maxRadius, int increment, int baseAngle)
    {
        // Calculate points
        var segmentCalculator = new SegmentCalculator();
        var points = segmentCalculator.Calculate(new PointF(centerX, centerY), circles, maxRadius, increment, baseAngle);

        // Calculate bounds of original points
        float minX = centerX - maxRadius / 100;
        float maxX = centerX + maxRadius / 100;
        float minY = centerY - maxRadius / 100;
        float maxY = centerY + maxRadius / 100;
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

        // Calculate scaled dimensions of points bounding box
        float scaledWidth  = pointsWidth  * scale;
        float scaledHeight = pointsHeight * scale;

        // Calculate offsets to center the drawing
        float offsetX = (canvasWidth  - scaledWidth)  / 2;
        float offsetY = (canvasHeight - scaledHeight) / 2;

        // Transform points to canvas coordinates and flip Y axis
        var scaledPoints = points.Select(p => new PointF
        {
            X = (p.X - minX) * scale + offsetX,
            Y = scaledHeight - (p.Y - minY) * scale + offsetY
        }).ToList();

        // Draw points on canvas
        bool first = true;
        foreach (PointF point in scaledPoints)
        {
            var ellipse = new Ellipse
            {
                Width = 8,
                Height = 8,
                Fill = first ? Brushes.Red : Brushes.Green
            };

            Canvas.SetLeft(ellipse, point.X - ellipse.Width / 2);
            Canvas.SetTop(ellipse, point.Y - ellipse.Height / 2);
            canvas.Children.Add(ellipse);

            first = false;
        }
    }
}
